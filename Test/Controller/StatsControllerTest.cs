using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
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
        private ITokenManager _tokenManager;
        private User _user;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _statController = new StatController();
            _betManager = SingletonManager.Instance.SetBetManager(Substitute.For<IBetManager>());
            _userManager = SingletonManager.Instance.SetUserManager(Substitute.For<IUserManager>());
            _tokenManager = SingletonManager.Instance.SetTokenManager(Substitute.For<ITokenManager>());
            _user = new User { Email = "test", Password = "test", Username = "test" };
        }

        [TearDown]
        public void TearDown()
        {
            _betManager.ClearReceivedCalls();
        }

        [Test]
        public async Task AssertThatGetUserCoinStatsDoesNotReturnInternalServerErrorAndCallsManager()
        {
            _statController.Request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:9000/stats/coin");
            var data = new Dictionary<string, object>()
            {
                {"Authorization", "fake_token"} // fake whatever  you need here.
            };
            var context = new OwinContext(data);
            _statController.Request.SetOwinContext(context);
            _statController.Request.GetOwinContext().Request.RemoteIpAddress = "127.0.0.1";
            _statController.Request.Headers.Authorization = new AuthenticationHeaderValue("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6Imx1Y2FzYm91cmdlb2lzNjBAaG90bWFpbC5mciIsInJvbGUiOiJBRE1JTiIsInVuaXF1ZV9uYW1lIjoibGJvIiwibmJmIjoxNTU4NTMyMDI3LCJleHAiOjE1OTAxNTQ0MjcsImlhdCI6MTU1ODUzMjAyN30.a3Co739HOGU5cBmziUdOt6-YuzLau0JVfW0gj5khonQ");

            var response = await _statController.GetUserCoinStats();
            var cuckloard = response;
            var test = "cuck";
            await _userManager.Received().GetUserCoinStats(_user);

            Assert.Throws<Exception>(() => { _userManager.GetUserCoinStats(_user); });

            Assert.DoesNotThrowAsync(async () => { await _statController.GetUserCoinStats(); });
        }

        [Test]
        public void AssertThatGetUserIncomesPerMonthDoesNotReturnInternalServerErrorAndCallsManager()
        {

        }

        [Test]
        public void AssertThatGetUserIncomesPerYearDoesNotReturnInternalServerErrorAndCallsManager()
        {

        }

        [Test]
        public void AssertThatGetUserBetsPerTypeDoesNotReturnInternalServerErrorAndCallsManager()
        {

        }

        [Test]
        public void AssertThatGetUserBetsEarningsPerTypeDoesNotReturnInternalServerErrorAndCallsManager()
        {

        }

        [Test]
        public void AssertThatGetUserBetsPerTypePublicDoesNotReturnInternalServerErrorAndCallsManager()
        {

        }

        [Test]
        public void AssertThatGetPublicUserCoinStatsDoesNotReturnInternalServerErrorAndCallsManager()
        {

        }

        [Test]
        public void AssertThatGetPublicUserIncomesPerMonthDoesNotReturnInternalServerErrorAndCallsManager()
        {

        }

        [Test]
        public void AssertThatGetPublicUserIncomesPerYearDoesNotReturnInternalServerErrorAndCallsManager()
        {

        }

        [Test]
        public void AssertThatGetPublicUserBetsEarningsPerTypeDoesNotReturnInternalServerErrorAndCallManager()
        {

        }
    }
}