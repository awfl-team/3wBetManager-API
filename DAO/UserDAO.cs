using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DAO
{
    public class UserDao: IDisposable
    {
        private IMongoCollection<User> _collection;

        public UserDao(IMongoCollection<User> collection = null)
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("3wBetManager");
            _collection = collection ?? database.GetCollection<User>("user");
        }

        public void Dispose()
        {
            _collection = null;
        }

        public async Task<List<User>> GetAllUser()
        {
            return await _collection.Find(new BsonDocument()).ToListAsync();
        }

        public async Task<User> GetUser(ObjectId uid)
        {
            return await _collection.Find(user => user.UId == uid).SingleAsync();
        }

        public async void AddUser(User user)
        {
            await _collection.InsertOneAsync(user);
        }

        public async void DeleteUser(ObjectId uId)
        {
            await _collection.DeleteOneAsync(user => user.UId == uId);
        }

        public void UpdateBook(ObjectId uId, User userParam)
        {
            _collection.FindOneAndUpdateAsync(
                Builders<User>.Filter.Eq(user => user.UId, uId),
                Builders<User>.Update.Set(user => user.Point, userParam.Point));
        }

    }
}
