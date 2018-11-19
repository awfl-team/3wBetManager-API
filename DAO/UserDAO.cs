using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DAO.Interfaces;
using Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DAO
{
    public class UserDao : IUserDao
    {
        private IMongoCollection<User> _collection;

        public UserDao(IMongoCollection<User> collection = null)
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("3wBetManager");
            _collection = collection ?? database.GetCollection<User>("user");
        }


        public async Task<List<User>> GetAllUser()
        {
            return await _collection.Find(new BsonDocument()).ToListAsync();
        }

        public async Task<User> GetUser(string id)
        {
            var uid = ObjectId.Parse(id);
            return await _collection.Find(user => user.Id == uid).SingleAsync();
        }

        public async void AddUser(User user)
        {
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(user.Password);
            user.Password = passwordHash;
            await _collection.InsertOneAsync(user);
        }

        public async void DeleteUser(string id)
        {
            var uid = ObjectId.Parse(id);
            await _collection.DeleteOneAsync(user => user.Id == uid);
        }

        public void UpdateBook(string id, User userParam)
        {
            var uid = ObjectId.Parse(id);
            _collection.FindOneAndUpdateAsync(
                Builders<User>.Filter.Eq(user => user.Id, uid),
                Builders<User>.Update.Set(user => user.Point, userParam.Point));
        }
    }
}