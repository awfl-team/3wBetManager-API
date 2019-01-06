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

namespace Test
{
    public class MatchDaoTest
    {
        private Match _match;
        private IMongoCollection<Match> _collection;
        private IMatchDao _matchDao;
        private Team _team1;
        private Team _team2;

        [SetUp]
        public void SetUp()
        {
            _collection = Substitute.For<IMongoCollection<Match>>();
            _matchDao = new MatchDao(_collection);
            
            _team1 = new Team { Name = "test" , Email = "test", ShortName = "test", Tla = "test", CrestUrl = "test",
                Address = "test", Phone = "test", Colors = "test", Venue = "test"};
            _team2 = new Team { Name = "test" , Email = "test", ShortName = "test", Tla = "test", CrestUrl = "test",
                Address = "test", Phone = "test", Colors = "test", Venue = "test"};
            
            _match = new Match
            {
                Cote = 123, Status = "test", LastUpdated = Arg.Any<DateTime>(), HomeTeam = _team1, AwayTeam = _team2,
                Score = Arg.Any<Score>()
                
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
        public void FindMatchTest()
        {
            _matchDao.FindMatch(1);
            _collection.Received().Find(match => match.Id == Arg.Any<int>());
            Assert.IsInstanceOf<Task<Match>>(_matchDao.FindMatch(Arg.Any<int>()));
        }
        
        [Test]
        public void ReplaceMatchTest()
        {
            _matchDao.ReplaceMatch(1, _match);
            _collection.Received().ReplaceOneAsync(Arg.Any<ExpressionFilterDefinition<Match>>(),
                Arg.Any<Match>(), Arg.Any<UpdateOptions>(), Arg.Any<CancellationToken>()
            );
        }
    }
}