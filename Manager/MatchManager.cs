using System;
using DAO;
using DAO.Interfaces;
using Manager.Interfaces;
using Models;

namespace Manager
{
    public class MatchManager : IMatchManager
    {
        private readonly IBetDao _betDao;
        private readonly IMatchDao _matchDao;

        public MatchManager(IBetDao betDao = null, IMatchDao matchDao = null)
        {
            _betDao = betDao ?? SingletonDao.Instance.BetDao;
            _matchDao = matchDao ?? SingletonDao.Instance.MatchDao;
        }

        public async void CalculateMatchRating(Match match)
        {
            var bets = await _betDao.FindBetsByMatch(match);

            var homeTeamCount = bets.FindAll(bet => bet.AwayTeamScore < bet.HomeTeamScore).Count;
            var awayTeamCount = bets.FindAll(bet => bet.AwayTeamScore > bet.HomeTeamScore).Count;
            var drawCount = bets.FindAll(bet => bet.AwayTeamScore == bet.HomeTeamScore).Count;


            match.DrawRating = drawCount == 0d ? 0d : 1d / (Convert.ToDouble(drawCount) / Convert.ToDouble(bets.Count));
            match.HomeTeamRating = homeTeamCount == 0d
                ? 0d
                : 1d / (Convert.ToDouble(homeTeamCount) / Convert.ToDouble(bets.Count));
            match.AwayTeamRating = awayTeamCount == 0d
                ? 0d
                : 1d / (Convert.ToDouble(awayTeamCount) / Convert.ToDouble(bets.Count));

            _matchDao.UpdateMatch(match.Id, match);
        }
    }
}