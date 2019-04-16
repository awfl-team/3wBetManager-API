using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAO;
using Models;

namespace Manager
{
    public class ItemManager
    {
        public static async Task BuyItemsToUser(List<Item> items, User user)
        {
            var totalCost = 0;
            foreach (var item in items)
            {
                await Singleton.Instance.UserDao.AddUserItem(item, user);
                totalCost += item.Cost;
            }

            await Singleton.Instance.UserDao.UpdateUserPoints(user, (user.Point - totalCost),
                user.TotalPointsUsedToBet);
        }

        public static async Task<List<Item>> AddItemsToUser(User user)
        {
            var itemsLooted = await GenerateLoot();
            foreach (var item in itemsLooted)
            {
              await Singleton.Instance.UserDao.AddUserItem(item, user);
            }

            return itemsLooted;
        }

        public static async Task UseBomb(string userId)
        {
            var user = await Singleton.Instance.UserDao.FindUser(userId);
            await Singleton.Instance.UserDao.UpdateUserPoints(user, (user.Point - 30),
                user.TotalPointsUsedToBet);
        }

        public static async Task UseMultiplier(string betId, int multiply, User currentUser)
        {
            await Singleton.Instance.BetDao.UpdateBetMultiply(betId, multiply);
        }

        public static async Task<List<Item>> GenerateLoot()
        {
            var items = await Singleton.Instance.ItemDao.FindAllItems();
            var itemLooted = new List<Item>();
            var randomizer = new Random();

            var commonItems = items.FindAll(i => i.Rarity == Item.Common);
            var rareItems = items.FindAll(i => i.Rarity == Item.Rare);
            var epicItems = items.FindAll(i => i.Rarity == Item.Epic);
            var legendaryItems = items.FindAll(i => i.Rarity == Item.Legendary);

            while (itemLooted.Count < Item.MaxLoot)
            {
                var lootDropChanceFactor = randomizer.NextDouble() * 100;
            
                switch (lootDropChanceFactor)
                {
                    case double dropFactor when (dropFactor > 0 && dropFactor <= Item.CommonDropChance && commonItems.Count > 0):
                        itemLooted.Add(commonItems[randomizer.Next(commonItems.Count)]);
                        break;

                    case double dropFactor when (dropFactor > Item.RareDropChanceMin && dropFactor <= Item.RareDropChanceMax && rareItems.Count > 0):
                        itemLooted.Add(rareItems[randomizer.Next(rareItems.Count)]);
                        break;

                    case double dropFactor when (dropFactor > Item.EpicDropChanceMin && dropFactor <= Item.EpicDropChanceMax && epicItems.Count > 0):
                        itemLooted.Add(epicItems[randomizer.Next(epicItems.Count)]);
                        break;
                    case double dropFactor when (dropFactor > Item.LegendaryDropChanceMin && dropFactor <= Item.LegendaryDropChanceMax && legendaryItems.Count > 0):
                        itemLooted.Add(legendaryItems[randomizer.Next(legendaryItems.Count)]);
                        break;
                }
            }

            return itemLooted;
        }

        public static async Task CreateDefaultItems()
        {
            Console.WriteLine("Purge item collection");
            await Singleton.Instance.ItemDao.PurgeItemCollection();
            var items = new List<Item>();

            var bomb = new Item
            {
                Cost = 50,
                Description = "Can use on players and the target player lose 30 coins",
                Name = "Bomb",
                Type = Item.Bomb,
                Rarity = Item.Rare
            };
            items.Add(bomb);

            var lootBoxe = new Item
            {
                Cost = 10,
                Description = "Random item",
                Name = "Loot Boxe",
                Type = Item.LootBox,
                Rarity = Item.Legendary
            };
            items.Add(lootBoxe);

            var multiply = new Item
            {
                Cost = 100,
                Description = "Multiply the coins earned on a bet by 10",
                Name = "Multiplier",
                Type = Item.Multiply,
                Rarity = Item.Epic
            };
            items.Add(multiply);

            var life = new Item
            {
                Cost = 25,
                Description = "Life for reset account",
                Name = "Life",
                Type = Item.Life,
                Rarity = Item.Common
            };
            items.Add(life);

            var key = new Item
            {
                Cost = 75,
                Description = "Watch on detail any profile even private ",
                Name = "Key",
                Type = Item.Key,
                Rarity = Item.Rare

            };
            items.Add(key);
            Console.WriteLine("Load default item");
            await Singleton.Instance.ItemDao.AddListItem(items);
        }
    }
}