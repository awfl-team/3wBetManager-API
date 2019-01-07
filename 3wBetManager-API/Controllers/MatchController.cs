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
    public class MatchController: ApiController
    {
        public async Task<IHttpActionResult> GetAll()
        {
            try
            {
                return Ok(await GetMatchDao().FindAll());
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }
       
        private IMatchDao GetMatchDao()
        {
            return Singleton.Instance.MatchDao;
        }
    }
}
