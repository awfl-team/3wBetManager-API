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
                if (match.Score.FullTime.HomeTeam == bet.HomeTeamScore && match.Score.FullTime.AwayTeam == bet.AwayTeamScore)
                {
                    Singleton.Instance.BetDao.UpdateBetPointsWon(bet, Bet.PerfectBet);
                    continue;
                }

                var betTeamHightScore = GetTeamNameWithTheBestHightScore(bet);

                if (betTeamHightScore == "HOME" && match.Score.Winner == "HOME_TEAM")
                {
                    Singleton.Instance.BetDao.UpdateBetPointsWon(bet, Bet.OkBet);
                    continue;
                }

                if (betTeamHightScore == "AWAY" && match.Score.Winner == "AWAY_TEAM")
                {
                    Singleton.Instance.BetDao.UpdateBetPointsWon(bet, Bet.OkBet);
                    continue;
                }

                // TODO Verifi if "DRAW" is good
                if (betTeamHightScore == "DRAW" && match.Score.Winner =="DRAW" )
                {
                    Singleton.Instance.BetDao.UpdateBetPointsWon(bet, Bet.OkBet);
                    continue;
                }

                Singleton.Instance.BetDao.UpdateBetPointsWon(bet, Bet.WrongBet);


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