﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    }
}
