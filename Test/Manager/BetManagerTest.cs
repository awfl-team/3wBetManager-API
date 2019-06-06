using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using DAO;
using DAO.Interfaces;
using Manager;
using Manager.Interfaces;
using Models;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using Test.Controller;

namespace Test.Manager
{
    [TestFixture]
    internal class BetManagerTest
    {
        private IBetManager _betManager;
        private IBetDao _betDao;
        private IMatchDao _matchDao;
        private ITeamDao _teamDao;
        private List<Bet> _betsByUser;

        private static readonly List<User> Users =
            JsonConvert.DeserializeObject<List<User>>(TestHelper.GetDbResponseByCollectionAndFileName("users"));

        private User _user = Users[0];
        private Team _team;
        private Match _match;
        private Match _matchScheduled;
        private List<Match> _matchesScheduled;
        private List<Bet> _bets;
        private IBetManager _betManagerMock;

        [OneTimeSetUp]
        public void SetUp()
        {
            _betDao = Substitute.For<IBetDao>();
            _teamDao = Substitute.For<ITeamDao>();
            _matchDao = Substitute.For<IMatchDao>();
            _betManagerMock = Substitute.For<IBetManager>();
            _betManager = SingletonManager.Instance.SetBetManager(new BetManager(_betDao, _teamDao, _matchDao));
            _betsByUser =
                JsonConvert.DeserializeObject<List<Bet>>(TestHelper.GetDbResponseByCollectionAndFileName("betsByUser"));
            _team = JsonConvert.DeserializeObject<Team>(TestHelper.GetDbResponseByCollectionAndFileName("team"));
            _match = JsonConvert.DeserializeObject<Match>(TestHelper.GetDbResponseByCollectionAndFileName("match"));
            _matchScheduled =
                JsonConvert.DeserializeObject<Match>(TestHelper.GetDbResponseByCollectionAndFileName("matchScheduled"));
            _matchesScheduled =
                JsonConvert.DeserializeObject<List<Match>>(
                    TestHelper.GetDbResponseByCollectionAndFileName("matchesScheduled"));
            _bets = JsonConvert.DeserializeObject<List<Bet>>(TestHelper.GetDbResponseByCollectionAndFileName("bets"));
        }

        [TearDown]
        public void TearDown()
        {
            _betDao.ClearReceivedCalls();
            _teamDao.ClearReceivedCalls();
            _matchDao.ClearReceivedCalls();
        }

        [Test]
        public async Task AssertThatGetFinishBetsCallsFindBetsByUserAndReturnsOnlyFinishedBets()
        {
            _betDao.FindBetsByUser(Arg.Any<User>()).Returns(Task.FromResult(_betsByUser));
            _teamDao.FindTeam(Arg.Any<int>()).Returns(Task.FromResult(_team));
            _matchDao.FindMatch(Arg.Any<int>()).Returns(Task.FromResult(_match));

            var finishedBets = await _betManager.GetFinishBets(_user, 2000);

            await _betDao.Received().FindBetsByUser(Arg.Any<User>());
            await _teamDao.Received().FindTeam(Arg.Any<int>());
            await _matchDao.Received().FindMatch(Arg.Any<int>());

            Assert.IsNotEmpty(finishedBets);
            Assert.IsTrue(finishedBets.All(bet => bet.Match.Status == Match.FinishedStatus));
            Assert.IsTrue(finishedBets.All(b => b.Match.Competition.Id == 2000));
        }

        [Test]
        public async Task AssertThatGetFinishBetsLimitedCallsFindBetsByUserAndReturnsOnlyFinishedBets()
        {
            _betDao.FindBetsByUser(Arg.Any<User>(), Arg.Any<int>()).Returns(Task.FromResult(_betsByUser));
            _teamDao.FindTeam(Arg.Any<int>()).Returns(Task.FromResult(_team));
            _matchDao.FindMatch(Arg.Any<int>()).Returns(Task.FromResult(_match));

            var finishedBetsLimited = await _betManager.GetFinishBetsLimited(_user);

            await _betDao.Received().FindBetsByUser(Arg.Any<User>(), Arg.Any<int>());
            await _teamDao.Received().FindTeam(Arg.Any<int>());
            await _matchDao.Received().FindMatch(Arg.Any<int>());

            Assert.IsNotEmpty(finishedBetsLimited);
            Assert.IsTrue(finishedBetsLimited.All(bet => bet.Match.Status == Match.FinishedStatus));
            Assert.IsTrue(finishedBetsLimited.Count < Bet.DashboardMaxToShow);
        }


        [Test]
        public async Task AssertThatGetCurrentBetsLimitedCallsFindBetsByUseAndReturnsOnlyScheduledBets()
        {
            _betDao.FindBetsByUser(Arg.Any<User>(), Arg.Any<int>()).Returns(Task.FromResult(_betsByUser));
            _teamDao.FindTeam(Arg.Any<int>()).Returns(Task.FromResult(_team));
            _matchDao.FindMatch(Arg.Any<int>()).Returns(Task.FromResult(_matchScheduled));

            var currentBetsLimited = await _betManager.GetCurrentBetsLimited(_user);

            await _betDao.Received().FindBetsByUser(Arg.Any<User>(), Arg.Any<int>());
            await _teamDao.Received().FindTeam(Arg.Any<int>());
            await _matchDao.Received().FindMatch(Arg.Any<int>());

            Assert.IsNotEmpty(currentBetsLimited);
            Assert.IsTrue(currentBetsLimited.All(bet => bet.Match.Status == Match.ScheduledStatus));
            Assert.IsTrue(currentBetsLimited.Count < Bet.DashboardMaxToShow);
        }

