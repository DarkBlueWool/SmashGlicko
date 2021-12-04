using Smash_Glicko_Frontend.Models;

namespace Smash_Glicko_Frontend.Data
{
    public class TestEvents
    {
        private static List<EventModel> Events = new List<EventModel>()
        {
            new EventModel(0, 32),
            new EventModel(1, 64),
            new EventModel(2, 35),
            new EventModel(3, 89)
        };

        public static TimeFrameModel timeFrame = new TimeFrameModel(Events, new Dictionary<uint, PlayerModel>());
    }
    public class TestData
    {
        private static List<EventModel> _events = new List<EventModel>();
        private static List<TimeFrameModel> _timeFrames = new List<TimeFrameModel> { new TimeFrameModel(_events, new Dictionary<uint, PlayerModel>()) };
        public static List<LeagueModel> Leagues = new List<LeagueModel>() {
            new LeagueModel("A League", "SSBU", _timeFrames, DateTime.Now, new TimeSpan(7,0,0,0), true),
            new LeagueModel("B League", "SSBU", _timeFrames, DateTime.Now, new TimeSpan(7,0,0,0), true),
            new LeagueModel("C League", "SSBU", _timeFrames, DateTime.Now, new TimeSpan(7,0,0,0), true),
            new LeagueModel("D League", "SSBU", _timeFrames, DateTime.Now, new TimeSpan(7,0,0,0), true),
            new LeagueModel("E League", "SSBU", _timeFrames, DateTime.Now, new TimeSpan(7,0,0,0), true),
            new LeagueModel("F League", "SSBU", _timeFrames, DateTime.Now, new TimeSpan(7,0,0,0), true),
            new LeagueModel("G League", "SSBU", _timeFrames, DateTime.Now, new TimeSpan(7,0,0,0), true),
            new LeagueModel("H League", "SSBU", _timeFrames, DateTime.Now, new TimeSpan(7,0,0,0), true),
            new LeagueModel("I League", "SSBU", _timeFrames, DateTime.Now, new TimeSpan(7,0,0,0), true),
            new LeagueModel("J League", "SSBU", _timeFrames, DateTime.Now, new TimeSpan(7,0,0,0), true),
            new LeagueModel("K League", "SSBU", _timeFrames, DateTime.Now, new TimeSpan(7,0,0,0), true),
            new LeagueModel("L League", "SSBU", _timeFrames, DateTime.Now, new TimeSpan(7,0,0,0), true),
        };
    }
}
