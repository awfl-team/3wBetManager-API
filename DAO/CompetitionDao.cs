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
    public class CompetitionDao : ICompetitionDao
    {
        private readonly IMongoCollection<Competition> _collection;

        public CompetitionDao(IMongoCollection<Competition> collection = null)
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("3wBetManager");
            _collection = collection ?? database.GetCollection<Competition>("competition");
        }

        public async Task<List<Competition>> FindAllCompetitions()
        {
            return await _collection.Find(new BsonDocument()).ToListAsync();
        }

        public async void AddCompetition(Competition competition)
        {
            await _collection.InsertOneAsync(competition);
        }

        public async void ReplaceCompetition(int id, Competition competition)
        {
            await _collection.ReplaceOneAsync(competitionFilter => competitionFilter.Id == id,
                competition,
                new UpdateOptions {IsUpsert = true}
            );
        }

        public async Task<Competition> FindCompetition(int id)
        {
            var result = await _collection.Find(competition => competition.Id == id).ToListAsync();
            return result.FirstOrDefault();
        }
    }
}