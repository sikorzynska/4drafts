using _4drafts.Data;
using _4drafts.Data.Models;
using _4drafts.Models.Drafts;
using _4drafts.Models.Shared;
using _4drafts.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using static _4drafts.Services.ControllerExtensions;
using static _4drafts.Data.DataConstants;
using System.Collections.Generic;
using _4drafts.Models.Genres;

namespace _4drafts.Controllers
{
    public class DraftsController : Controller
    {
        private readonly _4draftsDbContext data;
        private readonly UserManager<User> userManager;
        private readonly ITimeWarper timeWarper;

        public DraftsController(_4draftsDbContext data,
            UserManager<User> userManager,
            ITimeWarper timeWarper)
        {
            this.data = data;
            this.userManager = userManager;
            this.timeWarper = timeWarper;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Save(string title, string content, int[] genreIds, string draftId = null, int typeId = 0)
        {
            var user = await this.userManager.GetUserAsync(this.User);

            if (user == null) return Unauthorized();

            var draftCount = this.data.Drafts.Count(d => d.AuthorId == user.Id && d.ThreadTypeId == typeId);

            if (title == null) return Json(new { isValid = false, msg = Drafts.MissingTitle });

            if(draftCount >= 10) return Json(new { isValid = false, msg = Drafts.ReachedLimit });

            if (typeId < 1 || typeId > 2) return Json(new { isValid = false, msg = Global.GeneralError });

            if (draftId == null)
            {
                var draft = new Draft();

                switch (typeId)
                {
                    case 1:
                        {
                            draft = new Draft
                            {
                                Title = title,
                                Content = content,
                                ThreadTypeId = typeId,
                                FirstGenre = genreIds.Length > 0 ? genreIds[0] : 0,
                                SecondGenre = genreIds.Length > 1 ? genreIds[1] : 0,
                                ThirdGenre = genreIds.Length > 2 ? genreIds[2] : 0,
                                CreatedOn = DateTime.UtcNow.ToLocalTime(),
                                AuthorId = user.Id,
                            };
                            break;
                        }
                    case 2:
                        {
                            draft = new Draft
                            {
                                Title = title,
                                Content = content,
                                ThreadTypeId = typeId,
                                FirstGenre = genreIds.Length > 0 ? genreIds[0] : 0,
                                SecondGenre = genreIds.Length > 1 ? genreIds[1] : 0,
                                ThirdGenre = genreIds.Length > 2 ? genreIds[2] : 0,
                                CreatedOn = DateTime.UtcNow.ToLocalTime(),
                                AuthorId = user.Id,
                            };
                            break;
                        }
                    default:
                        break;
                }

                this.data.Drafts.Add(draft);
                await this.data.SaveChangesAsync();

                return Json(new { isValid = true, msg = Drafts.Saved });
            }
            else
            {
                var draft = await this.data.Drafts.FirstOrDefaultAsync(d => d.Id == draftId);

                if(draft == null) return Json(new { isValid = false, msg = Drafts.Inexistent });

                draft.Title = title;
                draft.Content = content;
                draft.FirstGenre = genreIds.Length > 0 ? genreIds[0] : 0;
                draft.SecondGenre = genreIds.Length > 1 ? genreIds[1] : 0;
                draft.ThirdGenre = genreIds.Length > 2 ? genreIds[2] : 0;
                draft.CreatedOn = DateTime.UtcNow.ToLocalTime();

                await this.data.SaveChangesAsync();

                return Json(new { isValid = true , msg = Drafts.Updated });
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> All(int typeId = 1)
        {
            var user = await this.userManager.GetUserAsync(this.User);
            if (user == null) return Unauthorized();

            var drafts = this.data.Drafts
                .Where(d => d.AuthorId == user.Id && d.ThreadTypeId == typeId)
                .Include(d => d.ThreadType)
                .OrderByDescending(d => d.CreatedOn)
                .Select(d => new DraftViewModel
                {
                    Id = d.Id,
                    Title = d.Title,
                    TypeId = d.ThreadTypeId,
                    Content = d.Content,
                    CreatedOn = this.timeWarper.TimeAgo(d.CreatedOn),
                    FullDate = this.timeWarper.FullDate(d.CreatedOn),
                    AuthorId = d.AuthorId,
                })
                .ToList();

            return View(drafts);
        }

        [NoDirectAccess]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Delete(string Id)
        {
            var draft = await this.data.Drafts.FirstOrDefaultAsync(d => d.Id == Id);

            if (draft == null) return Json(new { isValid = false, msg = Drafts.Inexistent });

            var user = await this.userManager.GetUserAsync(this.User);

            if (user == null || user.Id != draft.AuthorId) return Json(new { isValid = false, msg = Global.UnauthorizedAction });

            return Json(new { isValid = true, html = RenderRazorViewToString(this, "DeleteEntity", new GlobalViewModel { Id = draft.Id, Name = "draft", Path = "/Drafts/Delete/" }) });
        }

        [NoDirectAccess]
        [Authorize]
        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string Id)
        {
            var draft = await this.data.Drafts.FirstOrDefaultAsync(d => d.Id == Id);

            if (draft == null) return Json(new { isValid = false, msg = Drafts.Inexistent });

            var user = await this.userManager.GetUserAsync(this.User);

            if (user == null || user.Id != draft.AuthorId) return Json(new { isValid = false, msg = Global.UnauthorizedAction });

            this.data.Drafts.Remove(draft);
            await this.data.SaveChangesAsync();

            var draftCount = this.data.Drafts.Count(d => d.AuthorId == draft.AuthorId);
            return Json(new { isValid = true, msg = Drafts.Deleted, entity = "draft", count = draftCount });
        }

        [NoDirectAccess]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit(string Id)
        {
            var draft = await this.data.Drafts
                .Include(d => d.ThreadType)
                .FirstOrDefaultAsync(d => d.Id == Id);

            if (draft == null) return Json(new { isValid = false, msg = Drafts.Inexistent });

            var user = await this.userManager.GetUserAsync(this.User);

            if (user == null || user.Id != draft.AuthorId) return Json(new { isValid = false, msg = Global.UnauthorizedAction });

            return Json(new { 
                isValid = true,
                html = RenderRazorViewToString(this, 
                "Edit", 
                new DraftViewModel 
                { 
                    Id = draft.Id, 
                    Genres = GetGenres(this.data, draft.ThreadTypeId), 
                    GenreIds = GetGenreIds(draft.Id, this.data), 
                    TypeId = draft.ThreadTypeId, 
                    TypeName = draft.ThreadType.Name, 
                    Title = draft.Title, 
                    Content = draft.Content 
                }) 
            });
        }

        [NoDirectAccess]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Edit(DraftViewModel model)
        {
            var draft = await this.data.Drafts.FirstOrDefaultAsync(d => d.Id == model.Id);

            if (draft == null) return Json(new { isValid = false, msg = Drafts.Inexistent });

            var user = await this.userManager.GetUserAsync(this.User);

            if (user == null || user.Id != draft.AuthorId) return Json(new { isValid = false, msg = Global.UnauthorizedAction });

            if (!ModelState.IsValid)
            {
                model.Genres = GetGenres(this.data, model.TypeId);
                return Json(new { isValid = false,
                    html = RenderRazorViewToString(this, "Edit", model)
                });
            }

            draft.Title = model.Title;
            draft.FirstGenre = 0;
            draft.SecondGenre = 0;
            draft.ThirdGenre = 0;
            draft.Content = model.Content;

            if (model.GenreIds != null)
            {
                draft.FirstGenre = model.GenreIds.Count() > 0 ? model.GenreIds[0] : 0;
                draft.SecondGenre = model.GenreIds.Count() > 1 ? model.GenreIds[1] : 0;
                draft.ThirdGenre = model.GenreIds.Count() > 2 ? model.GenreIds[2] : 0;
            }

            await this.data.SaveChangesAsync();

            var drafts = this.data.Drafts
                .Where(d => d.AuthorId == draft.AuthorId)
                .OrderByDescending(d => d.CreatedOn)
                .Select(d => new DraftViewModel
                {
                    Id = d.Id,
                    GenreIds = new List<int> 
                    {
                        d.FirstGenre,
                        d.SecondGenre,
                        d.ThirdGenre,
                    },
                    TypeId = d.ThreadTypeId,
                    Title = d.Title,
                    Content = d.Content,
                    CreatedOn = this.timeWarper.TimeAgo(d.CreatedOn),
                })
                .ToList();

            return Json(new { isValid = true, 
                msg = Drafts.Updated, 
                entity = "draft",
                html = RenderRazorViewToString(this, "_DraftsPartial", drafts)
            });
        }

        private static List<int> GetGenreIds(string draftId, _4draftsDbContext data)
        {
            var genreIds = new List<int>();

            var draft = data.Drafts.FirstOrDefault(d => d.Id == draftId);

            if (draft.FirstGenre != 0) genreIds.Add(draft.FirstGenre);
            if (draft.SecondGenre != 0) genreIds.Add(draft.SecondGenre);
            if (draft.ThirdGenre != 0) genreIds.Add(draft.ThirdGenre);

            return genreIds;
        }

        private static List<GenresBrowseModel> GetGenres(_4draftsDbContext data, int typeId = 0)
        {
            var genres = new List<GenresBrowseModel>();

            if (typeId != 0)
            {
                genres = data
                  .Genres
                  .Where(g => g.GenreTypeId == typeId)
                  .Select(c => new GenresBrowseModel
                  {
                      Id = c.Id,
                      Name = c.Name
                  })
                  .ToList();
            }
            else
            {
                genres = data
                  .Genres
                  .Select(c => new GenresBrowseModel
                  {
                      Id = c.Id,
                      Name = c.Name
                  })
                  .ToList();
            }

            return genres;
        }
    }
}
