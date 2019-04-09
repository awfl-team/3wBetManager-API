﻿using System;
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
            var betsByUser = await Singleton.Instance.BetDao.FindBetsByUser(user, 1);
            foreach (var bet in betsByUser)
            {
                var matchInformation = await Singleton.Instance.MatchDao.FindMatch(bet.Match.Id);
                bet.Match = matchInformation;
                var awayTeamInformation = await Singleton.Instance.TeamDao.FindTeam(bet.Match.AwayTeam.Id);
                bet.Match.AwayTeam = awayTeamInformation;
                var homeTeamInformation = await Singleton.Instance.TeamDao.FindTeam(bet.Match.HomeTeam.Id);
                bet.Match.HomeTeam = homeTeamInformation;
            }

            return betsByUser.FindAll(bet => bet.Match.Status == Match.FinishedStatus).Take(Bet.DashboardMaxToShow)
                .ToList();
        }

        public static async Task<List<Bet>> GetCurrentBetsLimited(User user)
        {
            var betsByUser = await Singleton.Instance.BetDao.FindBetsByUser(user, 1);
            foreach (var bet in betsByUser)
            {
                var matchInformation = await Singleton.Instance.MatchDao.FindMatch(bet.Match.Id);
                bet.Match = matchInformation;
                var awayTeamInformation = await Singleton.Instance.TeamDao.FindTeam(bet.Match.AwayTeam.Id);
                bet.Match.AwayTeam = awayTeamInformation;
                var homeTeamInformation = await Singleton.Instance.TeamDao.FindTeam(bet.Match.HomeTeam.Id);
                bet.Match.HomeTeam = homeTeamInformation;
            }

            return betsByUser.FindAll(bet => bet.Match.Status == Match.ScheduledStatus).Take(Bet.DashboardMaxToShow)
                .ToList();
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
                if (findMatch != null) matchesByCompetition.Remove(findMatch);
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
                return new ExpandoObject();
            dynamic numberCurrentMatchAndBet = new ExpandoObject();
            numberCurrentMatchAndBet.NbBet = currentBetsAndMatches.Bets.Count;
            numberCurrentMatchAndBet.NbMatch = currentBetsAndMatches.Matches.Count;
            return numberCurrentMatchAndBet;
        }

        public static async Task<dynamic> GetUserBetsPerType(User user)
        {
            var userBets = await Singleton.Instance.BetDao.FindBetsByUser(user);

            if (userBets.Count == 0) return new ExpandoObject();

            var perfectBets = userBets.FindAll(bet => bet.Status == Bet.PerfectStatus);
            var okBets = userBets.FindAll(bet => bet.Status == Bet.OkStatus);
            var wrongBets = userBets.FindAll(bet => bet.Status == Bet.WrongStatus);

            dynamic userBetsPerType = new ExpandoObject();
            userBetsPerType.wrongBets = wrongBets.Count;
            userBetsPerType.okBets = okBets.Count;
            userBetsPerType.perfectBets = perfectBets.Count;

            return userBetsPerType;
        }

        public static async Task<dynamic> GetUserIncomesPerMonth(User user)
        {
            var userBets = await Singleton.Instance.BetDao.FindBetsByUser(user, 1);

            if (userBets.Count == 0) return new ExpandoObject();

            var userIncomesPerMonth = userBets.GroupBy(bet => bet.Date.ToString("yyyy/MM"))
                .Select(bet => new
                {
                    Date = bet.Key,
                    Points = bet.Sum(bet2 => bet2.PointsWon)
                }).ToList();

            return userIncomesPerMonth;
        }

        public static async Task<dynamic> GetUserIncomesPerYear(User user)
        {
            var userBets = await Singleton.Instance.BetDao.FindBetsByUser(user, 1);

            if (userBets.Count == 0) return new ExpandoObject();

            var userIncomesPerYear = userBets.GroupBy(bet => bet.Date.ToString("yyyy"))
                .Select(bet => new
                {
                    Date = bet.Key,
                    Points = bet.Sum(bet2 => bet2.PointsWon)
                }).ToList();

            return userIncomesPerYear;
        }

        public static async Task<dynamic> GetUserBetsEarningsPerType(User user)
        {
            var userBets = await Singleton.Instance.BetDao.FindBetsByUser(user);

            if (userBets.Count == 0) return new ExpandoObject();

            var perfectBets = userBets.FindAll(bet => bet.Status == Bet.PerfectStatus);
            var okBets = userBets.FindAll(bet => bet.Status == Bet.OkStatus);
            var wrongBets = userBets.FindAll(bet => bet.Status == Bet.WrongStatus);

            dynamic userBetsPerType = new ExpandoObject();
            userBetsPerType.okBets = okBets.Count * Bet.OkBet;
            userBetsPerType.perfectBets = perfectBets.Count * Bet.PerfectBet;

            return userBetsPerType;
        }

        public static async Task<dynamic> NumberFinishMatchAndBet(User user, int competitionId)
        {
            var finishBetsAndMatches = await GetFinishBets(user, competitionId);
            dynamic numberFinishBetsAndMatches = new ExpandoObject();
            if (finishBetsAndMatches.Count == 0) return numberFinishBetsAndMatches;
            numberFinishBetsAndMatches.NbBet = finishBetsAndMatches.Count;
            return numberFinishBetsAndMatches;
        }

        public static List<Bet> AddGuidList(User user, List<Bet> bets)
        {
            foreach (var bet in bets)
            {
                bet.Guid = Guid.NewGuid().ToString();
                bet.User = user;
            }

            return bets;
        }

        public static async Task<dynamic> GetUserScheduledBetsPaginated(User user, int page)
        {
            var betsByUser = await Singleton.Instance.BetDao.FindBetsByUser(user, 1);
           
            foreach (var bet in betsByUser)
            {
                var matchInformation = await Singleton.Instance.MatchDao.FindMatch(bet.Match.Id);
                bet.Match = matchInformation;
                var awayTeamInformation = await Singleton.Instance.TeamDao.FindTeam(bet.Match.AwayTeam.Id);
                bet.Match.AwayTeam = awayTeamInformation;
                var homeTeamInformation = await Singleton.Instance.TeamDao.FindTeam(bet.Match.HomeTeam.Id);
                bet.Match.HomeTeam = homeTeamInformation;
            }

            var finishedBets = betsByUser.FindAll(bet => bet.Match.Status == Match.FinishedStatus);
            var totalBets = finishedBets.Count();
            var totalPages = totalBets / 10 + 1;
            page = page - 1;
           

            var betsToPass = 10 * page;
            var betsPaginated = await Singleton.Instance.BetDao.PaginatedScheduledBets(betsToPass);
            dynamic obj = new ExpandoObject();
            obj.Items = betsPaginated;
            obj.TotalPages = totalPages;
            obj.TotalBets = totalBets;
            obj.Page = page + 1;

            return obj;
        }
    }
}