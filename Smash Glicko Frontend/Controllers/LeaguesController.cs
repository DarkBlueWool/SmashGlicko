using Smash_Glicko_Frontend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;


namespace Smash_Glicko_Frontend.Controllers
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
            return Ok("You did it :)\n" + leagueModel.LeagueName + " is created for the game " + leagueModel.LeagueGame + " with the publicity set to " + leagueModel.IsPublic);
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
