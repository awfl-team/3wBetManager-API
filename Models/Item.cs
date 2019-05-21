using MongoDB.Bson;

namespace Models
{
    public class Item
    {
        /* COMMON & TRASH */
        public const string Bomb = "BOMB";
        public const int BombPoints = 30;

        /* RARE */
        public const string MultiplyByTwo = "MULTIPLY_BY_TWO";
        public const string Key = "KEY";
        public const string Mystery = "MYSTERY";

        /* EPIC */
        public const string MultiplyByFive = "MULTIPLY_BY_FIVE";

        /* LEGENDARY */
        public const string MultiplyByTen = "MULTIPLY_BY_TEN";
        public const string LootBox = "LOOT_BOX";
        public const string Life = "LIFE";

        /* RARITY */
        public const string Common = "Common";
        public const string Rare = "Rare";
        public const string Epic = "Epic";
        public const string Legendary = "Legendary";


        public const int MaxLoot = 3;

        /* LOOTBOX DROP CHANCE RATIOS */
        public const float CommonDropChance = 72;
        public const float RareDropChance = 23;
        public const float EpicDropChance = 4;
        public const float LegendaryDropChance = 1;
        public const float RareDropChanceMin = CommonDropChance;
        public const float RareDropChanceMax = CommonDropChance + RareDropChance;
        public const float EpicDropChanceMin = CommonDropChance + RareDropChance;
        public const float EpicDropChanceMax = CommonDropChance + RareDropChance + EpicDropChance;
        public const float LegendaryDropChanceMin = CommonDropChance + RareDropChance + EpicDropChance;

        public const float LegendaryDropChanceMax =
            CommonDropChance + RareDropChance + EpicDropChance + LegendaryDropChance;


        /* MODEL */
        public ObjectId Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Cost { get; set; }
        public string Rarity { get; set; }
    }
}