using _4drafts.Data;
using _4drafts.Models.Genres;
using _4drafts.Services;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using _4drafts.Models.Threads;
using Microsoft.EntityFrameworkCore;

namespace _4drafts.Controllers
{
    public class GenresController : Controller
    {
        private readonly ITimeWarper timeWarper;
        private readonly _4draftsDbContext data;
        public GenresController(ITimeWarper timeWarper,
            _4draftsDbContext data)
        {
            this.timeWarper = timeWarper;
            this.data = data;
        }
    }
}
