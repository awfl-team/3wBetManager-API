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
        Task RegisterUser(User user);
        Task AddUser(User user);
        Task DeleteUser(string id);
        Task UpdateUserLifes(User user);
        Task UpdateUser(string id, User userParam);
        Task UpdateUserIsPrivate(ObjectId id, bool isPrivate);
        Task UpdateUserPoints(ObjectId id, int point);
        Task UpdateUserRole(string id, string role);
        Task<List<User>> OrderUserByPoint();
        Task<List<User>> SearchUser(string value);
        Task<List<User>> PaginatedUsers(int usersToPass);
    }
}
