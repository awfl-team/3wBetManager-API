using System.Collections.Generic;
using System.Threading.Tasks;
using Models;
using MongoDB.Bson;

namespace Manager.Interfaces
{
    public interface IUserManager
    {
        Task<string> UsernameAndEmailExist(User user);
        Task<string> CanUpdate(string id, User userParam);
        List<User> RemoveUserFromList(List<User> users, string userId);
        Task<List<dynamic>> GetBestBetters();
        Task<List<dynamic>> GetUserPositionAmongSiblings(User userParam);
        Task<List<dynamic>> GetTop3();
        Task<dynamic> GetAllUsersPaginated(int page);
        Task<dynamic> GetUserCoinStats(User user);
        Task<User> GetUserByEmail(string email);
        Task AddUser(User user, string role);
        Task ChangePassword(User user);
        Task ChangeIsEnabled(ObjectId id, bool isEnable);
        Task ChangeUserPoint(User user, float point, int pointUsedToBet);
        Task<User> GetUser(string id);
        Task DeleteUserItem(User user, string type);
        Task<List<User>> GetAllUser();
        Task ChangeUser(string id, User user);
        Task ChangeUserIsPrivate(ObjectId id, bool isPrivate);
        Task ChangeUserRole(string id, string role);
        Task ResetUser(User user);
        Task DeleteUser(string id);
        Task<List<User>> SearchUser(string value);
        void RecalculateUserPoints();
    }
}