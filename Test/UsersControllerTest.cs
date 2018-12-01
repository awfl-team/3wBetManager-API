using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using DAO;
using DAO.Interfaces;
using Models;
using MongoDB.Bson;
using NSubstitute;
using NUnit.Framework;
using _3wBetManager_API.Controllers;

namespace Test
{
    [TestFixture]
    public class UsersControllerTest
    {
        private UsersController _usersController;
        private IUserDao _userDao;
        private User _user;
                

        [SetUp]
        public void SetUp()
        {
            _usersController = new UsersController();
            _userDao = Singleton.Instance.SetUserDao(Substitute.For<IUserDao>());
            _user = new User {Email = "test", Password = "test", Username = "test"};

        }

        [TearDown]
        public void TearDown()
        {
            _userDao.ClearReceivedCalls();
        }

        [Test]
        public void GetAll()
        {
            _usersController.GetAll();
            _userDao.Received().FindAllUser();
            Assert.IsInstanceOf<Task<IHttpActionResult>>(_usersController.GetAll());
        }

        [Test]
        public void Get()
        {
            _usersController.Get("test");
            _userDao.Received().FindUser(Arg.Any<string>());
            Assert.IsInstanceOf<Task<IHttpActionResult>>(_usersController.Get(Arg.Any<string>()));
        }

        [Test]
        public void Login()
        {
       
        }

        [Test]
        public void Register()
        {

        }



    }
}
