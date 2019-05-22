using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using DAO;
using DAO.Interfaces;
using Models;
using MongoDB.Bson;
using MongoDB.Driver;
using NSubstitute;
using NUnit.Framework;

namespace Test.Manager
{
    [TestFixture]
    internal class BetManagerTest
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
            _match = new Match
            {
                Status = "test",
                LastUpdated = DateTime.Now,
                HomeTeam = null,
                AwayTeam = null,
                Score = null
            };
            _bet = new Bet
            {
                Date = DateTime.Now,
                PointsWon = 1,
                User = _user,
                Match = _match
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
        private Match _match;
        private Bet _bet;
        private IBetDao _betDao;
        private IMatchDao _matchDao;
        private ITeamDao _teamDao;
        private IMongoDatabase _database;

        [Test]
        public void AddGuidListTest()
        {
            var bets = new List<Bet>();
            bets.Add(_bet);
            foreach (var bet in bets)
            {
                bet.Guid = Guid.NewGuid().ToString();
                bet.User = _user;
            }

            foreach (var bet in bets) Assert.IsTrue(bet.Guid != null);
            Assert.IsInstanceOf<Bet>(bets[0]);
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
            var matchesByCompetition = matchByStatus.Result.FindAll(m =>
                m.Competition.Id == competitionId && now <= DateTimeOffset.Parse(m.UtcDate));

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
        public void GetCurrentBetsLimitedTest()
        {
            var betsByUser = _betDao.FindBetsByUser(_user);
            Assert.IsInstanceOf<Task<List<Bet>>>(betsByUser);
            Assert.IsTrue(betsByUser.Result.All(bet => bet.User != null));

            var bets = betsByUser.Result.FindAll(bet => bet.Match.Status == Match.ScheduledStatus)
                .Take(Bet.DashboardMaxToShow)
                .ToList();

            Assert.IsTrue(bets.All(bet => bet.Match.Status == Match.ScheduledStatus));
            Assert.IsInstanceOf<List<Bet>>(bets);
            Assert.IsTrue(bets.Count < Bet.DashboardMaxToShow);
        }

        [Test]
        public void GetFinishBetsLimitedTest()
        {
            var betsByUser = _betDao.FindBetsByUser(_user);
            Assert.IsInstanceOf<Task<List<Bet>>>(betsByUser);
            Assert.IsTrue(betsByUser.Result.All(bet => bet.User != null));

            var bets = betsByUser.Result.FindAll(bet => bet.Match.Status == Match.FinishedStatus)
                .Take(Bet.DashboardMaxToShow)
                .ToList();

            Assert.IsTrue(bets.All(bet => bet.Match.Status == Match.FinishedStatus));
            Assert.IsInstanceOf<List<Bet>>(bets);
            Assert.IsTrue(bets.Count < Bet.DashboardMaxToShow);
        }


        [Test]
        public void GetFinishBetsTest()
        {
            var competitionId = 150;
            var betsByUser = _betDao.FindBetsByUser(_user);
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
        public void GetUserBetsEarningsPerTypeTest()
        {
            var userBets = _betDao.FindBetsByUser(_user);
            Assert.IsInstanceOf<Task<List<Bet>>>(userBets);


            var perfectBetsPoint = 0;
            var okBetsPoint = 0;
            foreach (var bet in userBets.Result)
            {
                switch (bet.Status)
                {
                    case Bet.PerfectStatus:
                        perfectBetsPoint += bet.PointsWon;
                        break;
                    case Bet.OkStatus:
                        okBetsPoint += bet.PointsWon;
                        break;
                }

                Assert.IsTrue(bet.PointsWon >= okBetsPoint);
            }

            dynamic userBetsPerType = new ExpandoObject();
            userBetsPerType.okBets = okBetsPoint;
            userBetsPerType.perfectBets = perfectBetsPoint;

            Assert.IsFalse(userBetsPerType.okBets < 0);
            Assert.IsFalse(userBetsPerType.perfectBets < 0);
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

            dynamic userIncomesPerMonth = userBets.Result.GroupBy(bet => bet.Date.ToString("yyyy/MM"))
                .Select(bet => new
                {
                    Date = bet.Key,
                    Points = bet.Sum(bet2 => bet2.PointsWon)
                }).ToList();

            // TODO Check format of date and points > 0
        }

        [Test]
        public void GetUserIncomesPerYearTest()
        {
            var userBets = _betDao.FindBetsByUser(_user, 1);
            Assert.IsInstanceOf<Task<List<Bet>>>(userBets);

            dynamic userIncomesPerYear = userBets.Result.GroupBy(bet => bet.Date.ToString("yyyy"))
                .Select(bet => new
                {
                    Date = bet.Key,
                    Points = bet.Sum(bet2 => bet2.PointsWon)
                }).ToList();

            // TODO Check format of date and points > 0
        }

        [Test]
        public void GetUserScheduledBetsPaginatedTest()
        {
            var betsByUser = _betDao.FindBetsByUser(_user, 1);
            var page = 1;
            var finishedBets = betsByUser.Result.FindAll(bet => bet.Match.Status == Match.ScheduledStatus);
            var totalBets = finishedBets.Count();
            var totalPages = totalBets / 10 + 1;
            page = page - 1;

            var betsToPass = 10 * page;
            var betsPaginated = _betDao.PaginatedScheduledBets(betsToPass, _user);
            dynamic obj = new ExpandoObject();
            obj.Items = betsPaginated.Result;
            obj.TotalPages = totalPages;
            obj.TotalBets = totalBets;
            obj.Page = page + 1;

            Assert.IsTrue(betsPaginated.Result.Count <= 10);
            Assert.IsInstanceOf<List<Bet>>(betsPaginated.Result);
            Assert.IsFalse(obj.Page < 0);
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
            var matchesByCompetition = matchByStatus.Result.FindAll(m =>
                m.Competition.Id == competitionId && now <= DateTimeOffset.Parse(m.UtcDate));

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
        public void NumberFinishMatchAndBetTest()
        {
            var competitionId = 150;
            var betsByUser = _betDao.FindBetsByUser(_user);
            Assert.IsInstanceOf<Task<List<Bet>>>(betsByUser);
            Assert.IsTrue(betsByUser.Result.All(bet => bet.User != null));

            var betsByCompetition = betsByUser.Result.FindAll(bet => bet.Match.Competition.Id == competitionId);
            var betsByMatchStatus = betsByCompetition.FindAll(bet => bet.Match.Status == Match.FinishedStatus);

            Assert.IsTrue(betsByMatchStatus.All(bet => bet.Match.Status == Match.FinishedStatus));
            Assert.IsInstanceOf<List<Bet>>(betsByCompetition);
            Assert.IsInstanceOf<List<Bet>>(betsByMatchStatus);
            Assert.IsNotNull(betsByCompetition);

            var finishBetsAndMatches = betsByMatchStatus;
            dynamic numberFinishBetsAndMatches = new ExpandoObject();
            numberFinishBetsAndMatches.NbBet = finishBetsAndMatches.Count;
            Assert.IsFalse(numberFinishBetsAndMatches.NbBet < 0);
        }

        [Test]
        public void ParseListBetTest()
        {
            // TODO
            /*   var initialBets = new List<Bet>();
               var betsParsed = new List<Bet>();
               initialBets.Add(_bet);
               var now = DateTime.UtcNow;
               foreach (var bet in initialBets)
               {
                   if (now <= DateTimeOffset.Parse(bet.Match.UtcDate) && (bet.AwayTeamScore >= 0 || bet.HomeTeamScore >= 0))
                   {
                       betsParsed.Add(bet);
                   }
               }
   
               Assert.IsTrue(initialBets.Count == betsParsed.Count);
   
               foreach (var bet in betsParsed)
               {
                   Assert.IsFalse(bet.AwayTeamScore < 0 || bet.HomeTeamScore < 0);
               }*/
        }
    }
}