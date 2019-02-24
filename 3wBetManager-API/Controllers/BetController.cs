using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Manager;
using Models;

namespace _3wBetManager_API.Controllers
{
    [RoutePrefix("bets")]
    public class BetController : BaseController
    {
        [Route("")]
        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody] List<Bet> bets)
        {
            return await HandleError(async () =>
            {
                var user = await GetUserByToken(Request);
                GetBetDao().AddOrUpdateBet(user, bets);
                return Ok();
            });
        }

        [Route("{competitionId}/result")]
        [HttpGet]
        public async Task<IHttpActionResult> GetBetsResult(int competitionId)
        {
            return await HandleError(async () =>
            {
                var user = await GetUserByToken(Request);
                return Ok(await BetManager.GetFinishBets(user, competitionId));
            });
        }

        [Route("result")]
        [HttpGet]
        public async Task<IHttpActionResult> GetBetsResultLimit()
        {
            return await HandleError(async () =>
            {
                var user = await GetUserByToken(Request);
                return Ok(await BetManager.GetFinishBetsLimit4(user));
            });
        }

        [Route("current")]
        [HttpGet]
        public async Task<IHttpActionResult> GetBetsCurrentLimit()
        {
            return await HandleError(async () =>
            {
                var user = await GetUserByToken(Request);
                return Ok(await BetManager.GetCurrentBetsLimit4(user));
            });
        }

        [Route("{competitionId}/current")]
        [HttpGet]
        public async Task<IHttpActionResult> GetBetsAndMatches(int competitionId)
        {
            return await HandleError(async () =>
            {
                var user = await GetUserByToken(Request);
                return Ok(await BetManager.GetCurrentBetsAndScheduledMatches(user, competitionId));
            });
        }

        [Route("{competitionId}/current/number")]
        [HttpGet]
        public async Task<IHttpActionResult> GetCurrentNumberMatchAndBet(int competitionId)
        {
            return await HandleError(async () =>
            {
                var user = await GetUserByToken(Request);
                return Ok(await BetManager.NumberCurrentMatchAndBet(user, competitionId));
            });
        }

        [Route("{competitionId}/result/number")]
        [HttpGet]
        public async Task<IHttpActionResult> GetFinishNumberMatchAndBet(int competitionId)
        {
            return await HandleError(async () =>
            {
                var user = await GetUserByToken(Request);
                return Ok(await BetManager.NumberFinishMatchAndBet(user, competitionId));
            });
        }

        [Route("stats/type")]
        [HttpGet]
        public async Task<IHttpActionResult> GetUserBetsPerType()
        {
            return await HandleError(async () =>
            {
                var user = await GetUserByToken(Request);
                return Ok(await BetManager.GetUserBetsPerType(user));
            });
        }
    }
}