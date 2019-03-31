using System;
using System.Runtime.CompilerServices;
using DAO;
using Models;

namespace Manager
{
    public class MatchManager
    {
        public static async void CalculateMatchRating(Match match)
        {
            var betDao = Singleton.Instance.BetDao;
            var matchDao = Singleton.Instance.MatchDao;

            var bets = await betDao.FindBetsByMatch(match);

            double homeTeamCount = bets.FindAll(bet => bet.AwayTeamScore < bet.HomeTeamScore).Count;
            double awayTeamCount = bets.FindAll(bet => bet.AwayTeamScore > bet.HomeTeamScore).Count;
            double drawCount = bets.FindAll(bet => bet.AwayTeamScore == bet.HomeTeamScore).Count;


            match.DrawRating = drawCount == 0d ? 0d : 1d / (drawCount/ bets.Count);
            match.HomeTeamRating = homeTeamCount == 0d ? 0d : 1d / (homeTeamCount / bets.Count);
            match.AwayTeamRating = awayTeamCount == 0d ? 0d : 1d / (awayTeamCount/ bets.Count);

            matchDao.UpdateMatch(match.Id, match);
        }
    }
}