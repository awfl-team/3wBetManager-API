using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Manager;
using Manager.Interfaces;
using Microsoft.Owin;
using Models;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using _3wBetManager_API.Controllers;

namespace Test.Controller
{
    [TestFixture]
    internal class ItemControllerTest : BaseController
    {
        private ItemController _itemController;
        private IItemManager _itemManager;
        private IUserManager _userManager;
        private Dictionary<string, object> _data;
        private OwinContext _context;
        private AuthenticationHeaderValue _authHeader;
        private HttpRequestMessage _httpRequestGet = new HttpRequestMessage(HttpMethod.Get, "http://localhost:9000/");
        private HttpRequestMessage _httpRequestPost = new HttpRequestMessage(HttpMethod.Post, "http://localhost:9000/");
        private HttpRequestMessage _httpRequestPut = new HttpRequestMessage(HttpMethod.Put, "http://localhost:9000/");
        private List<Item> _items = new List<Item>();
        private static List<User> _users = JsonConvert.DeserializeObject<List<User>>(TestHelper.GetDbResponseByCollectionAndFileName("user", "users"));
        private User _user = _users[0];

        private readonly string _ip = "127.0.0.1";
        private readonly string _token =
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6Imx1Y2FzYm91cmdlb2lzNjBAaG90bWFpbC5mciIsInJvbGUiOiJBRE1JTiIsInVuaXF1ZV9uYW1lIjoibGJvIiwibmJmIjoxNTU4NTMyMDI3LCJleHAiOjE1OTAxNTQ0MjcsImlhdCI6MTU1ODUzMjAyN30.a3Co739HOGU5cBmziUdOt6-YuzLau0JVfW0gj5khonQ";


        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _data = new Dictionary<string, object>() { { "Authorization", _token } };
            _itemController = new ItemController() { Configuration = new HttpConfiguration() };
            _context = new OwinContext(_data);
            _authHeader = new AuthenticationHeaderValue(_token);

            _itemManager = SingletonManager.Instance.SetItemManager(Substitute.For<IItemManager>());
            _userManager = SingletonManager.Instance.SetUserManager(Substitute.For<IUserManager>());
            SingletonManager.Instance.SetTokenManager(new TokenManager());

            for (int i = 0; i < 3; i++)
            {
                _items.Add(new Item());
            }
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _userManager.ClearReceivedCalls();
            _itemManager.ClearReceivedCalls();
        }

        [Test]
        public async Task AssertThatBuyItemsReturnsAValidResponseCodeAndCallsManager()
        {
            InitRequestHelper(HttpMethod.Post.Method);
            var action = await _itemController.BuyItems(_items);
            var response = await action.ExecuteAsync(new CancellationToken());
            await _itemManager.Received().BuyItemsToUser(Arg.Any<List<Item>>(), Arg.Any<User>());
            Assert.False(response.StatusCode == HttpStatusCode.InternalServerError, "InternalServerError is thrown");
            Assert.IsTrue(response.StatusCode == HttpStatusCode.Created, "Status code is valid");
        }

        [Test]
        public async Task AssertThatAddItemsReturnsAValidResponseCodeAndCallsManager()
        {
            InitRequestHelper(HttpMethod.Get.Method);
            var action = await _itemController.AddItems();
            var response = await action.ExecuteAsync(new CancellationToken());
            await _itemManager.Received().AddItemsToUser(Arg.Any<User>());
            await _userManager.Received().DeleteUserItem(Arg.Any<User>(), Item.LootBox);
            Assert.False(response.StatusCode == HttpStatusCode.InternalServerError, "InternalServerError is thrown");
            Assert.IsTrue(response.StatusCode == HttpStatusCode.Created, "Status code is valid");
        }

        [Test]
        public async Task AssertThatAddMysteryItemReturnsAValidResponseCodeAndCallsManager()
        {
            InitRequestHelper(HttpMethod.Get.Method);
            var action = await _itemController.AddMysteryItem();
            var response = await action.ExecuteAsync(new CancellationToken());
            await _itemManager.Received().AddMysteryItemToUser(Arg.Any<User>());
            await _userManager.Received().DeleteUserItem(Arg.Any<User>(), Item.Mystery);
            Assert.False(response.StatusCode == HttpStatusCode.InternalServerError, "InternalServerError is thrown");
            Assert.IsTrue(response.StatusCode == HttpStatusCode.Created, "Status code is valid");
        }

