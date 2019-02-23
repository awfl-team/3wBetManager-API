using System.Threading.Tasks;
using System.Web.Http;
using DAO;
using Manager;
using NSubstitute;
using NUnit.Framework;
using _3wBetManager_API.Controllers;
using DAO.Interfaces;

namespace Test.Controller
{
    [TestFixture]
    public class CompetitionControllerTest
    {
        private CompetitionController _competitionController;
        private ICompetitionDao _competitionDao;

        [SetUp]
        public void SetUp()
        {
            _competitionController = new CompetitionController();
            _competitionDao = Singleton.Instance.SetCompetitionDao(Substitute.For<ICompetitionDao>());
        }

        [TearDown]
        public void TearDown()
        {
            _competitionDao.ClearReceivedCalls();
        }

        [Test]
        public void GetAllTest()
        {
            var getAllCompetition = _competitionController.GetAll();

            _competitionDao.Received().FindAllCompetitions();
            Assert.IsInstanceOf<Task<IHttpActionResult>>(getAllCompetition);

        }
    }
}
