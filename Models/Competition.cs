namespace Models
{
    public class Competition
    {
        public static readonly int[] AvailableCompetitions =
            {2000, 2001, 2002, 2003, 2013, 2014, 2015, 2016, 2017, 2018, 2019, 2021};

        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string EmblemUrl { get; set; }
        public string Plan { get; set; }
        public string LastUpdated { get; set; }
    }
}