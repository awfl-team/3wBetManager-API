using System;

namespace Models
{
    public class Match
    {
        public const string FinishedStatus = "FINISHED";
        public const string ScheduledStatus = "SCHEDULED";

        public int Id { get; set; }
        public string Status { get; set; }
        public DateTime LastUpdated { get; set; }
        public Team HomeTeam { get; set; }
        public double HomeTeamRating { get; set; }
        public Team AwayTeam { get; set; }
        public double AwayTeamRating { get; set; }
        public double DrawRating { get; set; }
        public Score Score { get; set; }
        public Competition Competition { get; set; }
        public string UtcDate { get; set; }

        
    }
}