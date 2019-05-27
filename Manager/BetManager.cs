using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using DAO;
using DAO.Interfaces;
using Manager.Interfaces;
using Models;

namespace Manager
{
    public class BetManager : IBetManager
    {
        private readonly IBetDao _betDao;
        private readonly IMatchDao _matchDao;
        private readonly ITeamDao _teamDao;

        public BetManager(IBetDao betDao = null, ITeamDao teamDao = null, IMatchDao matchDao = null)
        {
            _betDao = betDao ?? SingletonDao.Instance.BetDao;
            _teamDao = teamDao ?? SingletonDao.Instance.TeamDao;
            _matchDao = matchDao ?? SingletonDao.Instance.MatchDao;
        }

        public async Task<List<Bet>> GetFinishBets(User user, int competitionId)
        {
            var betsByUser = await _betDao.FindBetsByUser(user);
            foreach (var bet in betsByUser)
            {
                var matchInformation = await _matchDao.FindMatch(bet.Match.Id);
                bet.Match = matchInformation;
                var awayTeamInformation = await _teamDao.FindTeam(bet.Match.AwayTeam.Id);
                if (awayTeamInformation != null) bet.Match.AwayTeam = awayTeamInformation;
                var homeTeamInformation = await _teamDao.FindTeam(bet.Match.HomeTeam.Id);
                if (homeTeamInformation != null) bet.Match.HomeTeam = homeTeamInformation;
            }

            var betsByCompetition = betsByUser.FindAll(bet => bet.Match.Competition.Id == competitionId);
            var betsByMatchStatus = betsByCompetition.FindAll(bet => bet.Match.Status == Match.FinishedStatus);

            return betsByMatchStatus;
        }

        public async Task<List<Bet>> GetFinishBetsLimited(User user)
        {
            var betsByUser = await _betDao.FindBetsByUser(user, 1);
            foreach (var bet in betsByUser)
            {
                var matchInformation = await _matchDao.FindMatch(bet.Match.Id);
                bet.Match = matchInformation;
                var awayTeamInformation = await _teamDao.FindTeam(bet.Match.AwayTeam.Id);
                if (awayTeamInformation != null) bet.Match.AwayTeam = awayTeamInformation;
                var homeTeamInformation = await _teamDao.FindTeam(bet.Match.HomeTeam.Id);
                if (homeTeamInformation != null) bet.Match.HomeTeam = homeTeamInformation;
            }

            return betsByUser.FindAll(bet => bet.Match.Status == Match.FinishedStatus).Take(Bet.DashboardMaxToShow)
                .ToList();
        }

        public async Task<List<Bet>> GetCurrentBetsLimited(User user)
        {
            var betsByUser = await SingletonDao.Instance.BetDao.FindBetsByUser(user, 1);
            foreach (var bet in betsByUser)
            {
                var matchInformation = await _matchDao.FindMatch(bet.Match.Id);
                bet.Match = matchInformation;
                var awayTeamInformation = await _teamDao.FindTeam(bet.Match.AwayTeam.Id);
                if (awayTeamInformation != null) bet.Match.AwayTeam = awayTeamInformation;
                var homeTeamInformation = await _teamDao.FindTeam(bet.Match.HomeTeam.Id);
                if (homeTeamInformation != null) bet.Match.HomeTeam = homeTeamInformation;
            }

            return betsByUser.FindAll(bet => bet.Match.Status == Match.ScheduledStatus).Take(Bet.DashboardMaxToShow)
                .ToList();
        }

        public async Task<dynamic> GetCurrentBetsAndScheduledMatches(User user, int competitionId)
        {
            var betsByUser = await _betDao.FindBetsByUser(user);
            foreach (var bet in betsByUser)
            {
                var matchInformation = await _matchDao.FindMatch(bet.Match.Id);
                bet.Match = matchInformation;
                var awayTeamInformation = await _teamDao.FindTeam(bet.Match.AwayTeam.Id);
                if (awayTeamInformation != null) bet.Match.AwayTeam = awayTeamInformation;
                var homeTeamInformation = await _teamDao.FindTeam(bet.Match.HomeTeam.Id);
                if (homeTeamInformation != null) bet.Match.HomeTeam = homeTeamInformation;
            }

            var now = DateTime.UtcNow;

            var betsByCompetition = betsByUser.FindAll(bet => bet.Match.Competition.Id == competitionId);
            var betsByMatchStatus = betsByCompetition.FindAll(bet => bet.Match.Status == Match.ScheduledStatus);
            var matchByStatus = await _matchDao.FindByStatus(Match.ScheduledStatus);
            var matchesByCompetition = matchByStatus.FindAll(m =>
                m.Competition.Id == competitionId && now <= DateTimeOffset.Parse(m.UtcDate));

            foreach (var bet in betsByMatchStatus)
            {
                var findMatch = matchesByCompetition.Find(m => m.Id == bet.Match.Id);
                if (findMatch != null) matchesByCompetition.Remove(findMatch);
            }

            foreach (var match in matchesByCompetition)
            {
                var awayTeamInformation = await _teamDao.FindTeam(match.AwayTeam.Id);
                if (awayTeamInformation != null) match.AwayTeam = awayTeamInformation;
                var homeTeamInformation = await _teamDao.FindTeam(match.HomeTeam.Id);
                if (homeTeamInformation != null) match.HomeTeam = homeTeamInformation;
            }

            dynamic betsAndMatches = new ExpandoObject();
            betsAndMatches.Bets = betsByMatchStatus;
            betsAndMatches.Matches = matchesByCompetition;
            return betsAndMatches;
        }

