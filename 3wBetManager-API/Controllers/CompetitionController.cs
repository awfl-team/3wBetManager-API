using System.Threading.Tasks;
using System.Web.Http;
using DAO;

namespace _3wBetManager_API.Controllers
{
    [IsGranted]
    [RoutePrefix("competitions")]
    public class CompetitionController : BaseController
    {
        [Route("")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAll()
        {
            return await HandleError(async () => Ok(await SingletonDao.Instance.CompetitionDao.FindAllCompetitions()));
        }
    }
}