﻿using _4drafts.Data;
using _4drafts.Data.Models;
using _4drafts.Models.Categories;
using _4drafts.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using _4drafts.Models.Threads;

namespace _4drafts.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ITimeWarper timeWarper;
        private readonly _4draftsDbContext data;
        private readonly UserManager<User> userManager;
        public CategoriesController(ITimeWarper timeWarper,
            _4draftsDbContext data,
            UserManager<User> userManager)
        {
            this.timeWarper = timeWarper;
            this.data = data;
            this.userManager = userManager;
        }

        public IActionResult All()
        {
            var categories = this.data.Categories
                .Select(c => new CategoriesBrowseModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    ThreadCount = this.data.Threads.Where(t => t.CategoryId == c.Id).Count(),
                })
                .OrderByDescending(c => c.ThreadCount)
                .ToList();

            return View(categories);
        }

        public IActionResult Browse(int categoryId)
        {
            if (!this.data.Categories.Any(c => c.Id == categoryId)) return NotFound();

            var name = this.data.Categories.FirstOrDefault(c => c.Id == categoryId).Name;
            var desc = this.data.Categories.FirstOrDefault(c => c.Id == categoryId).Description;

            var threads = this.data.Threads
                .Where(t => t.CategoryId == categoryId)
                .Select(t => new ThreadsBrowseModel
                {
                    Id = t.Id,
                    Title = t.Title,
                    CreatedOn = this.timeWarper.TimeAgo(t.CreatedOn),
                    Views = t.Views,
                    Points = t.Points,
                    AuthorId = t.AuthorId,
                    AuthorName = t.Author.UserName,
                    CommentCount = CommentCount(t.Id),
                })
                .OrderByDescending(t => t.CreatedOn)
                .ToList();

            return View(new CategoryBrowseModel 
            { 
                Id = categoryId,
                Name = name,
                Description = desc,
                Threads = threads,
            });
        }

        private int CommentCount(string threadId)
        {
            var result = this.data.Threads.FirstOrDefault(t => t.Id == threadId).Comments.Count();

            return result;
        }
    }
}
