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

            if (userByEmail == null && userByUsername == null) return "";

            if (userByEmail != null && userByUsername == null) return "email already taken";

            if (userByUsername != null && userByEmail == null) return "username already taken";

            return "username and email already taken";
        }

        public static async Task<string> CanUpdate(string id, User userParam)
        {
            var users = await Singleton.Instance.UserDao.FindAllUser();
            users.Remove(users.Single(user => user.Id == ObjectId.Parse(id)));
            var userByEmail = users.Find(user => user.Email == userParam.Email);
            var userByUsername = users.Find(user => user.Username == userParam.Username);


            if (userByEmail == null && userByUsername == null) return "";

            if (userByEmail != null && userByUsername == null) return "email already taken";

            if (userByUsername != null && userByEmail == null) return "username already taken";

            return "username and email already taken";
        }

        public static async Task<List<dynamic>> GetBestBetters()
        {
            var users = new List<dynamic>();
            var bestUser = await Singleton.Instance.UserDao.OrderUserByPoint();

            foreach (var user in bestUser)
            {
                dynamic obj = new ExpandoObject();
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
                if (obj.NbBets > 0) users.Add(obj);
            }

            return users;
        }

        public static async Task<List<dynamic>> GetUserPositionAmongSiblings(User userParam)
        {
            var users = new List<dynamic>();
            var usersByPoint = await Singleton.Instance.UserDao.FindAllUserByPoint();
            var userPlace = usersByPoint.FindIndex(u => u.Id == userParam.Id);
            var usersRange = new List<User>();
            if (userPlace - 5 < 0 || userPlace + 5 > usersByPoint.Count)
                usersRange = usersByPoint;
            else
                usersRange = usersByPoint.GetRange(userPlace - 5, userPlace + 5);

            foreach (var user in usersRange)
            {
                dynamic obj = new ExpandoObject();
                var betsByUser = await Singleton.Instance.BetDao.FindBetsByUser(user);
                if (user.Id == userParam.Id) obj.IsCurrent = true;
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

        public static async Task<List<dynamic>> GetTop3()
        {
            var users = new List<dynamic>();
            var bestUser = await Singleton.Instance.UserDao.OrderUserByPoint();
            foreach (var user in bestUser.Take(3))
            {
                dynamic obj = new ExpandoObject();
                var betsByUser = await Singleton.Instance.BetDao.FindBetsByUser(user);
                obj.Id = user.Id;
                obj.Point = user.Point;
                obj.Life = user.Life;
                obj.Username = user.Username;
                obj.IsPrivate = user.IsPrivate;
                obj.NbBets = betsByUser.Count;
                if (obj.NbBets > 0) users.Add(obj);
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
                    await Singleton.Instance.UserDao.UpdateUserPoints(user, user.Point + bet.PointsWon, 0);
            }
        }

        public static async Task<dynamic> GetAllUsersPaginated(int page)
        {
            var users = await Singleton.Instance.UserDao.FindAllUser();
            var totalUsers = users.Count();
            var totalPages = totalUsers / 10 + 1;
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

        public static async Task ResetUser(User user)
        {
            await Singleton.Instance.UserDao.ResetUserPoints(user);
            await Singleton.Instance.UserDao.UpdateUserLives(user);
            Singleton.Instance.BetDao.DeleteBetsByUser(user.Id);
        }

        public static async Task<dynamic> GetUserCoinStats(User user)
        {
            var userBets = await Singleton.Instance.BetDao.FindBetsByUser(user);
            var totalBetsEarnings = 0;
            if (userBets.Count == 0) return new ExpandoObject();

            foreach (var userBet in userBets) totalBetsEarnings += userBet.PointsWon;

            dynamic obj = new ExpandoObject();
            obj.TotalPointsUsedToBet = user.TotalPointsUsedToBet;
            obj.TotalBetsEarnings = totalBetsEarnings;

            return obj;
        }
    }
}