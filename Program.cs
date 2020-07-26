using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WebBot.Models;

namespace WebBot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }

    public static class PermanentValues
    {
        public static BotStatus BotStatus { get; set; }

        public static ConfigModel Config { get; set; }

        public static string GoogleSheetsCredentialsPath { get; set; }

        public static void ClearPermanentValues()
        {
            BotStatus = BotStatus.NotWorking;
            Config = new ConfigModel();
            GoogleSheetsCredentialsPath = null;
        }
    }
}
