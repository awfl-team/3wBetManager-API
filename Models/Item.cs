using MongoDB.Bson;

namespace Models
{
    public class Item
    {
        public const string Bomb = "BOMB";
        public const string LootBox = "LOOT_BOX";
        public const string Multiply = "MULTIPLY_BY_TEN";
        public const string Life = "LIFE";
        public const string Key = "KEY";
        public const string Common = "Common";
        public const string Rare = "Rare";
        public const string Epic = "Epic";
        public const string Legendary = "Legendary";
        public const int MaxLoot = 5;
        public const float CommonDropChance = 75.00F;
        public const float RareDropChance = 20.00F;
        public const float EpicDropChance = 4.00F;
        public const float LegendaryDropChance = 1.00F;
        public const float RareDropChanceMin = CommonDropChance;
        public const float RareDropChanceMax = CommonDropChance + RareDropChance;
        public const float EpicDropChanceMin = CommonDropChance + RareDropChance;
        public const float EpicDropChanceMax = CommonDropChance + RareDropChance + EpicDropChance;
        public const float LegendaryDropChanceMin = CommonDropChance + RareDropChance + EpicDropChance;
        public const float LegendaryDropChanceMax = CommonDropChance + RareDropChance + EpicDropChance + LegendaryDropChance;

        public ObjectId Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Cost { get; set; }
        public string Rarity { get; set; }
    }
}