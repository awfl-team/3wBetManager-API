using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAO.Interfaces;
using Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DAO
{
    public class MatchDao : IMatchDao
    {
        private readonly IMongoCollection<Match> _collection;

        public MatchDao(IMongoDatabase database, IMongoCollection<Match> collection = null)
        {
            _collection = collection ?? database.GetCollection<Match>("match");
        }

        public async void AddMatch(Match match)
        {
            await _collection.InsertOneAsync(match);
        }

        public async void ReplaceMatch(int id, Match match)
        {
            await _collection.ReplaceOneAsync(matchFilter => matchFilter.Id == id,
                match,
                new UpdateOptions {IsUpsert = true}
            );
        }

        public async Task<Match> FindMatch(int id)
        {
            var result = await _collection.Find(match => match.Id == id).ToListAsync();
            return result.FirstOrDefault();
        }

        public async Task<List<Match>> FindAll()
        {
            return await _collection.Find(new BsonDocument()).ToListAsync();
        }

        public async Task<List<Match>> FindByStatus(string status)
        {
            return await _collection.Find(m => m.Status == status).ToListAsync();
        }

        public async void UpdateMatch(int id, Match matchParam)
        {
            await _collection.UpdateOneAsync(
                match => match.Id == id,
                Builders<Match>.Update.Set(match => match.Status, matchParam.Status)
                    .Set(match => match.LastUpdated, matchParam.LastUpdated)
                    .Set(match => match.HomeTeam, matchParam.HomeTeam)
                    .Set(match => match.HomeTeamRating, matchParam.HomeTeamRating)
                    .Set(match => match.Score, matchParam.Score)
                    .Set(match => match.AwayTeam, matchParam.AwayTeam)
                    .Set(match => match.AwayTeamRating, matchParam.AwayTeamRating)
                    .Set(match => match.DrawRating, matchParam.DrawRating)
                    .Set(match => match.Competition, matchParam.Competition)
                    .Set(match => match.UtcDate, matchParam.UtcDate)
            );
        }
    }
}