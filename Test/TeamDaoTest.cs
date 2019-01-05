using System.Collections.Generic;
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
    public class TeamDaoTest
    {
        private Team _team;
        private IMongoCollection<Team> _collection;
        private ITeamDao _teamDao;

        [SetUp]
        public void SetUp()
        {
            _collection = Substitute.For<IMongoCollection<Team>>();
            _teamDao = new TeamDao(_collection);
            _team = new Team { Name = "test" , Email = "test", ShortName = "test", Tla = "test", CrestUrl = "test",
                Address = "test", Phone = "test", Colors = "test", Venue = "test"};
        }

        [TearDown]
        public void TearDown()
        {
            _collection.ClearReceivedCalls();
        }
        
        [Test]
        public void AddTeamTest()
        {
            _teamDao.AddTeam(_team);
            _collection.Received().InsertOneAsync(Arg.Any<Team>());
        }
        
        [Test]
        public void FindTeamTest()
        {
            _teamDao.FindTeam(1);
            _collection.Received().Find(team => team.Id == Arg.Any<int>());
            Assert.IsInstanceOf<Task<Team>>(_teamDao.FindTeam(Arg.Any<int>()));
        }

        [Test]
        public void ReplaceTeamTest()
        {
            _teamDao.ReplaceTeam(1, _team);
            _collection.Received().ReplaceOneAsync(teamFilter => teamFilter.Id == Arg.Any<int>(),
                Arg.Any<Team>(), Arg.Any<UpdateOptions>()
                );
        }
    }
}