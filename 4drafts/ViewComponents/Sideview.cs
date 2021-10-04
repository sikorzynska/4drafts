using _4drafts.Data;
using _4drafts.Models.Shared;
using _4drafts.Models.Threads;
using _4drafts.Models.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _4drafts.ViewComponents
{
    public class Sideview : ViewComponent
    {
        private readonly _4draftsDbContext data;

        public Sideview(_4draftsDbContext data)
        {
            this.data = data;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var stories = this.data.Threads
                .Include(t => t.Author)
                .Where(t => t.ThreadTypeId == 1 || t.ThreadTypeId == 4)
                .OrderByDescending(t => t.Points)
                .Select(t => new ThreadsBrowseModel
                {
                    Id = t.Id,
                    ThreadTypeId = t.ThreadTypeId,
                    Title = t.Title,
                    Content = t.Content,
                    AuthorId = t.AuthorId,
                    AuthorName = t.Author.UserName,
                    Points = t.Points,
                })
                .Take(5)
                .ToList();

            var poems = this.data.Threads
                .Include(t => t.Author)
                .Where(t => t.ThreadTypeId == 2)
                .OrderByDescending(t => t.Points)
                .Select(t => new ThreadsBrowseModel
                {
                    Id = t.Id,
                    ThreadTypeId = t.ThreadTypeId,
                    Title = t.Title,
                    Content = t.Content,
                    AuthorId = t.AuthorId,
                    AuthorName = t.Author.UserName,
                    Points = t.Points,
                })
                .Take(5)
                .ToList();

            var users = this.data.Users
                .Include(u => u.Threads)
                .Include(u => u.Comments)
                .OrderByDescending(u => u.Points)
                .Select(u => new UserViewModel
                {
                    Id = u.Id,
                    Username = u.UserName,
                    AvatarUrl = u.AvatarUrl,
                    ThreadCount = u.Threads.Count(),
                    CommentCount = u.Comments.Count(),
                })
                .Take(5)
                .ToList();

            var rules = new List<string>();
            rules.Add("Stories must have a minimum of 500 characters.");
            rules.Add("Poems must have a minimum of 100 characters.");
            rules.Add("Prompts must be between 50 and 500 characters long.");
            rules.Add("Plagiarism will result in a ban");
            rules.Add("No immature / sexually explicit content");
            rules.Add("Avoid racism and political debates");
            rules.Add("Avoid detailed uses of suicide, and mental health stereotypes");

            return View("SideStats", new StatisticsViewModel 
            {
                Stories = stories,
                Poems = poems,
                Users = users,
                Rules = rules,
            });
        }
    }
}