        public async Task<dynamic> NumberCurrentMatchAndBet(User user, int competitionId)
        {
            var currentBetsAndMatches = await GetCurrentBetsAndScheduledMatches(user, competitionId);
            if (currentBetsAndMatches.Bets.Count == 0 && currentBetsAndMatches.Matches.Count == 0)
                return new ExpandoObject();
            dynamic numberCurrentMatchAndBet = new ExpandoObject();
            numberCurrentMatchAndBet.NbBet = currentBetsAndMatches.Bets.Count;
            numberCurrentMatchAndBet.NbMatch = currentBetsAndMatches.Matches.Count;
            return numberCurrentMatchAndBet;
        }

        public async Task<dynamic> GetUserBetsPerType(User user)
        {
            var userBets = await _betDao.FindBetsByUser(user);

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

        public async Task<dynamic> GetUserIncomesPerMonth(User user)
        {
            var userBets = await _betDao.FindBetsByUser(user, 1);

            if (userBets.Count == 0) return new ExpandoObject();

            var userIncomesPerMonth = userBets.GroupBy(bet => bet.Date.ToString("yyyy/MM"))
                .Select(bet => new
                {
                    Date = bet.Key,
                    Points = bet.Sum(bet2 => bet2.PointsWon)
                }).ToList();

            return userIncomesPerMonth;
        }

        public async Task<dynamic> GetUserIncomesPerYear(User user)
        {
            var userBets = await _betDao.FindBetsByUser(user, 1);

            if (userBets.Count == 0) return new ExpandoObject();

            var userIncomesPerYear = userBets.GroupBy(bet => bet.Date.ToString("yyyy"))
                .Select(bet => new
                {
                    Date = bet.Key,
                    Points = bet.Sum(bet2 => bet2.PointsWon)
                }).ToList();

            return userIncomesPerYear;
        }

        public async Task<dynamic> GetUserBetsEarningsPerType(User user)
        {
            var userBets = await _betDao.FindBetsByUser(user);

            if (userBets.Count == 0) return new ExpandoObject();

            var perfectBetsPoint = 0;
            var okBetsPoint = 0;
            foreach (var bet in userBets)
                switch (bet.Status)
                {
                    case Bet.PerfectStatus:
                        perfectBetsPoint += bet.PointsWon;
                        break;
                    case Bet.OkStatus:
                        okBetsPoint += bet.PointsWon;
                        break;
                }

            dynamic userBetsPerType = new ExpandoObject();
            userBetsPerType.okBets = okBetsPoint;
            userBetsPerType.perfectBets = perfectBetsPoint;

            return userBetsPerType;
        }


        public async Task<dynamic> NumberFinishMatchAndBet(User user, int competitionId)
        {
            var finishBetsAndMatches = await GetFinishBets(user, competitionId);
            dynamic numberFinishBetsAndMatches = new ExpandoObject();
            if (finishBetsAndMatches.Count == 0) return numberFinishBetsAndMatches;
            numberFinishBetsAndMatches.NbBet = finishBetsAndMatches.Count;
            return numberFinishBetsAndMatches;
        }

        public List<Bet> AddGuidList(User user, List<Bet> bets)
        {
            foreach (var bet in bets)
            {
                bet.Guid = Guid.NewGuid().ToString();
                bet.User = user;
            }

            return bets;
        }

        public async Task<dynamic> GetUserScheduledBetsPaginated(User user, int page)
        {
            var betsByUser = await _betDao.FindBetsByUser(user, 1);

            foreach (var bet in betsByUser)
            {
                var matchInformation = await _matchDao.FindMatch(bet.Match.Id);
                bet.Match = matchInformation;
                var awayTeamInformation = await _teamDao.FindTeam(bet.Match.AwayTeam.Id);
                if (awayTeamInformation != null) bet.Match.AwayTeam = awayTeamInformation;
                var homeTeamInformation = await _teamDao.FindTeam(bet.Match.HomeTeam.Id);
                if (homeTeamInformation != null) bet.Match.HomeTeam = homeTeamInformation;
            }

            var finishedBets = betsByUser.FindAll(bet => bet.Match.Status == Match.ScheduledStatus);
            var totalBets = finishedBets.Count();
            var totalPages = totalBets / 10 + 1;
            page = page - 1;

            var betsToPass = 10 * page;
            var betsPaginated = await _betDao.PaginatedScheduledBets(betsToPass, user);
            dynamic obj = new ExpandoObject();
            obj.Items = betsPaginated;
            obj.TotalPages = totalPages;
            obj.TotalBets = totalBets;
            obj.Page = page + 1;

            return obj;
        }

        public List<Bet> ParseListBet(List<Bet> bets)
        {
            var betsParsed = new List<Bet>();
            var now = DateTime.UtcNow;
            foreach (var bet in bets)
                if (now <= DateTimeOffset.Parse(bet.Match.UtcDate) &&
                    (bet.AwayTeamScore >= 0 || bet.HomeTeamScore >= 0))
                    betsParsed.Add(bet);

            return betsParsed;
        }

        public async Task AddBets(List<Bet> bets)
        {
            await _betDao.AddListBet(bets);
        }

        public async Task ChangeBet(Bet bet)
        {
            await _betDao.UpdateBet(bet);
        }
    }
}