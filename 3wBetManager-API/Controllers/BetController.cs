using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Manager;
using Models;

namespace _3wBetManager_API.Controllers
{
    [IsGranted]
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
                using (var betManager = new BetManager())
                {
                    var betsParsed = betManager.ParseListBet(bets);
                    betsParsed = betManager.AddGuidList(user, betsParsed);
                    await GetBetDao().AddListBet(betsParsed);
                }

                await GetUserDao().UpdateUserPoints(user, user.Point - (bets.Count * 10), (bets.Count * 10));
                foreach (var bet in bets)
                {
                    new MatchManager().CalculateMatchRating(match: bet.Match);
                }

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

                using (var betManager = new BetManager())
                {
                    var betsParsed = betManager.ParseListBet(bets);
                    foreach (var bet in betsParsed)
                    {
                        await GetBetDao().UpdateBet(bet);
                        new MatchManager().CalculateMatchRating(match: bet.Match);
                    }
                }

                await GetUserDao().UpdateUserPoints(user, user.Point - bets.Count * 10, bets.Count * 10);
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
                using (var betManager = new BetManager())
                {
                    return Ok(await betManager.GetFinishBets(user, competitionId));
                }
            });
        }

        [Route("result")]
        [HttpGet]
        public async Task<IHttpActionResult> GetBetsResultLimit()
        {
            return await HandleError(async () =>
            {
                var user = await GetUserByToken(Request);
                using (var betManager = new BetManager())
                {
                    return Ok(await betManager.GetFinishBetsLimited(user));
                }
            });
        }

        [Route("current")]
        [HttpGet]
        public async Task<IHttpActionResult> GetBetsCurrentLimit()
        {
            return await HandleError(async () =>
            {
                var user = await GetUserByToken(Request);
                using (var betManager = new BetManager())
                {
                    return Ok(await betManager.GetCurrentBetsLimited(user));
                }
            });
        }

        [Route("{id}/result/key")]
        [HttpGet]
        public async Task<IHttpActionResult> GetBetsResultLimitWithKey(string id)
        {
            return await HandleError(async () =>
            {
                var user = await GetUserDao().FindUser(id);
                using (var betManager = new BetManager())
                {
                    return Ok(await betManager.GetFinishBetsLimited(user));
                }
            });
        }

        [Route("{id}/current/key")]
        [HttpGet]
        public async Task<IHttpActionResult> GetBetsCurrentLimitWithKey(string id)
        {
            return await HandleError(async () =>
            {
                var user = await GetUserDao().FindUser(id);
                using (var betManager = new BetManager())
                {
                    return Ok(await betManager.GetCurrentBetsLimited(user));
                }
            });
        }

        [Route("{competitionId}/current")]
        [HttpGet]
        public async Task<IHttpActionResult> GetBetsAndMatches(int competitionId)
        {
            return await HandleError(async () =>
            {
                var user = await GetUserByToken(Request);
                using (var betManager = new BetManager())
                {
                    return Ok(await betManager.GetCurrentBetsAndScheduledMatches(user, competitionId));
                }
            });
        }

        [Route("{competitionId}/current/number")]
        [HttpGet]
        public async Task<IHttpActionResult> GetCurrentNumberMatchAndBet(int competitionId)
        {
            return await HandleError(async () =>
            {
                var user = await GetUserByToken(Request);
                using (var betManager = new BetManager())
                {
                    return Ok(await betManager.NumberCurrentMatchAndBet(user, competitionId));
                }
            });
        }

        [Route("{competitionId}/result/number")]
        [HttpGet]
        public async Task<IHttpActionResult> GetFinishNumberMatchAndBet(int competitionId)
        {
            return await HandleError(async () =>
            {
                var user = await GetUserByToken(Request);
                using (var betManager = new BetManager())
                {
                    return Ok(await betManager.NumberFinishMatchAndBet(user, competitionId));
                }
            });
        }

        [Route("{page}/result/paginated")]
        [HttpGet]
        public async Task<IHttpActionResult> GetUserScheduledBetsPaginated(int page)
        {
            return await HandleError(async () =>
            {
                var user = await GetUserByToken(Request);
                using (var betManager = new BetManager())
                {
                    return Ok(await betManager.GetUserScheduledBetsPaginated(user, page));
                }
            });
        }
    }
}