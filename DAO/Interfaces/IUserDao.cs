using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Models;
using MongoDB.Bson;

namespace DAO.Interfaces
{
    public interface IUserDao
    {
        Task<List<User>> GetAllUser();
        Task<User> GetUser(string uid);
        void AddUser(User user);
        void DeleteUser(string uId);
        void UpdateBook(string uId, User userParam);
    }
}
