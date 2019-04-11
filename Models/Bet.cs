using System;
using MongoDB.Bson;

namespace Models
{
    public class Bet
    {
        public const string WrongStatus = "Wrong";
        public const string OkStatus = "Ok";
        public const string PerfectStatus = "Perfect";
        public const double PerfectBet = 100;
        public const double OkBet = 50;
        public const double WrongBet = 0;
        public const int DashboardMaxToShow = 4;

        public ObjectId Id { get; set; }

        public string Guid { get; set; }
        public DateTime Date { get; set; }
        public int PointsWon { get; set; }
        public string Status { get; set; }
        public int Multiply { get; set; }
        public User User { get; set; }
        public int HomeTeamScore { get; set; }
        public int AwayTeamScore { get; set; }
        public Match Match { get; set; }
    }
}