using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DAO;
using DAO.Interfaces;
using Models;
using MongoDB.Bson;
using MongoDB.Driver;
using NSubstitute;
using NUnit.Framework;

namespace Test.DAO
{
    class ItemDaoTest
    {
        [SetUp]
        public void SetUp()
        {
            _database = Substitute.For<IMongoDatabase>();
            _collectionMatch = Substitute.For<IMongoCollection<Match>>();
            _collectionItem = Substitute.For<IMongoCollection<Item>>();
            _matchDao = new MatchDao(_database, _collectionMatch);
            _itemDao = new ItemDao(_database, _collectionItem);

            _team1 = new Team
            {
                Name = "test",
                Email = "test",
                ShortName = "test",
                Tla = "test",
                CrestUrl = "test",
                Address = "test",
                Phone = "test",
                Colors = "test",
                Venue = "test"
            };
            _team2 = new Team
            {
                Name = "test",
                Email = "test",
                ShortName = "test",
                Tla = "test",
                CrestUrl = "test",
                Address = "test",
                Phone = "test",
                Colors = "test",
                Venue = "test"
            };
            _fullTime = new FullTime { AwayTeam = 1, HomeTeam = 1 };
            _halfTime = new HalfTime { AwayTeam = 1, HomeTeam = 1 };
            _extraTime = new ExtraTime { AwayTeam = 1, HomeTeam = 1 };
            _penalties = new Penalties { AwayTeam = 1, HomeTeam = 1 };
            _score = new Score
            {
                Winner = "test",
                Duration = "test",
                ExtraTime = _extraTime,
                FullTime = _fullTime,
                Penalties = _penalties
            };
        }

        [TearDown]
        public void TearDown()
        {
            _collectionMatch.ClearReceivedCalls();
            _collectionItem.ClearReceivedCalls();
        }

        private Match _match;
        private Item _item;
        private IMongoCollection<Match> _collectionMatch;
        private IMongoCollection<Item> _collectionItem;
        private IMatchDao _matchDao;
        private IItemDao _itemDao;
        private Team _team1;
        private Team _team2;
        private Score _score;
        private FullTime _fullTime;
        public HalfTime _halfTime;
        public ExtraTime _extraTime;
        public Penalties _penalties;
        private IMongoDatabase _database;
        private ExpressionFilterDefinition<Item> _filterExpression;

        [Test]
        public void FindAllItemsTest()
        {
            _itemDao.FindAllItems();
            _collectionItem.Received().Find(new BsonDocument());
            Assert.IsInstanceOf<Task<List<Item>>>(_itemDao.FindAllItems());
        }

        [Test]
        public void FindItemsFilteredTest()
        {
            var itemType = "bomb";
            _itemDao.FindItemsFiltered(itemType);
            _filterExpression = new ExpressionFilterDefinition<Item>(item => item.Type != itemType);
            _collectionItem.Received().Find(_filterExpression);
            Assert.IsInstanceOf<Task<List<Item>>>(_itemDao.FindItemsFiltered(itemType));
        }

        [Test]
        public void FindItemTest()
        {
            var id = Arg.Any<ObjectId>().ToString();
            _itemDao.FindItem(id);
            _filterExpression = new ExpressionFilterDefinition<Item>(i => i.Id == ObjectId.Parse(id));
            _collectionItem.Received().Find(_filterExpression);
            Assert.IsInstanceOf<Task<Item>>(_itemDao.FindItem(id));
        }

        [Test]
        public void AddListItemTest()
        {
            var items = Arg.Any<List<Item>>();
            _itemDao.AddListItem(items);
            _collectionItem.Received().InsertManyAsync(items);
            // Assert.IsInstanceOf<Task<List<Item>>>(_itemDao.AddListItem(items));
        }

    }
}
