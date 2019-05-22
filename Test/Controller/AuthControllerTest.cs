using System.Threading.Tasks;
using System.Web.Http;
using _3wBetManager_API.Controllers;
using DAO.Interfaces;
using Models;
using NUnit.Framework;

namespace Test.Controller
{
    [TestFixture]
    public class AuthControllerTest
    {
        [SetUp]
        public void SetUp()
        {
            _authController = new AuthController();
            _user = new User {Email = "test", Password = "test", Username = "test"};
        }

        [TearDown]
        public void TearDown()
        {
        }

        private AuthController _authController;
        private IUserDao _userDao;
        private User _user;

        [Test]
        public void RegisterTest()
        {
            var register = _authController.Register(_user);
            Assert.IsInstanceOf<Task<IHttpActionResult>>(register);
            // full feur
            // Assert.IsInstanceOf<OkResult>(register); 
        }
    }
}