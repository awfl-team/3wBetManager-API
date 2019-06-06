using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using _3wBetManager_API.Controllers;
using Manager;
using Manager.Interfaces;
using Microsoft.Owin;
using NSubstitute;
using NUnit.Framework;

namespace Test.Controller
{
    [TestFixture]
    internal class CronControllerTest
    {
        [TearDown]
        public void TearDown()
        {
            _footballDataManager.ClearReceivedCalls();
        }

        private CronController _cronController;
        private IFootballDataManager _footballDataManager;
        private Dictionary<string, object> _data;
        private OwinContext _context;
        private AuthenticationHeaderValue _authHeader;

        private readonly string _ip = "127.0.0.1";

        private readonly string _token =
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6Imx1Y2FzYm91cmdlb2lzNjBAaG90bWFpbC5mciIsInJvbGUiOiJBRE1JTiIsInVuaXF1ZV9uYW1lIjoibGJvIiwibmJmIjoxNTU4NTMyMDI3LCJleHAiOjE1OTAxNTQ0MjcsImlhdCI6MTU1ODUzMjAyN30.a3Co739HOGU5cBmziUdOt6-YuzLau0JVfW0gj5khonQ";

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _data = new Dictionary<string, object> {{"Authorization", _token}};
            _cronController = new CronController
            {
                Configuration = new HttpConfiguration(),
                Request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:9000/")
            };
            _context = new OwinContext(_data);
            _authHeader = new AuthenticationHeaderValue(_token);
            _cronController.Request.SetOwinContext(_context);
            _cronController.Request.GetOwinContext().Request.RemoteIpAddress = _ip;
            _cronController.Request.Headers.Authorization = _authHeader;
            _footballDataManager =
                SingletonManager.Instance.SetFootballDataManager(Substitute.For<IFootballDataManager>());
        }

        [Test]
        public async Task AssertThatRefreshCompetitionsReturnsAValidResponseCodeAndCallsManager()
        {
            var action = await _cronController.RefreshCompetitions();
            var response = await action.ExecuteAsync(new CancellationToken());
            await _footballDataManager.Received().GetAllCompetitions();
            Assert.False(response.StatusCode == HttpStatusCode.InternalServerError, "InternalServerError is thrown");
            Assert.IsTrue(response.IsSuccessStatusCode, "Status code is success");
        }

        [Test]
        public async Task AssertThatRefreshMatchesReturnsAValidResponseCodeAndCallsManager()
        {
            var action = await _cronController.RefreshMatches();
            var response = await action.ExecuteAsync(new CancellationToken());
            await _footballDataManager.Received().GetAllMatchForAWeek();
            Assert.False(response.StatusCode == HttpStatusCode.InternalServerError, "InternalServerError is thrown");
            Assert.IsTrue(response.IsSuccessStatusCode, "Status code is success");
        }

        [Test]
        public async Task AssertThatRefreshTeamsReturnsAValidResponseCodeAndCallsManager()
        {
            var action = await _cronController.RefreshTeams();
            var response = await action.ExecuteAsync(new CancellationToken());
            await _footballDataManager.Received().GetAllTeams();
            Assert.False(response.StatusCode == HttpStatusCode.InternalServerError, "InternalServerError is thrown");
            Assert.IsTrue(response.IsSuccessStatusCode, "Status code is success");
        }
    }
}