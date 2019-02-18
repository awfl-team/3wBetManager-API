using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using DAO;
using DAO.Interfaces;
using Models;
using _3wBetManager_API.Manager;

namespace _3wBetManager_API.Controllers
{
    public class BaseController : ApiController
    {
        protected IUserDao GetUserDao()
        {
            return Singleton.Instance.UserDao;
        }

        protected IBetDao GetBetDao()
        {
            return Singleton.Instance.BetDao;
        }

        protected static async Task<User> GetUserByToken(HttpRequestMessage request)
        {
            var token = TokenManager.GetTokenFromRequest(request);
            var user = TokenManager.ValidateToken(token);
            return await Singleton.Instance.UserDao.FindUserByEmail(user["email"]);
        }
    }
}