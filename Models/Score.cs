namespace Models
{
    public class Score
    {
        public string Winner { get; set; }
        public string Duration { get; set; }
        public FullTime FullTime { get; set; }
        public HalfTime HalfTime { get; set; }
        public ExtraTime ExtraTime { get; set; }
        public Penalties Penalties { get; set; }
    }

    public class FullTime
    {
        public int? HomeTeam { get; set; }
        public int? AwayTeam { get; set; }
    }

    public class HalfTime
    {
        public int? HomeTeam { get; set; }
        public int? AwayTeam { get; set; }
    }

    public class ExtraTime
    {
        public int? HomeTeam { get; set; }
        public int? AwayTeam { get; set; }
    }

    public class Penalties
    {
        public int? HomeTeam { get; set; }
        public int? AwayTeam { get; set; }
    }
}