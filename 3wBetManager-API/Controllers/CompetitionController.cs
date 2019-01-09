using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using DAO;
using DAO.Interfaces;

namespace _3wBetManager_API.Controllers
{
    public class CompetitionController : ApiController
    {
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