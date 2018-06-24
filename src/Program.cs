using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using EthereumBalance.Caches;
using EthereumBalance.Configs;
using EthereumBalance.Database;

namespace EthereumBalance
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Gets config file from command line
            string configFile = null;
            for (int i = 0; i < args.Length; i++)
                if (args[i] == "-C")
                    configFile = args[i + 1];

            // Config file must be specified
            if (configFile == null)
            {
                Console.WriteLine($"Please specify a config file with -C");
                return;
            }

            // Checks and reads from config file
            if (!File.Exists(configFile))
            {
                Console.WriteLine($"Config file {configFile} does not exist");
                return;
            }
            var configs = JsonConvert.DeserializeObject<ConfigObject>(File.ReadAllText(configFile));

            // Uses reflection to check for empty values (properties only)
            foreach (var pi in typeof(ConfigObject).GetProperties())
                if (pi.GetValue(configs) == null)
                {
                    Console.WriteLine($"Config value for {pi.Name} is missing");
                    return;
                }

            // Configures services
            var webHostBuilder = GetWebHostBuilder(configs.hostUrl);
            webHostBuilder.ConfigureServices((services) =>
            {
                services.AddSingleton<ConfigObject>(configs);
                services.AddTransient<IDBManager, DBManager>();
                services.AddSingleton<Cache>(new Cache());
            });

            // Builds host
            var webHost = webHostBuilder.Build();
            var isp = webHost.Services;

            // Initializes database
            if (!File.Exists(configs.sqlitePath))
                File.Create(configs.sqlitePath).Close();
            using (var dbMgr = isp.GetRequiredService<IDBManager>())
                dbMgr.InitializeTables();

            // Initializes caches
            var caches = isp.GetRequiredService<Cache>();
            using (var dbMgr = isp.GetRequiredService<IDBManager>())
            {
                // Loads blocks
                foreach (var block in dbMgr.blockMgr.ListBlocks())
                    caches.blocks.Add(block.Height, block);
            }

            // Runs host
            webHost.Run();
        }

        public static IWebHostBuilder GetWebHostBuilder(string hostUrl) =>
            WebHost.CreateDefaultBuilder()
                .UseStartup<Startup>()
                .UseUrls(hostUrl);
    }
}
