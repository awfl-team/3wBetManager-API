using System.Threading.Tasks;
using System.Web.Http;

namespace _3wBetManager_API.Controllers
{
    [IsGranted]
    [RoutePrefix("stats")]
    public class StatController : BaseController
    {
        /* UserToken routes */
        [Route("coins")]
        [HttpGet]
        public async Task<IHttpActionResult> GetUserCoinStats()
        {
            return await HandleError(async () =>
            {
                var user = await GetUserByToken(Request);
                return Ok(await GetUserManager().GetUserCoinStats(user));
            });
        }

        [Route("month")]
        [HttpGet]
        public async Task<IHttpActionResult> GetUserIncomesPerMonth()
        {
            return await HandleError(async () =>
            {
                var user = await GetUserByToken(Request);
                return Ok(await GetBetManager().GetUserIncomesPerMonth(user));
            });
        }

        [Route("year")]
        [HttpGet]
        public async Task<IHttpActionResult> GetUserIncomesPerYear()
        {
            return await HandleError(async () =>
            {
                var user = await GetUserByToken(Request);

                return Ok(await GetBetManager().GetUserIncomesPerYear(user));
            });
        }

        [Route("type")]
        [HttpGet]
        public async Task<IHttpActionResult> GetUserBetsPerType()
        {
            return await HandleError(async () =>
            {
                var user = await GetUserByToken(Request);

                return Ok(await GetBetManager().GetUserBetsPerType(user));
            });
        }

        [Route("earnings/type")]
        [HttpGet]
        public async Task<IHttpActionResult> GetUserBetsEarningsPerType()
        {
            return await HandleError(async () =>
            {
                var user = await GetUserByToken(Request);
                return Ok(await GetBetManager().GetUserBetsEarningsPerType(user));
            });
        }

        /* ConsultUser routes */
        [Route("public/type/{id}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetUserBetsPerTypePublic(string id)
        {
            return await HandleNotFound(async () =>
            {
                var user = await GetUserManager().GetUser(id);
                return Ok(await GetBetManager().GetUserBetsPerType(user));
            });
        }

        [Route("public/coins/{id}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetPublicUserCoinStats(string id)
        {
            return await HandleNotFound(async () =>
            {
                var user = await GetUserManager().GetUser(id);
                return Ok(await GetUserManager().GetUserCoinStats(user));
            });
        }

        [Route("public/month/{id}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetPublicUserIncomesPerMonth(string id)
        {
            return await HandleNotFound(async () =>
            {
                var user = await GetUserManager().GetUser(id);
                return Ok(await GetBetManager().GetUserIncomesPerMonth(user));
            });
        }

        [Route("public/year/{id}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetPublicUserIncomesPerYear(string id)
        {
            return await HandleNotFound(async () =>
            {
                var user = await GetUserManager().GetUser(id);
                return Ok(await GetBetManager().GetUserIncomesPerYear(user));
            });
        }

        [Route("public/earnings/type/{id}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetPublicUserBetsEarningsPerType(string id)
        {
            return await HandleNotFound(async () =>
            {
                var user = await GetUserManager().GetUser(id);
                return Ok(await GetBetManager().GetUserBetsEarningsPerType(user));
            });
        }
    }
}