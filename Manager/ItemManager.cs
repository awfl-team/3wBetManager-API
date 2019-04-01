using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DAO;
using Models;

namespace Manager
{
    public class ItemManager
    {
        public static async Task AddListItemsToUser(List<Item> items, User user)
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
                Type = Item.Bomb
            };
            items.Add(bomb);

            var lootBoxe = new Item
            {
                Cost = 10,
                Description = "Random item",
                Name = "Loot Boxe",
                Type = Item.LootBoxe
            };
            items.Add(lootBoxe);

            var multiply = new Item
            {
                Cost = 100,
                Description = "Multiply the coins earned on a bet by 10",
                Name = "Multiplier",
                Type = Item.Multiply
            };
            items.Add(multiply);

            var life = new Item
            {
                Cost = 25,
                Description = "Life for reset account",
                Name = "Life",
                Type = Item.Life
            };
            items.Add(life);

            var key = new Item
            {
                Cost = 75,
                Description = "Watch on detail any profile even private ",
                Name = "Key",
                Type = Item.Key
            };
            items.Add(key);
            Console.WriteLine("Load default item");
            await Singleton.Instance.ItemDao.AddListItem(items);
        }
    }
}