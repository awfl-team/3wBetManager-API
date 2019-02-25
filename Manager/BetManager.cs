using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
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

        public static async Task<List<Bet>> GetFinishBetsLimited(User user)
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

            return betsByUser.FindAll(bet => bet.Match.Status == Match.FinishedStatus).Take(Bet.DashboardMaxToShow).ToList();

        }

        public static async Task<List<Bet>> GetCurrentBetsLimited(User user)
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

            return betsByUser.FindAll(bet => bet.Match.Status == Match.ScheduledStatus).Take(Bet.DashboardMaxToShow).ToList();

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

            dynamic betsAndMatches = new ExpandoObject();
            betsAndMatches.Bets = betsByMatchStatus;
            betsAndMatches.Matches = matchesByCompetition;
            return betsAndMatches;
        }

        public static async Task<dynamic> NumberCurrentMatchAndBet(User user, int competitionId)
        {
            var currentBetsAndMatches = await GetCurrentBetsAndScheduledMatches(user, competitionId);
            if (currentBetsAndMatches.Bets.Count == 0 && currentBetsAndMatches.Matches.Count == 0)
            {
                return new ExpandoObject();
            }
            dynamic numberCurrentMatchAndBet = new ExpandoObject();
            numberCurrentMatchAndBet.NbBet = currentBetsAndMatches.Bets.Count;
            numberCurrentMatchAndBet.NbMatch = currentBetsAndMatches.Matches.Count;
            return numberCurrentMatchAndBet;
        }

        public static async Task<dynamic> GetUserBetsPerType(User user)
        {
            var userBets = await Singleton.Instance.BetDao.FindBetsByUser(user);
           
            if (userBets.Count == 0)
            {
               return new ExpandoObject();
            }

            var perfectBets = userBets.FindAll(bet => bet.Status == Bet.PerfectStatus);
            var okBets = userBets.FindAll(bet => bet.Status == Bet.OkStatus);
            var wrongBets = userBets.FindAll(bet => bet.Status == Bet.WrongStatus);

            dynamic userBetsPerType = new ExpandoObject();
            userBetsPerType.wrongBets = wrongBets.Count;
            userBetsPerType.okBets = okBets.Count;
            userBetsPerType.perfectBets = perfectBets.Count;

            return userBetsPerType;
        }

        public static async Task<dynamic> NumberFinishMatchAndBet(User user, int competitionId)
        {
            var finishBetsAndMatches = await GetFinishBets(user, competitionId);
            if (finishBetsAndMatches.Count == 0)
            {
                return new ExpandoObject();
            }
            dynamic numberFinishBetsAndMatches = new ExpandoObject();
            numberFinishBetsAndMatches.NbBet = finishBetsAndMatches.Count;
            return numberFinishBetsAndMatches;
        }

        public static List<Bet> AddGuidList(User user,List<Bet> bets)
        {
            foreach (var bet in bets)
            {
                bet.Guid = Guid.NewGuid().ToString();
                bet.User = user;
            }

            return bets;
        }
    }
}
