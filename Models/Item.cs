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
        public const string Common = "Common";
        public const string Rare = "Rare";
        public const string Epic = "Epic";
        public const string Legendary = "Legendary";

        public ObjectId Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Cost { get; set; }
        public string Rarity { get; set; }
    }
}