using MongoDB.Bson;

namespace Models
{
    public class Competition
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string EmblemUrl { get; set; }
        public string Plan { get; set; }
        public string LastUpdated { get; set; }
        
    }
}