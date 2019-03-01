using System;
using System.Threading.Tasks;
using System.Web.Http;
using FetchFootballData;
using Manager;

namespace _3wBetManager_API.Controllers
{
    [RoutePrefix("cron")]
    public class CronController : BaseController
    {
        [Route("competitions")]
        [HttpPost]
        public async Task<IHttpActionResult> RefreshCompetitions()
        {
            return await HandleError(async () =>
            {
                var footballDataManager = new FootballDataManager();
                await footballDataManager.GetAllCompetitions();
                return Ok();
            });
        }

        [Route("teams")]
        [HttpPost]
        public async Task<IHttpActionResult> RefreshTeams()
        {
            return await HandleError(async () =>
            {
                var footballDataManager = new FootballDataManager();
                await footballDataManager.GetAllTeams();
                return Ok();
            });
        }

        [Route("matches")]
        [HttpPost]
        public async Task<IHttpActionResult> RefreshMatches()
        {
            return await HandleError(async () =>
            {
                var footballDataManager = new FootballDataManager();
                await footballDataManager.GetAllMatchForAWeek();
                return Ok();
            });
        }
    }
}