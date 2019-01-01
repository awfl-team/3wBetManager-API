using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public Nullable<int> HomeTeam { get; set; }
        public Nullable<int> AwayTeam { get; set; }
    }

    public class HalfTime
    {
        public Nullable<int> HomeTeam { get; set; }
        public Nullable<int> AwayTeam { get; set; }
    }

    public class ExtraTime
    {
        public Nullable<int> HomeTeam { get; set; }
        public Nullable<int> AwayTeam { get; set; }
    }

    public class Penalties
    {
        public Nullable<int> HomeTeam { get; set; }
        public Nullable<int> AwayTeam { get; set; }
    }
}
