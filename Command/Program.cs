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
            SingletonDao.Instance.SetAllDao(database);
            SetAllManager();
            if (args[0] == "REFRESH")
            {
                Console.WriteLine("----- Begin Fetch football data ----- ");
                await SingletonManager.Instance.FootballDataManager.GetAllCompetitions();
                await SingletonManager.Instance.FootballDataManager.GetAllTeams();
                await SingletonManager.Instance.FootballDataManager.GetAllMatchForAWeek();
                Console.WriteLine("----- End Fetch football data ----- ");
                SingletonManager.Instance.UserManager.RecalculateUserPoints();
                Thread.Sleep(10000);
            }


            if (args[0] == "MONITORING") await SingletonManager.Instance.MonitoringManager.ResponseApi();


            if (args[0] == "FIXTURE") await SingletonManager.Instance.ItemManager.CreateDefaultItems();

            Environment.Exit(0);
        }

        internal static void SetAllManager()
        {
            SingletonManager.Instance.SetAssignmentPointManager(new AssignmentPointManager());
            SingletonManager.Instance.SetBetManager(new BetManager());
            SingletonManager.Instance.SetEmailManager(new EmailManager());
            SingletonManager.Instance.SetFootballDataManager(new FootballDataManager());
            SingletonManager.Instance.SetItemManager(new ItemManager());
            SingletonManager.Instance.SetMatchManager(new MatchManager());
            SingletonManager.Instance.SetMonitoringManager(new MonitoringManager());
            SingletonManager.Instance.SetTokenManager(new TokenManager());
            SingletonManager.Instance.SetUserManager(new UserManager());
        }
    }
}