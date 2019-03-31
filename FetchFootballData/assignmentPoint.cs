using DAO;
using Models;

namespace FetchFootballData
{
    public class AssignmentPoint
    {
        public static async void AddPointToBet(Match match)
        {
            var bets = await Singleton.Instance.BetDao.FindBetsByMatch(match);
            foreach (var bet in bets)
            {
                if (match.Score.FullTime.HomeTeam == bet.HomeTeamScore &&
                    match.Score.FullTime.AwayTeam == bet.AwayTeamScore)
                {
                    Singleton.Instance.BetDao.UpdateBetPointsWon(bet, Bet.PerfectBet);
                    return;
                }

                switch (match.Score.Winner)
                {
                    case "HOME_TEAM":
                    {
                        if (bet.HomeTeamScore > bet.AwayTeamScore)
                        {
                            Singleton.Instance.BetDao.UpdateBetPointsWon(bet, Bet.OkBet);
                            return;
                        }

                        break;
                    }
                    case "AWAY_TEAM":
                    {
                        if (bet.AwayTeamScore > bet.HomeTeamScore)
                        {
                            Singleton.Instance.BetDao.UpdateBetPointsWon(bet, Bet.OkBet);
                            return;
                        }

                        break;
                    }
                }

                if (match.Score.FullTime.HomeTeam != bet.HomeTeamScore &&
                    match.Score.FullTime.AwayTeam != bet.AwayTeamScore)
                {
                    Singleton.Instance.BetDao.UpdateBetPointsWon(bet, Bet.WrongBet);
                    return;
                }
            }
        }
    }
}