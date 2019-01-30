using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace DAO.Interfaces
{
    public interface ICompetitionDao
    {
        Task<List<Competition>> FindAllCompetitions();
        void AddCompetition(Competition competition);
        void ReplaceCompetition(int id, Competition competition);
        Task<Competition> FindCompetition(int id);
    }
}
