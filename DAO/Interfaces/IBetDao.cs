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
        Task<List<Bet>> FindBetsByUser(User user);
        void UpdateBet(Bet bet);
        void DeleteBetsByUser(ObjectId id);
        void AddOrUpdateBet(User user, List<Bet> bets);
        Task<List<Bet>> FindBetsByMatch(Match match);
        void UpdateBetPointsWon(Bet bet, int point);
    }
}
