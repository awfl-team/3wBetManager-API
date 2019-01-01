using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAO.Interfaces;
using Models;
using MongoDB.Driver;

namespace DAO
{
    public class MatchDao: IMatchDao
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
                new UpdateOptions { IsUpsert = true }
            );
        }

        public async Task<Match> FindMatch(int id)
        {
            var result = await _collection.Find(match => match.Id == id).ToListAsync();
            return result.FirstOrDefault();
        }
    }
}
