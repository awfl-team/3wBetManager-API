using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using DAO;
using DAO.Interfaces;
using Models;
using MongoDB.Bson;
using MongoDB.Driver;
using NSubstitute;
using NUnit.Framework;
using _3wBetManager_API.Controllers;
using _3wBetManager_API.Manager;

namespace Test
{
    [TestFixture]
    public class BetDaoTest
    {
        private Bet _bet;
        private IMongoCollection<Bet> _collection;
        private IBetDao _betDao;
        private User _user;
        private Match _match;
        private List<Bet> _bets = new List<Bet>();

        [SetUp]
        public void SetUp()
        {
            _collection = Substitute.For<IMongoCollection<Bet>>();
            _betDao = new BetDao(_collection);
            _user = new User { Email = "test", Password = "test", Username = "test" };
            _match = new Match
            {
                Cote = 123,
                Status = "test",
                LastUpdated = DateTime.Now,
                HomeTeam = null,
                AwayTeam = null,
                Score = null

            };
            _bet = new Bet()
            {
                Date = DateTime.Now, PointsWon = 1,User = _user , Match = _match
            };
            _bets.Add(_bet);
            _bets.Add(_bet);

        }

        [TearDown]
        public void TearDown()
        {
            _collection.ClearReceivedCalls();
        }

        [Test]
        public void AddTest()
        {
            _betDao.AddBet(_bet);
            _collection.Received().InsertOneAsync(Arg.Any<Bet>());
        }

        [Test]
        public void AddTestList()
        {
            _betDao.AddListBet(_bets);
            _collection.Received().InsertManyAsync(Arg.Any<List<Bet>>());
        }
    }
}
