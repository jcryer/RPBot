using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using System;
using Newtonsoft.Json;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace RPBot
{
    static class RPClass
    {
        public static List<ulong> RPCategories = new List<ulong>()
        { 526492402941296651, 510299090656034816, 510299212382994443,
            510299976329330698, 510299305513451541, 510299354301595669,
            510299441627136020, 510299398501171251,537714909941727232,
            511895853674528778, 513361784955207701 };

        public static List<UserObject.RootObject> Users = new List<UserObject.RootObject>();
        public static List<GuildObject.RootObject> Guilds = new List<GuildObject.RootObject>();
        public static List<SpeechObject.RootObject> SpeechList = new List<SpeechObject.RootObject>();
        public static List<InstanceObject.RootObject> InstanceList = new List<InstanceObject.RootObject>();
        public static List<InstanceObject.ChannelTemplate> ChannelTemplates = new List<InstanceObject.ChannelTemplate>();
        public static List<TagObject.RootObject> TagsList = new List<TagObject.RootObject>();
        public static List<SignupObject.RootObject> SignupList = new List<SignupObject.RootObject>();
        public static Dictionary<ulong, ulong> approvalsList = new Dictionary<ulong, ulong>(); // Channel ID : User ID
        public static Dictionary<string, string> CardList = new Dictionary<string, string>();
        public static SVObject.RootObject SVData = new SVObject.RootObject();
        public static WeatherList WeatherList = new WeatherList();
        public static DiscordChannel PlayerRankingChannel;
        public static DiscordChannel StatsChannel;
        public static DiscordChannel ApprovalsCategory;
        public static DiscordChannel GuildRankingChannel;
        public static DiscordChannel InstanceCategory;
        public static DiscordChannel GameChannel;
        public static DiscordChannel FameChannel;
        public static DiscordRole AdminRole;
        public static DiscordRole MuteRole;

        public static DiscordGuild RPGuild;
        public static Random Random = new Random();
        
        public static bool FirstRun = true;
        public static Extensions.SlidingBuffer<KeyValuePair<ulong, string>> MessageBuffer = new Extensions.SlidingBuffer<KeyValuePair<ulong, string>>(500);
        public static bool Restarted = false;

        public static void SaveData(int saveType)
        {
            if (saveType == -1)
            {
                SaveData(1);
                SaveData(2);
                SaveData(3);
                SaveData(4);
                SaveData(5);
                SaveData(6);
                SaveData(7);
                SaveData(8);
                SaveData(9);
                SaveData(10);

            }
            if (saveType == 1)
            {
                string output = JsonConvert.SerializeObject(Users, Formatting.Indented);
                File.WriteAllText("Data/UserData.txt", output);
            }
            else if (saveType == 2)
            {
                string output = JsonConvert.SerializeObject(SignupList, Formatting.Indented);

                File.WriteAllText("Data/SignupList.txt", output);
            }
            else if (saveType == 3)
            {
                string output = JsonConvert.SerializeObject(Guilds, Formatting.Indented);

                File.WriteAllText("Data/GuildData.txt", output);
            }
            else if (saveType == 4)
            {
                string output = JsonConvert.SerializeObject(SpeechList, Formatting.Indented);
                File.WriteAllText("Data/SpeechData.txt", output);
            }
            else if (saveType == 5)
            {
                string output = JsonConvert.SerializeObject(CardList, Formatting.Indented);
                File.WriteAllText("Data/CardData.txt", output);
            }
            else if (saveType == 6)
            {
                string output = JsonConvert.SerializeObject(ChannelTemplates, Formatting.Indented);
                File.WriteAllText("Data/ChannelTemplates.txt", output);
            }
            else if (saveType == 7)
            {
                string output = JsonConvert.SerializeObject(InstanceList, Formatting.Indented);
                File.WriteAllText("Data/InstanceData.txt", output);
            }
            else if (saveType == 8)
            {
                string output = JsonConvert.SerializeObject(approvalsList, Formatting.Indented);
                File.WriteAllText("Data/ApprovalsList.txt", output);
            }
            else if (saveType == 9)
            {

                string output = JsonConvert.SerializeObject(TagsList, Formatting.Indented);
                File.WriteAllText("Data/TagsList.txt", output);
            }
            else if (saveType == 10)
            {

                string output = JsonConvert.SerializeObject(WeatherList, Formatting.Indented);
                File.WriteAllText("Data/WeatherList.txt", output);
            }
        }

        public static void LoadData()
        {
            if (!File.Exists("Data/WeatherList.txt")) File.Create("Data/WeatherList.txt");

            if (File.ReadAllLines("Data/UserData.txt").Any())
            {
                List<UserObject.RootObject> input = JsonConvert.DeserializeObject<List<UserObject.RootObject>>(File.ReadAllText("Data/UserData.txt"));
                Users = input;
            }
            TriviaClass.TriviaList = Directory.GetFiles("trivia/").ToList();
            for (int i = 0; i < TriviaClass.TriviaList.Count; i++)
            {
                TriviaClass.TriviaList[i] = TriviaClass.TriviaList[i].Replace(".txt", "").Replace("trivia/", "");
            }
            if (File.ReadAllLines("Data/GuildData.txt").Any())
            {
                List<GuildObject.RootObject> input = JsonConvert.DeserializeObject<List<GuildObject.RootObject>>(File.ReadAllText("Data/GuildData.txt"));
                Guilds = input;
            }
            if (File.ReadAllLines("Data/SpeechData.txt").Any())
            {
                List<SpeechObject.RootObject> input = JsonConvert.DeserializeObject<List<SpeechObject.RootObject>>(File.ReadAllText("Data/SpeechData.txt"));
                SpeechList = input;
            }
            if (File.ReadAllLines("Data/ChannelTemplates.txt").Any())
            {
                List<InstanceObject.ChannelTemplate> input = JsonConvert.DeserializeObject<List<InstanceObject.ChannelTemplate>>(File.ReadAllText("Data/ChannelTemplates.txt"));
                ChannelTemplates = input;
            }
            CardList = new Dictionary<string, string>();
            if (Directory.GetFiles("Cards/Done/") != null)
            {
                foreach (var file in Directory.GetFiles("Cards/Done/")) {
                    if (!CardList.ContainsKey(file.Split('-')[1].Split('.')[0]))
                    {
                        CardList.Add(file.Split('-')[1].Split('.')[0], $"Cards/Done/front-{ file.Split('-')[1].Split('.')[0] }.png¬Cards/Done/back-{file.Split('-')[1].Split('.')[0] }.png");
                    }
                }
            }
            if (File.ReadAllLines("Data/InstanceData.txt").Any())
            {
                List<InstanceObject.RootObject> input = JsonConvert.DeserializeObject<List<InstanceObject.RootObject>>(File.ReadAllText("Data/InstanceData.txt"));
                InstanceList = input;
            }
            if (File.ReadAllLines("Data/ApprovalsList.txt").Any())
            {
                Dictionary<ulong, ulong> input = JsonConvert.DeserializeObject<Dictionary<ulong, ulong>>(File.ReadAllText("Data/ApprovalsList.txt"));
                approvalsList = input;
            }
            if (File.ReadAllLines("Data/TagsList.txt").Any())
            {
                List<TagObject.RootObject> input = JsonConvert.DeserializeObject<List<TagObject.RootObject>>(File.ReadAllText("Data/TagsList.txt"));
                TagsList = input;
            }
            if (File.ReadAllLines("Data/SignupList.txt").Any())
            {
                List<SignupObject.RootObject> input = JsonConvert.DeserializeObject<List<SignupObject.RootObject>>(File.ReadAllText("Data/SignupList.txt"));
                SignupList = input;
            }
            if (File.ReadAllLines("Data/WeatherList.txt").Any())
            {
                WeatherList input = JsonConvert.DeserializeObject<WeatherList>(File.ReadAllText("Data/WeatherList.txt"));
                WeatherList = input;
            }
        }

        public static async Task AddOrUpdateUsers(DiscordGuild guild, bool update)
        {
            List<DiscordMember> AllUsers = new List<DiscordMember>(await guild.GetAllMembersAsync());
            AllUsers.RemoveAll(x => x.IsBot);
            List<DiscordChannel> Channels = new List<DiscordChannel>(await guild.GetChannelsAsync());
            foreach (DiscordMember user in AllUsers)
            {
                if (!Users.Any(x => x.UserData.UserID == user.Id))
                {
                    Users.Add(new UserObject.RootObject(new UserObject.UserData(user.Id, user.DisplayName), new UserObject.InvData()));
                }
                Users.Find(x => x.UserData.UserID == user.Id).UserData.Username = user.DisplayName;
            }
            if (update)
            {
                await XPClass.UpdateStats(StatsChannel);
                await XPClass.UpdateGuildRanking(guild);
                await XPClass.UpdatePlayerRanking(guild);

            }
            SaveData(-1);
        }

		public static async Task UpdateClock(MessageCreateEventArgs e, DiscordClient d)
        {
            List<DiscordChannel> RPChannels = new List<DiscordChannel>(await e.Guild.GetChannelsAsync());
            DiscordChannel AnnouncementChannel = RPChannels.First(x => x.Id == 543522226058690560);
            DateTime y = DateTime.UtcNow.AddHours(-4);

            while (true)
            {
                if (WeatherList.DatePosted != DateTime.Today)
                {
                    WeatherList = JsonConvert.DeserializeObject<WeatherList>(File.ReadAllText("Data/WeatherList.txt"));

                    if (DateTime.UtcNow.DayOfWeek == DayOfWeek.Sunday)
                    {
                        if (!WeatherList.WeatherObjects.Any()) WeatherList.WeatherObjects.Add(new WeatherObject(DateTime.Today.AddDays(-1), 12, 6, WeatherType.Rain, 15, "NW", 50));

                        if (!WeatherList.WeatherObjects.Any(x => x.Date == DateTime.Today))
                        {
                            for (int i = 0; i < 7; i++)
                            {
                                WeatherList.WeatherObjects.Add(Weather.NextDay(WeatherList.WeatherObjects.Last()));
                            }
                        }
                        WeatherList.DatePosted = DateTime.Today;
                        SaveData(10);
                        List<WeatherObject> week = WeatherList.WeatherObjects.Where(x => x.Date >= DateTime.Today && x.Date < DateTime.Today.AddDays(8)).ToList();

                        for (int i = 1; i < week.Count; i++)
                        {
                            int displace = Random.Next(-4, 4);
                            week[i].High += Random.Next(-1, 1) + displace;
                            week[i].Low -= Random.Next(-1, 1) + displace;
                            int weatherType = (int)week[i].Type + Random.Next(-2, 2);
                            if (weatherType < 0) weatherType = 0;
                            if (weatherType > 11) weatherType = 11;
                            week[i].Type = (WeatherType)weatherType;
                            if (week[i].High < week[i].Low)
                            {
                                int high = week[i].High;
                                week[i].High = week[i].Low;
                                week[i].Low = high;
                            }
                        }

                        string fileName = Weather.GenerateSevenDays(week, DateTime.Today.ToString("dd-MM-yyyy.png"));
                        await RPChannels.First(x => x.Id == 544790402071265280).SendFileAsync(fileName);
                        File.Delete(fileName);

                    }
                    else
                    {
                        if (!WeatherList.WeatherObjects.Any()) WeatherList.WeatherObjects.Add(new WeatherObject(DateTime.Today.AddDays(-1), 12, 6, WeatherType.Rain, 15, "NW", 50));

                        if (!WeatherList.WeatherObjects.Any(x => x.Date == DateTime.Today))
                        {
                            for (int i = 0; i < 7; i++)
                            {
                                WeatherList.WeatherObjects.Add(Weather.NextDay(WeatherList.WeatherObjects.Last()));
                            }
                        }

                        string fileName = Weather.GenerateOneDay(WeatherList.WeatherObjects.First(x => x.Date == DateTime.Today), DateTime.Today.ToString("dd-MM-yyyy.png"));
                        await RPChannels.First(x => x.Id == 544790402071265280).SendFileAsync(fileName);
                        File.Delete(fileName);
                        WeatherList.DatePosted = DateTime.Today;
                        SaveData(10);
                    }
                }

                await d.UpdateStatusAsync(new DiscordActivity("Time pass: " + DateTime.UtcNow.Hour + ":" + DateTime.UtcNow.Minute.ToString("00"), ActivityType.Watching)); 
                string TimePhase = "";
                if (DateTime.UtcNow.Minute == 0 && DateTime.UtcNow.Hour == 6 && y.AddHours(2) < DateTime.UtcNow)
                {
                    TimePhase = "It is now dawn, on " + DateTime.UtcNow.DayOfWeek;
                    y = DateTime.UtcNow;
					await AnnouncementChannel.SendMessageAsync(TimePhase);
                }
                else if (DateTime.UtcNow.Minute == 0 && DateTime.UtcNow.Hour == 12 && y.AddHours(2) < DateTime.UtcNow)
                {
                    TimePhase = "It is now midday, on " + DateTime.UtcNow.DayOfWeek;
                    y = DateTime.UtcNow;
                    await AnnouncementChannel.SendMessageAsync(TimePhase);

                    
                }

                else if (DateTime.UtcNow.Minute == 0 && DateTime.UtcNow.Hour == 18 && y.AddHours(2) < DateTime.UtcNow)
                {
                    
                    TimePhase = "It is now dusk, on " + DateTime.UtcNow.DayOfWeek;
                    y = DateTime.UtcNow;
                    await AnnouncementChannel.SendMessageAsync(TimePhase);
                }

                else if (DateTime.UtcNow.Minute == 0 && (DateTime.UtcNow.Hour == 0 || DateTime.UtcNow.Hour == 24) && y.AddHours(2) < DateTime.UtcNow)
                {

                    TimePhase = "It is now midnight, on " + DateTime.UtcNow.DayOfWeek;
                    y = DateTime.UtcNow;
                    await AnnouncementChannel.SendMessageAsync(TimePhase);
                }
                await Task.Delay(45000);
            }
        }
    }
}
