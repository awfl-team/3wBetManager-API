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
            try
            {
                var footballDataManager = new FootballDataManager();
                await footballDataManager.GetAllCompetitions();
                return Ok();
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        [Route("teams")]
        [HttpPost]
        public async Task<IHttpActionResult> RefreshTeams()
        {
            try
            {
                var footballDataManager = new FootballDataManager();
                await footballDataManager.GetAllTeams();
                return Ok();
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        [Route("matches")]
        [HttpPost]
        public async Task<IHttpActionResult> RefreshMatches()
        {
            try
            {
                var footballDataManager = new FootballDataManager();
                await footballDataManager.GetAllMatchForAWeek();
                return Ok();
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        [Route("all")]
        [HttpPost]
        public async Task<IHttpActionResult> RefreshAll()
        {
            try
            {
                var footballDataManager = new FootballDataManager();
                await footballDataManager.GetAllCompetitions();
                await footballDataManager.GetAllTeams();
                await footballDataManager.GetAllMatchForAWeek();
                return Ok();
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }
    }
}