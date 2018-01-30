using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RPBot
{
    [Group("xp", CanInvokeWithoutSubcommand = true), Description("Admin command to give XP"), RequireRoles(RoleCheckMode.Any, "Staff")]
    class XPClass : BaseCommandModule
    {
        public async Task ExecuteGroupAsync(CommandContext e, [Description("User to change stats of")] DiscordMember user, [Description("How much you wish to change it by")] int xpNum)
        {
            if (xpNum != 0)
            {
                UserObject.RootObject userData = RPClass.Users.Find(x => x.UserData.UserID == user.Id);
                userData.Xp += xpNum;
                if (userData.Xp < 0) userData.Xp = 0;
                
                await UpdateStats(RPClass.StatsChannel);
                RPClass.SaveData(1);
                UserObject.RootObject newUserData = RPClass.Users.Find(x => x.UserData.UserID == user.Id);
                switch (newUserData.UserData.Role)
                {
                    case 1:
                        await UpdatePlayerRanking(e.Guild, 1);
                        break;
                    case 2:
                        await UpdatePlayerRanking(e.Guild, 2);
                        break;
                    case 3:
                        await UpdatePlayerRanking(e.Guild, 3);
                        break;
                    case 4:
                        await UpdatePlayerRanking(e.Guild, 4);
                        break;
                    case 0:
                        await UpdatePlayerRanking(e.Guild, 1);
                        await UpdatePlayerRanking(e.Guild, 2);
                        await UpdatePlayerRanking(e.Guild, 3);
                        await UpdatePlayerRanking(e.Guild, 4);
                        break;
                }
                await e.RespondAsync("Stat changed.");
            }
        }
        [Command("update"), Description("Updates saved data")]
        public async Task Update(CommandContext e)
        {
            await RPClass.AddOrUpdateUsers(e.Guild, true);
            await e.RespondAsync("Done!");
        }
        
        [Command("bulk"), Description("Staff command to give multiple people XP (Better for bot).")]
        public async Task Bulk (CommandContext e)
        {

            await e.RespondAsync("Change stats by typing `<mention> <xp amount>.\nTo end this process and save, type `stop`.");
            var interactivity = e.Client.GetInteractivity();

            AnotherMessage:
            
            var msg = await interactivity.WaitForMessageAsync(x => x.Author == e.Member, TimeSpan.FromSeconds(120));
            if (msg != null)
            {
                if (msg.Message.Content == "stop")
                {
                    await UpdateStats(RPClass.StatsChannel);
                    RPClass.SaveData(1);
                    await UpdatePlayerRanking(e.Guild, 1);
                    await UpdatePlayerRanking(e.Guild, 2);
                    await UpdatePlayerRanking(e.Guild, 3);
                    await UpdatePlayerRanking(e.Guild, 4);

                    await e.RespondAsync("Stats updated.");
                }
                else
                {
                    try
                    {
                        string[] args = msg.Message.Content.Split(" ");
                        DiscordMember member = await e.CommandsNext.ConvertArgument(args[0], e, typeof(DiscordMember)) as DiscordMember;
                        UserObject.RootObject userData = RPClass.Users.Find(x => x.UserData.UserID == member.Id);
                        userData.Xp += int.Parse(args[1]);
                        if (userData.Xp < 0) userData.Xp = 0;
                        await e.RespondAsync("Stat changed. \nSend another, by typing `-<mention> <xp amount>`.\nTo end this process, type `stop`.");
                    }
                    catch
                    {
                        await e.RespondAsync("No user found, or xp was in invalid format.");
                    }
                    goto AnotherMessage;
                }
            }
            else
            {
                await UpdateStats(RPClass.StatsChannel);
                RPClass.SaveData(1);
                await UpdatePlayerRanking(e.Guild, 1);
                await UpdatePlayerRanking(e.Guild, 2);
                await UpdatePlayerRanking(e.Guild, 3);
                await UpdatePlayerRanking(e.Guild, 4);

                await e.RespondAsync("Stats updated.");
            }
        }

        // Non-Command Methods
        public static async Task UpdateStats(DiscordChannel c)
        {
            int longestName = 1;
            if (RPClass.Users.Any()) longestName = RPClass.Users.Where(x => x.Xp > 0).Max(x => x.UserData.Username.Length) + 1;
            int longestXP = 5;

            string Name = "Name".PadRight(longestName) + "| ";
            string XP = "XP ";

            string value = "```" + Name + XP + "\n---------" + new string('-', longestName) + "\n";

            List<UserObject.RootObject> SortedUsers = new List<UserObject.RootObject>();

            SortedUsers = RPClass.Users.OrderByDescending(x => x.Xp).ToList();
            try
            {
                List<DiscordMessage> msgs = new List<DiscordMessage>(await c.GetMessagesAsync(100));
                foreach (DiscordMessage msg in msgs)
                {
                    await msg.DeleteAsync();
                    await Task.Delay(500);
                }
            }
            catch { }
            foreach (UserObject.RootObject user in SortedUsers)
            {
                if (user.Xp > 0)
                {
                    if (value.Length > 1500)
                    {
                        await c.SendMessageAsync(value + "```");
                        value = "```";
                    }

                    value += user.UserData.Username.PadRight(longestName) + "| " + user.Xp.ToString().PadRight(longestXP) + "\n";
                }
            }
            await c.SendMessageAsync(value + "```");
        }

        public static async Task UpdatePlayerRanking(DiscordGuild e, int type)
		{
			DiscordChannel RankingChannel = RPClass.PlayerRankingChannel;

            if (type == 2) RankingChannel = RPClass.VillainRankingChannel;
            else if (type == 3) RankingChannel = RPClass.RogueRankingChannel;
            else if (type == 4) RankingChannel = RPClass.AcademyRankingChannel;

            int longestName = 0;
            var l = RPClass.Users;
            if (type == 1) longestName = RPClass.Users.Where(x => x.UserData.Role == 1).Max(x => x.UserData.Username.Length) + 1;
            else if (type == 2) longestName = RPClass.Users.Where(x => x.UserData.Role == 2).Max(x => x.UserData.Username.Length) + 1;
            else if (type == 3) longestName = RPClass.Users.Where(x => x.UserData.Role == 3).Max(x => x.UserData.Username.Length) + 1;
            else if (type == 4) longestName = RPClass.Users.Where(x => x.UserData.Role == 4).Max(x => x.UserData.Username.Length) + 1;

            int longestCount = 5;
			int longestStatus = 7;
			int longestStats = 7;
            if (type == 3 || type == 4) longestStats = 13;
            int longestGuild = 6;
            if (RPClass.Guilds.Any()) longestGuild = RPClass.Guilds.Max(x => x.Name.Length) + 1;

            string Count = "Rank".PadRight(longestCount) + "| ";
			string Name = "Name".PadRight(longestName) + "| ";
			string Status = "Status".PadRight(longestStatus) + "| ";
			string Stats = "Cases".PadRight(longestStats) + "| ";
            if (type == 2) Stats = "Crimes".PadRight(longestStats) + "| ";
            else if (type == 3) Stats = "Cases/Crimes".PadRight(longestStats) + "| ";
			string Guild = "Guild".PadRight(longestGuild) + "| ";
			string Rank = "Rank";
			string value = "";
			value += "```" + Count + Name + Status + Stats + Guild + Rank + "\n----------------------------------------------------------------\n";
			List<UserObject.RootObject> SortedUsers = new List<UserObject.RootObject>();

			if (type == 1) SortedUsers = RPClass.Users.Where(x => x.UserData.Role == 1).OrderByDescending(x => (x.Xp)).ToList();
			else if (type == 2) SortedUsers = RPClass.Users.Where(x => x.UserData.Role == 2).OrderByDescending(x => (x.Xp)).ToList();
            else if (type == 3) SortedUsers = RPClass.Users.Where(x => x.UserData.Role == 3).OrderByDescending(x => (x.Xp)).ToList();
            else if (type == 4) SortedUsers = RPClass.Users.Where(x => x.UserData.Role == 4).OrderByDescending(x => (x.Xp)).ToList();

            try
            {
                List<DiscordMessage> msgs = new List<DiscordMessage>(await RankingChannel.GetMessagesAsync(100));
                foreach (DiscordMessage msg in msgs)
                {
                    await msg.DeleteAsync();
                    await Task.Delay(500);
                }
            }
            catch { }
            int countNum = 1;
			foreach (UserObject.RootObject user in SortedUsers)
			{
                string UserStats = "";
                if (type == 1) UserStats += user.UserData.ResolvedCases;
                else if (type == 2) UserStats += user.UserData.CrimesCommitted;
                else if (type == 3 || type == 4) UserStats += user.UserData.ResolvedCases + user.UserData.CrimesCommitted;

                int rank = user.Xp;
                string UserRank = "S1";
                if (rank <= 30500) UserRank = "S2";
                if (rank <= 25500) UserRank = "S3";
                if (rank <= 20500) UserRank = "A1";
                if (rank <= 17500) UserRank = "A2";
                if (rank <= 15000) UserRank = "A3";
                if (rank <= 12500) UserRank = "B1";
                if (rank <= 10500) UserRank = "B2";
                if (rank <= 8500) UserRank = "B3";
                if (rank <= 6500) UserRank = "C1";
                if (rank <= 5000) UserRank = "C2";
                if (rank <= 3750) UserRank = "C3";
                if (rank <= 2500) UserRank = "D1";
                if (rank <= 2000) UserRank = "D2";
                if (rank <= 1250) UserRank = "D3";

                string UserGuild = "";
				if (user.UserData.GuildID == 0) UserGuild += "N/A";
				else UserGuild += RPClass.Guilds.First(x => x.Id == user.UserData.GuildID && x.UserIDs.Contains(user.UserData.UserID)).Name;

				if (value.Length > 1500)
				{
					await RankingChannel.SendMessageAsync(value + "```");
					value = "```";
				}
                value += (countNum.ToString().PadRight(longestCount) + "| " +  user.UserData.Username.PadRight(longestName) + "| " + (user.UserData.Status == 1 ? "Active" : user.UserData.Status == 2 ? "Retired" : user.UserData.Status == 3 ? "Deceased" : "Error").PadRight(longestStatus) + "| " + UserStats.PadRight(longestStats) + "| " + UserGuild.PadRight(longestGuild) + "| " + UserRank + "\n");
                countNum += 1;
            }
            await RankingChannel.SendMessageAsync(value + "```");
		}

        public static async Task UpdateGuildRanking(DiscordGuild e)
        {
            DiscordChannel RankingChannel = RPClass.GuildRankingChannel;
            int longestCount = 5;
            int longestName = 10;
            if (RPClass.Guilds.Any()) longestName = RPClass.Guilds.Max(x => x.Name.Length) + 1;
            int longestStatus = 10;
            int longestStats = 13;

            string Count = "Rank".PadRight(longestCount) + "| ";
            string Name = "Name".PadRight(longestName) + "| ";
            string Status = "Status".PadRight(longestStatus) + "| ";
            string  Stats = "Cases/Crimes".PadRight(longestStats) + "| ";
            string Rank = "Rank";
            string value = "```" + Count + Name + Status + Stats  + Rank + "\n--------------------------------------------------------------------\n";

            List<GuildObject.RootObject> GuildsNew = new List<GuildObject.RootObject>();
            foreach (GuildObject.RootObject guild in RPClass.Guilds)
            {
                int stats = 0;
                int xp = 0;
                UserObject.RootObject user;
                if (guild.UserIDs.Count > 0)
                {
                    foreach (ulong num in guild.UserIDs)
                    {
                        user = RPClass.Users.FirstOrDefault(x => x.UserData.UserID == num);
                        if (user != null)
                        {
                            xp += user.Xp;
                            stats += user.UserData.ResolvedCases + user.UserData.CrimesCommitted;
                        }
                    }
                    xp = (xp / guild.UserIDs.Count);
                    GuildsNew.Add(new GuildObject.RootObject(0, guild.Name, guild.Status, new List<ulong>() { (ulong)stats, (ulong)xp }));

                }
            }
            List<GuildObject.RootObject> SortedGuilds = GuildsNew.OrderByDescending(x => x.UserIDs[0]).ToList();
            try
            {
                List<DiscordMessage> msgs = new List<DiscordMessage>(await RankingChannel.GetMessagesAsync(100));
                foreach (DiscordMessage msg in msgs)
                {
                    await msg.DeleteAsync();
                    await Task.Delay(500);
                }
            }
            catch { }
            int countNum = 1;
            foreach (GuildObject.RootObject guild in SortedGuilds)
            {
                ulong rank = guild.UserIDs[1];
                string GuildRank = "S1";
                if (rank <= 30500) GuildRank = "S2";
                if (rank <= 25500) GuildRank = "S3";
                if (rank <= 20500) GuildRank = "A1";
                if (rank <= 17500) GuildRank = "A2";
                if (rank <= 15000) GuildRank = "A3";
                if (rank <= 12500) GuildRank = "B1";
                if (rank <= 10500) GuildRank = "B2";
                if (rank <= 8500) GuildRank = "B3";
                if (rank <= 6500) GuildRank = "C1";
                if (rank <= 5000) GuildRank = "C2";
                if (rank <= 3750) GuildRank = "C3";
                if (rank <= 2500) GuildRank = "D1";
                if (rank <= 2000) GuildRank = "D2";
                if (rank <= 1250) GuildRank = "D3";

                if (value.Length > 1500)
                {
                    await RankingChannel.SendMessageAsync(value + "```");
                    value = "```";
                }

                value += (countNum.ToString().PadRight(longestCount) + "| " + guild.Name.PadRight(longestName) + "| " + (guild.Status == 1 ? "Active" : guild.Status == 2 ? "Inactive" : "Error").PadRight(longestStatus) + "| " + guild.UserIDs[0].ToString().PadRight(longestStats) + "| " + GuildRank + "\n");
                countNum += 1;

            }
            await RankingChannel.SendMessageAsync(value + "```");
        }
    }
}
