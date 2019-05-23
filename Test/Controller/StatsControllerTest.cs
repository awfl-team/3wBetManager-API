using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Results;
using Manager;
using Manager.Interfaces;
using Microsoft.Owin;
using Models;
using NSubstitute;
using NUnit.Framework;
using _3wBetManager_API.Controllers;

namespace Test.Controller
{
    [TestFixture]
    public class StatsControllerTest
    {
        private StatController _statController;
        private IBetManager _betManager;
        private IUserManager _userManager;
        private Dictionary<string, object> _data;
        private OwinContext _context;
        private AuthenticationHeaderValue _authHeader;

        private readonly string _ip = "127.0.0.1";
        private readonly string _token =
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6Imx1Y2FzYm91cmdlb2lzNjBAaG90bWFpbC5mciIsInJvbGUiOiJBRE1JTiIsInVuaXF1ZV9uYW1lIjoibGJvIiwibmJmIjoxNTU4NTMyMDI3LCJleHAiOjE1OTAxNTQ0MjcsImlhdCI6MTU1ODUzMjAyN30.a3Co739HOGU5cBmziUdOt6-YuzLau0JVfW0gj5khonQ";


        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _data = new Dictionary<string, object>() {{"Authorization", _token}};
            _statController = new StatController
            {
                Configuration = new HttpConfiguration(),
                Request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:9000/")
            };
            _context = new OwinContext(_data);
            _authHeader = new AuthenticationHeaderValue(_token);
            _statController.Request.SetOwinContext(_context);
            _statController.Request.GetOwinContext().Request.RemoteIpAddress = _ip;
            _statController.Request.Headers.Authorization = _authHeader;
            _betManager = SingletonManager.Instance.SetBetManager(Substitute.For<IBetManager>());
            _userManager = SingletonManager.Instance.SetUserManager(Substitute.For<IUserManager>());
            SingletonManager.Instance.SetTokenManager(new TokenManager());
        }

        [TearDown]
        public void TearDown()
        {
            _betManager.ClearReceivedCalls();
            _userManager.ClearReceivedCalls();
        }

        [Test]
        public async Task AssertThatGetUserCoinStatsReturnsAValidResponseCodeAndCallsManager()
        {
            var action = await _statController.GetUserCoinStats();
            var response = await action.ExecuteAsync(new CancellationToken());
            await _userManager.Received().GetUserCoinStats(Arg.Any<User>());
            Assert.False(response.StatusCode == HttpStatusCode.InternalServerError, "InternalServerError is thrown");
            Assert.IsTrue(response.IsSuccessStatusCode, "Status code is success");
        }

        [Test]
        public async Task AssertThatGetUserIncomesPerMonthReturnsAValidResponseCodeAndCallsManager()
        {
            var action = await _statController.GetUserIncomesPerMonth();
            var response = await action.ExecuteAsync(new CancellationToken());
            await _betManager.Received().GetUserIncomesPerMonth(Arg.Any<User>());
            Assert.False(response.StatusCode == HttpStatusCode.InternalServerError, "InternalServerError is thrown");
            Assert.IsTrue(response.IsSuccessStatusCode, "Status code is success");
        }

        [Test]
        public async Task AssertThatGetUserIncomesPerYearReturnsAValidResponseCodeAndCallsManager()
        {
            var action = await _statController.GetUserIncomesPerYear();
            var response = await action.ExecuteAsync(new CancellationToken());
            await _betManager.Received().GetUserIncomesPerYear(Arg.Any<User>());
            Assert.False(response.StatusCode == HttpStatusCode.InternalServerError, "InternalServerError is thrown");
            Assert.IsTrue(response.IsSuccessStatusCode, "Status code is success");
        }

