using System.Threading.Tasks;
using System.Web.Http;
using FetchFootballData;
using Manager;
using NSubstitute;
using NUnit.Framework;
using _3wBetManager_API.Controllers;

namespace Test.Controller
{
    [TestFixture]
    public class CronControllerTest
    {
        private CronController _cronController;
        private FootballDataManager _footballDataManager;

        [SetUp]
        public void SetUp()
        {
            _cronController = new CronController();
            _footballDataManager = Substitute.For<FootballDataManager>();
        }

        [TearDown]
        public void TearDown()
        {
            _footballDataManager.ClearReceivedCalls();
        }

        [Test]
        public void RefreshCompetitionsTest()
        {
            var refreshCompetitions = _cronController.RefreshCompetitions();
            _footballDataManager.Received().GetAllCompetitions();
            Assert.IsInstanceOf<Task<IHttpActionResult>>(refreshCompetitions);
        }

        [Test]
        public void RefreshTeamTest()
        {
            var refreshTeam = _cronController.RefreshCompetitions();
            _footballDataManager.Received().GetAllTeams();
            Assert.IsInstanceOf<Task<IHttpActionResult>>(refreshTeam);
        }

        [Test]
        public void RefreshMatchTest()
        {
            var refreshMatch = _cronController.RefreshCompetitions();
            _footballDataManager.Received().GetAllMatchForAWeek();
            Assert.IsInstanceOf<Task<IHttpActionResult>>(refreshMatch);
        }

        [Test]
        public void RefreshAllTest()
        {
            var refreshAll = _cronController.RefreshAll();
            _footballDataManager.Received().GetAllCompetitions();
            _footballDataManager.Received().GetAllTeams();
            _footballDataManager.Received().GetAllMatchForAWeek();
            Assert.IsInstanceOf<Task<IHttpActionResult>>(refreshAll);
        }


    }
}
