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
        private IMongoCollection<User> _collection;
        private IUserDao _userDao;
        private IMongoDatabase _database;
        private ExpressionFilterDefinition<User> _filterExpression;

        [SetUp]
        public void SetUp()
        {
            _collection = Substitute.For<IMongoCollection<User>>();
            _database = Substitute.For<IMongoDatabase>();
            _userDao = new UserDao(_database,_collection);
            _user = new User { Email = "test", Password = "test", Username = "test", Id = new ObjectId("5c06f4b43cd1d72a48b44237"), TotalPointsUsedToBet = 40, Point = 100};
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
            _filterExpression = new ExpressionFilterDefinition<User>(user => user.Id == _user.Id);
            _collection.Received().Find(_filterExpression);
            Assert.IsInstanceOf<Task<User>>(_userDao.FindUser(Arg.Any<string>()));
        }

        [Test]
        public void FindUserByEmailTest()
        {
            _userDao.FindUserByEmail("email");
            _filterExpression = new ExpressionFilterDefinition<User>(user => user.Email == _user.Email);
            _collection.Received().Find(_filterExpression);
            Assert.IsInstanceOf<Task<User>>(_userDao.FindUserByEmail(Arg.Any<string>()));
        }

        [Test]
        public void FindUserByUsernameTest()
        {
            _userDao.FindUserByUsername("username");
            _filterExpression = new ExpressionFilterDefinition<User>(user => user.Username == _user.Username);
            _collection.Received().Find(_filterExpression);
            Assert.IsInstanceOf<Task<User>>(_userDao.FindUserByUsername(Arg.Any<string>()));
        }

        [Test]
        public void AddUserTest()
        {
            _userDao.AddUser(_user,  User.UserRole);
            _collection.Received().InsertOneAsync(Arg.Any<User>());
        }

        [Test]
        public void DeleteUserTest()
        {
            _userDao.DeleteUser("5c06f4b43cd1d72a48b44237");
            _collection.Received()
                .DeleteOneAsync(Arg.Any<ExpressionFilterDefinition<User>>());
        }

        [Test]
        public void UpdateUserLivesTest()
        {
            _userDao.UpdateUserLives(_user);
            _collection.Received().UpdateOneAsync(Arg.Any<ExpressionFilterDefinition<User>>(),
                Arg.Any<UpdateDefinition<User>>()
            );
        }

        [Test]
        public void FindAllUsersByPoints()
        {
            _userDao.FindAllUserByPoint();
            _collection.Received().Find(new BsonDocument());
            Assert.IsInstanceOf<Task<List<User>>>(_userDao.FindAllUserByPoint());
        }

        [Test]
        public void UpdateUserTest()
        {
            _userDao.UpdateUser("5c06f4b43cd1d72a48b44237", _user);
            _collection.Received().UpdateOneAsync(Arg.Any<ExpressionFilterDefinition<User>>(),
                Arg.Any<UpdateDefinition<User>>()
            );
        }

        [Test]
        public void UpdateUserIsPrivateTest()
        {
            _userDao.UpdateUserIsPrivate(_user.Id, _user.IsPrivate);
            _collection.Received().UpdateOneAsync(Arg.Any<ExpressionFilterDefinition<User>>(),
                Arg.Any<UpdateDefinition<User>>()
            );
        }

        [Test]
        public void UpdateUserPointsTest()
        {
            _userDao.UpdateUserPoints(_user, _user.Point ,_user.TotalPointsUsedToBet);
            _collection.Received().UpdateOneAsync(Arg.Any<ExpressionFilterDefinition<User>>(),
                Arg.Any<UpdateDefinition<User>>()
            );
        }

        [Test]
        public void ResetUserTest()
        {
            _userDao.ResetUserPoints(_user);
            _collection.Received().UpdateOneAsync(Arg.Any<ExpressionFilterDefinition<User>>(),
                Arg.Any<UpdateDefinition<User>>()
            );
        }

        [Test]
        public void UpdateUserRoleTest()
        {
            _userDao.UpdateUserRole("5c06f4b43cd1d72a48b44237", "ROLE");
            _collection.Received().UpdateOneAsync(Arg.Any<ExpressionFilterDefinition<User>>(),
                Arg.Any<UpdateDefinition<User>>()
            );
        }


        [Test]
        public void OrderUserByPointsTest()
        {
            _userDao.OrderUserByPoint();
            _collection.Received().Find(new BsonDocument());
            Assert.IsInstanceOf<Task<List<User>>>(_userDao.OrderUserByPoint());
        }

        [Test]
        public void SearchUserTest()
        {
            _userDao.SearchUser("test");
            _filterExpression = new ExpressionFilterDefinition<User>(u => u.Email.Contains("test") || u.Username.Contains("test"));
            _collection.Received().Find(_filterExpression);
            Assert.IsInstanceOf<Task<List<User>>>(_userDao.SearchUser(Arg.Any<string>()));
        }

        [Test]
        public void PaginatedUsersTest()
        {
            _userDao.PaginatedUsers(10);
            _collection.Received().Find(new BsonDocument());
            Assert.IsInstanceOf<Task<List<User>>>(_userDao.PaginatedUsers(10));
        }
    }
}
