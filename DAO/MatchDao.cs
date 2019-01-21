using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public MatchDao(IMongoCollection<Match> collection = null)
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("3wBetManager");
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

        public async void UpdateMatch(int id, Match matchParam)
        {
            await _collection.UpdateOneAsync(
                match => match.Id == id,
                Builders<Match>.Update.Set(match => match.Status, matchParam.Status)
                    .Set(match => match.LastUpdated, matchParam.LastUpdated)
                    .Set(match => match.HomeTeam, matchParam.HomeTeam)
                    .Set(match => match.Score, matchParam.Score)
                    .Set(match => match.AwayTeam, matchParam.AwayTeam)
                    .Set(match => match.Competition, matchParam.Competition)
                    .Set(match => match.UtcDate, matchParam.UtcDate)
            );
        }
    }
}