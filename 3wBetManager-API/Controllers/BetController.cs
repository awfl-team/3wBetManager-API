using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using DAO;
using DAO.Interfaces;
using Models;
using _3wBetManager_API.Manager;

namespace _3wBetManager_API.Controllers
{
    [RoutePrefix("bets")]
    public class BetController : ApiController
    {
        [Route("")]
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

        [Route("list")]
        [HttpPut]
        public IHttpActionResult PutList([FromBody] List<Bet> bets)
        {
            try
            {
                GetBetDao().UpdateListBet(bets);
                return Ok();
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        [Route("list")]
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

        [Route("{competitionId:int}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetBetsResult(int competitionId)
        {
            try
            {
                var token = TokenManager.GetTokenFromRequest(Request);
                var user = TokenManager.ValidateToken(token);
                var fullUser = Singleton.Instance.UserDao.FindUserByEmailSingle(user["email"]);
                return Ok(await GetBetDao().FindFinishBets(fullUser.Result, competitionId));
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