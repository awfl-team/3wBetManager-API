using System;
using System.Collections.Generic;
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

        public static async Task AddItemsToUser(List<Item> items, User user)
        {
            foreach (var item in items)
            {
                await Singleton.Instance.UserDao.AddUserItem(item, user);
            }

        }

        public static async Task UseBomb(string userId)
        {
            var user = await Singleton.Instance.UserDao.FindUser(userId);
            await Singleton.Instance.UserDao.UpdateUserPoints(user, (user.Point - 30),
                user.TotalPointsUsedToBet);
        }

        public static async Task UseMultiplier(string betId, int multiply)
        {
            await Singleton.Instance.BetDao.UpdateBetMultiply(betId, multiply);
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
                Type = Item.LootBoxe,
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