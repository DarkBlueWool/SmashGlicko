using Glicko_Smash_Implementation_Web_Frontend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Glicko_Smash_Implementation_Web_Frontend.Controllers
{
    [Authorize]
    public class LeaguesController : Controller
    {
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(CreateLeagueModel leagueModel)
        {
            if (!ModelState.IsValid)
            {
                return this.LocalRedirect("/Home/Index");
            }
            return Ok("You did it :)");
        }
    }
}
