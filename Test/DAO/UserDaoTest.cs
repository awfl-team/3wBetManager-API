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
    [TestFixture]
    public class UserDaoTest
    {
        private User _user;
        private List<Item> _items;
        private IMongoCollection<User> _collection;
        private IMongoCollection<Item> _collectionItem;
        private IUserDao _userDao;
        private IItemDao _itemDao;
        private IMongoDatabase _database;
        private ExpressionFilterDefinition<User> _filterExpression;

        [SetUp]
        public void SetUp()
        {
            _collection = Substitute.For<IMongoCollection<User>>();
            _collectionItem = Substitute.For<IMongoCollection<Item>>();
            _database = Substitute.For<IMongoDatabase>();
            _userDao = new UserDao(_database, _collection);
            _itemDao = new ItemDao(_database, _collectionItem);
            _user = new User
            {
                Email = "test", Password = "test", Username = "test", Id = new ObjectId("5c06f4b43cd1d72a48b44237"),
                TotalPointsUsedToBet = 40, Point = 100
            };
            _items = new List<Item>();
            for (int i = 0; i < 2; i++)
            {
                var item = new Item();
                item.Type = Item.Life;
            }
            for (int i = 0; i < 2; i++)
            {
                var item = new Item();
                item.Type = Item.Bomb;
            }
            for (int i = 0; i < 2; i++)
            {
                var item = new Item();
                item.Type = Item.Key;
            }

            _user.Items = _items;

        }

        [TearDown]
        public void TearDown()
        {
            _collection.ClearReceivedCalls();
            _collectionItem.ClearReceivedCalls();
        }

        [Test]
        public void AssertThatFindAllUserIsCalled()
        {
            _userDao.FindAllUser();
            _collection.Received().Find(new BsonDocument());
        }

        [Test]
        public void AssertThatFindUserIsCalled()
        {
            _userDao.FindUser("5c06f4b43cd1d72a48b44237");
            _filterExpression = new ExpressionFilterDefinition<User>(user => user.Id == _user.Id);
            _collection.Received().Find(_filterExpression);
        }

        [Test]
        public void AssertThatOrderUserByPointsIsCalled()
        {
            _userDao.OrderUserByPoint();
            _collection.Received().Find(new BsonDocument());
        }

        [Test]
        public void AssertThatFindAllUsersByPointsIsCalled()
        {
            _userDao.FindAllUserByPoint();
            _collection.Received().Find(new BsonDocument());
        }

        [Test]
        public void AssertThatFindUserByEmailIsCalled()
        {
            _userDao.FindUserByEmail("email");
            _filterExpression = new ExpressionFilterDefinition<User>(user => user.Email == _user.Email);
            _collection.Received().Find(_filterExpression);
        }

        [Test]
        public void AssertThatFindUserByUsernameTestIsCalled()
        {
            _userDao.FindUserByUsername("username");
            _filterExpression = new ExpressionFilterDefinition<User>(user => user.Username == _user.Username);
            _collection.Received().Find(_filterExpression);
        }

        [Test]
        public void AssertThatAddUserIsCalled()
        {
            _userDao.AddUser(_user, User.UserRole);
            _collection.Received().InsertOneAsync(Arg.Any<User>());
        }

        [Test]
        public void AssertThatDeleteUserIsCalled()
        {
            _userDao.DeleteUser("5c06f4b43cd1d72a48b44237");
            _collection.Received()
                .DeleteOneAsync(Arg.Any<ExpressionFilterDefinition<User>>());
        }

        [Test]
        public void AssertThatUpdateUserIsCalled()
        {
            _userDao.UpdateUser("5c06f4b43cd1d72a48b44237", _user);
            _collection.Received().UpdateOneAsync(Arg.Any<ExpressionFilterDefinition<User>>(),
                Arg.Any<UpdateDefinition<User>>()
            );
        }

        [Test]
        public void AssertThatUpdateUserIsPrivateIsCalled()
        {
            _userDao.UpdateUserIsPrivate(_user.Id, _user.IsPrivate);
            _collection.Received().UpdateOneAsync(Arg.Any<ExpressionFilterDefinition<User>>(),
                Arg.Any<UpdateDefinition<User>>()
            );
        }

        [Test]
        public void AssertThatUpdateUserRoleIsCalled()
        {
            _userDao.UpdateUserRole("5c06f4b43cd1d72a48b44237", "ROLE");
            _collection.Received().UpdateOneAsync(Arg.Any<ExpressionFilterDefinition<User>>(),
                Arg.Any<UpdateDefinition<User>>()
            );
        }

        [Test]
        public void AssertThatUpdateUserIsEnabledIsCalled()
        {
            _userDao.UpdateUserIsEnabled(ObjectId.Parse( "5c06f4b43cd1d72a48b44237"), false);
            _collection.Received().UpdateOneAsync(Arg.Any<ExpressionFilterDefinition<User>>(),
                Arg.Any<UpdateDefinition<User>>()
            );
        }

        [Test]
        public void AssertThatUpdateUserPointsIsCalled()
        {
            _userDao.UpdateUserPoints(_user, _user.Point, _user.TotalPointsUsedToBet);
            _collection.Received().UpdateOneAsync(Arg.Any<ExpressionFilterDefinition<User>>(),
                Arg.Any<UpdateDefinition<User>>()
            );
        }

        [Test]
        public void AssertThatUpdateUserAfterBombIsCalled()
        {
            _userDao.UpdateUserPointsAfterBomb(_user);
            _collection.Received().UpdateOneAsync(Arg.Any<ExpressionFilterDefinition<User>>(),
                Arg.Any<UpdateDefinition<User>>()
            );
        }

        [Test]
        public void AssertThatResetUserPointsIsCalled()
        {
            _userDao.ResetUserPoints(_user);
            _collection.Received().UpdateOneAsync(Arg.Any<ExpressionFilterDefinition<User>>(),
                Arg.Any<UpdateDefinition<User>>()
            );
        }

        [Test]
        public void AssertThatUpdateUserLivesIsCalled()
        {
            _userDao.UpdateUserLives(_user);
            _collection.Received().UpdateOneAsync(Arg.Any<ExpressionFilterDefinition<User>>(),
                    Arg.Any<UpdateDefinition<User>>());
        }

        [Test]
        public void AssertThatResetUserItemsIsCalled()
        {
            _itemDao.FindAllItems();
            _collectionItem.Received().Find(new BsonDocument());
            // Todo Foreach test if userDao is called
        }

        [Test]
        public void AssertThatUpdateUserPasswordIsCalled()
        {
            _userDao.UpdateUserPassword(_user);
            _collection.Received().UpdateOneAsync(Arg.Any<ExpressionFilterDefinition<User>>(),
                Arg.Any<UpdateDefinition<User>>());
        }

        [Test]
        public void AssertThatAddUserItemIsCalled()
        {
            _userDao.AddUserItem(new Item(), _user);
            _collection.Received().UpdateOneAsync(Arg.Any<ExpressionFilterDefinition<User>>(),
                Arg.Any<UpdateDefinition<User>>());
        }

        [TestCase(Item.Bomb)]
        [TestCase(Item.Key)]
        [TestCase(Item.Life)]
        public void AssertThatRemoveUserItemIsCalled(string itemType)
        {
            _userDao.RemoveUserItem(_user, itemType);
            _collection.Received().UpdateOneAsync(Arg.Any<ExpressionFilterDefinition<User>>(),
                Arg.Any<UpdateDefinition<User>>());
        }

        [Test]
        public void AssertThatSearchUserIsCalled()
        {
            _userDao.SearchUser("test");
            _filterExpression =
                new ExpressionFilterDefinition<User>(u => u.Email.Contains("test") || u.Username.Contains("test"));
            _collection.Received().Find(_filterExpression);
        }

        [Test]
        public void AssertThatPaginatedUsersIsCalled()
        {
            _userDao.PaginatedUsers(10);
            _collection.Received().Find(new BsonDocument());
        }


    }
}