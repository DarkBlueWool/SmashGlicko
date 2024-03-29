﻿using Smash_Glicko_Frontend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Smash_Glicko_Frontend.Data;

namespace Smash_Glicko_Frontend.Controllers
{
    [Authorize]
    public class LeaguesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LeaguesController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult List()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(CreateLeagueModel model)
        {
            if (!ModelState.IsValid)
            {
                return this.LocalRedirect("/Home/Index");
            }
            var League = new DatabaseLeagueModel(model.LeagueName, model.LeagueGame, new List<uint>(), DateTime.UtcNow, model.TimeFrameSpan, true);
            EntityEntry<DatabaseLeagueModel> Entry = _context.Leagues.Add(League);
            _context.SaveChanges();
            return LocalRedirect("/Leagues/Display/" + Entry.Entity.LeagueId);
        }

        [HttpGet]
        public IActionResult Display(uint LeagueId)
        {
            return View(LeagueId);
        }
    }
}
namespace Glicko_Smash_Implementation_Web_Frontend.Shortcuts
{
    public class Shortcuts
    {
        public static List<GameData> GetGameData(string GameDataURL)
        {   
            List<GameData> Output = new List<GameData> { };
            XmlReader reader = XmlReader.Create(GameDataURL);
            int i = -1;
            reader.ReadToFollowing("GameData");
            while (reader.Read()) {
                if (reader.NodeType == XmlNodeType.Element) {
                    switch (reader.Name)
                    {
                        case "Game":
                            i++;
                            Output.Add(new GameData());
                            break;
                        case "Name":
                            Output[i].Name = (string)reader.ReadElementContentAsString();
                            break;
                        case "InternalID":
                            Output[i].InternalID = (string)reader.ReadElementContentAsString();
                            break;
                        case "SmashName":
                            Output[i].SmashName = (string)reader.ReadElementContentAsString();
                            break;
                    }
                }
            }
            return Output;
        }

    }
    public class GameData
    {
        public string Name = "null";
        public string InternalID = "null";
        public string SmashName = "null";
        public GameData(string name, string internalID, string smashName)
        {
            Name = name;
            InternalID = internalID;
            SmashName = smashName;
        }
        public GameData() {}
    }
}
