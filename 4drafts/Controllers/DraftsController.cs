using _4drafts.Data;
using _4drafts.Data.Models;
using _4drafts.Models.Drafts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace _4drafts.Controllers
{
    public class DraftsController : Controller
    {
        private readonly _4draftsDbContext data;
        private readonly UserManager<User> userManager;

        public DraftsController(_4draftsDbContext data,
            UserManager<User> userManager)
        {
            this.data = data;
            this.userManager = userManager;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Save(string title, string description, string content, string draftId = null)
        {
            var user = await this.userManager.GetUserAsync(this.User);

            if (user == null) return Unauthorized();

            if(title == null) return Json(new { isValid = false, msg = "Whoops! Drafts require a title..." });

            if (draftId == null)
            {
                var draft = new Draft
                {
                    Title = title,
                    Description = description,
                    Content = content,
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
                    AuthorId = d.AuthorId,
                })
                .ToList();

            return View(drafts);
        }
    }
}
