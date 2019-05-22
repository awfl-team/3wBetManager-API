using System.Collections.Generic;
using System.Threading.Tasks;
using Models;

namespace Manager.Interfaces
{
    public interface IBetManager
    {
        Task<List<Bet>> GetFinishBets(User user, int competitionId);
        Task<List<Bet>> GetFinishBetsLimited(User user);
        Task<List<Bet>> GetCurrentBetsLimited(User user);
        Task<dynamic> GetCurrentBetsAndScheduledMatches(User user, int competitionId);
        Task<dynamic> NumberCurrentMatchAndBet(User user, int competitionId);
        Task<dynamic> GetUserBetsPerType(User user);
        Task<dynamic> GetUserIncomesPerMonth(User user);
        Task<dynamic> GetUserIncomesPerYear(User user);
        Task<dynamic> GetUserBetsEarningsPerType(User user);
        Task<dynamic> NumberFinishMatchAndBet(User user, int competitionId);
        List<Bet> AddGuidList(User user, List<Bet> bets);
        Task<dynamic> GetUserScheduledBetsPaginated(User user, int page);
        List<Bet> ParseListBet(List<Bet> bets);
        Task AddBets(List<Bet> bets);
        Task ChangeBet(Bet bet);
    }
}