using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Manager;
using Manager.Interfaces;
using Models;
using Proxy;

namespace _3wBetManager_API.Controllers
{
    public abstract class BaseController : ApiController
    {
        protected IUserManager GetUserManager()
        {
            return SingletonManager.Instance.UserManager;
        }

        protected ITokenManager GetTokenManager()
        {
            return SingletonManager.Instance.TokenManager;
        }

        protected IBetManager GetBetManager()
        {
            return SingletonManager.Instance.BetManager;
        }

        protected IMatchManager GetMatchManager()
        {
            return SingletonManager.Instance.MatchManager;
        }

        protected IFootballDataManager GetFootballDataManager()
        {
            return SingletonManager.Instance.FootballDataManager;
        }

        protected IItemManager GetItemManager()
        {
            return SingletonManager.Instance.ItemManager;
        }

        protected IEmailManager GetEmailManager()
        {
            return SingletonManager.Instance.EmailManager;
        }

        protected ICompetitionManager GetCompetitionManager()
        {
            return SingletonManager.Instance.CompetitionManager;
        }


        protected async Task<User> GetUserByToken(HttpRequestMessage request)
        {
            var token = GetTokenManager().GetTokenFromRequest(request);
            var user = GetTokenManager().ValidateToken(token);
            return await SingletonManager.Instance.UserManager.GetUserByEmail(user["email"]);
        }

        protected async Task<IHttpActionResult> HandleError(Func<Task<IHttpActionResult>> getHttpActionResult)
        {
           using (new ElasticsSearchControllerContext(Request.Method.Method,
                Request.RequestUri.AbsolutePath, Request.GetOwinContext().Request.RemoteIpAddress))
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
        }

        protected async Task<IHttpActionResult> HandleNotFound(Func<Task<IHttpActionResult>> getHttpActionResult)
        {
            using (new ElasticsSearchControllerContext(Request.Method.Method,
                Request.RequestUri.AbsolutePath, Request.GetOwinContext().Request.RemoteIpAddress))
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
}