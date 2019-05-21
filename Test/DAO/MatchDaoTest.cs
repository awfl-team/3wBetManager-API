using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DAO;
using DAO.Interfaces;
using Models;
using MongoDB.Bson;
using MongoDB.Driver;
using NSubstitute;
using NUnit.Framework;

namespace Test.DAO
{
    [TestFixture]
    public class MatchDaoTest
    {
        private Match _match;
        private IMongoCollection<Match> _collection;
        private IMatchDao _matchDao;
        private Team _team1;
        private Team _team2;
        private Score _score;
        private FullTime _fullTime;
        public HalfTime _halfTime;
        public ExtraTime _extraTime;
        public Penalties _penalties;
        private IMongoDatabase _database;
        private ExpressionFilterDefinition<Match> _filterExpression;


        [SetUp]
        public void SetUp()
        {
            _database = Substitute.For<IMongoDatabase>();
            _collection = Substitute.For<IMongoCollection<Match>>();
            _matchDao = new MatchDao(_database, _collection);

            _team1 = new Team
            {
                Name = "test", Email = "test", ShortName = "test", Tla = "test", CrestUrl = "test",
                Address = "test", Phone = "test", Colors = "test", Venue = "test"
            };
            _team2 = new Team
            {
                Name = "test", Email = "test", ShortName = "test", Tla = "test", CrestUrl = "test",
                Address = "test", Phone = "test", Colors = "test", Venue = "test"
            };
            _fullTime = new FullTime {AwayTeam = 1, HomeTeam = 1};
            _halfTime = new HalfTime {AwayTeam = 1, HomeTeam = 1};
            _extraTime = new ExtraTime {AwayTeam = 1, HomeTeam = 1};
            _penalties = new Penalties {AwayTeam = 1, HomeTeam = 1};
            _score = new Score
            {
                Winner = "test", Duration = "test", ExtraTime = _extraTime, FullTime = _fullTime, Penalties = _penalties
            };

            _match = new Match
            {
                Status = "test", LastUpdated = DateTime.Now, HomeTeam = _team1, AwayTeam = _team2,
                Score = _score
            };
        }

        [TearDown]
        public void TearDown()
        {
            _collection.ClearReceivedCalls();
        }

        [Test]
        public void AddMatchTest()
        {
            _matchDao.AddMatch(_match);
            _collection.Received().InsertOneAsync(Arg.Any<Match>());
        }

        [Test]
        public void AssertThatReplaceMatchIsCalled()
        {
            _matchDao.ReplaceMatch(1, _match);
            _collection.Received().ReplaceOneAsync(Arg.Any<ExpressionFilterDefinition<Match>>(),
                Arg.Any<Match>(), Arg.Any<UpdateOptions>(), Arg.Any<CancellationToken>()
            );
        }

        [Test]
        public void AssertThatFindMatchIsCalled()
        {
            _matchDao.FindMatch(1);
            _filterExpression = new ExpressionFilterDefinition<Match>(match => match.Id == _match.Id);
            _collection.Received().Find(_filterExpression);
        }

        [Test]
        public void AssertThatFindAllIsCalled()
        {
            _matchDao.FindAll();
            _collection.Received().Find(new BsonDocument());
            Assert.IsInstanceOf<Task<List<Match>>>(_matchDao.FindAll());
        }

        [Test]
        public void AssertThatFindByStatusIsCalled()
        {
            _matchDao.FindByStatus(_match.Status);
            _filterExpression = new ExpressionFilterDefinition<Match>(m => m.Status == _match.Status);
            _collection.Received().Find(_filterExpression);
        }

        [Test]
        public void AssertThatUpdateMatchIsCalled()
        {
            _matchDao.UpdateMatch(1, _match);
            _collection.Received().UpdateOneAsync(Arg.Any<ExpressionFilterDefinition<Match>>(),
                Arg.Any<UpdateDefinition<Match>>()
            );
        }
    }
}