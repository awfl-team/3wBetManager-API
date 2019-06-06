using Models;

namespace Manager.Interfaces
{
    public interface IAssignmentPointManager
    {
        void AddPointToBet(Match match);
        string GetTeamNameWithTheBestHightScore(Bet bet);
    }
}