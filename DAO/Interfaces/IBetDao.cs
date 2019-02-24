using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DAO.Interfaces
{
    public interface IBetDao
    {
        Task AddBet(Bet bet);
        Task<List<Bet>> FindBetsByUser(User user);
        Task<Bet> Find(Bet bet);
        Task<UpdateResult> UpdateBet(Bet bet);
        void DeleteBetsByUser(ObjectId id);
        void AddOrUpdateBet(User user, List<Bet> bets);
        Task<List<Bet>> FindBetsByMatch(Match match);
        void UpdateBetPointsWon(Bet bet, int point);
    }
}
