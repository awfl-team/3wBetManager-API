using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
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