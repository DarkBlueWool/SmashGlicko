using Microsoft.EntityFrameworkCore.ChangeTracking;
using Smash_Glicko_Frontend.Data;
using Smash_Glicko_Frontend.Models;

namespace Smash_Glicko_Frontend.Shortcuts
{
    public class DatabaseInteractor
    {
        //Adds an event to the database. Returns "ERROR : *" on error and a 
        public static async Task<ReturnModel> AddEventFromSmashGG(string slug, ApplicationDbContext _context)
        {
            //Verify no duplicate entry based on slug
            IEnumerable<DatabaseEventModel> Events = _context.Events.Where(SQLEvent => SQLEvent.EventSlug == slug);
            foreach (DatabaseEventModel Event in Events)
            {
                return new ReturnModel();
            }

            //Get the event
            EventModel NewEvent = await GQLInteractor.GetEventData(slug, GQLInteractor.GetSmashGGAuthToken());

            //Some error handling - still partly unfinished (esp in the GetEventData side of things)
            if (!NewEvent.EventSlug.Equals(slug))
            {
                return new ReturnModel() { Error = NewEvent.EventSlug, Success = -1 };
            }
            if (NewEvent.SmashID == null)
            {
                return new ReturnModel() { Success = -2, Error = "Null SmashID????" };
            }

            //Verify no duplicate entry based on smash.gg ID
            Events = _context.Events.Where(SQLEvent => SQLEvent.SmashGGEventID == NewEvent.SmashID);
            foreach (DatabaseEventModel Event in Events)
            {
                //Change entires slug to be the new slug (in case of editting) - UNIMPLEMENTED - XXXXXXXXXXXXXXXX
                Event.EventSlug = slug;
                _context.Events.Update(Event);
                _context.SaveChanges();
                //Since only 1 event should exist with the same EventID, just return here.
                return new ReturnModel();
            }

            //Add the event to the database
            DatabaseEventModel DatabaseModel = new DatabaseEventModel(NewEvent.PlayerCount, (uint)NewEvent.SmashID, slug, NewEvent.Player1ID.ToArray(), NewEvent.Player2ID.ToArray(), NewEvent.Player1Wins.ToArray(), NewEvent.Player2Wins.ToArray());
            EntityEntry<DatabaseEventModel> Entry = _context.Events.Add(DatabaseModel);
            _context.SaveChanges();

            return new ReturnModel();
        }
    }
}
