using System.Collections.Generic;
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
        private IMongoDatabase _database;
        private ExpressionFilterDefinition<Item> _filterExpression;

        private IItemDao _itemDao;

        [SetUp]
        public void SetUp()
        {
            _database = Substitute.For<IMongoDatabase>();
            _collectionItem = Substitute.For<IMongoCollection<Item>>();
            _itemDao = new ItemDao(_database, _collectionItem);
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
            for (var i = 0; i < 3; i++) itemsToAdd.Add(new Item());
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

            _itemDao.UpdateItem(id, new Item());
            _collectionItem.Received().UpdateOneAsync(Arg.Any<ExpressionFilterDefinition<Item>>(),
                Arg.Any<UpdateDefinition<Item>>());
        }
    }
}