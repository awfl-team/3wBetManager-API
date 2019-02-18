﻿using System;
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
            try
            {
                var user = await GetUserByToken(Request);
                GetBetDao().AddOrUpdateBet(user, bets);
                return Ok();
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        [Route("{competitionId}/result")]
        [HttpGet]
        public async Task<IHttpActionResult> GetBetsResult(int competitionId)
        {
            try
            {
                var user = await GetUserByToken(Request);
                return Ok(await BetManager.GetFinishBets(user, competitionId));
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        [Route("{competitionId}/current")]
        [HttpGet]
        public async Task<IHttpActionResult> GetBetsAndMatches(int competitionId)
        {
            try
            {
                var user = await GetUserByToken(Request);
                return Ok(await BetManager.GetCurrentBetsAndScheduledMatches(user, competitionId));
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        [Route("{competitionId}/current/number")]
        [HttpGet]
        public async Task<IHttpActionResult> GetCurrentNumberMatchAndBet(int competitionId)
        {
            try
            {
                var user = await GetUserByToken(Request);
                return Ok(await BetManager.NumberCurrentMatchAndBet(user, competitionId));
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        [Route("{competitionId}/result/number")]
        [HttpGet]
        public async Task<IHttpActionResult> GetFinishNumberMatchAndBet(int competitionId)
        {
            try
            {
                var user = await GetUserByToken(Request);
                return Ok(await BetManager.NumberFinishMatchAndBet(user, competitionId));
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }
    }
}