using System;
using System.Diagnostics;
using System.Net.Http;
using System.Runtime.InteropServices;
using DAO;
using Microsoft.Owin.Hosting;

namespace _3wBetManager_API
{
    public class Program
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        static void Main(string[] args)
        {
            const string baseAddress = "http://151.80.136.92:9000/";
            Singleton.Instance.SetUserDao(new UserDao());
            Singleton.Instance.SetBetDao(new BetDao());
            Singleton.Instance.SeMatchDao(new MatchDao());
            Singleton.Instance.SetCompetitionDao(new CompetitionDao());
            Singleton.Instance.SetTeamDao(new TeamDao());
            var handle = GetConsoleWindow();
            ShowWindow(handle, SW_HIDE);
            //Process.Start(baseAddress + "swagger");

            // Start OWIN host 
            using (WebApp.Start<Startup>(url: baseAddress))
            {
                Console.ReadLine();
            }
        }
    }
}