        [Test]
        public async Task AssertThatGetUserBetsPerTypeReturnsAValidResponseCodeAndCallsManager()
        {
            var action = await _statController.GetUserBetsPerType();
            var response = await action.ExecuteAsync(new CancellationToken());
            await _betManager.Received().GetUserBetsPerType(Arg.Any<User>());
            Assert.False(response.StatusCode == HttpStatusCode.InternalServerError, "InternalServerError is thrown");
            Assert.IsTrue(response.IsSuccessStatusCode, "Status code is success");
        }

        [Test]
        public async Task AssertThatGetUserBetsEarningsPerTypeReturnsAValidResponseCodeAndCallsManager()
        {
            var action = await _statController.GetUserBetsEarningsPerType();
            var response = await action.ExecuteAsync(new CancellationToken());
            await _betManager.Received().GetUserBetsEarningsPerType(Arg.Any<User>());
            Assert.False(response.StatusCode == HttpStatusCode.InternalServerError, "InternalServerError is thrown");
            Assert.IsTrue(response.IsSuccessStatusCode, "Status code is success");
        }

        [Test]
        public async Task AssertThatGetUserBetsPerTypePublicReturnsAValidResponseCodeAndCallsManager()
        {
            var action = await _statController.GetUserBetsPerTypePublic("1");
            var response = await action.ExecuteAsync(new CancellationToken());
            await _userManager.Received().GetUser(Arg.Any<string>());
            await _betManager.Received().GetUserBetsPerType(Arg.Any<User>());
            Assert.False(response.StatusCode == HttpStatusCode.InternalServerError, "InternalServerError is thrown");
            Assert.IsTrue(response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.NotFound, "Status code is success or NotFound");
        }

        [Test]
        public async Task AssertThatGetPublicUserCoinStatsReturnsAValidResponseCodeAndCallsManager()
        {
            var action = await _statController.GetPublicUserCoinStats("1");
            var response = await action.ExecuteAsync(new CancellationToken());
            await _userManager.Received().GetUser(Arg.Any<string>());
            await _userManager.Received().GetUserCoinStats(Arg.Any<User>());
            Assert.False(response.StatusCode == HttpStatusCode.InternalServerError, "InternalServerError is thrown");
            Assert.IsTrue(response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.NotFound, "Status code is success or NotFound");
        }

        [Test]
        public async Task AssertGetPucliUserIncomesPerMonthReturnsAValidResponseCodeAndCallsManager()
        {
            var action = await _statController.GetPublicUserIncomesPerMonth("1");
            var response = await action.ExecuteAsync(new CancellationToken());
            await _userManager.Received().GetUser(Arg.Any<string>());
            await _betManager.Received().GetUserIncomesPerMonth(Arg.Any<User>());
            Assert.False(response.StatusCode == HttpStatusCode.InternalServerError, "InternalServerError is thrown");
            Assert.IsTrue(response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.NotFound, "Status code is success or NotFound");
        }

        [Test]
        public async Task AssertThatGetPublicUserIncomesPerYearReturnsAValidResponseCodeAndCallsManager()
        {
            var action = await _statController.GetPublicUserIncomesPerYear("1");
            var response = await action.ExecuteAsync(new CancellationToken());
            await _userManager.Received().GetUser(Arg.Any<string>());
            await _betManager.Received().GetUserIncomesPerYear(Arg.Any<User>());
            Assert.False(response.StatusCode == HttpStatusCode.InternalServerError, "InternalServerError is thrown");
            Assert.IsTrue(response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.NotFound, "Status code is success or NotFound");
        }

        [Test]
        public async Task AssertThatGetPublicUserBetsEarningsPerTypeReturnsAValidResponseCodeAndCallsManager()
        {
            var action = await _statController.GetPublicUserBetsEarningsPerType("1");
            var response = await action.ExecuteAsync(new CancellationToken());
            await _userManager.Received().GetUser(Arg.Any<string>());
            await _betManager.Received().GetUserBetsEarningsPerType(Arg.Any<User>());
            Assert.False(response.StatusCode == HttpStatusCode.InternalServerError, "InternalServerError is thrown");
            Assert.IsTrue(response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.NotFound, "Status code is success or NotFound");
        }
    }
}