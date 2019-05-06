using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DAO;
using DAO.Interfaces;
using Models;

namespace Manager
{
    public class ItemManager : IDisposable
    {
        private IItemDao _itemDao;
        private IUserDao _userDao;
        private IBetDao _betDao;

        public ItemManager(IItemDao itemDao = null, IUserDao userDao = null, IBetDao betDao = null)
        {
            _itemDao = itemDao ?? Singleton.Instance.ItemDao;
            _userDao = userDao ?? Singleton.Instance.UserDao;
            _betDao = betDao ?? Singleton.Instance.BetDao;

        }
        public async Task BuyItemsToUser(List<Item> items, User user)
        {
            var totalCost = 0;
            foreach (var item in items)
            {
                await _userDao.AddUserItem(item, user);
                totalCost += item.Cost;
            }

            await _userDao.UpdateUserPoints(user, (user.Point - totalCost),
                user.TotalPointsUsedToBet);
        }

        public async Task<List<Item>> AddItemsToUser(User user)
        {
            var itemsLooted = await GenerateLoot();
            foreach (var item in itemsLooted)
            {
              await _userDao.AddUserItem(item, user);
            }

            return itemsLooted;
        }

        public async Task<Item> AddMysteryItemToUser(User user)
        {
            var items = await _itemDao.FindAllItems();
            var randomizer = new Random();
            var item = items[randomizer.Next(items.Count)];

            await _userDao.AddUserItem(item, user);

            return item;
        }

        public async Task<User> UseBomb(string userId)
        {
            var user = await _userDao.FindUser(userId);
            await _userDao.UpdateUserPoints(user, (user.Point - 30),
                user.TotalPointsUsedToBet);
            return user;
        }

        public async Task UseMultiplier(string betId, int multiply, User currentUser)
        {
            await _betDao.UpdateBetMultiply(betId, multiply);
        }

        public async Task<List<Item>> GenerateLoot()
        {
            var items = await _itemDao.FindAllItems();
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

        public async Task CreateDefaultItems()
        {
            Console.WriteLine("Purge item collection");
            await _itemDao.PurgeItemCollection();
            var items = new List<Item>();

            /* COMMON AND TRASH */
            var bomb = new Item
            {
                Cost = 50,
                Description = "Can be used on players to make them lose 30 coins per bomb.",
                Name = "Bomb",
                Type = Item.Bomb,
                Rarity = Item.Common
            };
            items.Add(bomb);

            /* RARE */
            var key = new Item
            {
                Cost = 75,
                Description = "Allow you to spy any profile even private ones.",
                Name = "Key",
                Type = Item.Key,
                Rarity = Item.Rare
            };
            items.Add(key);

            var multiplierByTwo = new Item
            {
                Cost = 125,
                Description = "Double your incomes on a bet results.",
                Name = "2x Multiplier",
                Type = Item.MultiplyByTwo,
                Rarity = Item.Rare

            };
            items.Add(multiplierByTwo);

            var mysteryItem = new Item
            {
                Cost = 100,
                Description = "Get a random item of any rarity",
                Name = "Mystery item",
                Type = Item.Mystery,
                Rarity = Item.Rare
            };
            items.Add(mysteryItem);

            /* EPIC */
            var multiplierByFive = new Item
            {
                Cost = 100,
                Description = "Multiply the coins earned on a bet by 5.",
                Name = "5x Multiplier",
                Type = Item.MultiplyByFive,
                Rarity = Item.Epic

            };
            items.Add(multiplierByFive);
                
            /* LEGENDARY */
            var lootBox = new Item
            {
                Cost = 175,
                Description = "Get random items of any rarity",
                Name = "Loot Box",
                Type = Item.LootBox,
                Rarity = Item.Legendary
            };
            items.Add(lootBox);


            var life = new Item
            {
                Cost = 150,
                Description = "Life for reset your account.",
                Name = "Life",
                Type = Item.Life,
                Rarity = Item.Legendary
            };
            items.Add(life);

            var multiplyByTen = new Item
            {
                Cost = 250,
                Description = "Multiply the coins earned on a bet by 10.",
                Name = "x10 Multiplier",
                Type = Item.MultiplyByTen,
                Rarity = Item.Legendary
            };
            items.Add(multiplyByTen);


            Console.WriteLine("Load default item");
            await _itemDao.AddListItem(items);
        }

        public void Dispose()
        {
            _itemDao = null;
            _betDao = null;
            _userDao = null;
        }
    }
}