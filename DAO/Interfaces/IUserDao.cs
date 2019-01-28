using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Models;
using MongoDB.Bson;

namespace DAO.Interfaces
{
    public interface IUserDao
    {
        Task<List<User>> FindAllUser();
        Task<User> FindUser(string uid);
        Task<User> FindUserByEmailSingle(string email);
        Task<User> FindUserByEmailToList(string email);
        Task<User> FindUserByUsername(string username);
        Task<List<User>> FindAllUserByOrder(int order);
        Task<List<ExpandoObject>> FindBestBetters();
        bool UsernameAndEmailExist(User user, out string errorMessage);
        bool CanUpdate(string id, User userParam, out string errorMessage);
        void AddUser(User user);
        void DeleteUser(string id);
        void ResetUser(string id);
        void UpdateUser(string id, User userParam);
        void UpdateUserIsPrivate(string id, bool isPrivate);
    }
}
