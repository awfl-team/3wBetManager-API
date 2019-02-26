using System;
using System.Collections.Generic;
using DAO;
using DAO.Interfaces;
using Models;
using MongoDB.Bson;
using MongoDB.Driver;
using NSubstitute;
using NUnit.Framework;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Test.Manager
{
    public class FootballDataManagerTest
    {
        private IMongoCollection<Competition> _collectionCompetition;
        private IMongoCollection<Team> _collectionTeam;
        private IMongoCollection<Match> _collectionMatch;
        private ICompetitionDao _competitionDao;
        private ITeamDao _teamDao;
        private IMatchDao _matchDao;
        private IMongoDatabase _database;
        private HttpClient _http;
        private Competition _competition;
        private Team _team;
        private Team _team1;
        private Team _team2;
        private Match _match;
        private Score _score;
        private FullTime _fullTime;
        private HalfTime _halfTime;
        private ExtraTime _extraTime;
        private Penalties _penalties;
        private ExpressionFilterDefinition<Competition> _filterExpressionCompetition;
        private ExpressionFilterDefinition<Team> _filterExpressionTeam;
        private ExpressionFilterDefinition<Match> _filterExpressionMatch;

        [SetUp]
        public void SetUp()
        {
            _collectionCompetition = Substitute.For<IMongoCollection<Competition>>();
            _collectionTeam = Substitute.For<IMongoCollection<Team>>();
            _collectionMatch = Substitute.For<IMongoCollection<Match>>();
            _http = Substitute.For<HttpClient>();
            _database = Substitute.For<IMongoDatabase>();
            _competitionDao = new CompetitionDao(_database, _collectionCompetition);
            _teamDao = new TeamDao(_database, _collectionTeam);
            _matchDao = new MatchDao(_database, _collectionMatch);
            _http.BaseAddress = new Uri("https://api.football-data.org/v2/");
            _http.DefaultRequestHeaders.Add("X-Auth-Token", "f74e0beb5501485895a1ebb03ba925db");
            _competition = new Competition
            {
                Name = "test",
                Code = "test",
                EmblemUrl = "test",
                Plan = "test",
                LastUpdated = "test"
            };
            _team = new Team
            {
                Name = "test",
                Email = "test",
                ShortName = "test",
                Tla = "test",
                CrestUrl = "test",
                Address = "test",
                Phone = "test",
                Colors = "test",
                Venue = "test"
            };
            _team1 = new Team
            {
                Name = "test",
                Email = "test",
                ShortName = "test",
                Tla = "test",
                CrestUrl = "test",
                Address = "test",
                Phone = "test",
                Colors = "test",
                Venue = "test"
            };
            _team2 = new Team
            {
                Name = "test",
                Email = "test",
                ShortName = "test",
                Tla = "test",
                CrestUrl = "test",
                Address = "test",
                Phone = "test",
                Colors = "test",
                Venue = "test"
            };
            _fullTime = new FullTime { AwayTeam = 1, HomeTeam = 1 };
            _halfTime = new HalfTime { AwayTeam = 1, HomeTeam = 1 };
            _extraTime = new ExtraTime { AwayTeam = 1, HomeTeam = 1 };
            _penalties = new Penalties { AwayTeam = 1, HomeTeam = 1 };
            _score = new Score { Winner = "test", Duration = "test", ExtraTime = _extraTime, FullTime = _fullTime, Penalties = _penalties };

            _match = new Match
            {
                Status = "test",
                LastUpdated = DateTime.Now,
                HomeTeam = _team1,
                AwayTeam = _team2,
                Score = _score

            };
        }

        [TearDown]
        public void TearDown()
        {
            _collectionCompetition.ClearReceivedCalls();
            _collectionTeam.ClearReceivedCalls();
            _collectionMatch.ClearReceivedCalls();
            _http.ClearReceivedCalls();
        }

        [Test]
        public void GetCompetitionTest()
        {
            _competitionDao.FindCompetition(1);
            _filterExpressionCompetition = new ExpressionFilterDefinition<Competition>(competition => competition.Id == _competition.Id);
            _collectionCompetition.Received().Find(_filterExpressionCompetition);
        }

        [Test]
        public void GetAllCompetitionTest()
        {
            _competitionDao.FindAllCompetitions();
            _collectionCompetition.Received().Find(new BsonDocument());
            Assert.IsInstanceOf<Task<List<Competition>>>(_competitionDao.FindAllCompetitions());
        }

        [Test]
        public void GetCompetitionsAddTest()
        {
            _competitionDao.AddCompetition(_competition);
            _collectionCompetition.Received().InsertOneAsync(Arg.Any<Competition>());
        }

        [Test]
        public void GetCompetitionsReplaceTest()
        {
            _competitionDao.ReplaceCompetition(1, _competition);
            _collectionCompetition.Received().ReplaceOneAsync(Arg.Any<ExpressionFilterDefinition<Competition>>(),
                Arg.Any<Competition>(), Arg.Any<UpdateOptions>(), Arg.Any<CancellationToken>()
            );
        }

        [Test]
        public void GetCompetitionsHttpTest()
        { 
            _http.GetAsync("competitions/" + 1);
            _http.Received().GetAsync("test");
        }

        [Test]
        public void GetTeamTest()
        {
            _teamDao.FindTeam(1);
            _filterExpressionTeam = new ExpressionFilterDefinition<Team>(team => team.Id == _team.Id);
            _collectionTeam.Received().Find(_filterExpressionTeam);
        }

        [Test]
        public void GetAllTeamsReplaceTest()
        {
            _teamDao.ReplaceTeam(1, _team);
            _collectionTeam.Received().ReplaceOneAsync(Arg.Any<ExpressionFilterDefinition<Team>>(),
                Arg.Any<Team>(), Arg.Any<UpdateOptions>(), Arg.Any<CancellationToken>()
            );
        }

        [Test]
        public void GetAllTeamsHttpTest()
        {
            _http.GetAsync("competitions/" + 1 + "/teams");
            _http.Received().GetAsync("test");
        }

        [Test]
        public void GetMatchTest()
        {
            _matchDao.FindMatch(1);
            _filterExpressionMatch = new ExpressionFilterDefinition<Match>(m => m.Status == _match.Status);
            _collectionMatch.Received().Find(_filterExpressionMatch);
        }

        [Test]
        public void GetAllMatchAddTest()
        {
            _matchDao.AddMatch(_match);
            _collectionMatch.Received().InsertOneAsync(Arg.Any<Match>());
        }

        [Test]
        public void GetAllMatchReplaceTest()
        {
            _matchDao.ReplaceMatch(1, _match);
            _collectionMatch.Received().ReplaceOneAsync(Arg.Any<ExpressionFilterDefinition<Match>>(),
                Arg.Any<Match>(), Arg.Any<UpdateOptions>(), Arg.Any<CancellationToken>()
            );
        }

        [Test]
        public void GetAllMatchHttpTest()
        {
            _http.GetAsync("matches?dateFrom=" + DateTime.Now.AddDays(7).ToString("yyyy-MM-dd") + "&dateTo=" +
                           DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd"));
            _http.Received().GetAsync("test");
        }
    }
}
