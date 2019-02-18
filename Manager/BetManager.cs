using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAO;
using Models;

namespace Manager
{
    public class BetManager
    {
        public static async Task<List<Bet>> GetFinishBets(User user, int competitionId)
        {
            var betsByUser = await Singleton.Instance.BetDao.FindBetsByUser(user);
            foreach (var bet in betsByUser)
            {
                var matchInformation = await Singleton.Instance.MatchDao.FindMatch(bet.Match.Id);
                bet.Match = matchInformation;
                var awayTeamInformation = await Singleton.Instance.TeamDao.FindTeam(bet.Match.AwayTeam.Id);
                bet.Match.AwayTeam = awayTeamInformation;
                var homeTeamInformation = await Singleton.Instance.TeamDao.FindTeam(bet.Match.HomeTeam.Id);
                bet.Match.HomeTeam = homeTeamInformation;
            }

            var betsByCompetition = betsByUser.FindAll(bet => bet.Match.Competition.Id == competitionId);
            var betsByMatchStatus = betsByCompetition.FindAll(bet => bet.Match.Status == Match.FinishedStatus);

            return betsByMatchStatus;
        }

        public static async Task<dynamic> GetCurrentBetsAndScheduledMatches(User user, int competitionId)
        {
            var betsByUser = await Singleton.Instance.BetDao.FindBetsByUser(user);
            foreach (var bet in betsByUser)
            {
                var matchInformation = await Singleton.Instance.MatchDao.FindMatch(bet.Match.Id);
                bet.Match = matchInformation;
                var awayTeamInformation = await Singleton.Instance.TeamDao.FindTeam(bet.Match.AwayTeam.Id);
                bet.Match.AwayTeam = awayTeamInformation;
                var homeTeamInformation = await Singleton.Instance.TeamDao.FindTeam(bet.Match.HomeTeam.Id);
                bet.Match.HomeTeam = homeTeamInformation;
            }

            var betsByCompetition = betsByUser.FindAll(bet => bet.Match.Competition.Id == competitionId);
            var betsByMatchStatus = betsByCompetition.FindAll(bet => bet.Match.Status == Match.ScheduledStatus);

            var matchByStatus = await Singleton.Instance.MatchDao.FindByStatus(Match.ScheduledStatus);
            var matchesByCompetition = matchByStatus.FindAll(m => m.Competition.Id == competitionId);

            foreach (var bet in betsByMatchStatus)
            {
                var findMatch = matchesByCompetition.Find(m => m.Id == bet.Match.Id);
                if (findMatch != null)
                {
                    matchesByCompetition.Remove(findMatch);
                }
            }

            foreach (var match in matchesByCompetition)
            {
                var awayTeamInformation = await Singleton.Instance.TeamDao.FindTeam(match.AwayTeam.Id);
                match.AwayTeam = awayTeamInformation;
                var homeTeamInformation = await Singleton.Instance.TeamDao.FindTeam(match.HomeTeam.Id);
                match.HomeTeam = homeTeamInformation;
            }

            dynamic betsAndMatches = new { };
            betsAndMatches.Bets = betsByMatchStatus;
            betsAndMatches.Matches = matchesByCompetition;
            return betsAndMatches;
        }

        public static async Task<dynamic> NumberCurrentMatchAndBet(User user, int competitionId)
        {
            var currentBetsAndMatches = await GetCurrentBetsAndScheduledMatches(user, competitionId);
            if (currentBetsAndMatches.Bets.Count == 0 && currentBetsAndMatches.Matches.Count == 0)
            {
                return null;
            }
            dynamic numberCurrentMatchAndBet = new { };
            numberCurrentMatchAndBet.NbBet = currentBetsAndMatches.Bets.Count;
            numberCurrentMatchAndBet.NbMatch = currentBetsAndMatches.Matches.Count;
            return numberCurrentMatchAndBet;
        }

        public static async Task<dynamic> NumberFinishMatchAndBet(User user, int competitionId)
        {
            var finishBetsAndMatches = await GetFinishBets(user, competitionId);
            if (finishBetsAndMatches.Count == 0)
            {
                return null;
            }
            dynamic numberFinishBetsAndMatches = new { };
            numberFinishBetsAndMatches.NbBet = finishBetsAndMatches.Count;
            return numberFinishBetsAndMatches;
        }
    }
}
