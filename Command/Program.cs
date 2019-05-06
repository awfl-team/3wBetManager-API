using System;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using DAO;
using Manager;
using MongoDB.Driver;

namespace Command
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var client = new MongoClient(ConfigurationManager.AppSettings["dbUrl"]);
            var database = client.GetDatabase(ConfigurationManager.AppSettings["dbName"]);
            Singleton.Instance.SetAll(database);
            if (args[0] == "REFRESH")
                using (var footballDataManager = new FootballDataManager())
                using (var userManager = new UserManager())
                {
                    Console.WriteLine("----- Begin Fetch football data ----- ");
                    await footballDataManager.GetAllCompetitions();
                    await footballDataManager.GetAllTeams();
                    await footballDataManager.GetAllMatchForAWeek();
                    Console.WriteLine("----- End Fetch football data ----- ");
                    userManager.RecalculateUserPoints();
                    Thread.Sleep(10000);
                }

            if (args[0] == "MONITORING")
                using (var monitoringManager = new MonitoringManager())
                {
                    await monitoringManager.ResponseApi();
                }

            if (args[0] == "FIXTURE")
            {
                using (var itemManager = new ItemManager())
                {
                await itemManager.CreateDefaultItems();

                }
            }

            Environment.Exit(0);
        }
    }
}