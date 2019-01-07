using System;
using System.Collections.Generic;
using System.Web.Http;
using DAO;
using DAO.Interfaces;
using Models;

namespace _3wBetManager_API.Controllers
{
    public class BetController : ApiController
    {
        [HttpPost]
        public IHttpActionResult Post([FromBody] Bet bet)
        {
            try
            {
                GetBetDao().AddBet(bet);
                return Ok();
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
            
        }

        [HttpPost]
        public IHttpActionResult PostList([FromBody] List<Bet> bets)
        {
            try
            {
                GetBetDao().AddListBet(bets);
                return Ok();
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }

        }

        private IBetDao GetBetDao()
        {
            return Singleton.Instance.BetDao;
        }
    }
}