        [Test]
        public async Task AssertThatUseBombReturnsAValidResponseCodeAndCallsManager()
        {
            InitRequestHelper(HttpMethod.Put.Method);
            GetUserByToken(_itemController.Request).Returns(Task.FromResult(_user));
            _itemManager.UseBomb("1").Returns(Task.FromResult(_user));
            var action = await _itemController.UseBomb("1");
            var response = await action.ExecuteAsync(new CancellationToken());
            await _itemManager.Received().UseBomb(Arg.Any<string>());
            await _userManager.Received().DeleteUserItem(Arg.Any<User>(), Item.Bomb);
            Assert.False(response.StatusCode == HttpStatusCode.InternalServerError, "InternalServerError is thrown");
            Assert.IsTrue(response.StatusCode == HttpStatusCode.NoContent, "Status code is valid");
        }

        [TestCase(10, Item.MultiplyByTen)]
        [TestCase(5, Item.MultiplyByFive)]
        [TestCase(2, Item.MultiplyByTwo)]
        public async Task AssertThatUseMultiplierReturnsAValidResponseCodeAndCallsManager(int multiplierValue, string itemType)
        {
            InitRequestHelper(HttpMethod.Put.Method);
            var action = await _itemController.UseMultiplier(multiplierValue, "1");
            var response = await action.ExecuteAsync(new CancellationToken());
            await _itemManager.Received().UseMultiplier(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<User>());
            await _userManager.Received().DeleteUserItem(Arg.Any<User>(), itemType);
            Assert.False(response.StatusCode == HttpStatusCode.InternalServerError, "InternalServerError is thrown");
            Assert.IsTrue(response.StatusCode == HttpStatusCode.NoContent, "Status code is valid");
        }

        [Test]
        public async Task AssertThatUseKeyReturnsAValidResponseCodeAndCallsManager()
        {
            InitRequestHelper(HttpMethod.Get.Method);
            GetUserByToken(_itemController.Request).Returns(Task.FromResult(_user));
            _userManager.GetUser("1").Returns(Task.FromResult(_user));
            var action = await _itemController.UseKey("1");
            var response = await action.ExecuteAsync(new CancellationToken());
            await _userManager.Received().GetUser(Arg.Any<string>());
            await _userManager.Received().DeleteUserItem(Arg.Any<User>(), Item.Key);
            Assert.False(response.StatusCode == HttpStatusCode.InternalServerError, "InternalServerError is thrown");
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK, "Status code is valid");
        }

        [Test]
        public async Task AssertThatGetAllReturnsAValidResponseCodeAndCallsManager()
        {
            InitRequestHelper(HttpMethod.Get.Method);
            var action = await _itemController.GetAll();
            var response = await action.ExecuteAsync(new CancellationToken());
            await _itemManager.Received().GetAllItems();
            Assert.False(response.StatusCode == HttpStatusCode.InternalServerError, "InternalServerError is thrown");
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK, "Status code is valid");
        }

        [Test]
        public async Task AssertThatUpdateItemReturnsAValidResponseCodeAndCallsManager()
        {
            InitRequestHelper(HttpMethod.Put.Method);
            var action = await _itemController.UpdateItem("1", _items[0]);
            var response = await action.ExecuteAsync(new CancellationToken());
            await _itemManager.Received().ChangeItem(Arg.Any<string>(), Arg.Any<Item>());
            Assert.False(response.StatusCode == HttpStatusCode.InternalServerError, "InternalServerError is thrown");
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK, "Status code is valid");
        }

        private void InitRequestHelper(string verb)
        {
            switch (verb)
            {
                case "GET":
                    _itemController.Request = _httpRequestGet;
                    break;
                case "POST":
                    _itemController.Request = _httpRequestPost;
                    break;
                case "PUT":
                    _itemController.Request = _httpRequestPut;
                    break;
                default:
                    _itemController.Request = _httpRequestGet;
                    break;
            }

            _itemController.Request.SetOwinContext(_context);
            _itemController.Request.GetOwinContext().Request.RemoteIpAddress = _ip;
            _itemController.Request.Headers.Authorization = _authHeader;
        }
    }
}