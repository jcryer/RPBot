using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RPBot
{
    class XPClass : RPClass
    {
        public static async Task XP(CommandContext e, string user, int xpNum)
        {
            ulong userNum = 0;
            if (xpNum != 0 && ulong.TryParse(user, out userNum))
            {
                if (e.Member.Roles.Any(x => x.Id == 313845882841858048))
                {
                    DiscordChannel StatsChannel = await e.Client.GetChannelAsync(312964092748890114);
                    await AddUsers(e.Guild, false);
                    await UpdateStats(StatsChannel);
                    try
                    {
                        UserObject.RootObject userData = Users.Find(x => x.UserData.userID == userNum);
                        userData.xp += xpNum;
                        if (userData.xp < 0) userData.xp = 0;

                        await UpdateStats(StatsChannel);
                        SaveData(1);
						await e.RespondAsync("Stat changed.");
						await UpdatePlayerRanking(e.Guild, 1);
						await UpdatePlayerRanking(e.Guild, 2);
                    }
                    catch
                    {
                        await e.RespondAsync("Mention a user to select them.");
                    }
                }
            }
        }

        public static async Task UpdateStats(DiscordChannel c)
        {
            int longestName = 1;
            if (Users.Any()) longestName = Users.Max(x => x.UserData.username.Length) + 1;
            int longestXP = 5;

            string Name = "Name".PadRight(longestName) + "|  ";
            string XP = "XP ";

            string value = "```" + Name + XP + "\n--------------" + new string('-', longestName) + "\n";

            List<UserObject.RootObject> SortedUsers = new List<UserObject.RootObject>();

            SortedUsers = Users.OrderByDescending(x => x.xp).ToList();

           
            List<DiscordMessage> msgs = new List<DiscordMessage>(await c.GetMessagesAsync(5));
            foreach (DiscordMessage msg in msgs)
            {
                await msg.DeleteAsync();
                Thread.Sleep(500);
            }

            foreach (UserObject.RootObject user in SortedUsers)
            {
                if (value.Length > 1500)
                {
                    await c.SendMessageAsync(value + "```");
                    value = "```";
                }

                value += user.UserData.username.PadRight(longestName) + "| " + user.xp.ToString().PadRight(longestXP) + "\n";
            }
            await c.SendMessageAsync(value + "```");
        }





        public static async Task Guild(CommandContext e, string cmd, string subcmd, string subsubcmd)
        {
            if (e.Member.Roles.Any(x => x.Id == 312961839359328266) || e.Member.Roles.Any(x => x.Id == 312961821063512065) || e.Member.Roles.Any(x => x.Id == 312979390516559885))
            {
                if (cmd == "create".ToLower())
                {
                    Guilds.Add(new GuildObject.RootObject(1 + Guilds.Count, subcmd, 1, new List<ulong>()));
                    await UpdateGuildRanking(e.Guild);
                    SaveData(3);
                    await e.RespondAsync("Guild created.");
                }
                else if (cmd == "destroy".ToLower())
                {
                    try
                    {
                        foreach (UserObject.RootObject user in Users.FindAll(x => x.UserData.guildID == Guilds.First(y => y.name == subcmd).id))
                        {
                            user.UserData.guildID = 0;

                        }
                        Guilds.Remove(Guilds.First(x => x.name == subcmd));
                        await UpdatePlayerRanking(e.Guild, 1);
                        await UpdatePlayerRanking(e.Guild, 2);
                        await UpdateGuildRanking(e.Guild);

                        SaveData(3);
                        SaveData(1);

                        await e.RespondAsync("Guild deleted.");
                    }
                    catch
                    {
                        await e.RespondAsync("No guild found with that name. Are you sure you typed it in correctly?");
                    }
                }
                else if (cmd == "changestatus".ToLower())
                {
                    if (subsubcmd != null)
                    {
                        int status = 0;
                        if (int.TryParse(subsubcmd, out status) && (status == 1 || status == 2))
                        {
                            try
                            {
                                Guilds.First(x => x.name == subcmd).status = status;
                                await UpdateGuildRanking(e.Guild);
                                SaveData(3);
                                await e.RespondAsync("Guild status changed.");
                            }
                            catch
                            {
                                await e.RespondAsync("No guild found with that name. Are you sure you typed it in correctly?");
                            }
                        }
                        else
                        {
                            await e.RespondAsync("Use 1 for Active or 2 for Retired.");
                        }
                    }
                    else
                    {
                        await e.RespondAsync("Specify the new status - 1 for Active or 2 for Retired.");

                    }
                }
                else if (cmd == "adduser".ToLower())
                {
                    if (subsubcmd != null)
                    {
                        string user = subsubcmd.Replace("<", "").Replace(">", "").Replace("@", "").Replace("!", "");

                        ulong userNum = 0;
                        if (ulong.TryParse(user, out userNum))
                        {
                            try
                            {
                                Guilds.First(x => x.name == subcmd).userIDs.Add(Users.First(x => x.UserData.userID == userNum).UserData.userID);
                                Users.First(x => x.UserData.userID == userNum).UserData.guildID = Guilds.First(x => x.name == subcmd).id;
                                if (Users.First(x => x.UserData.userID == userNum).UserData.role == 1) await UpdatePlayerRanking(e.Guild, 1);
                                else if (Users.First(x => x.UserData.userID == userNum).UserData.role == 2) await UpdatePlayerRanking(e.Guild, 2);
                                await UpdateGuildRanking(e.Guild);
                                SaveData(3);
                                SaveData(1);

                                await e.RespondAsync("User added to guild.");
                            }
                            catch
                            {
                                await e.RespondAsync("No guild found with that name. Are you sure you typed it in correctly?");
                            }
                        }
                        else
                        {
                            await e.RespondAsync("You must mention the user to add them to the guild.");
                        }
                    }
                    else
                    {
                        await e.RespondAsync("You must mention the user to add them to the guild.");
                    }
                }
                else if (cmd == "removeuser".ToLower())
                {
                    if (subsubcmd != null)
                    {
                        string user = subsubcmd.Replace("<", "").Replace(">", "").Replace("@", "").Replace("!", "");

                        ulong userNum = 0;
                        if (ulong.TryParse(user, out userNum))
                        {
                            try
                            {
                                Guilds.First(x => x.name == subcmd).userIDs.Remove(Users.First(x => x.UserData.userID == userNum).UserData.userID);
                                Users.First(x => x.UserData.userID == userNum).UserData.guildID = 0;
                                if (Users.First(x => x.UserData.userID == userNum).UserData.role == 1) await UpdatePlayerRanking(e.Guild, 1);
                                else if (Users.First(x => x.UserData.userID == userNum).UserData.role == 2) await UpdatePlayerRanking(e.Guild, 2);
                                await UpdateGuildRanking(e.Guild);

                                SaveData(3);
                                SaveData(1);
                                await e.RespondAsync("User removed from guild.");
                            }
                            catch
                            {
                                await e.RespondAsync("No guild found with that name. Are you sure you typed it in correctly?");
                            }
                        }
                        else
                        {
                            await e.RespondAsync("You must mention the user to remove them from the guild.");
                        }
                    }
                    else
                    {
                        await e.RespondAsync("You must mention the user to remove them from the guild.");
                    }

                }

                else if (cmd == "all".ToLower())
                {
                    await e.RespondAsync("You have been sent a PM " + e.User.Mention + "!");
                    DiscordMember user = await e.Guild.GetMemberAsync(e.User.Id);
                    DiscordDmChannel dm = await user.CreateDmChannelAsync();
                    DiscordEmbedBuilder embed = new DiscordEmbedBuilder();

                    foreach (GuildObject.RootObject guild in Guilds)
                    {
                        embed.AddField(guild.name, "Status: " + (guild.status == 1 ? "Active" : guild.status == 2 ? "Retired" : "ERROR"));
                    }
                    embed.Color = new DiscordColor(4589319);
                    embed.WithFooter("Heroes & Villains");
                    
                    await dm.SendMessageAsync("", embed: embed);
                }
            }
        }
        
        public static async Task UpdatePlayerRanking(DiscordGuild e, int type)
		{
			DiscordChannel RankingChannel = RPClass.PlayerRankingChannel;

            if (type == 2) RankingChannel = RPClass.VillainRankingChannel;

			int longestName = 0;
			if (type == 1) longestName = Users.Where(x => x.UserData.role == 1).Max(x => x.UserData.username.Length);
			else if (type == 2) longestName = Users.Where(x => x.UserData.role == 2).Max(x => x.UserData.username.Length);

			int longestStatus = 10;
			int longestStats = 17;
            int longestGuild = 10;
            if (Guilds.Any()) longestGuild = Guilds.Max(x => x.name.Length) + 1;

			string Name = "Name".PadRight(longestName) + "| ";
			string Status = "Status    | ";
			string Stats = "";
			if (type == 1) Stats = "Resolved Cases".PadRight(longestStats) + "| ";
			else if (type == 2) Stats = "Crimes Committed".PadRight(longestStats) + "| ";
			string Guild = "Guild".PadRight(longestGuild) + "| ";
			string Rank = "Rank";
			string value = "";
			value += "```" + Name + Status + Stats + Guild + Rank + "\n-------------------------------------------------------------------\n";
			List<UserObject.RootObject> SortedUsers = new List<UserObject.RootObject>();

			if (type == 1) SortedUsers = Users.Where(x => x.UserData.role == 1).OrderByDescending(x => ((x.xp + 1) * (x.UserData.resolvedCases + 1))).ToList();
			else if (type == 2) SortedUsers = Users.Where(x => x.UserData.role == 2).OrderByDescending(x => ((x.xp + 1) * (x.UserData.crimesCommitted + 1))).ToList();
			List<DiscordMessage> msgs = new List<DiscordMessage>(await RankingChannel.GetMessagesAsync(50));
			foreach (DiscordMessage msg in msgs)
			{
                await msg.DeleteAsync();
			}

			foreach (UserObject.RootObject user in SortedUsers)
			{
                string UserStats = "";
				if (type == 1) UserStats += user.UserData.resolvedCases;
                else if (type == 2) UserStats += user.UserData.crimesCommitted;


                int rank = user.xp;
				string UserRank = "S";
				if (rank < 10000) UserRank = "A";
				if (rank < 8000) UserRank = "B";
				if (rank < 4000) UserRank = "C";
				if (rank < 2000) UserRank = "D";

				string UserGuild = "";
				if (user.UserData.guildID == 0) UserGuild += "N/A";
				else UserGuild += Guilds.First(x => x.id == user.UserData.guildID && x.userIDs.Contains(user.UserData.userID)).name;

				if (value.Length > 1500)
				{
					await RankingChannel.SendMessageAsync(value + "```");
					value = "```";
				}

				value += (user.UserData.username.PadRight(longestName) + "| " + (user.UserData.status == 1 ? "Active" : user.UserData.status == 2 ? "Retired" : user.UserData.status == 3 ? "Deceased" : "Error").PadRight(longestStatus) + "| " + UserStats.PadRight(longestStats) + "| " + UserGuild.PadRight(longestGuild) + "| " + UserRank + "\n");
			}
			await RankingChannel.SendMessageAsync(value + "```");
		}

        public static async Task UpdateGuildRanking(DiscordGuild e)
        {
            DiscordChannel RankingChannel = RPClass.GuildRankingChannel;
            int longestName = 10;
            if (Guilds.Any()) longestName = Guilds.Max(x => x.name.Length) + 1;
            int longestStatus = 10;
            int longestStats = 31;

            string Name = "Name".PadRight(longestName) + "| ";
            string Status = "Status    | ";
            string  Stats = "Resolved Cases/Crimes Committed".PadRight(longestStats) + "| ";
            string Rank = "Rank";
            string value = "```" + Name + Status + Stats  + Rank + "\n-------------------------------------------------------------------\n";

            List<GuildObject.RootObject> GuildsNew = new List<GuildObject.RootObject>();
            foreach (GuildObject.RootObject guild in Guilds)
            {
                int stats = 0;
                UserObject.RootObject user;
                foreach (ulong num in guild.userIDs)
                {
                    user = Users.First(x => x.UserData.userID == num);
                    stats += user.UserData.resolvedCases + user.UserData.crimesCommitted;
                }
                int totalStats = stats;
                if (guild.userIDs.Count > 0) stats = (stats / guild.userIDs.Count);
                GuildsNew.Add(new GuildObject.RootObject(0, guild.name, guild.status, new List<ulong>() { ulong.Parse(stats.ToString()), ulong.Parse(totalStats.ToString()) }));
            }
            List<GuildObject.RootObject> SortedGuilds = GuildsNew.OrderByDescending(x => x.userIDs[0]).ToList();
            List<DiscordMessage> msgs = new List<DiscordMessage>(await RankingChannel.GetMessagesAsync(50));
            foreach (DiscordMessage msg in msgs)
            {
                await msg.DeleteAsync();
            }

            foreach (GuildObject.RootObject guild in SortedGuilds)
            {

                string GuildRank = "S";
                if (guild.userIDs[0] < 1500) GuildRank = "A";
                if (guild.userIDs[0] < 1200) GuildRank = "B";
                if (guild.userIDs[0] < 1000) GuildRank = "C";
                if (guild.userIDs[0] < 500) GuildRank = "D";

                if (value.Length > 1500)
                {
                    await RankingChannel.SendMessageAsync(value + "```");
                    value = "```";
                }

                value += (guild.name.PadRight(longestName) + "| " + (guild.status == 1 ? "Active" : guild.status == 2 ? "Retired" : "Error").PadRight(longestStatus) + "| " + guild.userIDs[1].ToString().PadRight(longestStats) + "| " + GuildRank + "\n");
            }
            await RankingChannel.SendMessageAsync(value + "```");
        }
    }
}
