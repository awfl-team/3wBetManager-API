using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAO.Interfaces;
using Manager;
using Manager.Interfaces;
using Models;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using NSubstitute;
using NUnit.Framework;
using Test.Controller;
using JsonConvert = Newtonsoft.Json.JsonConvert;

namespace Test.Manager
{
    [TestFixture]
    internal class UsersManagerTest
    {
        private IUserDao _userDao;
        private IBetDao _betDao;
        private IUserManager _userManager;
        private static readonly List<User> _users = JsonConvert.DeserializeObject<List<User>>(TestHelper.GetDbResponseByCollectionAndFileName("users"));
        private static readonly List<Bet> _betsByUser = JsonConvert.DeserializeObject<List<Bet>>(TestHelper.GetDbResponseByCollectionAndFileName("betsByUser"));
        private static readonly List<User> _usersOrderedByPoints = JsonConvert.DeserializeObject<List<User>>(TestHelper.GetDbResponseByCollectionAndFileName("users"));
        private static User _user = _users[0];
        private static readonly object[] UserEmailUsernameMessage =
        {
            new object[] { "", null, null},
            new object[] { "email already taken", null, _user},
            new object[] { "username already taken", _user, null},
            new object[] { "username and email already taken", _user, _user},
        };

        private static User _user1 = new User
            {
                Id = ObjectId.Parse("5c5b5d019922d50db047193e"),
                Point = 30,
                Role = "ADMIN",
                Password = "aaaaaaaaaaaaaaaa",
                Email = "alexis-60@hotmail.fr",
                Username = "gubs",
                IsPrivate = false,
                TotalPointsUsedToBet = 20,
                Items = new List<Item>()
        };

        private static User _user2 = new User
        {
            Id = ObjectId.Parse("5c5b5d019922220db047193e"),
            Point = 30,
            Role = "ADMIN",
            Password = "aaaaaaaaaaaaaaaa",
            Email = "alexis-60@hotmail.fr",
            Username = "gubs",
            IsPrivate = false,
            TotalPointsUsedToBet = 20,
            Items = new List<Item>()
        };

        [OneTimeSetUp]
        public void SetUp()
        {
            _userDao = Substitute.For<IUserDao>();
            _betDao = Substitute.For<IBetDao>();
            _userManager = SingletonManager.Instance.SetUserManager(new UserManager(_userDao, _betDao));
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _userDao.ClearReceivedCalls();
            _betDao.ClearReceivedCalls();
        }

        [Test]
        public async Task AssertThatUsernameAndEmailExistCallsFindUserByEmailFindUserByUsername()
        {
            await _userManager.UsernameAndEmailExist(_user);
            await _userDao.Received().FindUserByEmail(_user.Email);
           await _userDao.Received().FindUserByUsername(_user.Username);
        }

        [TestCaseSource("UserEmailUsernameMessage")]
        public async Task AssertThatUsernameAndEmailExistReturnsMessage(string message, User userFoundByUsername = null, User userFoundByEmail = null)
        {
            _userDao.FindUserByEmail(_user.Email).Returns(Task.FromResult(userFoundByEmail));
            _userDao.FindUserByUsername(_user.Username).Returns(Task.FromResult(userFoundByUsername));

            var userExists = await _userManager.UsernameAndEmailExist(_user);

            Assert.IsTrue(message == userExists );
        }

        [Test]
        public async Task AssertThatCanUpdateCallsReturnsMessage()
        {
            var message = "username and email already taken";

            var listOfUsersWithId = new List<User>();
            listOfUsersWithId.Add(_user1);
            listOfUsersWithId.Add(_user2);
            _userDao.FindAllUser().Returns(Task.FromResult(listOfUsersWithId));
           var result = await _userManager.CanUpdate(_user1.Id.ToString(), _user1);
           await _userDao.Received().FindAllUser();

            Assert.IsTrue(message == result);
        }

        [Test]
        public void AssertThatRemoveUserFromListWorks()
        {
            var listOfUsersWithId = new List<User>();
            listOfUsersWithId.Add(_user1);
            listOfUsersWithId.Add(_user2);
            var result = _userManager.RemoveUserFromList(listOfUsersWithId, _user1.Id.ToString());
            Assert.IsInstanceOf<List<User>>(result);
            Assert.IsTrue(listOfUsersWithId.Count < 2);
        }

