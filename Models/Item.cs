using MongoDB.Bson;

namespace Models
{
    public class Item
    {
        public const string Bomb = "BOMB";

        public ObjectId Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Cost { get; set; }
    }
}
