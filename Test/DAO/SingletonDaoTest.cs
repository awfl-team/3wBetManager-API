using DAO;
using DAO.Interfaces;
using MongoDB.Driver;
using NSubstitute;
using NUnit.Framework;

namespace Test.DAO
{
    [TestFixture]
    public class SingletonDaoTest
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            SingletonDao.Instance.SetAllDao(Substitute.For<IMongoDatabase>());
        }

        [Test]
        public void AssertThatSetAllDaoWorks()
        {
            Assert.IsInstanceOf<IItemDao>(SingletonDao.Instance.ItemDao, "ItemDao ok");
            Assert.IsNotNull(SingletonDao.Instance.ItemDao);
            Assert.IsInstanceOf<IMatchDao>(SingletonDao.Instance.MatchDao, "MatchDao ok");
            Assert.IsNotNull(SingletonDao.Instance.MatchDao);
            Assert.IsInstanceOf<IBetDao>(SingletonDao.Instance.BetDao, "BetDao ok");
            Assert.IsNotNull(SingletonDao.Instance.BetDao);
            Assert.IsInstanceOf<IUserDao>(SingletonDao.Instance.UserDao, "UserDao ok");
            Assert.IsNotNull(SingletonDao.Instance.UserDao);
            Assert.IsInstanceOf<ICompetitionDao>(SingletonDao.Instance.CompetitionDao, "CompetDao ok");
            Assert.IsNotNull(SingletonDao.Instance.CompetitionDao);
            Assert.IsInstanceOf<ITeamDao>(SingletonDao.Instance.TeamDao, "TeamDao ok");
            Assert.IsNotNull(SingletonDao.Instance.TeamDao);
        }
    }
}