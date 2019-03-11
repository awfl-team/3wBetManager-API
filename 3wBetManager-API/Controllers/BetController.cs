using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using DAO;
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
                bets = BetManager.AddGuidList(user, bets);
                await GetBetDao().AddListBet(bets);
                await GetUserDao().UpdateUserPoints(user, user.Point - (bets.Count * 10), (bets.Count * 10));
                return Created("", bets);
            });
        }

        [Route("")]
        [HttpPut]
        public async Task<IHttpActionResult> Put([FromBody] List<Bet> bets)
        {
            return await HandleError(async () =>
            {
                var user = await GetUserByToken(Request);
                foreach (var bet in bets)
                {
                    await GetBetDao().UpdateBet(bet);
                }

                await GetUserDao().UpdateUserPoints(user, user.Point - (bets.Count * 10), (bets.Count * 10));
                return Content(HttpStatusCode.NoContent, "");
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
                return Ok(await BetManager.GetFinishBetsLimited(user));
            });
        }

        [Route("current")]
        [HttpGet]
        public async Task<IHttpActionResult> GetBetsCurrentLimit()
        {
            return await HandleError(async () =>
            {
                var user = await GetUserByToken(Request);
                return Ok(await BetManager.GetCurrentBetsLimited(user));
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

        [Route("stats/earnings/type")]
        [HttpGet]
        public async Task<IHttpActionResult> GetUserBetsEarningsPerType()
        {
            return await HandleError(async () =>
            {
                var user = await GetUserByToken(Request);
                return Ok(await BetManager.GetUserBetsEarningsPerType(user));
            });
        }

        [Route("stats/public/type/{id}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetUserBetsPerTypePublic(string id)
        {
            return await HandleNotFound(async () =>
            {
                var user = await GetUserDao().FindUser(id);
                return Ok(await BetManager.GetUserBetsPerType(user));
            });
        }
    }
}