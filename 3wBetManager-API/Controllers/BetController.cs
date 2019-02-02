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
        public async Task<IHttpActionResult> Post([FromBody] List<Bet> bets)
        {
            try
            {
                var user = await TokenManager.GetUserByToken(Request);
                GetBetDao().AddOrUpdateBet(user, bets);
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
                var user = await TokenManager.GetUserByToken(Request);
                return Ok(await GetBetDao().FindFinishBets(user, competitionId));
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
                var user = await TokenManager.GetUserByToken(Request);
                return Ok(await GetBetDao().FindCurrentBetsAndScheduledMatches(user, competitionId));
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        [Route("{competitionId:int}/current/number")]
        [HttpGet]
        public async Task<IHttpActionResult> GetCurrentNumberMatchAndBet(int competitionId)
        {
            try
            {
                var user = await TokenManager.GetUserByToken(Request);
                return Ok(await GetBetDao().NumberCurrentMatchAndBet(user, competitionId));
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        [Route("{competitionId:int}/result/number")]
        [HttpGet]
        public async Task<IHttpActionResult> GetFinishNumberMatchAndBet(int competitionId)
        {
            try
            {
                var user = await TokenManager.GetUserByToken(Request);
                return Ok(await GetBetDao().NumberFinishMatchAndBet(user, competitionId));
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