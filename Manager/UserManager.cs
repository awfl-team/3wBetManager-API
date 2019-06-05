using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using DAO;
using DAO.Interfaces;
using Manager.Interfaces;
using Models;
using MongoDB.Bson;

namespace Manager
{
    public class UserManager : IUserManager
    {
        private readonly IBetDao _betDao;
        private readonly IUserDao _userDao;

        public UserManager(IUserDao userDao = null, IBetDao betDao = null)
        {
            _userDao = userDao ?? SingletonDao.Instance.UserDao;
            _betDao = betDao ?? SingletonDao.Instance.BetDao;
        }

        public async Task<string> UsernameAndEmailExist(User user)
        {
            var userByEmail = await _userDao.FindUserByEmail(user.Email);
            var userByUsername = await _userDao.FindUserByUsername(user.Username);

            if (userByEmail == null && userByUsername == null) return "";

            if (userByEmail != null && userByUsername == null) return "email already taken";

            if (userByUsername != null && userByEmail == null) return "username already taken";

            return "username and email already taken";
        }

        public async Task<string> CanUpdate(string id, User userParam)
        {
            var users = await _userDao.FindAllUser();
            users = RemoveUserFromList(users, id);
            var userByEmail = users.Find(user => user.Email == userParam.Email);
            var userByUsername = users.Find(user => user.Username == userParam.Username);


            if (userByEmail == null && userByUsername == null) return "";

            if (userByEmail != null && userByUsername == null) return "email already taken";

            if (userByUsername != null && userByEmail == null) return "username already taken";

            return "username and email already taken";
        }

        public List<User> RemoveUserFromList(List<User> userList, string userId)
        {
            userList.Remove(userList.Single(user => user.Id == ObjectId.Parse(userId)));
            return userList;
        }

