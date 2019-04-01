﻿using System;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using DAO;
using Manager;
using MongoDB.Driver;

namespace FetchFootballData
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
                {
                    Console.WriteLine("----- Begin Fetch football data ----- ");
                    await footballDataManager.GetAllCompetitions();
                    await footballDataManager.GetAllTeams();
                    await footballDataManager.GetAllMatchForAWeek();
                    Console.WriteLine("----- End Fetch football data ----- ");
                    UserManager.RecalculateUserPoints();
                    Thread.Sleep(10000);
                    Environment.Exit(0);
                }

            if (args[0] == "MONITORING")
                using (var monitoringManager = new MonitoringManager())
                {
                    await monitoringManager.ResponseApi();
                }

            if (args[0] == "FIXTURE")
            {
                await ItemManager.CreateDefaultItems();
            }
        }
    }
}