using System;
using MongoDB.Bson;

namespace Models
{
    public class Bet
    {
        public ObjectId Id { get; set; }
        public DateTime Date { get; set; }
        public int PointsWon { get; set; }
        public string Status { get; set; }
        public User User { get; set; }
        public Match Match { get; set; }
    }
}