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
        private List<Item> _lootForLootBox = _items.Take(3).ToList();
        private List<Item> _lootForMystery = _items.Take(1).ToList();

        [OneTimeSetUp]
        public void SetUp()
        {
            _itemManager = Substitute.For<IItemManager>();
            _itemDao = Substitute.For<IItemDao>();
            _betDao = Substitute.For<IBetDao>();
            _userDao = Substitute.For<IUserDao>();
        }

        [OneTimeSetUp]
        public void TearDown()
        {
            _itemDao.ClearReceivedCalls();
            _betDao.ClearReceivedCalls();
            _userDao.ClearReceivedCalls();
        }

        [Test]
        public async Task AssertThatBuyItemsToUserAddItemsAndSpendPoints()
        {
            var totalCost = 0;
            foreach (var item in _items)
            {
                await _userDao.AddUserItem(item, _user);
                totalCost += item.Cost;
            }

            await _userDao.UpdateUserPoints(_user, _user.Point - totalCost,
                _user.TotalPointsUsedToBet);

            Assert.IsTrue(totalCost > 0);
            await _userDao.Received().AddUserItem(Arg.Any<Item>(), Arg.Any<User>());
            await _userDao.Received().UpdateUserPoints(Arg.Any<User>(), Arg.Any<float>(), Arg.Any<int>());
        }

        [Test]
        public async Task AssertThatAddItemsToUserGenerateLootAddToUser()
        {
            _itemManager.GenerateLoot(Item.LootBox).Returns(Task.FromResult(_lootForLootBox));
            var itemsLooted = await _itemManager.GenerateLoot(Item.LootBox);
            foreach (var item in itemsLooted)
            {
                Assert.IsTrue(item.Type != Item.LootBox);
                await _userDao.AddUserItem(item, _user);
            }
            Assert.IsTrue(itemsLooted.Count == Item.MaxLoot);
            await _itemManager.Received().GenerateLoot(Item.LootBox);
            await _userDao.Received().AddUserItem(Arg.Any<Item>(), Arg.Any<User>());
        }

        [Test]
        public async Task AssertThatAddMysteryItemToUseGenerateRandomItemAddItemToUser()
        {
            _itemManager.GenerateLoot(Item.Mystery).Returns(Task.FromResult(_lootForMystery));
            var itemsLooted = await _itemManager.GenerateLoot(Item.Mystery);
            foreach (var item in itemsLooted)
            {
                Assert.IsTrue(item.Type != Item.Mystery);
                await _userDao.AddUserItem(item, _user);
            }
            Assert.IsTrue(itemsLooted.Count == 1);
            await _itemManager.Received().GenerateLoot(Item.Mystery);
            await _userDao.Received().AddUserItem(Arg.Any<Item>(), Arg.Any<User>());
        }

        [Test]
        public async Task AssertThatUseBombFindUserUpdateUserPointsAndReturnsUser()
        {
            _userDao.FindUser("1").Returns(Task.FromResult(_user));
            await _userDao.FindUser("1");
            await _userDao.UpdateUserPoints(_user, _user.Point - 30,
                _user.TotalPointsUsedToBet);
            await _userDao.Received().FindUser(Arg.Any<string>());
            await _userDao.Received().UpdateUserPoints(Arg.Any<User>(), Arg.Any<float>(), Arg.Any<int>());
        }

        [TestCase(10)]
        [TestCase(5)]
        [TestCase(2)]
        public async Task AssertThatUseMultiplierCallsDao(int multiplier)
        {
            await _betDao.UpdateBetMultiply("1", multiplier);
            await _betDao.Received().UpdateBetMultiply(Arg.Any<string>(), Arg.Any<int>());
        }

        [Test]
        public async Task AssertThatCreateDefaultItemsPurgeCollectionAndAddValidItems()
        {
            await _itemDao.PurgeItemCollection();
            await _itemDao.Received().PurgeItemCollection();
            await _itemDao.AddListItem(_items);
            await _itemDao.Received().AddListItem(Arg.Any<List<Item>>());
        }

    }
}