        public async Task<List<dynamic>> GetBestBetters()
        {
            var users = new List<dynamic>();
            var bestUser = await _userDao.OrderUserByPoint();

            foreach (var user in bestUser.Take(50))
            {
                dynamic obj = new ExpandoObject();
                var betsByUser = await _betDao.FindBetsByUser(user);
                obj.Id = user.Id;
                obj.Point = user.Point;
                obj.Life = user.Items.FindAll(i => i.Type == Item.Life).Count;
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

        public async Task<List<dynamic>> GetUserPositionAmongSiblings(User userParam)
        {
            var users = new List<dynamic>();
            var usersByPoint = await _userDao.FindAllUserByPoint();
            var rank = 1;
            foreach (var user in usersByPoint) user.Rank = rank++;

            var userPlace = usersByPoint.FindIndex(u => u.Id == userParam.Id);
            var usersRange = new List<User>();

            int index;
            int count;
            if (usersByPoint.Count < 5)
            {
                index = userPlace - usersByPoint.Count +
                        (userPlace - usersByPoint.Count < 0 ? 0 - (userPlace - usersByPoint.Count) : 0);
                count = usersByPoint.Count;
            }
            else
            {
                index = userPlace - 5 + (userPlace - 5 < 0 ? 0 - (userPlace - 5) : 0);
                count = 11 - (userPlace - 5 < 0 ? 0 - (userPlace - 5) : 0) -
                        (index + 11 >= usersByPoint.Count ? index + 11 - usersByPoint.Count : 0);
            }


            usersRange = usersByPoint.GetRange(index, count);

            foreach (var user in usersRange)
            {
                dynamic obj = new ExpandoObject();
                var betsByUser = await _betDao.FindBetsByUser(user);
                if (user.Id == userParam.Id) obj.IsCurrent = true;
                obj.Id = user.Id;
                obj.Point = user.Point;
                obj.Life = user.Items.FindAll(i => i.Type == Item.Life).Count;
                obj.Username = user.Username;
                obj.IsPrivate = user.IsPrivate;
                obj.Rank = user.Rank;
                obj.NbBets = betsByUser.Count;
                obj.NbPerfectBets = betsByUser.FindAll(b => b.Status == Bet.PerfectStatus).Count;
                obj.NbOkBets = betsByUser.FindAll(b => b.Status == Bet.OkStatus).Count;
                obj.NbWrongBets = betsByUser.FindAll(b => b.Status == Bet.WrongStatus).Count;
                users.Add(obj);
            }

            return users;
        }

        public async Task<List<dynamic>> GetTop3()
        {
            var users = new List<dynamic>();
            var bestUser = await _userDao.OrderUserByPoint();
            foreach (var user in bestUser.Take(3))
            {
                dynamic obj = new ExpandoObject();
                var betsByUser = await _betDao.FindBetsByUser(user);
                obj.Id = user.Id;
                obj.Point = user.Point;
                obj.Life = user.Items.FindAll(i => i.Type == Item.Life).Count;
                obj.Username = user.Username;
                obj.IsPrivate = user.IsPrivate;
                obj.NbBets = betsByUser.Count;
                users.Add(obj);
            }

            return users;
        }

        public async void RecalculateUserPoints()
        {
            var users = await _userDao.FindAllUser();
            foreach (var user in users)
            {
                var bets = await _betDao.FindBetsByUser(user);
                foreach (var bet in bets)
                    await _userDao.UpdateUserPoints(user, user.Point + bet.PointsWon, 0);
            }
        }

        public async Task<dynamic> GetAllUsersPaginated(int page)
        {
            var users = await _userDao.FindAllUser();
            var totalUsers = users.Count();
            var totalPages = totalUsers / 10 + 1;
            page = page - 1;

            var usersToPass = 10 * page;
            var usersPaginated = await _userDao.PaginatedUsers(usersToPass);
            dynamic obj = new ExpandoObject();
            obj.Items = usersPaginated;
            obj.TotalPages = totalPages;
            obj.TotalUsers = totalUsers;
            obj.Page = page + 1;

            return obj;
        }

        public async Task ResetUser(User user)
        {
            await _userDao.ResetUserPoints(user);
            await _userDao.UpdateUserLives(user);
            await _userDao.ResetUserItems(user);
            _betDao.DeleteBetsByUser(user.Id);
        }

        public async Task<dynamic> GetUserCoinStats(User user)
        {
            var userBets = await _betDao.FindBetsByUser(user);
            var totalBetsEarnings = 0;
            var totalShopCoinUsage = 0;
            if (userBets.Count == 0) return new ExpandoObject();

            foreach (var userBet in userBets) totalBetsEarnings += userBet.PointsWon;
            foreach (var item in user.Items) totalShopCoinUsage += item.Cost;

            dynamic obj = new ExpandoObject();
            obj.TotalShopCoinUsage = totalShopCoinUsage;
            obj.TotalPointsUsedToBet = user.TotalPointsUsedToBet;
            obj.TotalBetsEarnings = totalBetsEarnings;

            return obj;
        }

        public async Task<User> GetUserByEmail(string email)
        {
            return await _userDao.FindUserByEmail(email);
        }

        public async Task AddUser(User user, string role)
        {
            await _userDao.AddUser(user, role);
        }

        public async Task ChangePassword(User user)
        {
            await _userDao.UpdateUserPassword(user);
        }

        public async Task ChangeIsEnabled(ObjectId id, bool isEnable)
        {
            await _userDao.UpdateUserIsEnabled(id, isEnable);
        }

        public async Task ChangeUserPoint(User user, float point, int pointUsedToBet)
        {
            await _userDao.UpdateUserPoints(user, point, pointUsedToBet);
        }

        public async Task<User> GetUser(string id)
        {
            return await _userDao.FindUser(id);
        }

        public async Task DeleteUserItem(User user, string type)
        {
            await _userDao.RemoveUserItem(user, type);
        }

        public async Task<List<User>> GetAllUser()
        {
            return await _userDao.FindAllUser();
        }

        public async Task ChangeUser(string id, User user)
        {
            await _userDao.UpdateUser(id, user);
        }

        public async Task ChangeUserIsPrivate(ObjectId id, bool isPrivate)
        {
            await _userDao.UpdateUserIsPrivate(id, isPrivate);
        }

        public async Task ChangeUserRole(string id, string role)
        {
            await _userDao.UpdateUserRole(id, role);
        }

        public async Task DeleteUser(string id)
        {
            await _userDao.DeleteUser(id);
        }

        public async Task<List<User>> SearchUser(string value)
        {
            return await _userDao.SearchUser(value);
        }
    }
}