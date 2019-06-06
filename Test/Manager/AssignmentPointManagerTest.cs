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
    internal class AssignmentPointManagerTest
    {
        private IBetDao _betDao;
        private IAssignmentPointManager _assignmentPointManager;

        private static readonly List<Match> _matches =
            JsonConvert.DeserializeObject<List<Match>>(
                TestHelper.GetDbResponseByCollectionAndFileName("matchesHomeAwayAndDraw"));

        private static readonly List<Bet> _PerfectBets =
            JsonConvert.DeserializeObject<List<Bet>>(TestHelper.GetDbResponseByCollectionAndFileName("perfectBets"));

        private static readonly List<Bet> _OKBets =
            JsonConvert.DeserializeObject<List<Bet>>(TestHelper.GetDbResponseByCollectionAndFileName("okBets"));

        private static readonly List<Bet> _WrongBets =
            JsonConvert.DeserializeObject<List<Bet>>(TestHelper.GetDbResponseByCollectionAndFileName("wrongBets"));

        private static readonly Match _matchHomeTeamWin = _matches[0];
        private static readonly Match _matchHomeAwayWin = _matches[1];
        private static readonly Match _matchHomeDraw = _matches[2];

        private static readonly object[] BetsTestCase =
        {
            new object[] {_PerfectBets[2], "DRAW"},
            new object[] {_PerfectBets[1], "AWAY"},
            new object[] {_PerfectBets[0], "HOME"}
        };

        private static readonly object[] PerfectBetsTestCase =
        {
            new object[] {_matchHomeDraw, new List<Bet> {_PerfectBets[2]}},
            new object[] {_matchHomeTeamWin, new List<Bet> {_PerfectBets[0]}},
            new object[] {_matchHomeAwayWin, new List<Bet> {_PerfectBets[1]}}
        };

        private static readonly object[] OkBetsTestCase =
        {
            new object[] {_matchHomeDraw, new List<Bet> {_OKBets[2]}},
            new object[] {_matchHomeTeamWin, new List<Bet> {_OKBets[0]}},
            new object[] {_matchHomeAwayWin, new List<Bet> {_OKBets[1]}}
        };

        private static readonly object[] WrongBetsTestCase =
        {
            new object[] {_matchHomeDraw, new List<Bet> {_WrongBets[2]}},
            new object[] {_matchHomeTeamWin, new List<Bet> {_WrongBets[0]}},
            new object[] {_matchHomeAwayWin, new List<Bet> {_WrongBets[1]}}
        };

        [OneTimeSetUp]
        public void SetUp()
        {
            _betDao = Substitute.For<IBetDao>();
            _assignmentPointManager =
                SingletonManager.Instance.SetAssignmentPointManager(new AssignmentPointManager(_betDao));
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _betDao.ClearReceivedCalls();
        }

        [TestCaseSource("BetsTestCase")]
        public void AssertThatGetTeamNameWithTheBestHightScoreReturnsTheRighValue(Bet bet, string expectedResult)
        {
            var result = _assignmentPointManager.GetTeamNameWithTheBestHightScore(bet);
            Assert.IsTrue(result == expectedResult);
            Assert.NotNull(result);
        }

        [TestCaseSource("WrongBetsTestCase")]
        public void AssertThatAddPointToBetCallsRightUpdateFunctionsForWrongBets(Match match, List<Bet> wrongBets)
        {
            _betDao.FindBetsByMatch(match).Returns(Task.FromResult(wrongBets));
            _assignmentPointManager.AddPointToBet(match);
            foreach (var bet in wrongBets)
            {
                _betDao.Received().UpdateBetPointsWon(bet, Bet.WrongBet);
                _betDao.Received().UpdateBetStatus(bet, Bet.WrongStatus);
            }
        }

        [TestCaseSource("PerfectBetsTestCase")]
        public void AssertThatAddPointToBetCallsRightUpdateFunctionsForPerfectBets(Match match, List<Bet> perfectBet)
        {
            _betDao.FindBetsByMatch(match).Returns(Task.FromResult(perfectBet));
            _assignmentPointManager.AddPointToBet(match);
            foreach (var bet in perfectBet)
            {
                _betDao.Received().UpdateBetPointsWon(bet,
                    Bet.PerfectBet * match.AwayTeamRating * 4);
                _betDao.Received().UpdateBetStatus(bet, Bet.PerfectStatus);
            }
        }

        [TestCaseSource("OkBetsTestCase")]
        public void AssertThatAddPointToBetCallsRightUpdateFunctionsForOkBets(Match match, List<Bet> okBets)
        {
            _betDao.FindBetsByMatch(match).Returns(Task.FromResult(okBets));
            _assignmentPointManager.AddPointToBet(match);
            foreach (var bet in okBets)
            {
                _betDao.Received().UpdateBetPointsWon(bet, Bet.OkBet * match.HomeTeamRating * bet.Multiply);
                _betDao.Received().UpdateBetStatus(bet, Bet.OkStatus);
            }
        }

        [Test]
        public void AssertThatAddPointToBetCallsFindBet()
        {
            _assignmentPointManager.AddPointToBet(_matchHomeDraw);
            _betDao.Received().FindBetsByMatch(_matchHomeDraw);
        }
    }
}