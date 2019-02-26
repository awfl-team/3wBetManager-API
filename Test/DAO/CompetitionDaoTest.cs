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

        [SetUp]
        public void SetUp()
        {
            _database = Substitute.For<IMongoDatabase>();
            _collection = Substitute.For<IMongoCollection<Competition>>();
            _competitionDao = new CompetitionDao(_database,_collection);
            
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
        public void AddCompetitionTest()
        {
            _competitionDao.AddCompetition(_competition);
            _collection.Received().InsertOneAsync(Arg.Any<Competition>());
        }
        
        [Test]
        public void FindCompetitionTest()
        {
            _competitionDao.FindCompetition(1);
            _collection.Received().Find(Arg.Any<ExpressionFilterDefinition<Competition>>());
            Assert.IsInstanceOf<Task<Competition>>(_competitionDao.FindCompetition(Arg.Any<int>()));
        }
        
        [Test]
        public void FindAllCompetitionTest()
        {
            _competitionDao.FindAllCompetitions();
            _collection.Received().Find(new BsonDocument());
            Assert.IsInstanceOf<Task<List<Competition>>>(_competitionDao.FindAllCompetitions());
        }
        
        [Test]
        public void CompetitionMatchTest()
        {
            _competitionDao.ReplaceCompetition(1, _competition);
            _collection.Received().ReplaceOneAsync(Arg.Any<ExpressionFilterDefinition<Competition>>(),
                Arg.Any<Competition>(), Arg.Any<UpdateOptions>(), Arg.Any<CancellationToken>()
            );
        }
    }
}