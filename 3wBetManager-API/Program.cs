using System;
using System.Configuration;
using System.Runtime.InteropServices;
using DAO;
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

            Singleton.Instance.SetAll(database);
            var handle = GetConsoleWindow();
            ShowWindow(handle, SW_HIDE);

            // Start OWIN host 
            using (WebApp.Start<Startup>(baseAddress))
            {
                Console.ReadLine();
            }
        }
    }
}