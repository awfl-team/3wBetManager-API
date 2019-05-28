using System.Collections.Generic;
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
    internal class ItemDaoTest
    {
        private IMongoCollection<Item> _collectionItem;
        private IMongoCollection<Match> _collectionMatch;
        private IMongoDatabase _database;
        private ExtraTime _extraTime;
        private ExpressionFilterDefinition<Item> _filterExpression;
        private FullTime _fullTime;
        private HalfTime _halfTime;
        private Item _item;
        private IItemDao _itemDao;

        private Match _match;
        private IMatchDao _matchDao;
        private Penalties _penalties;
        private Score _score;
        private Team _team1;
        private Team _team2;

        [SetUp]
        public void SetUp()
        {
            _database = Substitute.For<IMongoDatabase>();
            _collectionItem = Substitute.For<IMongoCollection<Item>>();
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
            _fullTime = new FullTime {AwayTeam = 1, HomeTeam = 1};
            _halfTime = new HalfTime {AwayTeam = 1, HomeTeam = 1};
            _extraTime = new ExtraTime {AwayTeam = 1, HomeTeam = 1};
            _penalties = new Penalties {AwayTeam = 1, HomeTeam = 1};
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
            _collectionItem.ClearReceivedCalls();
        }

        [Test]
        public void AssertThatFindAllItemsIsCalled()
        {
            _itemDao.FindAllItems();
            _collectionItem.Received().Find(new BsonDocument());
            Assert.IsInstanceOf<Task<List<Item>>>(_itemDao.FindAllItems());
        }

        [TestCase(Item.Bomb)]
        [TestCase(Item.Key)]
        [TestCase(Item.Life)]
        public void AssertThatFindItemsFilteredIsCalled(string itemType)
        {
            _itemDao.FindItemsFiltered(itemType);
            _filterExpression = new ExpressionFilterDefinition<Item>(item => item.Type != itemType);
            _collectionItem.Received().Find(_filterExpression);
        }

        [Test]
        public void AssertThatFindItemIsCalled()
        {
            var id = "1";
            _itemDao.FindItem(id);
            _filterExpression = new ExpressionFilterDefinition<Item>(i => i.Id == ObjectId.Parse(id));
            _collectionItem.Received().Find(_filterExpression);
        }

        [Test]
        public async Task AddListItemTest()
        {
            var items = new List<Item>();
            await _itemDao.AddListItem(items);
            await _collectionItem.Received().InsertManyAsync(Arg.Any<List<Item>>(), Arg.Any<InsertManyOptions>());
        }

        [Test]
        public void AssertThatAddListItemIsCalled()
        {
            var itemsToAdd = new List<Item>();
            for (int i = 0; i < 3; i++)
            {
                itemsToAdd.Add(new Item());
            }
            _itemDao.AddListItem(itemsToAdd);
            _collectionItem.Received().InsertManyAsync(itemsToAdd);
        }

        [Test]
        public void AssertThatPurgeItemCollectionIsCalled()
        {
            _itemDao.PurgeItemCollection();
            _collectionItem.Received().DeleteManyAsync(Arg.Any<ExpressionFilterDefinition<Item>>());
        }

        [Test]
        public void AssertThatUpdateItemIsCalled()
        {
            var id = "1";
            var item = new Item {Cost = 10, Rarity = Item.Legendary};

            _itemDao.UpdateItem(id, new Item());
            _collectionItem.Received().UpdateOneAsync(Arg.Any<ExpressionFilterDefinition<Item>>(), 
                Arg.Any<UpdateDefinition<Item>>());
        }
    }
}