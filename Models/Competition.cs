using MongoDB.Bson;

namespace Models
{
    public class Competition
    {
        public ObjectId UId { get; set; }
        public int Id { get; set; }
        public int ApiId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string EmblemUrl { get; set; }
        public string Plan { get; set; }
        public string LastUpdated { get; set; }
        
    }
}