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
        Task<List<Bet>> FindBetsByUser(User user, int sortOrder = -1);
        Task<Bet> Find(Bet bet);
        Task<List<Bet>> FindAll();
        Task<UpdateResult> UpdateBet(Bet bet);
        void DeleteBetsByUser(ObjectId id);
        Task AddListBet(List<Bet> bets);
        Task<List<Bet>> FindBetsByMatch(Match match);
        void UpdateBetPointsWon(Bet bet, int point);
    }
}
