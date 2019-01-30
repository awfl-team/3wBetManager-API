using System;
using System.Threading.Tasks;
using System.Web.Http;
using DAO;
using DAO.Interfaces;

namespace _3wBetManager_API.Controllers
{
    [RoutePrefix("competitions")]
    public class CompetitionController : ApiController
    {
        [Route("")]
        public async Task<IHttpActionResult> GetAll()
        {
            try
            {
                return Ok(await GetCompetitionDao().FindAllCompetitions());
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        private ICompetitionDao GetCompetitionDao()
        {
            return Singleton.Instance.CompetitionDao;
        }
    }
}