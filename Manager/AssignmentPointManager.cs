using System;
using DAO;
using Models;

namespace Manager
{
    public class AssignmentPointManager
    {
        public static async void AddPointToBet(Match match)
        {
            var bets = await Singleton.Instance.BetDao.FindBetsByMatch(match);
            foreach (var bet in bets)
            {
                var betTeamHightScore = GetTeamNameWithTheBestHightScore(bet);

                if (match.Score.FullTime.HomeTeam == bet.HomeTeamScore && match.Score.FullTime.AwayTeam == bet.AwayTeamScore)
                {
                    switch (betTeamHightScore)
                    {
                        case "AWAY":
                            Singleton.Instance.BetDao.UpdateBetPointsWon(bet, Bet.PerfectBet * match.AwayTeamRating);
                            Singleton.Instance.BetDao.UpdateBetStatus(bet, Bet.PerfectStatus);
                            continue;
                        case "HOME":
                            Singleton.Instance.BetDao.UpdateBetPointsWon(bet, Bet.PerfectBet * match.HomeTeamRating);
                            Singleton.Instance.BetDao.UpdateBetStatus(bet, Bet.PerfectStatus);
                            continue;
                        case "DRAW":
                            Singleton.Instance.BetDao.UpdateBetPointsWon(bet, Bet.PerfectBet * match.DrawRating);
                            Singleton.Instance.BetDao.UpdateBetStatus(bet, Bet.PerfectStatus);
                            continue;
                    }
                }

                switch (betTeamHightScore)
                {
                    case "HOME" when match.Score.Winner == "HOME_TEAM":
                        Singleton.Instance.BetDao.UpdateBetPointsWon(bet, Bet.OkBet * match.HomeTeamRating);
                        Singleton.Instance.BetDao.UpdateBetStatus(bet, Bet.OkStatus);
                        continue;
                    case "AWAY" when match.Score.Winner == "AWAY_TEAM":
                        Singleton.Instance.BetDao.UpdateBetPointsWon(bet, Bet.OkBet * match.AwayTeamRating);
                        Singleton.Instance.BetDao.UpdateBetStatus(bet, Bet.OkStatus);
                        continue;
                    case "DRAW" when match.Score.Winner =="DRAW":
                        Singleton.Instance.BetDao.UpdateBetPointsWon(bet, Bet.OkBet * match.DrawRating);
                        Singleton.Instance.BetDao.UpdateBetStatus(bet, Bet.OkStatus);
                        continue;
                    default:
                        Singleton.Instance.BetDao.UpdateBetPointsWon(bet, Bet.WrongBet);
                        Singleton.Instance.BetDao.UpdateBetStatus(bet, Bet.WrongStatus);
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
    }
}