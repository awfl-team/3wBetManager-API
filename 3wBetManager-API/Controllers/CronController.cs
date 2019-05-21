using System.Threading.Tasks;
using System.Web.Http;

namespace _3wBetManager_API.Controllers
{
    [IsGranted(Models.User.AdminRole)]
    [RoutePrefix("cron")]
    public class CronController : BaseController
    {
        [Route("competitions")]
        [HttpPost]
        public async Task<IHttpActionResult> RefreshCompetitions()
        {
            return await HandleError(async () =>
            {
                await GetFootballDataManager().GetAllCompetitions();
                return Ok();
            });
        }

        [Route("teams")]
        [HttpPost]
        public async Task<IHttpActionResult> RefreshTeams()
        {
            return await HandleError(async () =>
            {
                await GetFootballDataManager().GetAllTeams();
                return Ok();
            });
        }

        [Route("matches")]
        [HttpPost]
        public async Task<IHttpActionResult> RefreshMatches()
        {
            return await HandleError(async () =>
            {
                await GetFootballDataManager().GetAllMatchForAWeek();
                return Ok();
            });
        }
    }
}