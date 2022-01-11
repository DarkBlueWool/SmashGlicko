using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Smash_Glicko_Frontend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Smash_Glicko_Frontend.Controllers
{
    public class TestController : Controller
    {
        public IActionResult Index()
        {
            return Ok("Index");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Where()
        {
            return Ok("GOD DAMN IT KRIS, WHERE THE FUCK ARE WE!?");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GqlApi(string ApiToken)
        {
            EventModel model = await Shortcuts.GQLInteractor.GetEventData("tournament/4o4-smash-night-35/event/singles", ApiToken);
            return Ok(model.Player1Wins.Count + " " + model.PlayerCount);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Timeframe()
        {
            return View();
        }
    }
}
