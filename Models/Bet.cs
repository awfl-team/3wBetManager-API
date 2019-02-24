using System;
using System.Security.Policy;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Models
{
    public class Bet
    {
        public const string WrongStatus = "Wrong";
        public const string OkStatus = "Ok";
        public const string PerfectStatus = "Perfect";
        public const int PerfectBet = 100;
        public const int OkBet = 50;
        public const int WrongBet = 0;

        [BsonId]
        public ObjectId Id { get; set; }
        public string Guid { get; set; }
        public DateTime Date { get; set; }
        public int PointsWon { get; set; }
        public string Status { get; set; }
        public User User { get; set; }
        public int HomeTeamScore { get; set; }
        public int AwayTeamScore { get; set; }
        public Match Match { get; set; }
    }
}