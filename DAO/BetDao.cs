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

        public BetDao(IMongoDatabase database, IMongoCollection<Bet> collection = null)
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

        public async Task<List<Bet>> FindBetsByUser(User user, int sortDirection = -1)
        {
            return await _collection.Find(bet => bet.User.Id == user.Id).Sort("{Date: " + sortDirection + "}")
                .ToListAsync();
        }

        public async Task<List<Bet>> FindBetsByMatch(Match match)
        {
            return await _collection.Find(bet => bet.Match.Id == match.Id).ToListAsync();
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

        public async void UpdateBetPointsWon(Bet bet, double point)
        {
            await _collection.UpdateOneAsync(b => b.Id == bet.Id,
                Builders<Bet>.Update.Set(b => b.PointsWon, point));
        }

        public async void UpdateBetStatus(Bet bet, string status)
        {
            await _collection.UpdateOneAsync(b => b.Id == bet.Id,
                Builders<Bet>.Update.Set(b => b.Status, status));
        }

        public async Task UpdateBetMultiply(string id, int multiply)
        {
            await _collection.UpdateOneAsync(b => b.Id == ObjectId.Parse(id),
                Builders<Bet>.Update.Set(b => b.Multiply, multiply));
        }

        public async Task<List<Bet>> PaginatedScheduledBets(int betsToPass, User user)
        {
            return await _collection.Find(bet => bet.Match.Status == Match.ScheduledStatus && bet.User.Id == user.Id).Skip(betsToPass).Limit(10)
                .ToListAsync();
        }
    }
}