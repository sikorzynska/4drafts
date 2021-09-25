using _4drafts.Data;
using _4drafts.Data.Models;
using _4drafts.Models.Drafts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace _4drafts.ViewComponents
{
    public class UserDrafts : ViewComponent
    {
        private readonly _4draftsDbContext data;
        private readonly UserManager<User> userManager;

        public UserDrafts(_4draftsDbContext data,
            UserManager<User> userManager)
        {
            this.data = data;
            this.userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await this.userManager.GetUserAsync(HttpContext.User);
            var drafts = this.data.Drafts
                .Where(d => d.AuthorId == user.Id)
                .Select(d => new DraftViewModel
                {
                    Title = d.Title,
                    Content = d.Content,
                    AuthorId = d.AuthorId
                })
                .ToList();

            return View("Drafts", drafts);
        }
    }
}
