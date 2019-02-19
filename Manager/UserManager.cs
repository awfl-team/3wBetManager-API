using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using DAO;
using Models;
using MongoDB.Bson;

namespace Manager
{
    public class UserManager
    {
        public static async Task<string> UsernameAndEmailExist(User user)
        {
            var userByEmail = await Singleton.Instance.UserDao.FindUserByEmail(user.Email);
            var userByUsername = await Singleton.Instance.UserDao.FindUserByUsername(user.Username);
            if (userByEmail == null && userByUsername == null)
            {
                return null;
            }

            if (userByEmail != null && userByUsername == null)
            {
                return "email already taken";
            }

            if (userByUsername != null && userByEmail == null)
            {
                return "username already taken";
            }

            return "username and email already taken";
        }

        public static async Task<string> CanUpdate(string id, User userParam)
        {
            var users = await Singleton.Instance.UserDao.FindAllUser();
            users.Remove(users.Single(user => user.Id == ObjectId.Parse(id)));
            var userByEmail = users.Find(user => user.Email == userParam.Email);
            var userByUsername = users.Find(user => user.Username == userParam.Username);
            if (userByEmail == null && userByUsername == null)
            {
                return null;
            }

            if (userByEmail != null && userByUsername == null)
            {
                return "email already taken";
            }

            if (userByUsername != null && userByEmail == null)
            {
                return "username already taken";
            }

            return "username and email already taken";
        }

        public static async Task<List<dynamic>> GetBestBetters()
        {
            var users = new List<dynamic>();
            var betsUser = await Singleton.Instance.UserDao.OrderUserByPoint();

            foreach (var user in betsUser)
            {
                dynamic obj = new { };
                var betsByUser = await Singleton.Instance.BetDao.FindBetsByUser(user);
                obj.Id = user.Id;
                obj.Point = user.Point;
                obj.Life = user.Life;
                obj.Username = user.Username;
                obj.IsPrivate = user.IsPrivate;
                obj.NbBets = betsByUser.Count;
                obj.NbPerfectBets = betsByUser.FindAll(b => b.Status == Bet.PerfectStatus).Count;
                obj.NbOkBets = betsByUser.FindAll(b => b.Status == Bet.OkStatus).Count;
                obj.NbWrongBets = betsByUser.FindAll(b => b.Status == Bet.WrongStatus).Count;
                users.Add(obj);
            }

            return users;
        }

        public static async void RecalculateUserPoints()
        {
            var users = await Singleton.Instance.UserDao.FindAllUser();
            foreach (var user in users)
            {
                var bets = await Singleton.Instance.BetDao.FindBetsByUser(user);
                foreach (var bet in bets)
                {
                    Singleton.Instance.UserDao.UpdateUserPoints(user.Id, user.Point + bet.PointsWon);
                }
            }
        }

        public static async Task<dynamic> GetAllUsersPaginated(int page)
        {
            var users = await Singleton.Instance.UserDao.FindAllUser();
            var totalUsers = users.Count();
            var totalPages = (totalUsers / 10) + 1;
            page = page - 1;

            var usersToPass = 10 * page;
            var usersPaginated = await Singleton.Instance.UserDao.PaginatedUsers(usersToPass);
            dynamic obj = new ExpandoObject();
            obj.Items = usersPaginated;
            obj.TotalPages = totalPages;
            obj.TotalUsers = totalUsers;
            obj.Page = page + 1;


            return obj;
        }
    }
}