using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAO;
using DAO.Interfaces;
using Models;
using MongoDB.Bson;
using MongoDB.Driver;
using NSubstitute;
using NUnit.Framework;

namespace Test.Manager
{
    [TestFixture]
    class ItemManagerTest
    {
        [SetUp]
        public void SetUp()
        {
            _collectionBet = Substitute.For<IMongoCollection<Bet>>();
            _collectionMatch = Substitute.For<IMongoCollection<Match>>();
            _collectionTeam = Substitute.For<IMongoCollection<Team>>();
            _collectionUser = Substitute.For<IMongoCollection<User>>();

            _database = Substitute.For<IMongoDatabase>();
            _betDao = new BetDao(_database, _collectionBet);
            _matchDao = new MatchDao(_database, _collectionMatch);
            _teamDao = new TeamDao(_database, _collectionTeam);
            _userDao = new UserDao(_database, _collectionUser);
            _user = new User
            {
                Email = "test",
                Password = "test",
                Username = "test",
                Id = new ObjectId("5c06f4b43cd1d72a48b44237"),
                TotalPointsUsedToBet = 40,
                Point = 100
            };
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
                Date = DateTime.Now,
                PointsWon = 1,
                User = _user,
                Match = _match
            };
            _item = new Item
            {

            };

        }

        [TearDown]
        public void TearDown()
        {

        }

        private IMongoCollection<Bet> _collectionBet;
        private IMongoCollection<Match> _collectionMatch;
        private IMongoCollection<Team> _collectionTeam;
        private IMongoCollection<User> _collectionUser;
        private Item _item;
        private User _user;
        private Match _match;
        private Bet _bet;
        private IBetDao _betDao;
        private IMatchDao _matchDao;
        private ITeamDao _teamDao;
        private IUserDao _userDao;
        private IMongoDatabase _database;

        [Test]
        public void BuyItemsToUserTest()
        {
            // !TODO
        }
    }
}
