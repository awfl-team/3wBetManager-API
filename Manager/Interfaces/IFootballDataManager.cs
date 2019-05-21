using System.Threading.Tasks;

namespace Manager.Interfaces
{
    public interface IFootballDataManager
    {
        Task GetAllCompetitions();
        Task GetAllTeams();
        Task GetAllMatchForAWeek();
    }
}