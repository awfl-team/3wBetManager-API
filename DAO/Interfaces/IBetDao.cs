using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using Models;
using MongoDB.Bson;

namespace DAO.Interfaces
{
    public interface IBetDao
    {
        void AddBet(Bet bet);
        Task<List<Bet>> FindAll();
        Task<List<Bet>> FindBetsByUser(User user);
        Task<List<Bet>> FindBetsByUserBetCriteria(User user, string criteria);
        Task<List<Bet>> FindFinishBets(User user, int competitionId);
        void UpdateBet(Bet bet);
        void DeleteBetsByUser(ObjectId id);
        Task<ExpandoObject> FindCurrentBetsAndScheduledMatches(User user, int competitionId);
        void AddOrUpdateBet(User user, List<Bet> bets);
    }
}
