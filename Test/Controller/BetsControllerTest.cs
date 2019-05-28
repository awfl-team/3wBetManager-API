using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Manager;
using Manager.Interfaces;
using Microsoft.Owin;
using Models;
using MongoDB.Bson;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using _3wBetManager_API.Controllers;

namespace Test.Controller
{
    [TestFixture]
    internal class BetsControllerTest : BaseController
    {
        private BetController _betController;
        private IBetManager _betManager;
        private IUserManager _userManager;
        private IMatchManager _matchManager;

        private Dictionary<string, object> _data;
        private OwinContext _context;
        private AuthenticationHeaderValue _authHeader;
        private HttpRequestMessage _httpRequestGet = new HttpRequestMessage(HttpMethod.Get, "http://localhost:9000/");
        private HttpRequestMessage _httpRequestPost = new HttpRequestMessage(HttpMethod.Post, "http://localhost:9000/");
        private HttpRequestMessage _httpRequestPut = new HttpRequestMessage(HttpMethod.Put, "http://localhost:9000/");
        private List<Bet> _bets = new List<Bet>();
        private static List<User> _users = JsonConvert.DeserializeObject<List<User>>(TestHelper.GetDbResponseByCollectionAndFileName("user", "users"));
        private User _user = _users[0];

        private readonly string _ip = "127.0.0.1";

        private readonly string _token =
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6Imx1Y2FzYm91cmdlb2lzNjBAaG90bWFpbC5mciIsInJvbGUiOiJBRE1JTiIsInVuaXF1ZV9uYW1lIjoibGJvIiwibmJmIjoxNTU4NTMyMDI3LCJleHAiOjE1OTAxNTQ0MjcsImlhdCI6MTU1ODUzMjAyN30.a3Co739HOGU5cBmziUdOt6-YuzLau0JVfW0gj5khonQ";


        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _data = new Dictionary<string, object>() { { "Authorization", _token } };
            _betController = new BetController() { Configuration = new HttpConfiguration() };
            _context = new OwinContext(_data);
            _authHeader = new AuthenticationHeaderValue(_token);
            _userManager = SingletonManager.Instance.SetUserManager(Substitute.For<IUserManager>());
            _betManager = SingletonManager.Instance.SetBetManager(Substitute.For<IBetManager>());
            _matchManager = SingletonManager.Instance.SetMatchManager(Substitute.For<IMatchManager>());
            SingletonManager.Instance.SetTokenManager(new TokenManager());

            for (int i = 0; i < 3; i++)
            {
                _bets.Add(new Bet());
            }
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _betManager.ClearReceivedCalls();
            _userManager.ClearReceivedCalls();
        }

        [Test]
        public async Task AssertThatPostReturnsAValidResponseCodeAndCallsManager()
        {
            InitRequestHelper(HttpMethod.Post.Method);
            _betController.Request.Content = new StringContent(_bets.ToJson(), Encoding.UTF8, "application/json");
            GetUserByToken(_betController.Request).Returns(Task.FromResult(_user));
            var action = await _betController.Post(_bets);
            var response = await action.ExecuteAsync(new CancellationToken());
            _betManager.Received().ParseListBet(Arg.Any<List<Bet>>());
            _betManager.Received().AddGuidList(Arg.Any<User>(), Arg.Any<List<Bet>>());
            await _betManager.Received().AddBets(Arg.Any<List<Bet>>());
            Assert.False(response.StatusCode == HttpStatusCode.InternalServerError, "InternalServerError is thrown");
            Assert.IsTrue(response.StatusCode == HttpStatusCode.Created, "Status code is valid");
        }

        [Test]
        public async Task AssertThatPutReturnsAValidResponseCodeAndCallsManager()
        {
            InitRequestHelper(HttpMethod.Put.Method);
            _betController.Request.Content = new StringContent(_bets.ToJson(), Encoding.UTF8, "application/json");
            GetUserByToken(_betController.Request).Returns(Task.FromResult(_user));
            _betManager.ParseListBet(_bets).Returns(_bets);
            var action = await _betController.Put(_bets);
            var response = await action.ExecuteAsync(new CancellationToken());
            _betManager.Received().ParseListBet(Arg.Any<List<Bet>>());
            await _betManager.Received().ChangeBet(Arg.Any<Bet>());
            _matchManager.Received().CalculateMatchRating(Arg.Any<Match>());
            await _userManager.Received().ChangeUserPoint(Arg.Any<User>(), Arg.Any<float>(), Arg.Any<int>());
            Assert.False(response.StatusCode == HttpStatusCode.InternalServerError, "InternalServerError is thrown");
            Assert.IsTrue(response.StatusCode == HttpStatusCode.NoContent, "Status code is valid");
        }

