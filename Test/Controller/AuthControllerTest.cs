using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using _3wBetManager_API.Controllers;
using DAO.Interfaces;
using Manager;
using Manager.Interfaces;
using Microsoft.Owin;
using Models;
using MongoDB.Bson;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;

namespace Test.Controller
{
    [TestFixture]
    internal class AuthControllerTest : BaseController
    {

        private AuthController _authController;
        private IUserManager _userManager;
        private static List<User> _users = JsonConvert.DeserializeObject<List<User>>(TestHelper.GetDbResponseByCollectionAndFileName("users"));
        private User _user = _users[0];

        private static TokenManager _tokenManager;
        private Dictionary<string, object> _data;
        private OwinContext _context;
        private AuthenticationHeaderValue _authHeader;
        private HttpRequestMessage _httpRequestGet = new HttpRequestMessage(HttpMethod.Get, "http://localhost:9000/");
        private HttpRequestMessage _httpRequestPost = new HttpRequestMessage(HttpMethod.Post, "http://localhost:9000/");
        private HttpRequestMessage _httpRequestPut = new HttpRequestMessage(HttpMethod.Put, "http://localhost:9000/");

        private readonly string _ip = "127.0.0.1";
        private readonly string _token =
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6Imx1Y2FzYm91cmdlb2lzNjBAaG90bWFpbC5mciIsInJvbGUiOiJBRE1JTiIsInVuaXF1ZV9uYW1lIjoibGJvIiwibmJmIjoxNTU4NTMyMDI3LCJleHAiOjE1OTAxNTQ0MjcsImlhdCI6MTU1ODUzMjAyN30.a3Co739HOGU5cBmziUdOt6-YuzLau0JVfW0gj5khonQ";


        [OneTimeSetUp]
        public void SetUp()
        {
            _data = new Dictionary<string, object>() { { "Authorization", _token } };
            _authController = new AuthController() { Configuration = new HttpConfiguration() };
            _context = new OwinContext(_data);
            _authHeader = new AuthenticationHeaderValue(_token);
            _userManager = SingletonManager.Instance.SetUserManager(Substitute.For<IUserManager>());
            SingletonManager.Instance.SetTokenManager(new TokenManager());

        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _userManager.ClearReceivedCalls();
        }

        [Test]
        public async Task AssertThatRegisterCallsManager()
        {
            InitRequestHelper(HttpMethod.Post.Method);
            _authController.Request.Content = new StringContent(_user.ToJson(), Encoding.UTF8, "application/json");
            var action = await _authController.Register(_user);
            await _userManager.Received().UsernameAndEmailExist(Arg.Any<User>());
            await _userManager.Received().AddUser(Arg.Any<User>(), Models.User.UserRole);
        }

        [TestCase("email already taken")]
        public async Task AssertThatRegisterReturnsValidError(string expectedMessage)
        {
            InitRequestHelper(HttpMethod.Post.Method);
            _authController.Request.Content = new StringContent(_user.ToJson(), Encoding.UTF8, "application/json");
            _userManager.UsernameAndEmailExist(_user).Returns(expectedMessage);
            var action = await _authController.Register(_user);
            var response = await action.ExecuteAsync(new CancellationToken());
            Assert.False(response.StatusCode == HttpStatusCode.InternalServerError, "InternalServerError is thrown");
            Assert.IsTrue(response.StatusCode == HttpStatusCode.BadRequest);
        }

        [TestCase("")]
        public async Task AssertThatRegisterReturnsValidResponse(string expectedMessage)
        {
            InitRequestHelper(HttpMethod.Post.Method);
            _authController.Request.Content = new StringContent(_user.ToJson(), Encoding.UTF8, "application/json");
            _userManager.UsernameAndEmailExist(_user).Returns(expectedMessage);
            var action = await _authController.Register(_user);
            var response = await action.ExecuteAsync(new CancellationToken());
            Assert.IsTrue(response.StatusCode == HttpStatusCode.Created, "Status code is valid");
        }

        [Test]
        public async Task AssertThatLoginReturnsAValidResponseCodeAndCallsManager()
        {
            InitRequestHelper(HttpMethod.Post.Method);
            _authController.Request.Content = new StringContent(_user.ToJson(), Encoding.UTF8, "application/json");
            var action = await _authController.Login(_user);
            var response = await action.ExecuteAsync(new CancellationToken());
            await _userManager.Received().GetUserByEmail(Arg.Any<string>());
            Assert.False(response.StatusCode == HttpStatusCode.InternalServerError, "InternalServerError is thrown");
            Assert.IsTrue(response.StatusCode == HttpStatusCode.BadRequest
                          || response.StatusCode == HttpStatusCode.OK
                , "Status code is valid");
        }

        [Test]
        public async Task AssertThatForgotPasswordReturnsAValidResponseCodeAndCallsManager()
        {
            InitRequestHelper(HttpMethod.Post.Method);
            _authController.Request.Content = new StringContent(_user.ToJson(), Encoding.UTF8, "application/json");
            var action = await _authController.ForgotPassword(_user);
            var response = await action.ExecuteAsync(new CancellationToken());
            await _userManager.Received().GetUserByEmail(Arg.Any<string>());
            Assert.False(response.StatusCode == HttpStatusCode.InternalServerError, "InternalServerError is thrown");
            Assert.IsTrue(response.StatusCode == HttpStatusCode.NotFound
                          || response.StatusCode == HttpStatusCode.OK
                , "Status code is valid");
        }

        [Test]
        public async Task AssertThatResetPasswordReturnsAValidResponseCodeAndCallsManager()
        {
            InitRequestHelper(HttpMethod.Put.Method);
            _authController.Request.Content = new StringContent(_user.ToJson(), Encoding.UTF8, "application/json");
            GetUserByToken(_authController.Request).Returns(Task.FromResult(_user));
            var action = await _authController.ResetPassword(_user);
            var response = await action.ExecuteAsync(new CancellationToken());
            await _userManager.Received().ChangePassword(Arg.Any<User>());
            Assert.False(response.StatusCode == HttpStatusCode.InternalServerError, "InternalServerError is thrown");
            Assert.IsTrue(response.StatusCode == HttpStatusCode.NoContent
                , "Status code is valid");
        }

        [Test]
        public async Task AssertThatVerifyAccountReturnsAValidResponseCodeAndCallsManager()
        {
            InitRequestHelper(HttpMethod.Put.Method);
            _authController.Request.Content = new StringContent(_user.ToJson(), Encoding.UTF8, "application/json");
            GetUserByToken(_authController.Request).Returns(Task.FromResult(_user));
            var action = await _authController.VerifyAccount(_user);
            var response = await action.ExecuteAsync(new CancellationToken());
            await _userManager.Received().ChangeIsEnabled(Arg.Any<ObjectId>(), true);
            Assert.False(response.StatusCode == HttpStatusCode.InternalServerError, "InternalServerError is thrown");
            Assert.IsTrue(response.StatusCode == HttpStatusCode.BadRequest
                          || response.StatusCode == HttpStatusCode.NoContent
                , "Status code is valid");
        }

        private void InitRequestHelper(string verb)
        {
            switch (verb)
            {
                case "POST":
                    _authController.Request = _httpRequestPost;
                    break;
                case "PUT":
                    _authController.Request = _httpRequestPut;
                    break;
                default:
                    _authController.Request = _httpRequestGet;
                    break;
            }

            _authController.Request.SetOwinContext(_context);
            _authController.Request.GetOwinContext().Request.RemoteIpAddress = _ip;
            _authController.Request.Headers.Authorization = _authHeader;
        }
    }
}