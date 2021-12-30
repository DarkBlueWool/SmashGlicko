using System.ComponentModel.DataAnnotations;

namespace Smash_Glicko_Frontend.Models
{
    public class CreateLeagueModel
    {
        [Required]
        public string LeagueName { get; set; }

        [Required]
        public string LeagueGame { get; set; }
        
        [Required]
        public uint TimeFrameSpan { get; set; }

        [Required]
        public bool IsPublic { get; set; }
    }

    public class LeagueModel
    {
        public uint LeagueId { get; set; }

        public string LeagueName { get; set; }

        public string LeagueGame { get; set; }

        public List<TimeFrameModel> TimeFrames { get; set; }

        public uint EventCount = 0;

        public DateTime InitalTimeFrameStart { get; set; }

        public TimeSpan TimeFrameSpan { get ; set; }

        public LeagueModel(uint ID, string Name, string Game, List<TimeFrameModel> timeFrames, DateTime initialTimeFrameStart, TimeSpan timeFrameSpan, bool isPublic)
        {
            LeagueId = ID;
            LeagueName = Name;
            LeagueGame = Game;
            TimeFrames = timeFrames;
            InitalTimeFrameStart = initialTimeFrameStart;
            TimeFrameSpan = timeFrameSpan;
            IsPublic = isPublic;
        }

        public bool IsPublic { get; set; }
    }

    public class ViewLeagueModel
    {
        [Required] public uint LeagueId { get; set; }

        public ViewLeagueModel(uint ID)
        {
            LeagueId = ID;
        }
    }

    public class TimeFrameModel
    {
        public Dictionary<uint, PlayerModel> Players = new Dictionary<uint, PlayerModel>();

        public uint EventCount;

        public List<EventModel> Events { get; set;}

        public TimeFrameModel(List<EventModel> IniEvents, Dictionary<uint, PlayerModel> IniPlayers)
        {
            Dictionary<uint, PlayerModel> AllPlayers = IniPlayers;
            Events = IniEvents;
        }

        public TimeFrameModel StartNextTimeFrame()
        {
            Dictionary<uint, PlayerModel> UpdatedPlayers = new Dictionary<uint, PlayerModel>();

            //Temp shit
            UpdatedPlayers = Players;

            TimeFrameModel NewFrame = new TimeFrameModel(new List<EventModel>(), UpdatedPlayers);
            return NewFrame;
        }

        public void AddPlayers(Dictionary<uint, PlayerModel> NewPlayers)
        {
            if (Players.Count == 0)
            {
                Players = NewPlayers;
            } else
            {
                foreach (KeyValuePair<uint, PlayerModel> Player in NewPlayers)
                {
                    if (!Players.Keys.Contains(Player.Key))
                    {
                        Players.Add(Player.Key, Player.Value);
                    }
                }
            }
        }
    }

    //UNFINISHED
    public class PlayerModel
    {
        public long Id { get; set; }

        public bool IsNew = true;

        public PlayerModel(uint PlayerID)
        {
            Id = PlayerID;
        }
    }
    public class CreateEventModel
    {
        [Required]
        public uint LeagueId { get; set; }
        [Required]
        public string EventLink { get; set; }

    }
    public class EventModel
    {
        //Internal EventID, NOT smash.gg id
        public uint? EventID { get; set; }

        public string EventSlug { get; set; }

        public uint? SmashID { get; set; }

        public uint PlayerCount { get; set; }

        public List<uint> Player1ID { get; set; }

        public List<uint> Player2ID { get; set; }

        public List<ushort> Player1Wins { get; set; }

        public List<ushort> Player2Wins { get; set; }

        public EventModel(string eventSlug)
        {
            EventSlug = eventSlug;
            PlayerCount = 0;
            Player1ID = new List<uint>();
            Player2ID = new List<uint>();
            Player1Wins = new List<ushort>();
            Player2Wins = new List<ushort>();
        }
    }

    public class SetModel
    {
        public uint Player1ID { get; set; }

        public uint Player2ID { get; set; }

        public uint Player1Wins { get; set; }

        public uint Player2Wins { get; set; }
    }
}
