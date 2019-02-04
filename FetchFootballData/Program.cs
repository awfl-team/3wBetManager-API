using System;
using System.Threading.Tasks;
using DAO;

namespace FetchFootballData
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var footballDataManager = new FootballDataManager();
            Singleton.Instance.SetAll();
            Console.WriteLine("----- Begin Fetch football data ----- ");
            await footballDataManager.GetAllCompetitions();
            await footballDataManager.GetAllTeams();
            await footballDataManager.GetAllMatchForAWeek();
            Console.WriteLine("----- End Fetch football data ----- ");
            System.Threading.Thread.Sleep(10000);
            Environment.Exit(0);
        }
    }
}