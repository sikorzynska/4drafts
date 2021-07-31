using _4drafts.Data;
using _4drafts.Data.Models;
using _4drafts.Models.Categories;
using _4drafts.Models.Threads;
using _4drafts.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _4drafts.Controllers
{
    public class ThreadsController : Controller
    {
        private readonly ITimeWarper timeWarper;
        private readonly _4draftsDbContext data;
        private readonly UserManager<User> userManager;
        public ThreadsController(ITimeWarper timeWarper,
            _4draftsDbContext data,
            UserManager<User> userManager)
        {
            this.timeWarper = timeWarper;
            this.data = data;
            this.userManager = userManager;
        }

        [HttpGet]
        [Authorize]
        public IActionResult Create(int categoryId)
        {
            if (categoryId == 0) return View(new CreateThreadFormModel 
            {
                Categories = this.GetCategories(),
                CategoryName = this.data.Categories.FirstOrDefault().Name
            });

            return View(new CreateThreadFormModel
            {
                Categories = this.GetCategories(),
                CategoryName = this.data.Categories.FirstOrDefault(c => c.Id == categoryId).Name
            });
        }

        [HttpPost]
        [Authorize]
        public IActionResult Create(CreateThreadFormModel model)
        {
            if (!this.data.Categories.Any(c => c.Id == model.CategoryId))
            {
                this.ModelState.AddModelError(nameof(model.CategoryId), "Category does not exist.");
            }

            if (string.IsNullOrWhiteSpace(model.Content))
            {
                this.ModelState.AddModelError(nameof(model.Content), "The content field cannot be empty.");
            }

            if (!ModelState.IsValid)
            {
                model.Categories = this.GetCategories();

                return View(model);
            }

            var thread = new Thread
            {
                Title = model.Title,
                Description = model.Content,
                CreatedOn = DateTime.UtcNow.ToLocalTime(),
                AuthorId = this.userManager.GetUserId(this.User),
                CategoryId = model.CategoryId,
            };

            this.data.Threads.Add(thread);
            this.data.SaveChanges();

            return Redirect($"/Categories/Browse?categoryId={model.CategoryId}");
        }
        private IEnumerable<CategoriesBrowseModel> GetCategories()
            => this.data
                .Categories
                .Select(c => new CategoriesBrowseModel
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToList();
    }
}
