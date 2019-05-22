using System.Threading;
using System.Threading.Tasks;
using DAO;
using DAO.Interfaces;
using Models;
using MongoDB.Driver;
using NSubstitute;
using NUnit.Framework;

namespace Test.DAO
{
    [TestFixture]
    public class TeamDaoTest
    {
        private Team _team;
        private IMongoCollection<Team> _collection;
        private ITeamDao _teamDao;
        private IMongoDatabase _database;
        private ExpressionFilterDefinition<Team> _filterExpression;

        [SetUp]
        public void SetUp()
        {
            _database = Substitute.For<IMongoDatabase>();
            _collection = Substitute.For<IMongoCollection<Team>>();
            _teamDao = new TeamDao(_database, _collection);
            _team = new Team
            {
                Name = "test", Email = "test", ShortName = "test", Tla = "test", CrestUrl = "test",
                Address = "test", Phone = "test", Colors = "test", Venue = "test"
            };
        }

        [TearDown]
        public void TearDown()
        {
            _collection.ClearReceivedCalls();
        }

        [Test]
        public void AssertThatAddTeamIsCalled()
        {
            _teamDao.AddTeam(_team);
            _collection.Received().InsertOneAsync(Arg.Any<Team>());
        }

        [Test]
        public void AssertThatReplaceTeamIsCalled()
        {
            _teamDao.ReplaceTeam(1, _team);
            _collection.Received().ReplaceOneAsync(Arg.Any<ExpressionFilterDefinition<Team>>(),
                Arg.Any<Team>(), Arg.Any<UpdateOptions>(), Arg.Any<CancellationToken>()
            );
        }

        [Test]
        public void AssertThatFindTeamIsCalled()
        {
            _teamDao.FindTeam(1);
            _filterExpression = new ExpressionFilterDefinition<Team>(team => team.Id == _team.Id);
            _collection.Received().Find(_filterExpression);
        }
    }
}