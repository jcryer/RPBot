using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using Newtonsoft.Json;

namespace SLOBot
{
    public class Program
    {

        public static string token;
        public static string LatestPosts;
        public static List<string> FullLog = new List<string>();
        public static List<AutoTranslate> UsersTranslated = new List<AutoTranslate>();
        public static List<int> Colours = new List<int>() { 15158332, 10181046, 3447003, 2667619, 15844367, 8622482, 1845299 };
        public static bool firstRun = true;
        public static Random random = new Random();
        public static bool processing = false;
        public static void Main(string[] args) => new Program().Run(args).GetAwaiter().GetResult();

        private async Task Run(string[] args)
        {
            LatestClass.LatestStartup();
			SAClass.LoadData();
            var cfg = new Config();
            var json = string.Empty;
            if (!File.Exists("config.json"))
            {
                json = JsonConvert.SerializeObject(cfg);
                File.WriteAllText("config.json", json, new UTF8Encoding(false));
                Console.WriteLine("Config file was not found, a new one was generated. Fill it with proper values and rerun this program");
                Console.ReadKey();

                return;
            }

            json = File.ReadAllText("config.json", new UTF8Encoding(false));
            cfg = JsonConvert.DeserializeObject<Config>(json);

            var tskl = new List<Task>();
            for (var i = 0; i < cfg.ShardCount; i++)
            {
                var bot = new SLOBot(cfg, i);
                tskl.Add(bot.RunAsync());
                await Task.Delay(7500);
            }

            await Task.WhenAll(tskl);
            await Task.Delay(-1);
        }

        
    }
}