using DSharpPlus.CommandsNext;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using System;
using System.Threading;
using Newtonsoft.Json;
using DSharpPlus.Interactivity;

namespace RPBot
{
    class RPClass : Program
    {
        public static List<UserObject.RootObject> Users = new List<UserObject.RootObject>();
        public static List<ShopObject.RootObject> ItemsList = new List<ShopObject.RootObject>();
        public static List<GuildObject.RootObject> Guilds = new List<GuildObject.RootObject>();
        public static List<SpeechObject.RootObject> SpeechList = new List<SpeechObject.RootObject>();
        public static DiscordChannel GuildRankingChannel;
        public static DiscordChannel PlayerRankingChannel;
        public static DiscordChannel VillainRankingChannel;

        public static async Task Roll(CommandContext e, int numSides, int numRolls)
        {
            var interactivity = e.Client.GetInteractivityModule();

            if (numSides > 0 && numRolls > 0)
            {
				string name = "";
				if (e.Member.Nickname != null) name = e.Member.Nickname;
				else name = e.User.Username;
					
                int total = 0;
                string ans = name + " rolled: (";
                for (int i = 0; i < numRolls; i++)
                {
                    int roll = random.Next(1, numSides+1);
                    total += roll;
                    ans += roll + "+";
                }
                ans = ans.TrimEnd('+') + ") = " + total;
				if (ans.Length > 1000)
				{
					ans = name + " rolled: " + total;
				}
                await e.RespondAsync(ans);
            }
        }

        public static async Task Choose(CommandContext e, string ChoiceList)
		{
			string[] Choices = ChoiceList.Split(',');
			int randomChoice = random.Next(0, Choices.Length);
			await e.RespondAsync("Hmm. I choose... " + Choices[randomChoice]);
		}

        public static void SaveData(int saveType)
        {
			if (saveType == 1)
            {
                string output = JsonConvert.SerializeObject(Users, Formatting.Indented);
              
                File.WriteAllText("UserData.txt", output);
            }
			else if (saveType == 2)
            {
                string output = JsonConvert.SerializeObject(ItemsList, Formatting.Indented);

                File.WriteAllText("ShopData.txt", output);
            }
            else if (saveType == 3)
            {
                string output = JsonConvert.SerializeObject(Guilds, Formatting.Indented);

                File.WriteAllText("GuildData.txt", output);
            }
            else if (saveType == 4)
            {
                string output = JsonConvert.SerializeObject(SpeechList, Formatting.Indented);
                File.WriteAllText("SpeechData.txt", output);
            }
        }

        public static void LoadData()
        {
            if (File.ReadAllLines("UserData.txt").Any())
            {
                List<UserObject.RootObject> input = JsonConvert.DeserializeObject<List<UserObject.RootObject>>(File.ReadAllText("UserData.txt"));
                Users = input;

            }

            if (File.ReadAllLines("ShopData.txt").Any())
            {
                List<ShopObject.RootObject> input = JsonConvert.DeserializeObject<List<ShopObject.RootObject>>(File.ReadAllText("ShopData.txt"));
                ItemsList = input;
            }
            if (File.ReadAllLines("GuildData.txt").Any())
            {
                List<GuildObject.RootObject> input = JsonConvert.DeserializeObject<List<GuildObject.RootObject>>(File.ReadAllText("GuildData.txt"));
                Guilds = input;
            }
            if (File.ReadAllLines("SpeechData.txt").Any())
            {
                List<SpeechObject.RootObject> input = JsonConvert.DeserializeObject<List<SpeechObject.RootObject>>(File.ReadAllText("SpeechData.txt"));
                SpeechList = input;
            }
        }

