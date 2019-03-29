﻿using System;
using System.Collections.Generic;
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
    public class BetDaoTest
    {
        [SetUp]
        public void SetUp()
        {
            _collection = Substitute.For<IMongoCollection<Bet>>();
            _database = Substitute.For<IMongoDatabase>();
            _betDao = new BetDao(_database, _collection);
            _user = new User {Email = "test", Password = "test", Username = "test"};
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
                Date = DateTime.Now, PointsWon = 1, User = _user, Match = _match
            };
            _bets.Add(_bet);
            _bets.Add(_bet);
        }

        [TearDown]
        public void TearDown()
        {
            _collection.ClearReceivedCalls();
        }

        private Bet _bet;
        private IMongoCollection<Bet> _collection;
        private IBetDao _betDao;
        private User _user;
        private Match _match;
        private readonly List<Bet> _bets = new List<Bet>();
        private IMongoDatabase _database;
        private ExpressionFilterDefinition<Bet> _filterExpression;

        [Test]
        public void AddBetTest()
        {
            _betDao.AddBet(_bet);
            _collection.Received().InsertOneAsync(Arg.Any<Bet>());
        }

        [Test]
        public void AddListBetTest()
        {
            _betDao.AddListBet(_bets);
            _collection.Received().InsertManyAsync(Arg.Any<List<Bet>>());
        }

        [Test]
        public void DeleteBetsByUserTest()
        {
            _betDao.DeleteBetsByUser(new ObjectId("5c06f4b43cd1d72a48b44237"));
            _collection.Received().DeleteManyAsync(Arg.Any<ExpressionFilterDefinition<Bet>>());
        }

        [Test]
        public void FindAllTest()
        {
            _betDao.FindAll();
            _collection.Received().Find(new BsonDocument());
            Assert.IsInstanceOf<Task<List<Bet>>>(_betDao.FindAll());
        }

        [Test]
        public void FindBetsByMatchTest()
        {
            _betDao.FindBetsByMatch(_match);
            _filterExpression = new ExpressionFilterDefinition<Bet>(b => b.Match.Id == _match.Id);
            _collection.Received().Find(_filterExpression);
            Assert.IsInstanceOf<Task<List<Bet>>>(_betDao.FindBetsByMatch(Arg.Any<Match>()));
        }

        [Test]
        public void FindBetsByUserTest()
        {
            _betDao.FindBetsByUser(_user);
            _filterExpression = new ExpressionFilterDefinition<Bet>(bet =>
                bet.User.Id == _user.Id && bet.Date >= DateTime.Today.AddDays(-7));
            _collection.Received().Find(_filterExpression);
            Assert.IsInstanceOf<Task<List<Bet>>>(_betDao.FindBetsByUser(Arg.Any<User>()));
        }

        [Test]
        public void FindBetTest()
        {
            _betDao.Find(_bet);
            _filterExpression = new ExpressionFilterDefinition<Bet>(b => b.Guid == _bet.Guid);
            _collection.Received().Find(_filterExpression);
            Assert.IsInstanceOf<Task<Bet>>(_betDao.Find(Arg.Any<Bet>()));
        }

        [Test]
        public void UpdateBetPointsWonTest()
        {
            _betDao.UpdateBetPointsWon(_bet, 10);
            _collection.Received().UpdateOneAsync(Arg.Any<ExpressionFilterDefinition<Bet>>(),
                Arg.Any<UpdateDefinition<Bet>>()
            );
        }

        [Test]
        public void UpdateBetTest()
        {
            _betDao.UpdateBet(_bet);
            _collection.Received().UpdateOneAsync(Arg.Any<ExpressionFilterDefinition<Bet>>(),
                Arg.Any<UpdateDefinition<Bet>>()
            );
        }
    }
}