        [Test]
        public async Task AssertThatGetBestBettersReturnsLessThan50UsersAndWorkingProperly()
        {
            var listOfUsersWithId = new List<User>();
            listOfUsersWithId.Add(_user1);
            listOfUsersWithId.Add(_user2);
            _userDao.OrderUserByPoint().Returns(Task.FromResult(listOfUsersWithId));
            _betDao.FindBetsByUser(Arg.Any<User>()).Returns(Task.FromResult(_betsByUser));
            var result = await _userManager.GetBestBetters();
            await _userDao.Received().OrderUserByPoint();
            await _betDao.Received().FindBetsByUser(Arg.Any<User>());
            Assert.IsTrue(result.Count < 50);
        }

        [Test]
        public async Task AssertThatGetUserPositionAmongSiblingsCalls()
        {
            _userDao.FindAllUserByPoint().Returns(Task.FromResult(_users));
            await _userManager.GetUserPositionAmongSiblings(_user);
            await _userDao.Received().FindAllUserByPoint();
            
        }

        [Test]
        public async Task AssertThatGetTop3CallsOrderUserByPointAndReturnsThreeValidDynamicUsers()
        {
            _userDao.OrderUserByPoint().Returns(Task.FromResult(_usersOrderedByPoints));
            _betDao.FindBetsByUser(Arg.Any<User>()).Returns(Task.FromResult(_betsByUser));

            var top3 = await _userManager.GetTop3();
            await _userDao.Received().OrderUserByPoint();
            await _betDao.Received().FindBetsByUser(Arg.Any<User>(), Arg.Any<int>());
            Assert.IsTrue(top3.Count <=3);
            Assert.IsTrue(top3.All(t => t.Life != null), "No life in top3");
            Assert.IsTrue(top3.All(t => t.Id != null), "Id not valid in top3");
            Assert.IsTrue(top3.All(t => t.Point != null), "Points not valid in top3");
            Assert.IsTrue(top3.All(t => t.Username is string), "Username not valid in top3");
            Assert.IsTrue(top3.All(t => t.IsPrivate is bool), "IsPrivate not valid in top3");
            Assert.IsTrue(top3.All(t => t.NbBets != null), "Nb bets not Valid in in top3");
        }

        [Test]
        public async Task AssertThatRecalculateUserPointsCallsFindAllUserFindBetsByUserAndUpdateUserPoints()
        {
            _userDao.FindAllUser().Returns(Task.FromResult(_users));
            _betDao.FindBetsByUser(_user).Returns(Task.FromResult(_betsByUser));

            _userManager.RecalculateUserPoints();
            await _userDao.Received().FindAllUser();
            await _betDao.Received().FindBetsByUser(Arg.Any<User>(), Arg.Any<int>());
            await _userDao.Received().UpdateUserPoints(Arg.Any<User>(), Arg.Any<float>(), Arg.Any<int>());

        }

        [Test]
        public async Task AssertThatGetAllUsersPaginatedCallsFindAllUserPaginatedUsersAndReturnsAValidObject()
        {
            _userDao.FindAllUser().Returns(Task.FromResult(_users));

            await _userManager.GetAllUsersPaginated(1);
            await _userDao.Received().PaginatedUsers(Arg.Any<int>());

        }

        [Test]
        public async Task AssertThatResetUserCallsResetUserPointsItemsLivesAndDeleteUsersBets()
        {
            await _userManager.ResetUser(_user);
            await _userDao.Received().ResetUserPoints(_user);
            await _userDao.Received().ResetUserItems(_user);
            await _userDao.Received().UpdateUserLives(_user);
            _betDao.Received().DeleteBetsByUser(_user.Id);
        }

        [Test]
        public async Task AssertThatGetUserCoinsStatsCallsFindBetsByUser()
        {
            _betDao.FindBetsByUser(_user).Returns(Task.FromResult(_betsByUser));

            await _userManager.GetUserCoinStats(_user);
            await _betDao.Received().FindBetsByUser(Arg.Any<User>(), Arg.Any<int>());
        }

