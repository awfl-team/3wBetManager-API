using System.Threading.Tasks;
using System.Web.Http;
using DAO;
using DAO.Interfaces;
using Models;
using NSubstitute;
using NUnit.Framework;
using _3wBetManager_API.Controllers;

namespace Test.Controller
{
    [TestFixture]
    public class AuthControllerTest
    {
        private AuthController _authController;
        private IUserDao _userDao;
        private User _user;

        [SetUp]
        public void SetUp()
        {
            _authController = new AuthController();
            _userDao = Singleton.Instance.SetUserDao(Substitute.For<IUserDao>());
            _user = new User { Email = "test", Password = "test", Username = "test" };
        }

        [TearDown]
        public void TearDown()
        {
            _userDao.ClearReceivedCalls();
        }

        //[Test]
        public void RegisterTest()
        {
            var register = _authController.Register(_user);
            var calls = _userDao.ReceivedCalls();
            //_userDao.Received().UsernameAndEmailExist(Arg.Any<User>(), out var message);
            _userDao.Received().AddUser(Arg.Any<User>());
            Assert.IsInstanceOf<Task<IHttpActionResult>>(register);
        }
    }
}
