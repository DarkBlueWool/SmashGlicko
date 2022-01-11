using Smash_Glicko_Frontend.Shortcuts;
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
        public uint? TimeframeId { get; set; }

        public uint TimeframeIndex = 0;

        public uint LeagueID { get; set; }

        public Dictionary<uint, PlayerModel> Players = new Dictionary<uint, PlayerModel>();

        public List<uint> EventIds { get; set; }

        public TimeFrameModel(List<uint> IniEventIds, Dictionary<uint, PlayerModel> IniPlayers, uint IniTimeframeIndex)
        {
            Players = IniPlayers;
            EventIds = IniEventIds;
            TimeframeIndex = IniTimeframeIndex;
        }

        public TimeFrameModel StartNextTimeFrame()
        {
            Dictionary<uint, PlayerModel> UpdatedPlayers = new Dictionary<uint, PlayerModel>();

            //Temp shit
            UpdatedPlayers = Players;

            TimeFrameModel NewFrame = new TimeFrameModel(new List<uint>(), UpdatedPlayers, TimeframeIndex + 1);
            return NewFrame;
        }

        public void AddPlayers(Dictionary<uint, PlayerModel> NewPlayers)
        {
            if (Players.Count == 0)
            {
                Players = NewPlayers;
            }
            else
            {
                foreach (KeyValuePair<uint, PlayerModel> NewPlayer in NewPlayers)
                {
                    if (!Players.ContainsKey(NewPlayer.Key))
                    {
                        Players.Add(NewPlayer.Key, NewPlayer.Value);
                    }
                }
            }
        }
        public void AddPlayers(List<uint> NewPlayers)
        {
            foreach (uint PlayerID in NewPlayers) {
                if (!Players.ContainsKey(PlayerID))
                {
                    Players.Add(PlayerID, new PlayerModel(PlayerID));
                }
            }
        }

        public void AddPlayersFromEvent(EventModel Event)
        {
            HashSet<uint> PlayerIDs = new HashSet<uint>();
            foreach(uint PlayerID in Event.Player1ID)
            {
                PlayerIDs.Add(PlayerID);
            }
            foreach (uint PlayerID in Event.Player2ID)
            {
                PlayerIDs.Add(PlayerID);
            }
            AddPlayers(PlayerIDs.ToList());
        }

        //Needs optimizing but it works for now
        public byte[] PlayerModelsToBytes()
        {
            List<byte> bytes = new List<byte>();
            foreach (KeyValuePair<uint, PlayerModel> Player in Players)
            {
                bytes = bytes.Concat(Player.Value.ToBytes()).ToList();
            }
            return bytes.ToArray();
        }

        public static Dictionary<uint, PlayerModel> BytesToPlayerModels(byte[] bytes) 
        {
            Dictionary<uint, PlayerModel> Output = new Dictionary<uint, PlayerModel>();
            int PlayerModelSize = PlayerModel.SizeOfBytes();
            PlayerModel NewPlayer;
            byte[] inputBytes = new byte[PlayerModelSize];
            for(int i = 0; i < bytes.Length; i += PlayerModelSize)
            {
                System.Buffer.BlockCopy(bytes, i, inputBytes, 0, PlayerModelSize);
                NewPlayer = PlayerModel.FromBytes(inputBytes);
                Output.Add(NewPlayer.Id, NewPlayer);
            }
            return Output;
        }

        public DatabaseTimeframeModel ToDatabaseModel()
        {
            return new DatabaseTimeframeModel() { EventIDs = ByteConvertion.UintToByte(EventIds.ToArray()), League = LeagueID, PlayerData = PlayerModelsToBytes(), TimeFrameID = (uint)TimeframeId, TimeFrameIndex = TimeframeIndex };
        }
    }

    public class PlayerModel
    {
        public uint Id = 0;

        public float Rating = 0;

        public float Deviation = (float) (350 / 173.7178);
        public float Volatility = (float) 0.06;

        public PlayerModel(uint ID)
        {
            Id = ID;
            Rating = 0;
            Deviation = (float)(350 / 173.7178);
            Volatility = (float)0.06;
        }

        //Used to make a blank Playermodel
        private PlayerModel() { }

        public byte[] ToBytes()
        {
            //This is fucking horrible, but serialization is deprecated and this is the easiest way.
            byte[] Output = new byte[SizeOfBytes()];
            System.Buffer.BlockCopy(BitConverter.GetBytes(Id), 0, Output, 0, sizeof(uint));
            System.Buffer.BlockCopy(BitConverter.GetBytes(Rating), 0, Output, sizeof(uint), sizeof(float));
            System.Buffer.BlockCopy(BitConverter.GetBytes(Deviation), 0, Output, sizeof(uint) + sizeof(float), sizeof(float));
            System.Buffer.BlockCopy(BitConverter.GetBytes(Volatility), 0, Output, sizeof(uint) + 2 * sizeof(float), sizeof(float));
            return Output;
        }

        public static PlayerModel FromBytes(byte[] input)
        {
            //A lot of this is unecessary, but I wanna future proof it and be sure
            if (input.Length != SizeOfBytes())
            {
                throw new ArgumentException();
            }
            byte[] temp = new byte[sizeof(uint)];
            System.Buffer.BlockCopy(input, 0, temp, 0, sizeof(uint));
            uint id = BitConverter.ToUInt32(temp);
            System.Buffer.BlockCopy(input, sizeof(uint), temp, 0, sizeof(float));
            System.Buffer.BlockCopy(input, 0, temp, 0, sizeof(float));
            float rating = BitConverter.ToSingle(temp);
            System.Buffer.BlockCopy(input, sizeof(uint) + sizeof(float), temp, 0, sizeof(float));
            float deviation = BitConverter.ToSingle(temp);
            System.Buffer.BlockCopy(input, sizeof(uint) + 2 * sizeof(float), temp, 0, sizeof(float));
            float volatility = BitConverter.ToSingle(temp);
            PlayerModel Output = new PlayerModel() { Id = id, Rating = rating, Deviation = deviation, Volatility = volatility };
            return Output;
        }

        public static int SizeOfBytes()
        {
            return sizeof(uint) + 3 * sizeof(float);
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
