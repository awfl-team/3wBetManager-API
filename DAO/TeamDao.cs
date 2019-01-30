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
    public class TeamDao: ITeamDao
    {
        private readonly IMongoCollection<Team> _collection;

        public TeamDao(IMongoCollection<Team> collection = null)
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("3wBetManager");
            _collection = collection ?? database.GetCollection<Team>("team");
        }

        public async void AddTeam(Team team)
        {
            await _collection.InsertOneAsync(team);
        }

        public async void ReplaceTeam(int id, Team team)
        {
            await _collection.ReplaceOneAsync(teamFilter => teamFilter.Id == id,
                team,
                new UpdateOptions { IsUpsert = true }
            );
        }

        public async Task<Team> FindTeam(int id)
        {
            var result = await _collection.Find(team => team.Id == id).ToListAsync();
            return result.FirstOrDefault();
        }
    }
}
