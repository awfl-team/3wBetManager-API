using System;
using DAO;
using DAO.Interfaces;
using Models;
using MongoDB.Driver;
using NSubstitute;
using NUnit.Framework;

namespace Test.Manager
{
    [TestFixture]
    internal class AssignmentPointManagerTest
    {
        [SetUp]
        public void SetUp()
        {
            _collection = Substitute.For<IMongoCollection<Bet>>();
            _database = Substitute.For<IMongoDatabase>();
            _betDao = new BetDao(_database, _collection);
            _match = new Match
            {
                Status = "test",
                LastUpdated = DateTime.Now,
                HomeTeam = null,
                AwayTeam = null,
                Score = null
            };
        }

        [TearDown]
        public void TearDown()
        {
        }

        private IMongoCollection<Bet> _collection;
        private IBetDao _betDao;
        private IMongoDatabase _database;
        private Match _match;


        [Test]
        public void AddPointToBet()
        {
            // !TODO
            _betDao.FindBetsByMatch(_match);
        }
    }
}