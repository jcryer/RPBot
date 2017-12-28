using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RPBot
{
    [Group("xp", CanInvokeWithoutSubcommand = true), Description("Admin command to give XP"), RequireRoles(RoleCheckMode.Any, "Staff")]
    class XPClass : RPClass
    {
        public async Task ExecuteGroupAsync(CommandContext e, [Description("User to change stats of")] DiscordMember user, [Description("How much you wish to change it by")] int xpNum)
        {
            if (xpNum != 0)
            {
                UserObject.RootObject userData = Users.Find(x => x.UserData.UserID == user.Id);
                userData.Xp += xpNum;
                if (userData.Xp < 0) userData.Xp = 0;
                
                await UpdateStats(StatsChannel);
                SaveData(1);
                UserObject.RootObject newUserData = Users.Find(x => x.UserData.UserID == user.Id);
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
                    case 0:
                        await UpdatePlayerRanking(e.Guild, 1);
                        await UpdatePlayerRanking(e.Guild, 2);
                        await UpdatePlayerRanking(e.Guild, 3);
                        break;
                }
                await e.RespondAsync("Stat changed.");
            }
        }
        [Command("update"), Description("Updates saved data")]
        public async Task Update(CommandContext e)
        {
            await AddOrUpdateUsers(e.Guild, true);
            SaveData(-1);

        }

        // Non-Command Methods
        public static async Task UpdateStats(DiscordChannel c)
        {
            int longestName = 1;
            if (Users.Any()) longestName = Users.Max(x => x.UserData.Username.Length) + 1;
            int longestXP = 5;

            string Name = "Name".PadRight(longestName) + "|  ";
            string XP = "XP ";

            string value = "```" + Name + XP + "\n--------------" + new string('-', longestName) + "\n";

            List<UserObject.RootObject> SortedUsers = new List<UserObject.RootObject>();

            SortedUsers = Users.OrderByDescending(x => x.Xp).ToList();
            
            List<DiscordMessage> msgs = new List<DiscordMessage>(await c.GetMessagesBeforeAsync(await c.GetMessageAsync(c.LastMessageId), 5));
            foreach (DiscordMessage msg in msgs)
            {
                await msg.DeleteAsync();
                await Task.Delay(500);
            }
            foreach (UserObject.RootObject user in SortedUsers)
            {
                if (user.Xp > 0 && user.Xp != 250)
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

			int longestName = 0;
            if (type == 1) longestName = Users.Where(x => x.UserData.Role == 1).Max(x => x.UserData.Username.Length);
            else if (type == 2) longestName = Users.Where(x => x.UserData.Role == 2).Max(x => x.UserData.Username.Length);
            else if (type == 3) longestName = Users.Where(x => x.UserData.Role == 3).Max(x => x.UserData.Username.Length);

			int longestStatus = 10;
			int longestStats = 17;
            int longestGuild = 10;
            if (Guilds.Any()) longestGuild = Guilds.Max(x => x.Name.Length) + 1;

			string Name = "Name".PadRight(longestName) + "| ";
			string Status = "Status    | ";
			string Stats = "";
            if (type == 1) Stats = "Resolved Cases".PadRight(longestStats) + "| ";
            else if (type == 2) Stats = "Crimes Committed".PadRight(longestStats) + "| ";
            else if (type == 3) Stats = "";
			string Guild = "Guild".PadRight(longestGuild) + "| ";
			string Rank = "Rank";
			string value = "";
			value += "```" + Name + Status + Stats + Guild + Rank + "\n-------------------------------------------------------------------\n";
			List<UserObject.RootObject> SortedUsers = new List<UserObject.RootObject>();

			if (type == 1) SortedUsers = Users.Where(x => x.UserData.Role == 1).OrderByDescending(x => (x.Xp)).ToList();
			else if (type == 2) SortedUsers = Users.Where(x => x.UserData.Role == 2).OrderByDescending(x => (x.Xp)).ToList();
            else if (type == 3) SortedUsers = Users.Where(x => x.UserData.Role == 3).OrderByDescending(x => (x.Xp)).ToList();

            List<DiscordMessage> msgs = new List<DiscordMessage>(await RankingChannel.GetMessagesBeforeAsync(await RankingChannel.GetMessageAsync(RankingChannel.LastMessageId), 100));
			foreach (DiscordMessage msg in msgs)
			{
                await msg.DeleteAsync();
			}

			foreach (UserObject.RootObject user in SortedUsers)
			{
                string UserStats = "";
				if (type == 1) UserStats += user.UserData.ResolvedCases;
                else if (type == 2) UserStats += user.UserData.CrimesCommitted;


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
				else UserGuild += Guilds.First(x => x.Id == user.UserData.GuildID && x.UserIDs.Contains(user.UserData.UserID)).Name;

				if (value.Length > 1500)
				{
					await RankingChannel.SendMessageAsync(value + "```");
					value = "```";
				}
                if (type != 3) value += (user.UserData.Username.PadRight(longestName) + "| " + (user.UserData.Status == 1 ? "Active" : user.UserData.Status == 2 ? "Retired" : user.UserData.Status == 3 ? "Deceased" : "Error").PadRight(longestStatus) + "| " + UserStats.PadRight(longestStats) + "| " + UserGuild.PadRight(longestGuild) + "| " + UserRank + "\n");
                else value += (user.UserData.Username.PadRight(longestName) + "| " + (user.UserData.Status == 1 ? "Active" : user.UserData.Status == 2 ? "Retired" : user.UserData.Status == 3 ? "Deceased" : "Error").PadRight(longestStatus) + "| " + UserGuild.PadRight(longestGuild) + "| " + UserRank + "\n");

            }
            await RankingChannel.SendMessageAsync(value + "```");
		}

        public static async Task UpdateGuildRanking(DiscordGuild e)
        {
            DiscordChannel RankingChannel = RPClass.GuildRankingChannel;
            int longestName = 10;
            if (Guilds.Any()) longestName = Guilds.Max(x => x.Name.Length) + 1;
            int longestStatus = 10;
            int longestStats = 31;

            string Name = "Name".PadRight(longestName) + "| ";
            string Status = "Status    | ";
            string  Stats = "Resolved Cases/Crimes Committed".PadRight(longestStats) + "| ";
            string Rank = "Rank";
            string value = "```" + Name + Status + Stats  + Rank + "\n-------------------------------------------------------------------------\n";

            List<GuildObject.RootObject> GuildsNew = new List<GuildObject.RootObject>();
            foreach (GuildObject.RootObject guild in Guilds)
            {
                int stats = 0;
                UserObject.RootObject user;
                foreach (ulong num in guild.UserIDs)
                {
                    user = Users.First(x => x.UserData.UserID == num);
                    stats += user.UserData.ResolvedCases + user.UserData.CrimesCommitted;
                }
                int totalStats = stats;
                if (guild.UserIDs.Count > 0) stats = (stats / guild.UserIDs.Count);
                GuildsNew.Add(new GuildObject.RootObject(0, guild.Name, guild.Status, new List<ulong>() { ulong.Parse(stats.ToString()), ulong.Parse(totalStats.ToString()) }));
            }
            List<GuildObject.RootObject> SortedGuilds = GuildsNew.OrderByDescending(x => x.UserIDs[0]).ToList();
            List<DiscordMessage> msgs = new List<DiscordMessage>(await RankingChannel.GetMessagesBeforeAsync(await RankingChannel.GetMessageAsync(RankingChannel.LastMessageId), 100));
            foreach (DiscordMessage msg in msgs)
            {
                await msg.DeleteAsync();
            }

            foreach (GuildObject.RootObject guild in SortedGuilds)
            {
                ulong rank = guild.UserIDs[0];
                string UserRank = "S1";
                if (rank <= 30000) UserRank = "S2";
                if (rank <= 25000) UserRank = "S3";
                if (rank <= 20000) UserRank = "A1";
                if (rank <= 17000) UserRank = "A2";
                if (rank <= 14500) UserRank = "A3";
                if (rank <= 12000) UserRank = "B1";
                if (rank <= 10000) UserRank = "B2";
                if (rank <= 8000) UserRank = "B3";
                if (rank <= 6000) UserRank = "C1";
                if (rank <= 4500) UserRank = "C2";
                if (rank <= 3250) UserRank = "C3";
                if (rank <= 2000) UserRank = "D1";
                if (rank <= 1500) UserRank = "D2";
                if (rank <= 750) UserRank = "D3";
                

                if (value.Length > 1500)
                {
                    await RankingChannel.SendMessageAsync(value + "```");
                    value = "```";
                }

                value += (guild.Name.PadRight(longestName) + "| " + (guild.Status == 1 ? "Active" : guild.Status == 2 ? "Inactive" : "Error").PadRight(longestStatus) + "| " + guild.UserIDs[1].ToString().PadRight(longestStats) + "| " + UserRank + "\n");
            }
            await RankingChannel.SendMessageAsync(value + "```");
        }
    }
}
