using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAO;
using DAO.Interfaces;
using Models;
using MongoDB.Bson;
using MongoDB.Driver;
using NSubstitute;
using NSubstitute.Core.Arguments;
using NUnit.Framework;

namespace Test.Manager
{
    [TestFixture]
    class BetManagerTest
    {
        [SetUp]
        public void SetUp()
        {
            _collectionBet = Substitute.For<IMongoCollection<Bet>>();
            _collectionMatch = Substitute.For<IMongoCollection<Match>>();
            _collectionTeam = Substitute.For<IMongoCollection<Team>>();

            _database = Substitute.For<IMongoDatabase>();
            _betDao = new BetDao(_database, _collectionBet);
            _matchDao = new MatchDao(_database, _collectionMatch);
            _teamDao = new TeamDao(_database, _collectionTeam);
            _user = new User
            {
                Email = "test",
                Password = "test",
                Username = "test",
                Id = new ObjectId("5c06f4b43cd1d72a48b44237"),
                TotalPointsUsedToBet = 40,
                Point = 100
            };
        }

        [TearDown]
        public void TearDown()
        {
        }

        private IMongoCollection<Bet> _collectionBet;
        private IMongoCollection<Match> _collectionMatch;
        private IMongoCollection<Team> _collectionTeam;
        private User _user;
        private IBetDao _betDao;
        private IMatchDao _matchDao;
        private ITeamDao _teamDao;
        private IMongoDatabase _database;


        [Test]
        public void GetFinishBetsTest()
        {
            var competitionId = 150;
            var betsByUser =  _betDao.FindBetsByUser(_user);
            Assert.IsInstanceOf<Task<List<Bet>>>(betsByUser);
            Assert.IsTrue(betsByUser.Result.All(bet => bet.User != null));

            var betsByCompetition = betsByUser.Result.FindAll(bet => bet.Match.Competition.Id == competitionId);
            var betsByMatchStatus = betsByCompetition.FindAll(bet => bet.Match.Status == Match.FinishedStatus);

            Assert.IsTrue(betsByMatchStatus.All(bet => bet.Match.Status == Match.FinishedStatus));
            Assert.IsInstanceOf<List<Bet>>(betsByCompetition);
            Assert.IsInstanceOf<List<Bet>>(betsByMatchStatus);
            Assert.IsNotNull(betsByCompetition);
        }

        [Test]
        public void GetFinishBetsLimitedTest()
        {
            var betsByUser = _betDao.FindBetsByUser(_user);
            Assert.IsInstanceOf<Task<List<Bet>>>(betsByUser);
            Assert.IsTrue(betsByUser.Result.All(bet => bet.User != null));

            var bets = betsByUser.Result.FindAll(bet => bet.Match.Status == Match.FinishedStatus).Take(Bet.DashboardMaxToShow)
                .ToList();

            Assert.IsTrue(bets.All(bet => bet.Match.Status == Match.FinishedStatus));
            Assert.IsInstanceOf<List<Bet>>(bets);
            Assert.IsTrue(bets.Count < Bet.DashboardMaxToShow);
        }

        [Test]
        public void GetCurrentBetsLimitedTest()
        {
            var betsByUser = _betDao.FindBetsByUser(_user);
            Assert.IsInstanceOf<Task<List<Bet>>>(betsByUser);
            Assert.IsTrue(betsByUser.Result.All(bet => bet.User != null));

            var bets = betsByUser.Result.FindAll(bet => bet.Match.Status == Match.ScheduledStatus).Take(Bet.DashboardMaxToShow)
                .ToList();

            Assert.IsTrue(bets.All(bet => bet.Match.Status == Match.ScheduledStatus));
            Assert.IsInstanceOf<List<Bet>>(bets);
            Assert.IsTrue(bets.Count < Bet.DashboardMaxToShow);
        }

