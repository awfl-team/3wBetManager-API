using DAO;
using DAO.Interfaces;
using Models;

namespace Manager
{
    public class MatchManager
    {
        private readonly IBetDao _betDao;
        private readonly IMatchDao _matchDao;

        public MatchManager(IBetDao betDao = null, IMatchDao matchDao = null)
        {
            _betDao = betDao ?? Singleton.Instance.BetDao;
            _matchDao = matchDao ?? Singleton.Instance.MatchDao;
        }

        public async void CalculateMatchRating(Match match)
        {
            var bets = await _betDao.FindBetsByMatch(match);

            var homeTeamCount = bets.FindAll(bet => bet.AwayTeamScore < bet.HomeTeamScore).Count;
            var awayTeamCount = bets.FindAll(bet => bet.AwayTeamScore > bet.HomeTeamScore).Count;
            var drawCount = bets.FindAll(bet => bet.AwayTeamScore == bet.HomeTeamScore).Count;


            match.DrawRating = drawCount == 0d ? 0d : 1d / (drawCount/ bets.Count);
            match.HomeTeamRating = homeTeamCount == 0d ? 0d : 1d / (homeTeamCount / bets.Count);
            match.AwayTeamRating = awayTeamCount == 0d ? 0d : 1d / (awayTeamCount/ bets.Count);

            _matchDao.UpdateMatch(match.Id, match);
        }

    }
}