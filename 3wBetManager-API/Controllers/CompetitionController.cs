﻿using System.Threading.Tasks;
using System.Web.Http;

namespace _3wBetManager_API.Controllers
{
    [RoutePrefix("competitions")]
    public class CompetitionController : BaseController
    {
        [Route("")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAll()
        {
            return await HandleError(async () => Ok(await GetCompetitionDao().FindAllCompetitions()));
    
        }

    }
}
