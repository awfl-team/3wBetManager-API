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
        /*[Route("")]
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
        }*/

        [Route("")]
        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody] List<Bet> bets)
        {
            try
            {
                var token = TokenManager.GetTokenFromRequest(Request);
                var user = TokenManager.ValidateToken(token);
                var fullUser = await Singleton.Instance.UserDao.FindUserByEmailSingle(user["email"]);
                GetBetDao().AddOrUpdateBet(fullUser, bets);
                return Ok();
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        [Route("{competitionId:int}/result")]
        [HttpGet]
        public async Task<IHttpActionResult> GetBetsResult(int competitionId)
        {
            try
            {
                var token = TokenManager.GetTokenFromRequest(Request);
                var user = TokenManager.ValidateToken(token);
                var fullUser = await Singleton.Instance.UserDao.FindUserByEmailSingle(user["email"]);
                return Ok(await GetBetDao().FindFinishBets(fullUser, competitionId));
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        [Route("{competitionId:int}/current")]
        [HttpGet]
        public async Task<IHttpActionResult> GetBetsAndMatches(int competitionId)
        {
            try
            {
                var token = TokenManager.GetTokenFromRequest(Request);
                var user = TokenManager.ValidateToken(token);
                var fullUser = await Singleton.Instance.UserDao.FindUserByEmailSingle(user["email"]);
                return Ok(await GetBetDao().FindCurrentBetsAndScheduledMatches(fullUser, competitionId));
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