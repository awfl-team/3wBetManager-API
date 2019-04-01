using MongoDB.Bson;

namespace Models
{
    public class Item
    {
        public const string Bomb = "BOMB";
        public const string LootBoxe = "LOOT_BOXE";
        public const string Multiply = "MULTIPLY_BY_TEN";
        public const string Life = "LIFE";
        public const string Key = "KEY";

        public ObjectId Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Cost { get; set; }
    }
}
