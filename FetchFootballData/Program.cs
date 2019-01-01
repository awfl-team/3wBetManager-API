using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAO;

namespace FetchFootballData
{
    class Program
    {
        static void Main(string[] args)
        {
            FootballDataManager footballDataManager = new FootballDataManager();
            Singleton.Instance.SetCompetitionDao(new CompetitionDao());
            Singleton.Instance.SetTeamDao(new TeamDao());
            Singleton.Instance.SeMatchDao(new MatchDao());
            footballDataManager.GetAllCompetitions();
            System.Threading.Thread.Sleep(60000);
            footballDataManager.GetAllTeams();
            System.Threading.Thread.Sleep(60000);
            footballDataManager.GetAllMatchForTheWeek();
            
            Console.ReadLine();
            
        }
    }
}
