using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