        public static async Task AddUsers(DiscordGuild guild, bool update)
        {
            List<DiscordMember> AllUsers = new List<DiscordMember>(await guild.GetAllMembersAsync());
            AllUsers.RemoveAll(x => x.IsBot == true);
           List<DiscordChannel> Channels = new List<DiscordChannel>(await guild.GetChannelsAsync());

            foreach (DiscordMember user in AllUsers)
            {
                int role = 0;
                if (user.Roles.Any(x => x.Id == 312980663638818817 || x.Id == 312981984790052866 || x.Id == 312980292086530048))
                {
                    role = 1;
                }
                else if (user.Roles.Any(x => x.Id == 312982325908471808 || x.Id == 312982262310502401))
                {
                    role = 2;
                }

                if (user.Nickname == "" || string.IsNullOrWhiteSpace(user.Nickname))
                {
                    if (!Users.Any(x => x.UserData.userID == user.Id))
                    {
						Users.Add(new UserObject.RootObject(new UserObject.UserData(user.Id, user.Username, role, 1, 0, 0, 0, 0, ulong.Parse("0")), 0, new UserObject.InvData(new List<int>())));
                    }
                    if (Users.Find(x => x.UserData.userID == user.Id).UserData.username != user.Username)
                    {
                        Users.Find(x => x.UserData.userID == user.Id).UserData.username = user.Username;
                    }
                    if (Users.Find(x => x.UserData.userID == user.Id).UserData.role != role)
                    {
                        Users.Find(x => x.UserData.userID == user.Id).UserData.role = role;
                    }
                }
                else
                {
                    if (!Users.Any(x => x.UserData.userID == user.Id))
                    {
                        Users.Add(new UserObject.RootObject(new UserObject.UserData(user.Id, user.Nickname, role, 1, 0, 0, 0, 0, ulong.Parse("0")), 0, new UserObject.InvData(new List<int>())));

                    }
                    if (Users.Find(x => x.UserData.userID == user.Id).UserData.username != user.Nickname)
                    {
                        Users.Find(x => x.UserData.userID == user.Id).UserData.username = user.Nickname;
                    }
                    if (Users.Find(x => x.UserData.userID == user.Id).UserData.role != role)
                    {
                        Users.Find(x => x.UserData.userID == user.Id).UserData.role = role;
                    }
                }

            }
            if (update)
            {
                await XPClass.UpdateStats(Channels.Find(x => x.Id == 312964092748890114));
                await XPClass.UpdateGuildRanking(guild);
                await XPClass.UpdatePlayerRanking(guild, 1);
                await XPClass.UpdatePlayerRanking(guild, 2);
            }
            SaveData(1);
        }

		public static async Task UpdateClock(MessageCreateEventArgs e)
        {
            DiscordGuild RPGuild = e.Guild;
            List<DiscordChannel> RPChannels = new List<DiscordChannel>(await e.Guild.GetChannelsAsync());
            DiscordChannel AnnouncementChannel = RPChannels.First(x => x.Id == 312918289988976653);
            DateTime y = DateTime.Now.AddHours(-4);
            while (true)
            {
				await e.Client.UpdateStatusAsync(new Game(DateTime.Now.DayOfWeek + " " + DateTime.Now.Hour + ":" + DateTime.Now.Minute.ToString("00")));

                string TimePhase = "";
                if (DateTime.Now.Hour == 6 && y.AddHours(3) < DateTime.Now)
                {
                    TimePhase = "It is now dawn, on " + DateTime.Now.DayOfWeek;
                    y = DateTime.Now;
					await AnnouncementChannel.SendMessageAsync(TimePhase);
                }
                else if (DateTime.Now.Hour == 12 && y.AddHours(3) < DateTime.Now)
                {
                    TimePhase = "It is now midday, on " + DateTime.Now.DayOfWeek;
                    y = DateTime.Now;
                    await AnnouncementChannel.SendMessageAsync(TimePhase);

                }
                else if (DateTime.Now.Hour == 18 && y.AddHours(3) < DateTime.Now)
                {
                    TimePhase = "It is now dusk, on " + DateTime.Now.DayOfWeek;
                    y = DateTime.Now;
                    await AnnouncementChannel.SendMessageAsync(TimePhase);

                }

                Thread.Sleep(45000);
            }
        }
    }
}
