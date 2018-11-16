using System;
using MongoDB.Bson;

namespace Models
{
    public class Match
    {
        public ObjectId UId { get; set; }
        public int Id { get; set; }
        public float Cote { get; set; }
        public string Status { get; set; }
        public DateTime LastUpdated { get; set; }
        public Team HomeTeam { get; set; }
        public Team AwayTeam { get; set; }
        public Array Goals { get; set; }
    }
}