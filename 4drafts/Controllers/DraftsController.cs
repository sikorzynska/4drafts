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
        public async Task<IActionResult> Save(string title, string description, string content, string draftId = null)
        {
            var user = await this.userManager.GetUserAsync(this.User);

            if (user == null) return Unauthorized();

            var draftCount = this.data.Drafts.Count(d => d.AuthorId == user.Id);

            if (title == null) return Json(new { isValid = false, msg = Drafts.MissingTitle });

            if(draftCount >= 10) return Json(new { isValid = false, msg = Drafts.ReachedLimit });

            if (draftId == null)
            {
                var draft = new Draft
                {
                    Title = title,
                    Description = description,
                    Content = content,
                    CreatedOn = DateTime.UtcNow.ToLocalTime(),
                    AuthorId = user.Id,
                };

                this.data.Drafts.Add(draft);
                await this.data.SaveChangesAsync();

                return Json(new { isValid = true, msg = Drafts.Saved });
            }
            else
            {
                var draft = await this.data.Drafts.FirstOrDefaultAsync(d => d.Id == draftId);

                if(draft == null) return Json(new { isValid = false, msg = Drafts.Inexistent });

                draft.Title = title;
                draft.Description = description;
                draft.Content = content;
                draft.CreatedOn = DateTime.UtcNow.ToLocalTime();
                draft.AuthorId = user.Id;

                await this.data.SaveChangesAsync();

                return Json(new { isValid = true , msg = Drafts.Updated });
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> All()
        {
            var user = await this.userManager.GetUserAsync(this.User);
            if (user == null) return Unauthorized();

            var drafts = this.data.Drafts
                .Where(d => d.AuthorId == user.Id)
                .OrderByDescending(d => d.CreatedOn)
                .Select(d => new DraftViewModel
                {
                    Id = d.Id,
                    Title = d.Title,
                    Description = d.Description,
                    Content = d.Content,
                    CreatedOn = this.timeWarper.TimeAgo(d.CreatedOn),
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
            var draft = await this.data.Drafts.FirstOrDefaultAsync(d => d.Id == Id);

            if (draft == null) return Json(new { isValid = false, msg = Drafts.Inexistent });

            var user = await this.userManager.GetUserAsync(this.User);

            if (user == null || user.Id != draft.AuthorId) return Json(new { isValid = false, msg = Global.UnauthorizedAction });

            return Json(new { 
                isValid = true,
                html = RenderRazorViewToString(this, 
                "Edit", 
                new DraftViewModel { Id = draft.Id, Title = draft.Title, Description = draft.Description, Content = draft.Content }) });
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
                return Json(new { isValid = false,
                    html = RenderRazorViewToString(this, "Edit", model)
                });
            }
            
            draft.Title = model.Title;
            draft.Description = model.Description;
            draft.Content = model.Content;

            await this.data.SaveChangesAsync();

            var drafts = this.data.Drafts
                .Where(d => d.AuthorId == draft.AuthorId)
                .Select(d => new DraftViewModel
                {
                    Id = d.Id,
                    Title = d.Title,
                    Description = d.Description,
                    Content = d.Content,
                    CreatedOn = this.timeWarper.TimeAgo(d.CreatedOn),
                }).ToList();

            return Json(new { isValid = true, 
                msg = Drafts.Updated, 
                entity = "draft",
                html = RenderRazorViewToString(this, "_DraftsPartial", drafts)
            });
        }
    }
}
