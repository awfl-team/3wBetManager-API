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
        Task AddUser(User user);
        Task DeleteUser(string id);
        Task UpdateUserLives(User user);
        Task<List<User>> FindAllUserByPoint();
        Task UpdateUser(string id, User userParam);
        Task UpdateUserIsPrivate(ObjectId id, bool isPrivate);
        Task UpdateUserPoints(User user, int point, int pointsUsedToBet);
        Task ResetUserPoints(User user);
        Task UpdateUserRole(string id, string role);
        Task<List<User>> OrderUserByPoint();
        Task<List<User>> SearchUser(string value);
        Task<List<User>> PaginatedUsers(int usersToPass);
    }
}
