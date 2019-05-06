using System;
using DAO;
using DAO.Interfaces;
using Models;

namespace Manager
{
    public class AssignmentPointManager : IDisposable
    {
        private IBetDao _betDao;

        public AssignmentPointManager(IBetDao betDao = null)
        {
            _betDao = betDao ?? Singleton.Instance.BetDao;
        }

        public async void AddPointToBet(Match match)
        {
            var bets = await _betDao.FindBetsByMatch(match);
            foreach (var bet in bets)
            {
                var betTeamHightScore = GetTeamNameWithTheBestHightScore(bet);
                var multiply = bet.Multiply;
                if (multiply == 0)
                {
                    multiply = 1;
                }

                if (match.Score.FullTime.HomeTeam == bet.HomeTeamScore &&
                    match.Score.FullTime.AwayTeam == bet.AwayTeamScore)
                {
                    switch (betTeamHightScore)
                    {
                        case "AWAY":
                            _betDao.UpdateBetPointsWon(bet,
                                (Bet.PerfectBet * match.AwayTeamRating) * multiply);
                            _betDao.UpdateBetStatus(bet, Bet.PerfectStatus);
                            continue;
                        case "HOME":
                            _betDao.UpdateBetPointsWon(bet,
                                (Bet.PerfectBet * match.HomeTeamRating) * multiply);
                            _betDao.UpdateBetStatus(bet, Bet.PerfectStatus);
                            continue;
                        case "DRAW":
                            _betDao.UpdateBetPointsWon(bet,
                                (Bet.PerfectBet * match.DrawRating) * multiply);
                            _betDao.UpdateBetStatus(bet, Bet.PerfectStatus);
                            continue;
                    }
                }

                switch (betTeamHightScore)
                {
                    case "HOME" when match.Score.Winner == "HOME_TEAM":
                        _betDao.UpdateBetPointsWon(bet,
                            (Bet.OkBet * match.HomeTeamRating) * multiply);
                        _betDao.UpdateBetStatus(bet, Bet.OkStatus);
                        continue;
                    case "AWAY" when match.Score.Winner == "AWAY_TEAM":
                        _betDao.UpdateBetPointsWon(bet,
                            (Bet.OkBet * match.AwayTeamRating) * multiply);
                        _betDao.UpdateBetStatus(bet, Bet.OkStatus);
                        continue;
                    case "DRAW" when match.Score.Winner == "DRAW":
                        _betDao.UpdateBetPointsWon(bet, (Bet.OkBet * match.DrawRating) * multiply);
                        _betDao.UpdateBetStatus(bet, Bet.OkStatus);
                        continue;
                    default:
                        _betDao.UpdateBetPointsWon(bet, Bet.WrongBet);
                        _betDao.UpdateBetStatus(bet, Bet.WrongStatus);
                        break;
                }
            }
        }

        public static string GetTeamNameWithTheBestHightScore(Bet bet)
        {
            if (bet.HomeTeamScore > bet.AwayTeamScore)
            {
                return "HOME";
            }

            if (bet.AwayTeamScore > bet.HomeTeamScore)
            {
                return "AWAY";
            }

            if (bet.AwayTeamScore == bet.HomeTeamScore)
            {
                return "DRAW";
            }

            return null;
        }

        public void Dispose()
        {
            _betDao = null;
        }
    }
}