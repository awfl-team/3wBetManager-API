using System.Collections.Generic;
using System.Threading.Tasks;
using DAO.Interfaces;
using Manager;
using Manager.Interfaces;
using Models;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using Test.Controller;

namespace Test.Manager
{
    [TestFixture]
    internal class MatchManagerTest
    {
        private IMatchDao _matchDao;
        private IBetDao _betDao;
        private IMatchManager _matchManager;

        private static readonly List<Match> _matches =
            JsonConvert.DeserializeObject<List<Match>>(
                TestHelper.GetDbResponseByCollectionAndFileName("betsByMatch262446"));

        private static readonly List<Bet> _bets =
            JsonConvert.DeserializeObject<List<Bet>>(TestHelper.GetDbResponseByCollectionAndFileName("bets"));

        private readonly Match _match = _matches[0];
        private Bet _bet = _bets[0];

        [OneTimeSetUp]
        public void SetUp()
        {
            _matchDao = Substitute.For<IMatchDao>();
            _betDao = Substitute.For<IBetDao>();
            _matchManager = SingletonManager.Instance.SetMatchManager(new MatchManager(_betDao, _matchDao));
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _matchDao.ClearReceivedCalls();
            _betDao.ClearReceivedCalls();
        }

        [Test]
        public async Task AssertThatCalculateMatchRatingCallsFindBetsByMatchAndUpdateMatches()
        {
            _betDao.FindBetsByMatch(_match).Returns(Task.FromResult(_bets));
            _matchManager.CalculateMatchRating(_match);
            await _betDao.Received().FindBetsByMatch(Arg.Any<Match>());
            _matchDao.Received().UpdateMatch(Arg.Any<int>(), Arg.Any<Match>());
        }

        [Test]
        public void AssertThatCalculateMatchRatingMakesGoodCalculations()
        {
            _betDao.FindBetsByMatch(_match).Returns(Task.FromResult(_bets));
            _matchManager.CalculateMatchRating(_match);
            Assert.IsTrue(_match.AwayTeamRating == 7d);
            Assert.IsTrue(_match.HomeTeamRating == 1.75d);
            Assert.IsTrue(_match.DrawRating == 3.5d);
        }
    }
}