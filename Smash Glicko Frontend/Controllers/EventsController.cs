using Microsoft.AspNetCore.Mvc;
using Smash_Glicko_Frontend.Data;
using Smash_Glicko_Frontend.Models;
using Smash_Glicko_Frontend.Shortcuts;

namespace Smash_Glicko_Frontend.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventsController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return Ok("Here we go again...");
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(CreateEventModel model)
        {
            //Verify link is likely good
            if (!ModelState.IsValid) return BadRequest(ModelState.ErrorCount);
            if (!model.EventLink.Contains("smash.gg/")) return BadRequest("Invalid Link");

            //Parse out the slug for the event
            //Example Link - https://smash.gg/tournament/4o4-smash-night-39/event/singles/...
            //Example Slug - tournament/4o4-smash-night-39/event/singles
            string slug;


            string[] temp = model.EventLink.Split(new String[] { "smash.gg/" }, StringSplitOptions.None)[1].Split('/');

            //If it's not long enough to contain the event or isn't obviously a real link, return bad request
            if (temp.Length < 4 || !temp[0].Equals("tournament") || !temp[2].Equals("event") && !temp[2].Equals("events")) return BadRequest("Link don't look right - " + temp[0] + " != tournament || " + temp[2] + " != event || Length (" + temp.Length.ToString() + ") < 4");

            slug = "tournament/" + temp[1] + "/event/" + temp[3];

            //Will still crash if the event name is bad. I'll deal with that later :)
            ReturnModel Output = await DatabaseInteractor.AddEventFromSmashGG(slug, _context);

            switch (Output.Success)
            {
                //General bad input stuff
                case -1:
                    return BadRequest(Output.Error);
                //SmashGG ID returned as null... That shouldn't happen
                case -2:
                    return this.StatusCode(500);
                default:
                    break;
            }

            return Ok("Made it :)");
        }
    }
}
