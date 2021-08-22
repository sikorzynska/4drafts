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

namespace _4drafts.Controllers
{
    public class DraftsController : Controller
    {
        private readonly _4draftsDbContext data;
        private readonly UserManager<User> userManager;
        private readonly ITimeWarper timeWarper;
        private readonly IHtmlHelper htmlHelper;

        public DraftsController(_4draftsDbContext data,
            UserManager<User> userManager,
            ITimeWarper timeWarper,
            IHtmlHelper htmlHelper)
        {
            this.data = data;
            this.userManager = userManager;
            this.timeWarper = timeWarper;
            this.htmlHelper = htmlHelper;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Save(string title, string description, string content, string draftId = null)
        {
            var user = await this.userManager.GetUserAsync(this.User);

            if (user == null) return Unauthorized();

            var draftCount = this.data.Drafts.Count(d => d.AuthorId == user.Id);

            if (title == null) return Json(new { isValid = false, msg = "Whoops! Drafts require a title..." });

            if(draftCount >= 10) return Json(new { isValid = false, msg = "Whoops! You can only have 10 drafts at a time..." });

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

                return Json(new { isValid = true, msg = "Draft has been saved successfully!" });
            }
            else
            {
                var draft = await this.data.Drafts.FirstOrDefaultAsync(d => d.Id == draftId);

                if(draft == null) return Json(new { isValid = false, msg = "Whoops! Looks like something went wrong, try creating a new draft" });

                draft.Title = title;
                draft.Description = description;
                draft.Content = content;
                draft.CreatedOn = DateTime.UtcNow.ToLocalTime();
                draft.AuthorId = user.Id;

                await this.data.SaveChangesAsync();

                return Json(new { isValid = true , msg = "Draft has been updated successfully!" });
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

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Delete(string Id)
        {
            var draft = await this.data.Drafts.FirstOrDefaultAsync(d => d.Id == Id);

            if (draft == null) return Json(new { isValid = false, msg = "Whoops! Looks like no such draft exists..." });

            var user = await this.userManager.GetUserAsync(this.User);

            if (user == null || user.Id != draft.AuthorId) return Json(new { isValid = false, msg = "Whoops! Looks like you're not authorized to do this..." });

            return Json(new { isValid = true, html = this.htmlHelper.RenderRazorViewToString(this, "DeleteEntity", new GlobalViewModel { Id = draft.Id, Name = "draft", Path = "/Drafts/Delete/" }) });
        }

        [Authorize]
        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string Id)
        {
            var draft = await this.data.Drafts.FirstOrDefaultAsync(d => d.Id == Id);

            if (draft == null) return Json(new { isValid = false, msg = "Whoops! Looks like no such draft exists..." });

            var user = await this.userManager.GetUserAsync(this.User);

            if (user == null || user.Id != draft.AuthorId) return Json(new { isValid = false, msg = "Whoops! Looks like you're not authorized to do this..." });

            this.data.Drafts.Remove(draft);
            await this.data.SaveChangesAsync();

            var draftCount = this.data.Drafts.Count(d => d.AuthorId == draft.AuthorId);
            return Json(new { isValid = true, msg = "The draft has been successfully deleted", entity = "draft", count = draftCount });
        }
    }
}
