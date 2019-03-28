using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAO.Interfaces;
using Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DAO
{
    public class UserDao : IUserDao
    {
        private readonly IMongoCollection<User> _collection;

        public UserDao(IMongoDatabase database, IMongoCollection<User> collection = null)
        {
            _collection = collection ?? database.GetCollection<User>("user");
        }


        public async Task<List<User>> FindAllUser()
        {
            return await _collection.Find(new BsonDocument()).ToListAsync();
        }

        public async Task<User> FindUser(string id)
        {
            var uid = ObjectId.Parse(id);
            var result = await _collection.Find(user => user.Id == uid).ToListAsync();
            return result.FirstOrDefault();
        }

        public async Task<List<User>> OrderUserByPoint()
        {
            return await _collection.Find(new BsonDocument()).Limit(50).Sort("{Point: -1}").ToListAsync();
        }


        public async Task<List<User>> FindAllUserByPoint()
        {
            return await _collection.Find(new BsonDocument()).Sort("{Point: -1}").ToListAsync();
        }


        public async Task<User> FindUserByEmail(string email)
        {
            var result = await _collection.Find(user => user.Email == email).ToListAsync();
            return result.FirstOrDefault();
        }

        public async Task<User> FindUserByUsername(string username)
        {
            var result = await _collection.Find(user => user.Username == username).ToListAsync();
            return result.FirstOrDefault();
        }

        public async Task AddUser(User user, string role)
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            user.IsPrivate = User.DefaultIsPrivate;
            user.Point = User.DefaultPoint;
            user.Life = User.DefaultLife;
            user.Role = role;
            user.TotalPointsUsedToBet = User.DefaultTotalPointsUsedToBet;
            await _collection.InsertOneAsync(user);
        }

        public async Task DeleteUser(string id)
        {
            var uid = ObjectId.Parse(id);
            await _collection.DeleteOneAsync(user => user.Id == uid);
        }

        public async Task UpdateUser(string id, User userParam)
        {
            var uid = ObjectId.Parse(id);
            await _collection.UpdateOneAsync(
                user => user.Id == uid,
                Builders<User>.Update.Set(user => user.Username, userParam.Username)
                    .Set(user => user.Email, userParam.Email)
                    .Set(user => user.Password, BCrypt.Net.BCrypt.HashPassword(userParam.Password))
            );
        }

        public async Task UpdateUserIsPrivate(ObjectId id, bool isPrivate)
        {
            await _collection.UpdateOneAsync(
                user => user.Id == id,
                Builders<User>.Update.Set(user => user.IsPrivate, isPrivate)
            );
        }

        public async Task UpdateUserRole(string id, string role)
        {
            await _collection.UpdateOneAsync(
                user => user.Id == ObjectId.Parse(id),
                Builders<User>.Update.Set(user => user.Role, role)
            );
        }

        public async Task UpdateUserPoints(User user, int point, int pointsUsedToBet)
        {
            await _collection.UpdateOneAsync(
                u => u.Id == user.Id,
                Builders<User>.Update.Set(u => u.Point, point).Set(u => u.TotalPointsUsedToBet,
                    user.TotalPointsUsedToBet + pointsUsedToBet)
            );
        }

        public async Task ResetUserPoints(User user)
        {
            await _collection.UpdateOneAsync(
                u => u.Id == user.Id,
                Builders<User>.Update.Set(u => u.Point, User.DefaultPoint)
                    .Set(u => u.TotalPointsUsedToBet, User.DefaultTotalPointsUsedToBet)
            );
        }

        public async Task UpdateUserLives(User user)
        {
            await _collection.UpdateOneAsync(
                u => u.Id == user.Id,
                Builders<User>.Update.Set(u => u.Life, user.Life - 1)
            );
        }

        public async Task UpdateUserPassword(User user)
        {
            await _collection.UpdateOneAsync(
                u => u.Id == user.Id,
                Builders<User>.Update.Set(u => u.Password, BCrypt.Net.BCrypt.HashPassword(user.Password))
            );
        }

        public async Task<List<User>> SearchUser(string value)
        {
            return await _collection.Find(u => u.Email.Contains(value) || u.Username.Contains(value)).ToListAsync();
        }

        public async Task<List<User>> PaginatedUsers(int usersToPass)
        {
            return await _collection.Find(new BsonDocument()).Skip(usersToPass).Limit(10).ToListAsync();
        }
    }
}