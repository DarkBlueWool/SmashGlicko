﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Smash_Glicko_Frontend.Shortcuts;

namespace Smash_Glicko_Frontend.Models
{
    public class DatabaseLeagueModel
    {
        [Key]
        public uint LeagueId { get; set; }

        public string LeagueName { get; set; }

        public string LeagueGame { get; set; }

        public byte[] TimeFrameIDs { get; set; }

        public uint EventCount = 0;

        public DateTime InitalTimeFrameStart { get; set; }

        public uint TimeFrameSpan { get; set; }

        public DatabaseLeagueModel(string Name, string Game, List<uint> timeFrameIDs, DateTime initialTimeFrameStart, uint timeFrameSpan, bool isPublic)
        {
            LeagueName = Name;
            LeagueGame = Game;
            TimeFrameIDs = ByteConvertion.UintToByte(timeFrameIDs.ToArray());
            InitalTimeFrameStart = initialTimeFrameStart;
            TimeFrameSpan = timeFrameSpan;
            IsPublic = isPublic;
        }

        public DatabaseLeagueModel()
        {
            LeagueName = "Unknown";
            LeagueGame = "Unknown";
            TimeFrameIDs = new byte[0];
            InitalTimeFrameStart = DateTime.MinValue;
            TimeFrameSpan = 7;
            IsPublic = false;
        }

        public bool IsPublic { get; set; }
    }

    public class DatabaseEventModel
    {
        [Key]
        public uint EventID { get; set; }

        public string EventSlug { get; set; }

        public uint PlayerCount { get; set; }

        public byte[] Player1ID { get; set; }

        public byte[] Player2ID { get; set; }

        public byte[] Player1Wins { get; set; }

        public byte[] Player2Wins { get; set; }

        public DatabaseEventModel()
        {
            PlayerCount = 0;
            Player1ID = new byte[0];
            Player2ID = new byte[0];
            Player1Wins = new byte[0];
            Player2Wins = new byte[0];
            EventSlug = "Unknown";
        }

        public DatabaseEventModel(uint playerCount, string eventSlug, uint[] player1ID, uint[] player2ID, short[] player1Wins, short[] player2Wins)
        {
            PlayerCount = playerCount;
            EventSlug = eventSlug;
            Player1ID = ByteConvertion.UintToByte(player1ID);
            Player2ID = ByteConvertion.UintToByte(player2ID);
            Player1Wins = ByteConvertion.ShortToByte(player1Wins);
            Player2Wins = ByteConvertion.ShortToByte(player2Wins);
        }
    }

}
namespace Smash_Glicko_Frontend.Shortcuts
{
    class ByteConvertion
    {
        public static byte[] UintToByte(uint[] input)
        {
            byte[] Output = new byte[input.Length * 4];
            System.Buffer.BlockCopy(input, 0, Output, 0, input.Length * 4);
            return Output;
        }
        public static uint[] ByteToUint(byte[] input)
        {
            uint[] Output = new uint[input.Length / 4];
            System.Buffer.BlockCopy(input, 0, Output, 0, input.Length);
            return Output;
        }
        public static byte[] ShortToByte(short[] input)
        {
            byte[] Output = new byte[input.Length * 2];
            System.Buffer.BlockCopy(input, 0, Output, 0, input.Length * 4);
            return Output;
        }
        public static short[] ByteToShort(byte[] input)
        {
            short[] Output = new short[input.Length / 2];
            System.Buffer.BlockCopy(input, 0, Output, 0, input.Length);
            return Output;
        }
    }
}