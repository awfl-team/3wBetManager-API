using System;
using System.Net.Http;
using Microsoft.Owin.Hosting;

namespace _3wBetManager_API
{
    public class Program
    {
        static void Main(string[] args)
        {
            const string baseAddress = "http://localhost:9000/";

            // Start OWIN host 
            using (WebApp.Start<Startup>(url: baseAddress))
            {
                // Create HttpCient and make a request to api/values 
                HttpClient client = new HttpClient();
            }
        }
    }
}
