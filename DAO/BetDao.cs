using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAO.Interfaces;
using Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DAO
{
    public class BetDao : IBetDao
    {
        private readonly IMongoCollection<Bet> _collection;

        public BetDao(IMongoDatabase database,IMongoCollection<Bet> collection = null)
        {
            _collection = collection ?? database.GetCollection<Bet>("bet");
        }

        public async Task<List<Bet>> FindAll()
        {
            return await _collection.Find(new BsonDocument()).ToListAsync();
        }

        public async Task<Bet> Find(Bet bet)
        {
            var result = await _collection.Find(b => b.Guid == bet.Guid).ToListAsync();
            return result.FirstOrDefault();
        }

        public async Task<List<Bet>> FindBetsByUser(User user)
        {
            return await _collection.Find(bet => bet.User.Id == user.Id).Sort("{Date: -1}").ToListAsync();
        }

        public async Task<List<Bet>> FindBetsByUserLimited(User user)
        {
            return await _collection.Find(bet => bet.User.Id == user.Id && bet.Date >= DateTime.Today.AddDays(-7)).Sort("{Date: -1}").ToListAsync();
        }

        public async Task AddBet(Bet bet)
        {
          await _collection.InsertOneAsync(bet);
        }

        public async Task AddListBet(List<Bet> bets)
        {
            await _collection.InsertManyAsync(bets);
        }


        public async Task<UpdateResult> UpdateBet(Bet bet)
        {
            return await _collection.UpdateOneAsync(b => b.Guid == bet.Guid,
                Builders<Bet>.Update.Set(b => b.HomeTeamScore, bet.HomeTeamScore)
                    .Set(b => b.AwayTeamScore, bet.AwayTeamScore));
        }
        
        public async void DeleteBetsByUser(ObjectId id)
        {
            await _collection.DeleteManyAsync(bet => bet.User.Id == id);
        }

        public async Task<List<Bet>> FindBetsByMatch(Match match)
        {
            var result = await _collection.Find(b => b.Match.Id == match.Id).ToListAsync();
            return result;
        }

        public async void UpdateBetPointsWon(Bet bet, int point)
        {
            await _collection.UpdateOneAsync(b => b.Id == bet.Id,
                Builders<Bet>.Update.Set(b => b.PointsWon, point));
        }
    }
}