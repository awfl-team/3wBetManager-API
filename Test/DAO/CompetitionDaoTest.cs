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
    public class CompetitionDaoTest
    {

        private Competition _competition;
        private IMongoCollection<Competition> _collection;
        private ICompetitionDao _competitionDao;
        private IMongoDatabase _database;
        private ExpressionFilterDefinition<Competition> _filterExpression;

        [SetUp]
        public void SetUp()
        {
            _database = Substitute.For<IMongoDatabase>();
            _collection = Substitute.For<IMongoCollection<Competition>>();
            _competitionDao = new CompetitionDao(_database, _collection);

            _competition = new Competition
            {
                Name = "test", Code = "test", EmblemUrl = "test", Plan = "test", LastUpdated = "test"
            };
        }

        [TearDown]
        public void TearDown()
        {
            _collection.ClearReceivedCalls();
        }

        [Test]
        public void FindAllCompetitionTest()
        {
            _competitionDao.FindAllCompetitions();
            _collection.Received().Find(new BsonDocument());
        }

        [Test]
        public void AssertThatAddCompetitionIsCalled()
        {
            _competitionDao.AddCompetition(_competition);
            _collection.Received().InsertOneAsync(Arg.Any<Competition>());
        }

        [Test]
        public void AssertThatReplaceCompetitionIsCalled()
        {
            _competitionDao.ReplaceCompetition(1, _competition);
            _collection.Received().ReplaceOneAsync(Arg.Any<ExpressionFilterDefinition<Competition>>(),
                Arg.Any<Competition>(), Arg.Any<UpdateOptions>(), Arg.Any<CancellationToken>()
            );
        }

        [Test]
        public void AssertThatFindCompetitionIsCalled()
        {
            _competitionDao.FindCompetition(_competition.Id);
            _filterExpression =
                new ExpressionFilterDefinition<Competition>(competition => competition.Id == _competition.Id);
            _collection.Received().Find(_filterExpression);
        }
    }
}