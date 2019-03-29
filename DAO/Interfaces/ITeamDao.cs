using System.Threading.Tasks;
using Models;

namespace DAO.Interfaces
{
    public interface ITeamDao
    {
        void AddTeam(Team team);
        void ReplaceTeam(int id, Team team);
        Task<Team> FindTeam(int id);
    }
}