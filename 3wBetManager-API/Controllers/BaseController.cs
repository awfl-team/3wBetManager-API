using System;
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

        protected ICompetitionDao GetCompetitionDao()
        {
            return Singleton.Instance.CompetitionDao;
        }

        protected static async Task<User> GetUserByToken(HttpRequestMessage request)
        {
            var token = TokenManager.GetTokenFromRequest(request);
            var user = TokenManager.ValidateToken(token);
            return await Singleton.Instance.UserDao.FindUserByEmail(user["email"]);
        }

        protected async Task<IHttpActionResult> HandleError(Func<Task<IHttpActionResult>> getHttpActionResult)
        {
            try
            {
                return await getHttpActionResult();
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        protected async Task<IHttpActionResult> HandleNotFound(Func<Task<IHttpActionResult>> getHttpActionResult)
        {
            try
            {
                return await getHttpActionResult();
            }
            catch (FormatException)
            {
                return NotFound();
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }
    }
}