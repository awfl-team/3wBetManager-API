using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using DAO.Interfaces;
using Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DAO
{
    public class BetDao : IBetDao
    {
        private readonly IMongoCollection<Bet> _collection;

        public BetDao(IMongoCollection<Bet> collection = null)
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("3wBetManager");
            _collection = collection ?? database.GetCollection<Bet>("bet");
        }

        public async Task<List<Bet>> FindAll()
        {
            return await _collection.Find(new BsonDocument()).ToListAsync();
        }

        public async Task<List<Bet>> FindFinishBets(User user, int competitionId)
        {
            var betsByUser = await FindBetsByUser(user);
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
            var betsByMatchStatus = betsByCompetition.FindAll(bet => bet.Match.Status == "FINISHED");

            return betsByMatchStatus;
        }

        public async Task<List<Bet>> FindBetsByUser(User user)
        {
            return await _collection.Find(bet => bet.User.Id == user.Id).ToListAsync();
        }

        public async Task<List<Bet>> FindBetsByUserBetCriteria(User user, string criteria)
        {
            return await _collection.Find(bet => bet.User.Id == user.Id && bet.Status == criteria).ToListAsync();
        }

        public async void AddBet(Bet bet)
        {
            await _collection.InsertOneAsync(bet);
        }

        public async Task<ExpandoObject> FindCurrentBetsAndScheduledMatches(User user, int competitionId)
        {
            var betsByUser = await _collection.Find(bet => bet.User.Id == user.Id).ToListAsync();
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
            var betsByMatchStatus = betsByCompetition.FindAll(bet => bet.Match.Status == "SCHEDULED");

            var matchByStatus = await Singleton.Instance.MatchDao.FindByStatus("SCHEDULED");
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

        public async void AddListBet(List<Bet> bets)
        {
            await _collection.InsertManyAsync(bets);
        }
        
        public async void DeleteBetsByUser(User user)
        {
            await _collection.DeleteManyAsync(bet => bet.User.Id == user.Id);
        }

        public async void UpdateListBet(List<Bet> bets)
        {
            foreach (var bet in bets)
            {
                await _collection.UpdateOneAsync(b => b.Id == bet.Id,
                    Builders<Bet>.Update.Set(b => b.HomeTeamScore, bet.HomeTeamScore)
                        .Set(b => b.AwayTeamScore, bet.AwayTeamScore));
            }
        }
    }
}