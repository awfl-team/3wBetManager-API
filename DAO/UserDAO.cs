using System;
using System.Collections.Generic;
using System.Dynamic;
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

        public UserDao(IMongoCollection<User> collection = null)
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("3wBetManager");
            _collection = collection ?? database.GetCollection<User>("user");
        }


        public async Task<List<User>> FindAllUser()
        {
            return await _collection.Find(new BsonDocument()).ToListAsync();
        }

        public async Task<List<User>> FindAllUserByOrder(int order)
        {
            return await _collection.Find(new BsonDocument()).Sort("{Point:" + order + "}").ToListAsync();
        }

        public async Task<User> FindUser(string id)
        {
            var uid = ObjectId.Parse(id);
            var result = await _collection.Find(user => user.Id == uid).ToListAsync();
            return result.FirstOrDefault();
        }

        public async Task<User> FindUserByEmailSingle(string email)
        {
            return await _collection.Find(user => user.Email == email).SingleAsync();
        }

        public bool UsernameAndEmailExist (User user, out string errorMessage)
        {
            var userByEmail = FindUserByEmailToList(user.Email);
            var userByUsername = FindUserByUsername(user.Username);
            if (userByEmail.Result == null && userByUsername.Result == null)
            {
                errorMessage = "";
                return true;
            }

            if (userByEmail.Result != null && userByUsername.Result == null)
            {
                errorMessage = "email already taken";
                return false;
            }

            if (userByUsername.Result != null && userByEmail.Result == null)
            {
                errorMessage = "username already taken";
                return false;
            }
            errorMessage = "username and email already taken";
            return false;
        }

        public bool CanUpdate(string id, User userParam, out string errorMessage)
        {

            var users = FindAllUser();
            users.Result.Remove(users.Result.Single(user => user.Id == ObjectId.Parse(id)));
            var userByEmail = users.Result.Find(user => user.Email == userParam.Email);
            var userByUsername = users.Result.Find(user => user.Username == userParam.Username);
            if (userByEmail == null && userByUsername == null)
            {
                errorMessage = "";
                return true;
            }

            if (userByEmail != null && userByUsername == null)
            {
                errorMessage = "email already taken";
                return false;
            }

            if (userByUsername != null && userByEmail == null)
            {
                errorMessage = "username already taken";
                return false;
            }
            errorMessage = "username and email already taken";
            return false;
        }
        
        
        public async Task<List<ExpandoObject>> FindBestBetters()
        {
            var users = new List<ExpandoObject>();
            var betsUser = await _collection.Find(new BsonDocument()).Limit(50).Sort("{Point: 1}").ToListAsync();

            foreach (var user in betsUser)
            {
                dynamic obj = new ExpandoObject();
                var betsByUser = await Singleton.Instance.BetDao.FindBetsByUser(user);
                obj.Id = user.Id;
                obj.Point = user.Point;
                obj.Username = user.Username;
                obj.Visible = user.Visible;
                obj.NbBets = betsByUser.Count;
                obj.NbPerfectBets = betsByUser.FindAll(b => b.Status == "Perfect").Count;
                obj.NbOkBets = betsByUser.FindAll(b => b.Status == "Ok").Count;
                obj.NbWrongBets = betsByUser.FindAll(b => b.Status == "Wrong").Count;
                users.Add(obj);
            }

            return users;
        }

        public async Task<User> FindUserByEmailToList(string email)
        {
            var result = await _collection.Find(user => user.Email == email).ToListAsync();
            return result.FirstOrDefault();
        }

        public async Task<User> FindUserByUsername(string username)
        {
            var result = await _collection.Find(user => user.Username == username).ToListAsync();
            return result.FirstOrDefault();
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

        public async void UpdateUser(string id, User userParam)
        {
            var uid = ObjectId.Parse(id);
            await _collection.UpdateOneAsync(
                user => user.Id == uid,
                Builders<User>.Update.Set(user => user.Username, userParam.Username)
                    .Set(user => user.Email, userParam.Email)
                    .Set(user => user.Password, BCrypt.Net.BCrypt.HashPassword(userParam.Password))
            );
        }
    }
}