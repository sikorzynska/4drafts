using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _4drafts.Controllers
{
    public class UsersController : Controller
    {
        public IActionResult Profile(string userId)
        {
            return View();
        }
    }
}