        [Test]
        public void GetCurrentBetsAndScheduledMatchesTest()
        {
            var competitionId = 150;
            var now = DateTime.UtcNow;
            var betsByUser = _betDao.FindBetsByUser(_user);
            Assert.IsInstanceOf<Task<List<Bet>>>(betsByUser);
            Assert.IsTrue(betsByUser.Result.All(bet => bet.User != null));

            var betsByCompetition = betsByUser.Result.FindAll(bet => bet.Match.Competition.Id == competitionId);
            var betsByMatchStatus = betsByCompetition.FindAll(bet => bet.Match.Status == Match.ScheduledStatus);
            var matchByStatus = _matchDao.FindByStatus(Match.ScheduledStatus);
            var matchesByCompetition = matchByStatus.Result.FindAll(m => m.Competition.Id == competitionId && now <= DateTimeOffset.Parse(m.UtcDate));

            dynamic betsAndMatches = new ExpandoObject();
            betsAndMatches.Bets = betsByMatchStatus;
            betsAndMatches.Matches = matchesByCompetition;

            Assert.IsTrue(betsByMatchStatus.All(bet => bet.Match.Status == Match.ScheduledStatus));
            Assert.IsTrue(betsByCompetition.All(bet => bet.Match.Competition.Id == competitionId));
            Assert.IsInstanceOf<List<Bet>>(betsByCompetition);
            Assert.IsInstanceOf<List<Bet>>(betsByMatchStatus);
            Assert.IsInstanceOf<Task<List<Match>>>(matchByStatus);
            Assert.IsInstanceOf<List<Match>>(matchesByCompetition);
        }

        [Test]
        public void NumberCurrentMatchAndBetTest()
        {
            var competitionId = 150;
            var now = DateTime.UtcNow;
            var betsByUser = _betDao.FindBetsByUser(_user);
            Assert.IsInstanceOf<Task<List<Bet>>>(betsByUser);

            var betsByCompetition = betsByUser.Result.FindAll(bet => bet.Match.Competition.Id == competitionId);
            var betsByMatchStatus = betsByCompetition.FindAll(bet => bet.Match.Status == Match.ScheduledStatus);
            var matchByStatus = _matchDao.FindByStatus(Match.ScheduledStatus);
            var matchesByCompetition = matchByStatus.Result.FindAll(m => m.Competition.Id == competitionId && now <= DateTimeOffset.Parse(m.UtcDate));

            Assert.IsTrue(betsByCompetition.All(bet => bet.Match.Competition.Id == competitionId));
            Assert.IsTrue(matchesByCompetition.All(m => m.Competition.Id == competitionId));

            Assert.IsInstanceOf<List<Bet>>(betsByCompetition);
            Assert.IsInstanceOf<List<Bet>>(betsByMatchStatus);
            Assert.IsInstanceOf<Task<List<Match>>>(matchByStatus);
            Assert.IsInstanceOf<List<Match>>(matchesByCompetition);
            dynamic betsAndMatches = new ExpandoObject();
            betsAndMatches.Bets = betsByMatchStatus;
            betsAndMatches.Matches = matchesByCompetition;

            var currentBetsAndMatches = betsAndMatches;

            dynamic numberCurrentMatchAndBet = new ExpandoObject();
            numberCurrentMatchAndBet.NbBet = currentBetsAndMatches.Bets.Count;
            numberCurrentMatchAndBet.NbMatch = currentBetsAndMatches.Matches.Count;
            Assert.IsFalse(numberCurrentMatchAndBet.NbBet < 0);
            Assert.IsFalse(numberCurrentMatchAndBet.NbMatch < 0);
        }

        [Test]
        public void GetUserBetsPerTypeTest()
        {
            var userBets = _betDao.FindBetsByUser(_user);

            var perfectBets = userBets.Result.FindAll(bet => bet.Status == Bet.PerfectStatus);
            var okBets = userBets.Result.FindAll(bet => bet.Status == Bet.OkStatus);
            var wrongBets = userBets.Result.FindAll(bet => bet.Status == Bet.WrongStatus);

            Assert.IsTrue(perfectBets.All(bet => bet.Status == Bet.PerfectStatus));
            Assert.IsTrue(okBets.All(bet => bet.Status == Bet.OkStatus));
            Assert.IsTrue(wrongBets.All(bet => bet.Status == Bet.WrongStatus));

            dynamic userBetsPerType = new ExpandoObject();
            userBetsPerType.wrongBets = wrongBets.Count;
            userBetsPerType.okBets = okBets.Count;
            userBetsPerType.perfectBets = perfectBets.Count;


            Assert.IsFalse(userBetsPerType.wrongBets < 0);
            Assert.IsFalse(userBetsPerType.okBets < 0);
            Assert.IsFalse(userBetsPerType.perfectBets < 0);
        }

        [Test]
        public void GetUserIncomesPerMonthTest()
        {
            var userBets = _betDao.FindBetsByUser(_user, 1);
            Assert.IsInstanceOf<Task<List<Bet>>>(userBets);

            dynamic userIncomesPerMonth = new ExpandoObject();
            userIncomesPerMonth = userBets.Result.GroupBy(bet => bet.Date.ToString("yyyy/MM"))
                .Select(bet => new
                {
                    Date = bet.Key,
                    Points = bet.Sum(bet2 => bet2.PointsWon)
                }).ToList();

            // TODO Check format of date and points > 0
        }
    }
}
