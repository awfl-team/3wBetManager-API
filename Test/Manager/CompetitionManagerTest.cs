using System.Threading.Tasks;
using DAO.Interfaces;
using Manager;
using Manager.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace Test.Manager
{
    [TestFixture]
    internal class CompetitionManagerTest
    {
        private ICompetitionDao _competitionDao;
        private ICompetitionManager _competitionManager;

        [OneTimeSetUp]
        public void SetUp()
        {
            _competitionDao = Substitute.For<ICompetitionDao>();
            _competitionManager =
                SingletonManager.Instance.SetCompetitionManager(new CompetitionManager(_competitionDao));
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _competitionDao.ClearReceivedCalls();
        }

        [Test]
        public async Task AssertThatGetAllCompetitionCallsFindAllCompetitions()
        {
            await _competitionManager.GetAllCompetition();
            await _competitionDao.Received().FindAllCompetitions();
        }
    }
}