        [Test]
        public async Task AssertThatGetBetsResultsReturnsAValidResponseCodeAndCallsManager()
        {
            InitRequestHelper(HttpMethod.Get.Method);
            var action = await _betController.GetBetsResult(2000);
            var response = await action.ExecuteAsync(new CancellationToken());
            await _betManager.Received().GetFinishBets(Arg.Any<User>(), Arg.Any<int>());
            Assert.False(response.StatusCode == HttpStatusCode.InternalServerError, "InternalServerError is thrown");
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK, "Status code is valid");
        }

        [Test]
        public async Task AssertThatGetBetsResultLimitReturnsAValidResponseCodeAndCallsManager()
        {
            InitRequestHelper(HttpMethod.Get.Method);
            var action = await _betController.GetBetsResultLimit();
            var response = await action.ExecuteAsync(new CancellationToken());
            await _betManager.Received().GetFinishBetsLimited(Arg.Any<User>());
            Assert.False(response.StatusCode == HttpStatusCode.InternalServerError, "InternalServerError is thrown");
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK, "Status code is valid");
        }

        [Test]
        public async Task AssertThatGetBetsCurrentLimitReturnsAValidResponseCodeAndCallsManager()
        {
            InitRequestHelper(HttpMethod.Get.Method);
            var action = await _betController.GetBetsResultLimit();
            var response = await action.ExecuteAsync(new CancellationToken());
            await _betManager.Received().GetFinishBetsLimited(Arg.Any<User>());
            Assert.False(response.StatusCode == HttpStatusCode.InternalServerError, "InternalServerError is thrown");
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK, "Status code is valid");
        }

        [Test]
        public async Task AssertThatGetBetsResultLimitWithKeyReturnsAValidResponseCodeAndCallsManager()
        {
            InitRequestHelper(HttpMethod.Get.Method);
            var action = await _betController.GetBetsResultLimitWithKey("1");
            var response = await action.ExecuteAsync(new CancellationToken());
            await _betManager.Received().GetFinishBetsLimited(Arg.Any<User>());
            Assert.False(response.StatusCode == HttpStatusCode.InternalServerError, "InternalServerError is thrown");
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK, "Status code is valid");
        }

        [Test]
        public async Task AssertThatGetBetsCurrentLimitWithKeyReturnsAValidResponseCodeAndCallsManager()
        {
            InitRequestHelper(HttpMethod.Get.Method);
            var action = await _betController.GetBetsCurrentLimitWithKey("1");
            var response = await action.ExecuteAsync(new CancellationToken());
            await _betManager.Received().GetCurrentBetsLimited(Arg.Any<User>());
            Assert.False(response.StatusCode == HttpStatusCode.InternalServerError, "InternalServerError is thrown");
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK, "Status code is valid");
        }

        [Test]
        public async Task AssertThatGetBetsAndMatchesReturnsAValidResponseCodeAndCallsManager()
        {
            InitRequestHelper(HttpMethod.Get.Method);
            var action = await _betController.GetBetsAndMatches(1);
            var response = await action.ExecuteAsync(new CancellationToken());
            await _betManager.Received().GetCurrentBetsAndScheduledMatches(Arg.Any<User>(), Arg.Any<int>());
            Assert.False(response.StatusCode == HttpStatusCode.InternalServerError, "InternalServerError is thrown");
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK, "Status code is valid");
        }

        [Test]
        public async Task AssertThatGetCurrentNumberMatchAndBetReturnsAValidResponseCodeAndCallsManager()
        {
            InitRequestHelper(HttpMethod.Get.Method);
            var action = await _betController.GetCurrentNumberMatchAndBet(1);
            var response = await action.ExecuteAsync(new CancellationToken());
            await _betManager.Received().NumberCurrentMatchAndBet(Arg.Any<User>(), Arg.Any<int>());
            Assert.False(response.StatusCode == HttpStatusCode.InternalServerError, "InternalServerError is thrown");
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK, "Status code is valid");
        }

        [Test]
        public async Task AssertThatGetFinishNumberMatchAndBetReturnsAValidResponseCodeAndCallsManager()
        {
            InitRequestHelper(HttpMethod.Get.Method);
            var action = await _betController.GetFinishNumberMatchAndBet(1);
            var response = await action.ExecuteAsync(new CancellationToken());
            await _betManager.Received().NumberCurrentMatchAndBet(Arg.Any<User>(), Arg.Any<int>());
            Assert.False(response.StatusCode == HttpStatusCode.InternalServerError, "InternalServerError is thrown");
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK, "Status code is valid");
        }

        [Test]
        public async Task AssertThatGetUserScheduledBetsPaginatedReturnsAValidResponseCodeAndCallsManager()
        {
            InitRequestHelper(HttpMethod.Get.Method);
            var action = await _betController.GetUserScheduledBetsPaginated(1);
            var response = await action.ExecuteAsync(new CancellationToken());
            await _betManager.Received().GetUserScheduledBetsPaginated(Arg.Any<User>(), Arg.Any<int>());
            Assert.False(response.StatusCode == HttpStatusCode.InternalServerError, "InternalServerError is thrown");
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK, "Status code is valid");
        }

        private void InitRequestHelper(string verb)
        {
            switch (verb)
            {
                case "GET":
                    _betController.Request = _httpRequestGet;
                    break;
                case "POST":
                    _betController.Request = _httpRequestPost;
                    break;
                case "PUT":
                    _betController.Request = _httpRequestPut;
                    break;
                default:
                    _betController.Request = _httpRequestGet;
                    break;
            }

            _betController.Request.SetOwinContext(_context);
            _betController.Request.GetOwinContext().Request.RemoteIpAddress = _ip;
            _betController.Request.Headers.Authorization = _authHeader;
        }
    }
}