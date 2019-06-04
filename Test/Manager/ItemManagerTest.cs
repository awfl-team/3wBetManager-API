using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAO;
using DAO.Interfaces;
using Manager;
using Manager.Interfaces;
using Models;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using Test.Controller;

namespace Test.Manager
{
    [TestFixture]
    internal class ItemManagerTest
    {
        private IItemManager _itemManager;
        private IBetDao _betDao;
        private IItemDao _itemDao;
        private IUserDao _userDao;
        private static readonly List<User> _users = JsonConvert.DeserializeObject<List<User>>(TestHelper.GetDbResponseByCollectionAndFileName("users"));
        private User _user = _users[0];
        private static readonly List<Bet> _bets = JsonConvert.DeserializeObject<List<Bet>>(TestHelper.GetDbResponseByCollectionAndFileName("bets"));
        private Bet _bet = _bets[0];
        private static readonly List<Item> _items = JsonConvert.DeserializeObject<List<Item>>(TestHelper.GetDbResponseByCollectionAndFileName("items"));
        private Item _item = _items[0];
        private readonly List<Item> _lootForLootBox = _items.Take(3).ToList();
        private readonly List<Item> _lootForMystery = _items.Take(1).ToList();

        [OneTimeSetUp]
        public void SetUp()
        {
            _itemDao = Substitute.For<IItemDao>();
            _betDao = Substitute.For<IBetDao>();
            _userDao = Substitute.For<IUserDao>();
            _itemManager = SingletonManager.Instance.SetItemManager(new ItemManager(_itemDao, _userDao, _betDao));
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _itemDao.ClearReceivedCalls();
            _betDao.ClearReceivedCalls();
            _userDao.ClearReceivedCalls();
        }

        [Test]
        public async Task AssertThatBuyItemsToUserAddItemsAndSpendPoints()
        {
            await _itemManager.BuyItemsToUser(_items, _user);
            await _userDao.Received().AddUserItem(Arg.Any<Item>(), Arg.Any<User>());
            await _userDao.Received().UpdateUserPoints(Arg.Any<User>(), Arg.Any<float>(), Arg.Any<int>());
        }

        [Test]
        public async Task AssertThatAddItemsToUserGenerateLootAddToUser()
        {
            _itemDao.FindItemsFiltered(Item.LootBox).Returns(Task.FromResult(_items));
            _itemManager.GenerateLoot(Item.LootBox).Returns(Task.FromResult(_lootForLootBox));
            var items = await _itemManager.AddItemsToUser(_user);
            Assert.IsTrue(items.Count == 3, "There is 3 items generated");
            Assert.IsTrue(items.All(i => i.Type != Item.LootBox), "No items of type Lootbox");
            await _userDao.Received().AddUserItem(Arg.Any<Item>(), Arg.Any<User>());
        }

        [Test]
        public async Task AssertThatAddMysteryItemToUseGenerateRandomItemAddItemToUser()
        {
            _itemDao.FindItemsFiltered(Item.Mystery).Returns(Task.FromResult(_items));
            _itemManager.GenerateLoot(Item.Mystery).Returns(Task.FromResult(_lootForMystery));
            var item = await _itemManager.AddMysteryItemToUser(_user);
            Assert.IsTrue(item.GetType() == typeof(Item), "There is 3 items generated");
            Assert.IsTrue(item.Type != Item.Mystery, "No items of type Lootbox");
            await _userDao.Received().AddUserItem(Arg.Any<Item>(), Arg.Any<User>());
        }

        [Test]
        public async Task AssertThatUseBombFindUserUpdateUserPointsAndReturnsUser()
        {
            _userDao.FindUser(_user.Id.ToString()).Returns(Task.FromResult(_user));
            await _itemManager.UseBomb(_user.Id.ToString());
            await _userDao.Received().FindUser(Arg.Any<string>());
            await _userDao.Received().UpdateUserPoints(Arg.Any<User>(), Arg.Any<float>(), Arg.Any<int>());
        }

        [TestCase(10)]
        [TestCase(5)]
        [TestCase(2)]
        public async Task AssertThatUseMultiplierCallsDao(int multiplier)
        {
            await _itemManager.UseMultiplier(_bet.Guid, multiplier, _user);
            await _betDao.Received().UpdateBetMultiply(Arg.Any<string>(), Arg.Any<int>());
        }

        [TestCase(Item.Mystery)]
        [TestCase(Item.LootBox)]
        public async Task AssertThatGenerateLootCallsFindItemsFilteredGenerateValidLootByItemType(string itemType)
        {
            _itemDao.FindItemsFiltered(itemType).Returns(_items.FindAll(i => i.Type != itemType));
            var items = await _itemManager.GenerateLoot(itemType);
            switch (itemType)
            {
                case Item.Mystery:
                    Assert.IsTrue(items.Count == 1);
                    break;
                case Item.LootBox:
                    Assert.IsTrue(items.Count == Item.MaxLoot);
                    break;
            }

            Assert.IsTrue(items.All(i => i.Type != itemType));
        }

        [Test]
        public async Task AssertThatCreateDefaultItemsPurgeCollectionAndAddValidItems()
        {
            await _itemManager.CreateDefaultItems();
            await _itemDao.Received().PurgeItemCollection();
            await _itemDao.Received().AddListItem(Arg.Any<List<Item>>());
        }

        [Test]
        public async Task AssertThatGetAllItemsCallsFindAllItems()
        {
            await _itemManager.GetAllItems();
            await _itemDao.Received().FindAllItems();
        }

        [Test]
        public async Task AssertThatChangeItemsCallsUpdateItem()
        {
            await _itemManager.ChangeItem(_item.Id.ToString(), _item);
            await _itemDao.Received().UpdateItem(Arg.Any<string>(), Arg.Any<Item>());
        }
    }
}