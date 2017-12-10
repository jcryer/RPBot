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
    class XPClass : RPClass
    {
        [Command("xp"), Description("Admin command to give XP"), RequireRolesAttribute("Staff")]
        public async Task XP(CommandContext e, [Description("User to change stats of")] DiscordMember user, [Description("How much you wish to change it by")] int xpNum)
        {
            if (xpNum != 0)
            {
                UserObject.RootObject userData = Users.Find(x => x.UserData.userID == user.Id);
                userData.xp += xpNum;
                if (userData.xp < 0) userData.xp = 0;

                await UpdateStats(StatsChannel);
                SaveData(1);
                await e.RespondAsync("Stat changed.");
                await UpdatePlayerRanking(e.Guild, 1);
                await UpdatePlayerRanking(e.Guild, 2);
            }
        }

        // Non-Command Methods
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
                await Task.Delay(500);
            }

            foreach (UserObject.RootObject user in SortedUsers)
            {
                if (user.xp > 0 && user.xp != 250)
                {
                    if (value.Length > 1500)
                    {
                        await c.SendMessageAsync(value + "```");
                        value = "```";
                    }

                    value += user.UserData.username.PadRight(longestName) + "| " + user.xp.ToString().PadRight(longestXP) + "\n";
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
            if (type == 1) longestName = Users.Where(x => x.UserData.role == 1).Max(x => x.UserData.username.Length);
            else if (type == 2) longestName = Users.Where(x => x.UserData.role == 2).Max(x => x.UserData.username.Length);
            else if (type == 3) longestName = Users.Where(x => x.UserData.role == 3).Max(x => x.UserData.username.Length);

			int longestStatus = 10;
			int longestStats = 17;
            int longestGuild = 10;
            if (Guilds.Any()) longestGuild = Guilds.Max(x => x.name.Length) + 1;

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

			if (type == 1) SortedUsers = Users.Where(x => x.UserData.role == 1).OrderByDescending(x => (x.xp)).ToList();
			else if (type == 2) SortedUsers = Users.Where(x => x.UserData.role == 2).OrderByDescending(x => (x.xp)).ToList();
            else if (type == 3) SortedUsers = Users.Where(x => x.UserData.role == 3).OrderByDescending(x => (x.xp)).ToList();

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
				if (user.UserData.guildID == 0) UserGuild += "N/A";
				else UserGuild += Guilds.First(x => x.id == user.UserData.guildID && x.userIDs.Contains(user.UserData.userID)).name;

				if (value.Length > 1500)
				{
					await RankingChannel.SendMessageAsync(value + "```");
					value = "```";
				}
                if (type != 3) value += (user.UserData.username.PadRight(longestName) + "| " + (user.UserData.status == 1 ? "Active" : user.UserData.status == 2 ? "Retired" : user.UserData.status == 3 ? "Deceased" : "Error").PadRight(longestStatus) + "| " + UserStats.PadRight(longestStats) + "| " + UserGuild.PadRight(longestGuild) + "| " + UserRank + "\n");
                else value += (user.UserData.username.PadRight(longestName) + "| " + (user.UserData.status == 1 ? "Active" : user.UserData.status == 2 ? "Retired" : user.UserData.status == 3 ? "Deceased" : "Error").PadRight(longestStatus) + "| " + UserGuild.PadRight(longestGuild) + "| " + UserRank + "\n");

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
            string value = "```" + Name + Status + Stats  + Rank + "\n-------------------------------------------------------------------------\n";

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
                ulong rank = guild.userIDs[0];
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

                value += (guild.name.PadRight(longestName) + "| " + (guild.status == 1 ? "Active" : guild.status == 2 ? "Inactive" : "Error").PadRight(longestStatus) + "| " + guild.userIDs[1].ToString().PadRight(longestStats) + "| " + UserRank + "\n");
            }
            await RankingChannel.SendMessageAsync(value + "```");
        }
    }
}
