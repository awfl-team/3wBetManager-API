using System;
using System.Configuration;
using System.Runtime.InteropServices;
using DAO;
using Manager;
using Microsoft.Owin.Hosting;
using MongoDB.Driver;

namespace _3wBetManager_API
{
    public class Program
    {
        private const int SW_HIDE = 0;
        private const int SW_SHOW = 5;

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private static void Main(string[] args)
        {
            var baseAddress = ConfigurationManager.AppSettings["baseUrl"];

            var client = new MongoClient(ConfigurationManager.AppSettings["dbUrl"]);
            var database = client.GetDatabase(ConfigurationManager.AppSettings["dbName"]);

            SingletonDao.Instance.SetAllDao(database);
            SetAllManager();

            var handle = GetConsoleWindow();
            //ShowWindow(handle, SW_HIDE);

            // Start OWIN host 
            using (WebApp.Start<Startup>(baseAddress))
            {
                Console.ReadLine();
            }
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
            SingletonManager.Instance.SetCompetitionManager(new CompetitionManager());
        }
    }
}