using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using _3wBetManager_API.Controllers;
using DAO;
using DAO.Interfaces;
using Manager;
using Manager.Interfaces;
using Microsoft.Owin;
using Models;
using NSubstitute;
using NUnit.Framework;

namespace Test.Controller
{
    [TestFixture]
    public class CompetitionControllerTest
    {
        private CompetitionController _competitionController;
        private ICompetitionManager _competitionManager;
        private Dictionary<string, object> _data;
        private OwinContext _context;
        private AuthenticationHeaderValue _authHeader;

        private readonly string _ip = "127.0.0.1";
        private readonly string _token =
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6Imx1Y2FzYm91cmdlb2lzNjBAaG90bWFpbC5mciIsInJvbGUiOiJBRE1JTiIsInVuaXF1ZV9uYW1lIjoibGJvIiwibmJmIjoxNTU4NTMyMDI3LCJleHAiOjE1OTAxNTQ0MjcsImlhdCI6MTU1ODUzMjAyN30.a3Co739HOGU5cBmziUdOt6-YuzLau0JVfW0gj5khonQ";

        [SetUp]
        public void OneTimeSetUp()
        {
            _data = new Dictionary<string, object>() { { "Authorization", _token } };
            _competitionController = new CompetitionController()
            {
                Configuration = new HttpConfiguration(),
                Request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:9000/")
            };
            _context = new OwinContext(_data);
            _authHeader = new AuthenticationHeaderValue(_token);
            _competitionController.Request.SetOwinContext(_context);
            _competitionController.Request.GetOwinContext().Request.RemoteIpAddress = _ip;
            _competitionController.Request.Headers.Authorization = _authHeader;
            _competitionManager = SingletonManager.Instance.SetCompetitionManager(Substitute.For<ICompetitionManager>());
            SingletonManager.Instance.SetTokenManager(new TokenManager());
        }

        [TearDown]
        public void TearDown()
        {
            _competitionManager.ClearReceivedCalls();
        }

        [Test]
        public async Task AssertThatGetAllReturnsAValidResponseCodeAndCallsManager()
        {
            var action = await _competitionController.GetAll();
            var response = await action.ExecuteAsync(new CancellationToken());
            await _competitionManager.Received().GetAllCompetition();
            Assert.False(response.StatusCode == HttpStatusCode.InternalServerError, "InternalServerError is thrown");
            Assert.IsTrue(response.IsSuccessStatusCode, "Status code is success");
        }

    }
}