using System;
using DAO;

namespace FetchFootballData
{
    class Program
    {
        static void Main(string[] args)
        {
            var footballDataManager = new FootballDataManager();
            Singleton.Instance.SetAll();
            footballDataManager.GetAllCompetitions();
            System.Threading.Thread.Sleep(180000);
            footballDataManager.GetAllTeams();
            System.Threading.Thread.Sleep(180000);
            footballDataManager.GetAllMatchForAWeek();
            Console.ReadLine();
        }
    }
}
