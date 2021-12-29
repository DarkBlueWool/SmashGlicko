using Microsoft.AspNetCore.Mvc;
using Smash_Glicko_Frontend.Models;
using Smash_Glicko_Frontend.Shortcuts;

namespace Smash_Glicko_Frontend.Controllers
{
    public class EventsController : Controller
    {
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
            string ApiToken = ":(";

            string[] temp = model.EventLink.Split(new String[] { "smash.gg/" }, StringSplitOptions.None)[1].Split('/');

            //If it's not long enough to contain the event or isn't obviously a real link, return bad request
            if (temp.Length < 4 || !temp[0].Equals("tournament") || !temp[2].Equals("event")) return BadRequest("Link don't look right - " + temp[0] + " != tournament || " + temp[2] + " != event || Length (" + temp.Length.ToString() + ") < 4");

            slug = "tournament/" + temp[1] + "/event/" + temp[2];

            EventModel NewEvent = await GQLInteractor.GetEventData(slug, ApiToken);

            if (NewEvent.EventID == null) return BadRequest("GraphQL Error (I haven't added proper error parsing :(");
            return Ok("Made it :)");
        }
    }
}
