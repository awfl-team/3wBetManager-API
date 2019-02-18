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
    public class UserDaoTest
    {
        private User _user;
        private IMongoCollection<User> _collection;
        private IUserDao _userDao;
        private IMongoDatabase _database;

        [SetUp]
        public void SetUp()
        {
            _collection = Substitute.For<IMongoCollection<User>>();
            _database = Substitute.For<IMongoDatabase>();
            _userDao = new UserDao(_database,_collection);
            _user = new User { Email = "test", Password = "test", Username = "test" };
        }

        [TearDown]
        public void TearDown()
        {
            _collection.ClearReceivedCalls();
        }

        [Test]
        public void FindAllUserTest()
        {
            _userDao.FindAllUser();
            _collection.Received().Find(new BsonDocument());
            Assert.IsInstanceOf<Task<List<User>>>(_userDao.FindAllUser());
        }

        [Test]
        public void FindUserTest()
        {
            _userDao.FindUser("5c06f4b43cd1d72a48b44237");
            _collection.Received().Find(Arg.Any<ExpressionFilterDefinition<User>>());
            Assert.IsInstanceOf<Task<User>>(_userDao.FindUser(Arg.Any<string>()));
        }

        [Test]
        public void FindUserByEmailTest()
        {
            _userDao.FindUserByEmail("email");
            _collection.Received().Find(Arg.Any<ExpressionFilterDefinition<User>>());
            Assert.IsInstanceOf<Task<User>>(_userDao.FindUserByEmail(Arg.Any<string>()));
        }

        [Test]
        public void FindUserByUsernameTest()
        {
            _userDao.FindUserByUsername("username");
            _collection.Received().Find(Arg.Any<ExpressionFilterDefinition<User>>());
            Assert.IsInstanceOf<Task<User>>(_userDao.FindUserByUsername(Arg.Any<string>()));
        }

        [Test]
        public void AddUserTest()
        {
            _userDao.AddUser(_user);
            _collection.Received().InsertOneAsync(Arg.Any<User>());
        }

        [Test]
        public void DeleteUserTest()
        {
            _userDao.DeleteUser("5c06f4b43cd1d72a48b44237");
            _collection.Received()
                .DeleteOneAsync(Arg.Any<ExpressionFilterDefinition<User>>());
        }

        // Need to find solution why when the name is "UpdateUserTest" the test failed in run all 
        [Test]
        public void A()
        {
            _userDao.UpdateUser("5c06f4b43cd1d72a48b44237", _user);
            _collection.Received().UpdateOneAsync(Arg.Any<ExpressionFilterDefinition<User>>(),
                Arg.Any<UpdateDefinition<User>>()
            );
        }

        [Test]
        public void B()
        {
            _userDao.UpdateUserIsPrivate(_user.Id, _user.IsPrivate);
            _collection.Received().UpdateOneAsync(Arg.Any<ExpressionFilterDefinition<User>>(),
                Arg.Any<UpdateDefinition<User>>()
            );
        }

        [Test]
        public void UpdatePoints()
        {
            _userDao.UpdateUserPoints(_user.Id, _user.Point);
            _collection.Received().UpdateOneAsync(Arg.Any<ExpressionFilterDefinition<User>>(),
                Arg.Any<UpdateDefinition<User>>()
            );
        }
    }
}
