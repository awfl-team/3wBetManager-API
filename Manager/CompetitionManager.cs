using System.Collections.Generic;
using System.Threading.Tasks;
using DAO;
using DAO.Interfaces;
using Manager.Interfaces;
using Models;

namespace Manager
{
    public class CompetitionManager: ICompetitionManager
    {
        private readonly ICompetitionDao _competitionDao;

        public CompetitionManager(ICompetitionDao competitionDao = null)
        {
            _competitionDao = competitionDao ?? SingletonDao.Instance.CompetitionDao;
        }
        public async Task<List<Competition>> GetAllCompetition()
        {
            return await _competitionDao.FindAllCompetitions();
        }
    }
}
