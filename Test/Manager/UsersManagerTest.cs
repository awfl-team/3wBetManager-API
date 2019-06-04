using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAO.Interfaces;
using Manager;
using Manager.Interfaces;
using Models;
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
        private static User _user = _users[0];
        private static readonly object[] UserEmailUsernameMessage =
        {
            new object[] { "", null, null},
            new object[] { "email already taken", null, _user},
            new object[] { "username already taken", _user, null},
            new object[] { "username and email already taken", _user, _user},
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

        [TestCaseSource("UserEmailUsernameMessage")]
        public async Task AssertThatUsernameAndEmailExistCallsFindUserByEmailFindUserByUsernameAndReturnMessage(string message, User userFoundByUsername = null, User userFoundByEmail = null)
        {
            _userDao.FindUserByEmail(_user.Email).Returns(Task.FromResult(userFoundByEmail));
            _userDao.FindUserByUsername(_user.Username).Returns(Task.FromResult(userFoundByUsername));

            var userExists = await _userManager.UsernameAndEmailExist(_user);

            Assert.IsTrue(message == userExists );
        }


        [Test]
        public async Task AssertThatCanUpdateCalls()
        {

        }

        [Test]
        public async Task AssertThatGetBestBettersCalls()
        {

        }

        [Test]
        public async Task AssertThatGetUserPositionAmongSiblingsCalls()
        {

        }

        [Test]
        public async Task AssertThatGetTop3Calls()
        {

        }

        [Test]
        public async Task AssertThatRecalculateUserPointsCalls()
        {

        }

        [Test]
        public async Task AssertThatGetAllUsersPaginatedCalls()
        {

        }

        [Test]
        public async Task AssertThatResetUserCalls()
        {

        }

        [Test]
        public async Task AssertThatGetUserCoinsStatsCalls()
        {
            //TODO -> MOVE TO StatsManagerTest
        }

        [Test]
        public async Task AssertThatGetUserByEmailsCallsFind()
        {

        }

        [Test]
        public async Task AssertThatAddUserCalls()
        {

        }

        [Test]
        public async Task AssertThatChangePasswordCalls()
        {

        }

        [Test]
        public async Task AssertThatChangeIsEnabledCalls()
        {

        }

        [Test]
        public async Task AssertThatChangeUserPointsCalls()
        {

        }

        [Test]
        public async Task AssertThatGetUserCalls()
        {

        }

        [Test]
        public async Task AssertThatDeleteUserItemsCalls()
        {

        }

        [Test]
        public async Task AssertThatGetAllUserCalls()
        {

        }

        [Test]
        public async Task AssertThatChangeUserCalls()
        {

        }

        [Test]
        public async Task AssertThatChangeUserIsPrivateCalls()
        {

        }

        [Test]
        public async Task AssertThatChangeUserRoleCalls()
        {

        }

        [Test]
        public async Task AssertThatDeleteUserCalls()
        {

        }

        [Test]
        public async Task AssertThatSearchUserCalls()
        {

        }
    }
}