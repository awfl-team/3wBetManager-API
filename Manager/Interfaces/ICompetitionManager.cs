using System.Collections.Generic;
using System.Threading.Tasks;
using Models;

namespace Manager.Interfaces
{
    public interface ICompetitionManager
    {
        Task<List<Competition>> GetAllCompetition();
    }
}