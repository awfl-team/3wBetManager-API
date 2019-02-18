using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using Models;
using MongoDB.Bson;

namespace DAO.Interfaces
{
    public interface IUserDao
    {
        Task<List<User>> FindAllUser();
        Task<User> FindUser(string uid);
        Task<User> FindUserByEmail(string email);
        Task<User> FindUserByUsername(string username);
        void AddUser(User user);
        void DeleteUser(string id);
        void ResetUser(User user);
        void UpdateUser(string id, User userParam);
        void UpdateUserIsPrivate(ObjectId id, bool isPrivate);
        void UpdateUserPoints(ObjectId id, int point);
        void UpdateUserRole(string id, string role);
        Task<List<User>> OrderUserByPoint();
        Task<List<User>> SearchUser(string value);
    }
}
