using System.ComponentModel.DataAnnotations;

namespace Smash_Glicko_Frontend.Models
{
    public class CreateLeagueModel
    {
        [Required]
        public string LeagueName { get; set; }

        [Required]
        public string LeagueGame { get; set; }

        public bool IsPublic { get; set; }
    }

    public class LeagueModel
    {
        public string LeagueName { get; set; }

        public string LeagueGame { get; set; }

        public List<TimeFrameModel> TimeFrames { get; set; }

        public DateTime InitalTimeFrameStart { get; set; }

        public TimeSpan TimeFrameSpan { get ; set; }

        public LeagueModel(string Name, string Game, List<TimeFrameModel> timeFrames, DateTime initialTimeFrameStart, TimeSpan timeFrameSpan, bool isPublic)
        {
            LeagueName = Name;
            LeagueGame = Game;
            TimeFrames = timeFrames;
            InitalTimeFrameStart = initialTimeFrameStart;
            TimeFrameSpan = timeFrameSpan;
            IsPublic = isPublic;
        }

        public bool IsPublic { get; set; }
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
    public class EventModel
    {
        public uint EventID;

        public uint PlayerCount;

        public List<SetModel> Sets = new List<SetModel>();

        public EventModel(uint eventID, uint playerCount)
        {
            EventID = eventID;
            PlayerCount = playerCount;
        }
        public EventModel(uint eventID, uint playerCount, List<SetModel> sets)
        {
            EventID = eventID;
            PlayerCount = playerCount;
            Sets = sets;
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
