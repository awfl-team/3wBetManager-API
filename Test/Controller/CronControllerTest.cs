using _3wBetManager_API.Controllers;
using Manager;
using NSubstitute;
using NUnit.Framework;

namespace Test.Controller
{
    [TestFixture]
    public class CronControllerTest
    {
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

        private CronController _cronController;
        private FootballDataManager _footballDataManager;

        /*[Test]
        public void RefreshCompetitionsTest()
        {
            var refreshCompetitions = _cronController.RefreshCompetitions();
            _footballDataManager.Received().GetAllCompetitions();
            Assert.IsInstanceOf<Task<IHttpActionResult>>(refreshCompetitions);
        }

        [Test]
        public void RefreshMatchTest()
        {
            var refreshMatch = _cronController.RefreshCompetitions();
            _footballDataManager.Received().GetAllMatchForAWeek();
            Assert.IsInstanceOf<Task<IHttpActionResult>>(refreshMatch);
        }

        [Test]
        public void RefreshTeamTest()
        {
            var refreshTeam = _cronController.RefreshCompetitions();
            _footballDataManager.Received().GetAllTeams();
            Assert.IsInstanceOf<Task<IHttpActionResult>>(refreshTeam);
        }*/
    }
}