        [Test]
        public async Task AssertThatGetUserByEmailsCallsFindUserByEmail()
        {
            _userDao.FindUserByEmail(_user.Email).Returns(Task.FromResult(_user));
            await _userManager.GetUserByEmail(_user.Email);
            await _userDao.Received().FindUserByEmail(Arg.Any<string>());
        }

        [TestCase(User.UserRole)]
        [TestCase(User.AdminRole)]
        public async Task AssertThatAddUserCallsAddUserAndDefineRole(string userRole)
        {
            await _userManager.AddUser(_user, userRole);
            await _userDao.Received().AddUser(Arg.Any<User>(), Arg.Any<string>());
        }

        [Test]
        public async Task AssertThatChangePasswordCallsUpdateUserPassword()
        {
            await _userManager.ChangePassword(_user);
            await _userDao.Received().UpdateUserPassword(Arg.Any<User>());
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task AssertThatChangeIsEnabledCallsUpdateUseIsEnabled(bool isEnable)
        {
            await _userManager.ChangeIsEnabled(_user.Id, isEnable);
            await _userDao.Received().UpdateUserIsEnabled(Arg.Any<ObjectId>(), Arg.Any<bool>());
        }

        [Test]
        public async Task AssertThatChangeUserPointsCallsUpdateUserPoints()
        {
            await _userManager.ChangeUserPoint(_user, _user.Point, _user.TotalPointsUsedToBet);
            await _userDao.Received().UpdateUserPoints(Arg.Any<User>(), Arg.Any<float>(), Arg.Any<int>());
        }

        [Test]
        public async Task AssertThatGetUserCallsFindUser()
        {
            await _userManager.GetUser(_user.Id.ToString());
            await _userDao.Received().FindUser(Arg.Any<string>());
        }

        [TestCase(Item.Mystery)]
        [TestCase(Item.LootBox)]
        [TestCase(Item.Bomb)]
        [TestCase(Item.Key)]
        [TestCase(Item.MultiplyByFive)]
        [TestCase(Item.MultiplyByTwo)]
        [TestCase(Item.MultiplyByTen)]
        [TestCase(Item.Life)]
        public async Task AssertThatDeleteUserItemsCallsRemoveUserItem(string itemType)
        {
            await _userManager.DeleteUserItem(_user, itemType);
            await _userDao.Received().RemoveUserItem(Arg.Any<User>(), Arg.Any<string>());
        }

        [Test]
        public async Task AssertThatGetAllUserCallsFindAllUser()
        {
            await _userManager.GetAllUser();
            await _userDao.Received().FindAllUser();
        }

        [Test]
        public async Task AssertThatChangeUserCallsUpdateUser()
        {
            await _userManager.ChangeUser(_user.Id.ToString(), _user);
            await _userDao.Received().UpdateUser(Arg.Any<string>(), Arg.Any<User>());
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task AssertThatChangeUserIsPrivateCallsUpdateUserIsPrivate(bool isPrivate)
        {
            await _userManager.ChangeUserIsPrivate(_user.Id, isPrivate);
            await _userDao.Received().UpdateUserIsPrivate(Arg.Any<ObjectId>(), Arg.Any<bool>());
        }

        [TestCase(User.AdminRole)]
        [TestCase(User.UserRole)]
        public async Task AssertThatChangeUserRoleCallsUpdateUserRole(string role)
        {
            await _userManager.ChangeUserRole(_user.Id.ToString(), role);
            await _userDao.Received().UpdateUserRole(Arg.Any<string>(), Arg.Any<string>());
        }

        [Test]
        public async Task AssertThatDeleteUserCallsDeleteUser()
        {
            await _userManager.DeleteUser(_user.Id.ToString());
            await _userDao.Received().DeleteUser(Arg.Any<string>());
        }

        [TestCase("fake@email.fr")]
        [TestCase("fakeUsername")]
        public async Task AssertThatSearchUserCallsSearchUser(string value)
        {
            await _userManager.SearchUser(value);
            await _userDao.Received().SearchUser(Arg.Any<string>());
        }
    }
}