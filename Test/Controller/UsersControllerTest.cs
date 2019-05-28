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
    internal class UsersControllerTest : BaseController
    {
        private UsersController _usersController;
        private IUserManager _userManager;
        private static List<User> _users = JsonConvert.DeserializeObject<List<User>>(TestHelper.GetDbResponseByCollectionAndFileName("user", "users"));
        private User _user = _users[0];

        private Dictionary<string, object> _data;
        private OwinContext _context;
        private AuthenticationHeaderValue _authHeader;
        private readonly HttpRequestMessage _httpRequestGet = new HttpRequestMessage(HttpMethod.Get, "http://localhost:9000/");
        private readonly HttpRequestMessage _httpRequestPost = new HttpRequestMessage(HttpMethod.Post, "http://localhost:9000/");
        private readonly HttpRequestMessage _httpRequestPut = new HttpRequestMessage(HttpMethod.Put, "http://localhost:9000/");
        private readonly HttpRequestMessage _httpRequestDelete = new HttpRequestMessage(HttpMethod.Delete, "http://localhost:9000/");

        private readonly string _ip = "127.0.0.1";
        private readonly string _token =
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6Imx1Y2FzYm91cmdlb2lzNjBAaG90bWFpbC5mciIsInJvbGUiOiJBRE1JTiIsInVuaXF1ZV9uYW1lIjoibGJvIiwibmJmIjoxNTU4NTMyMDI3LCJleHAiOjE1OTAxNTQ0MjcsImlhdCI6MTU1ODUzMjAyN30.a3Co739HOGU5cBmziUdOt6-YuzLau0JVfW0gj5khonQ";


        [OneTimeSetUp]
        public void SetUp()
        {
            _data = new Dictionary<string, object>() { { "Authorization", _token } };
            _usersController = new UsersController() { Configuration = new HttpConfiguration() };
            _context = new OwinContext(_data);
            _authHeader = new AuthenticationHeaderValue(_token);
            _userManager = SingletonManager.Instance.SetUserManager(Substitute.For<IUserManager>());
            SingletonManager.Instance.SetTokenManager(Substitute.For<ITokenManager>());
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _userManager.ClearReceivedCalls();
        }

        [Test]
        public async Task AssertThatGetAllReturnsAValidResponseCodeAndCallsManager()
        {
            InitRequestHelper(HttpMethod.Get.Method);
            var action = await _usersController.GetAll();
            var response = await action.ExecuteAsync(new CancellationToken());
            await _userManager.Received().GetAllUser();
            Assert.False(response.StatusCode == HttpStatusCode.InternalServerError, "InternalServerError is thrown");
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK, "Status code is valid");
        }

        [Test]
        public async Task AssertThatGetReturnsAValidResponseCodeAndCallsManager()
        {
            InitRequestHelper(HttpMethod.Get.Method);
            var action = await _usersController.Get("1");
            var response = await action.ExecuteAsync(new CancellationToken());
            await _userManager.Received().GetUser(Arg.Any<string>());
            Assert.False(response.StatusCode == HttpStatusCode.InternalServerError, "InternalServerError is thrown");
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK
                          || response.StatusCode == HttpStatusCode.NotFound
                , "Status code is valid");
        }

        [Test]
        public async Task AssertThatGetTop50ReturnsAValidResponseCodeAndCallsManager()
        {
            InitRequestHelper(HttpMethod.Get.Method);
            var action = await _usersController.GetTop50();
            var response = await action.ExecuteAsync(new CancellationToken());
            await _userManager.Received().GetBestBetters();
            Assert.False(response.StatusCode == HttpStatusCode.InternalServerError, "InternalServerError is thrown");
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK , "Status code is valid");
        }

        [Test]
        public async Task AssertThatGetUserPlaceReturnsAValidResponseCodeAndCallsManager()
        {
            InitRequestHelper(HttpMethod.Get.Method);
            var action = await _usersController.GetUserPlace();
            var response = await action.ExecuteAsync(new CancellationToken());
            await _userManager.Received().GetUserPositionAmongSiblings(Arg.Any<User>());
            Assert.False(response.StatusCode == HttpStatusCode.InternalServerError, "InternalServerError is thrown");
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK
                          || response.StatusCode == HttpStatusCode.NotFound
                , "Status code is valid");
        }

        [Test]
        public async Task AssertThatGetUserTop3ReturnsAValidResponseCodeAndCallsManager()
        {
            InitRequestHelper(HttpMethod.Get.Method);
            var action = await _usersController.GetTop3();
            var response = await action.ExecuteAsync(new CancellationToken());
            await _userManager.Received().GetTop3();
            Assert.False(response.StatusCode == HttpStatusCode.InternalServerError, "InternalServerError is thrown");
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK, "Status code is valid");
        }

        [Test]
        public async Task AssertThatGetUserFromTokenReturnsAValidResponseCodeAndCallsManager()
        {
            InitRequestHelper(HttpMethod.Get.Method);
            var action = await _usersController.GetUserFromToken();
            var response = await action.ExecuteAsync(new CancellationToken());
            Assert.False(response.StatusCode == HttpStatusCode.InternalServerError, "InternalServerError is thrown");
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK, "Status code is valid");
        }

        [TestCase("")]
        [TestCase("email already taken")]
        public async Task AssertThatPutReturnsAValidResponseCodeAndCallsManager(string canUpdate)
        {
            InitRequestHelper(HttpMethod.Put.Method);
            _usersController.Request.Content = new StringContent(_user.ToJson(), Encoding.UTF8, "application/json");
            _userManager.CanUpdate("1", _user).Returns(canUpdate);
            _userManager.GetUser("1").Returns(_user);
            var action = await _usersController.Put("1", _user);
            var response = await action.ExecuteAsync(new CancellationToken());
            await _userManager.Received().CanUpdate(Arg.Any<string>(), Arg.Any<User>());
            await _userManager.Received().ChangeUser(Arg.Any<string>(), Arg.Any<User>());
            await _userManager.Received().GetUser(Arg.Any<string>());
            Assert.False(response.StatusCode == HttpStatusCode.InternalServerError, "InternalServerError is thrown");
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK 
                || response.StatusCode == HttpStatusCode.BadRequest
                || response.StatusCode == HttpStatusCode.NotFound
                , "Status code is valid");
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task AssertThatPutIsPrivateReturnsAValidResponseCodeAndCallsManager(bool status)
        {
            InitRequestHelper(HttpMethod.Put.Method);
            _user.IsPrivate = status;
            _usersController.Request.Content = new StringContent(_user.ToJson(), Encoding.UTF8, "application/json");
            GetUserByToken(_usersController.Request).Returns(Task.FromResult(_user));

            var action = await _usersController.PutIsPrivate(_user);
            var response = await action.ExecuteAsync(new CancellationToken());
            await _userManager.Received().ChangeUserIsPrivate(Arg.Any<ObjectId>(), status);
            Assert.False(response.StatusCode == HttpStatusCode.InternalServerError, "InternalServerError is thrown");
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK
                , "Status code is valid");
        }

        [TestCase(Models.User.AdminRole)]
        [TestCase(Models.User.UserRole)]
        public async Task AssertThatPutRoleReturnsAValidResponseCodeAndCallsManager(string role)
        {
            InitRequestHelper(HttpMethod.Put.Method);
            _user.Role = role;
            _usersController.Request.Content = new StringContent(_user.ToJson(), Encoding.UTF8, "application/json");
            var action = await _usersController.PutRole("1", _user);
            var response = await action.ExecuteAsync(new CancellationToken());
            await _userManager.Received().ChangeUserRole(Arg.Any<string>(), role);
            Assert.False(response.StatusCode == HttpStatusCode.InternalServerError, "InternalServerError is thrown");
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK
                          || response.StatusCode == HttpStatusCode.NotFound
                , "Status code is valid");
        }

        [Test]
        public async Task AssertThatDeleteReturnsAValidResponseCodeAndCallsManager()
        {
            InitRequestHelper(HttpMethod.Delete.Method);
            var action = await _usersController.Delete("1");
            var response = await action.ExecuteAsync(new CancellationToken());
            await _userManager.Received().DeleteUser(Arg.Any<string>());
            Assert.False(response.StatusCode == HttpStatusCode.InternalServerError, "InternalServerError is thrown");
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK
                , "Status code is valid");
        }

        [Test]
        public async Task AssertThatSearchReturnsAValidResponseCodeAndCallsManager()
        {
            InitRequestHelper(HttpMethod.Get.Method);
            _userManager.SearchUser("test").Returns(_users);
            var action = await _usersController.Search("test");
            var response = await action.ExecuteAsync(new CancellationToken());
            await _userManager.Received().SearchUser(Arg.Any<string>());
            Assert.False(response.StatusCode == HttpStatusCode.InternalServerError, "InternalServerError is thrown");
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NotFound, "Status code is valid");
        }

        [Test]
        public async Task AssertThatGetAllUsersPaginatedReturnsAValidResponseCodeAndCallsManager()
        {
            InitRequestHelper(HttpMethod.Get.Method);
            var action = await _usersController.GetAllUsersPaginated(1);
            var response = await action.ExecuteAsync(new CancellationToken());
            await _userManager.Received().GetAllUsersPaginated(Arg.Any<int>());
            Assert.False(response.StatusCode == HttpStatusCode.InternalServerError, "InternalServerError is thrown");
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NotFound, "Status code is valid");
        }

        [Test]
        public async Task AssertThatAddUserReturnsAValidResponseCodeAndCallsManager()
        {   
            InitRequestHelper(HttpMethod.Post.Method);
            var action = await _usersController.AddUser(_user);
            var response = await action.ExecuteAsync(new CancellationToken());
            await _userManager.Received().AddUser(Arg.Any<User>(), Arg.Any<string>());
            Assert.False(response.StatusCode == HttpStatusCode.InternalServerError, "InternalServerError is thrown");
            Assert.IsTrue(response.StatusCode == HttpStatusCode.Created, "Status code is valid");
        }


        private void InitRequestHelper(string verb)
        {
            switch (verb)
            {
                case "GET":
                    _usersController.Request = _httpRequestGet;
                    break;
                case "POST":
                    _usersController.Request = _httpRequestPost;
                    break;
                case "PUT":
                    _usersController.Request = _httpRequestPut;
                    break;
                case "DELETE":
                    _usersController.Request = _httpRequestDelete;
                    break;
                default:
                    _usersController.Request = _httpRequestGet;
                    break;
            }

            _usersController.Request.SetOwinContext(_context);
            _usersController.Request.GetOwinContext().Request.RemoteIpAddress = _ip;
            _usersController.Request.Headers.Authorization = _authHeader;
        }
    }
}