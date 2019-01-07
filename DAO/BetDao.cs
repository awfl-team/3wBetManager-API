using System.Collections.Generic;
using DAO.Interfaces;
using Models;
using MongoDB.Driver;

namespace DAO
{
    public class BetDao: IBetDao
    {
        private readonly IMongoCollection<Bet> _collection;

        public BetDao(IMongoCollection<Bet> collection = null)
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("3wBetManager");
            _collection = collection ?? database.GetCollection<Bet>("bet");
        }

        public async void AddBet(Bet bet)
        {
            await _collection.InsertOneAsync(bet);
        }

        public async void AddListBet(List<Bet> bets)
        {
            await _collection.InsertManyAsync(bets);
        }
    }
}