        [Test]
        public async Task AssertThatAddBetsCallsAddListBet()
        {
            await _betManager.AddBets(_bets);
            await _betDao.Received().AddListBet(Arg.Any<List<Bet>>());
        }

        [Test]
        public void AssertThatParseListBetReturnsValidListBet()
        {
            var betsparsed = _betManager.ParseListBet(_bets);

            Assert.IsTrue(betsparsed.All(b => DateTime.Now <= DateTimeOffset.Parse(b.Match.UtcDate)));
            Assert.IsTrue(betsparsed.All(b => b.HomeTeamScore >= 0));
            Assert.IsTrue(betsparsed.All(b => b.AwayTeamScore >= 0));
        }

        [Test]
        public async Task AssertThatChangeBetCallsUpdateBet()
        {
            await _betManager.ChangeBet(_bets[0]);
            await _betDao.Received().UpdateBet(Arg.Any<Bet>());
        }

        [Test]
        public async Task AssertThatGetUserBetsPerTypeReturnsValidDynamicObject()
        {
            _betDao.FindBetsByUser(Arg.Any<User>(), Arg.Any<int>()).Returns(Task.FromResult(_betsByUser));

            var betsPerType = await _betManager.GetUserBetsPerType(_user);

            Assert.IsTrue(betsPerType.wrongBets != null);
            Assert.IsTrue(betsPerType.okBets != null);
            Assert.IsTrue(betsPerType.perfectBets != null);
        }

        [Test]
        public async Task AssertThatNumberCurrentMatchAndBetReturnsValidDynamicObject()
        {
            dynamic currentBetsAndScheduledMatches = new ExpandoObject();
            currentBetsAndScheduledMatches.Bets = _bets;
            currentBetsAndScheduledMatches.Matches = _matchesScheduled;

            //TODO improve this 
            _betManagerMock.GetCurrentBetsAndScheduledMatches(Arg.Any<User>(), Arg.Any<int>())
                .Returns(Task.FromResult("test"));
            var numberCurrentMatch = await _betManager.NumberCurrentMatchAndBet(_user, 2000);

            Assert.IsTrue(numberCurrentMatch.NbBet != null);
            Assert.IsTrue(numberCurrentMatch.NbMatch != null);
        }

        [Test]
        public async Task AssertThatNumberFinishMatchAndBetReturnsValidDynamicObject()
        {
            //TODO improve this 
            _betManagerMock.GetFinishBets(Arg.Any<User>(), Arg.Any<int>()).Returns(Task.FromResult(_bets));
            var numberFinishMatchAndBet = await _betManager.NumberFinishMatchAndBet(_user, 2000);
            Assert.IsTrue(numberFinishMatchAndBet.NbBet != null);
        }

        [Test]
        public void AssertThatAddGuidListReturnsValidBetList()
        {
            var bets = _betManager.AddGuidList(_user, _bets);
            Assert.IsTrue(bets.All(b => b.Guid != null));
            Assert.IsTrue(bets.All(b => b.User != null));
        }

        [Test]
        public async Task
            AssertThatGetCurrentBetsAndScheduledMatchesCallsFindBetsByUseAndReturnsOnlyScheduledBetsAndMatch()
        {
            _betDao.FindBetsByUser(Arg.Any<User>()).Returns(Task.FromResult(_betsByUser));
            _teamDao.FindTeam(Arg.Any<int>()).Returns(Task.FromResult(_team));
            _matchDao.FindMatch(Arg.Any<int>()).Returns(Task.FromResult(_matchScheduled));
            _matchDao.FindByStatus(Match.ScheduledStatus).Returns(Task.FromResult(_matchesScheduled));

            var currentBetsAndMatch = await _betManager.GetCurrentBetsAndScheduledMatches(_user, 2001);

            await _betDao.Received().FindBetsByUser(Arg.Any<User>());
            await _teamDao.Received().FindTeam(Arg.Any<int>());
            await _matchDao.Received().FindMatch(Arg.Any<int>());
            await _matchDao.Received().FindByStatus(Arg.Any<string>());

            var bets = currentBetsAndMatch.Bets as List<Bet>;
            var matches = currentBetsAndMatch.Matches as List<Match>;

            Assert.IsNotEmpty(bets, "bets empty");
            Assert.IsTrue(bets.All(b => b.Match.Status == Match.ScheduledStatus));
            Assert.IsTrue(bets.All(b => b.Match.Competition.Id == 2001));

            Assert.IsNotEmpty(matches, "matches empty");
            Assert.IsTrue(matches.All(m => m.Status == Match.ScheduledStatus));
            Assert.IsTrue(matches.All(m => m.Competition.Id == 2001));
        }
    }
}