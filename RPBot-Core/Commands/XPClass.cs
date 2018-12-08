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
    [Group("xp", CanInvokeWithoutSubcommand = true), Description("Admin command to give XP"), RequireRoles(RoleCheckMode.Any, "Staff"), IsMuted]
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

        public static string CreateTable(List<Table> tableData)
        {
            int positionMax = tableData.Select(x => x.Position.Length).Max();
            int nameMax = tableData.Select(x => x.Name.Length).Max();
            int fameMax = tableData.Select(x => x.Fame.Length).Max();
            int infamyMax = tableData.Select(x => x.Infamy.Length).Max();
            int guildMax = tableData.Select(x => x.Guild.Length).Max();
            int rankMax = tableData.Select(x => x.Rank.Length).Max();

            if (positionMax < 4) positionMax = 4;
            if (nameMax < 6) nameMax = 6;
            if (fameMax < 6) fameMax = 6;
            if (infamyMax < 8) infamyMax = 8;
            if (guildMax < 7) guildMax = 7;
            if (rankMax < 6) rankMax = 6;


            string table = $"╔{new string('═', positionMax)}╤{new string('═', nameMax)}╤{new string('═', fameMax)}╤{new string('═', infamyMax)}╤{new string('═', guildMax)}╤{new string('═', rankMax)}╗";
            table += $"║{"Pos".PadRight(positionMax)}│ {"Name".PadRight(nameMax)}│ {"Fame".PadRight(fameMax)}│ {"Infamy".PadRight(infamyMax)}│ {"Guild".PadRight(guildMax)}│ {"Rank".PadRight(rankMax)}║";
            table += $"╠{new string('═', positionMax)}╪{new string('═', nameMax)}╪{new string('═', fameMax)}╪{new string('═', infamyMax)}╪{new string('═', guildMax)}╪{new string('═', rankMax)}╣";

            foreach (var row in tableData)
            {
                table += $"║{row.Position.PadRight(positionMax)}│ {row.Name.PadRight(nameMax)}│ {row.Fame.PadRight(fameMax)}│ {row.Infamy.PadRight(infamyMax)}│ {row.Guild.PadRight(guildMax)}│ {row.Rank.PadRight(rankMax)}║";
            }
            table += $"╚{new string('═', positionMax)}╧{new string('═', nameMax)}╧{new string('═', fameMax)}╧{new string('═', infamyMax)}╧{new string('═', guildMax)}╧{new string('═', rankMax)}╝";

            return table;
        }

        public class Table
        {
            public string Position;
            public string Name;
            public string Fame;
            public string Infamy;
            public string Guild;
            public string Rank;

            public Table(string position, string name, string fame, string infamy, string guild, string rank)
            {
                Position = position;
                Name = name;
                Fame = fame;
                Infamy = infamy;
                Guild = guild;
                Rank = rank;
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
			DiscordChannel RankingChannel = RPClass.HeroRankingChannel;

            if (type == 2) RankingChannel = RPClass.VillainRankingChannel;
            else if (type == 3) RankingChannel = RPClass.RogueRankingChannel;
            else if (type == 4) RankingChannel = RPClass.AcademyRankingChannel;

			List<UserObject.RootObject> SortedUsers = new List<UserObject.RootObject>();

			SortedUsers = RPClass.Users.Where(x => x.UserData.Role == type).OrderByDescending(x => (x.Xp)).ToList();
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
            var tableData = new List<Table>();
			foreach (UserObject.RootObject user in SortedUsers)
			{
                string userGuild = "";
                if (user.UserData.GuildID == 0) userGuild += "N/A";
                else userGuild += RPClass.Guilds.First(x => x.Id == user.UserData.GuildID && x.UserIDs.Contains(user.UserData.UserID)).Name;

                tableData.Add(new Table(countNum.ToString(), user.UserData.Username, user.UserData.Fame.ToString(), user.UserData.Infamy.ToString(), userGuild, user.GetRank()));

				countNum += 1;
            }
            await RankingChannel.SendMessageAsync("```" + CreateTable(tableData) + "```");
		}

        public static async Task UpdateGuildRanking(DiscordGuild e)
        {
            DiscordChannel RankingChannel = RPClass.GuildRankingChannel;
            int longestCount = 5;
            int longestName = 10;
            if (RPClass.Guilds.Any()) longestName = RPClass.Guilds.Max(x => x.Name.Length) + 1;

            string Count = "Pos".PadRight(longestCount) + "| ";
            string Name = "Name".PadRight(longestName) + "| ";
            string Fame = "Fame | ";
            string Infamy = "Infamy | ";
            string Rank = "Rank";
            string value = $"```{Count}{Name}{Fame}{Infamy}{Rank}\n{new string('-', $"{Count}{Name}{Fame}{Infamy}{Rank}".Length)}\n";

            List<GuildObject.RootObject> GuildsNew = new List<GuildObject.RootObject>();
            foreach (GuildObject.RootObject guild in RPClass.Guilds)
            {
                int xp = 0;
                int fame = 0;
                int infamy = 0;
                UserObject.RootObject user;
                if (guild.UserIDs.Count > 0)
                {
                    foreach (ulong num in guild.UserIDs)
                    {
                        user = RPClass.Users.FirstOrDefault(x => x.UserData.UserID == num);
                        if (user != null)
                        {
                            xp += user.Xp;
                            fame += user.UserData.Fame;
                            infamy += user.UserData.Infamy;
                        }
                    }
                    xp = (xp / guild.UserIDs.Count);
                    GuildsNew.Add(new GuildObject.RootObject(0, guild.Name, new List<ulong>() { (ulong)xp, (ulong)fame, (ulong)infamy }));

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
                ulong rank = guild.UserIDs[0];
                RPClass.Users.Where(x => x.UserData.GuildID == guild.Id);
                string GuildRank = "S1";
                if (rank < 10000) GuildRank = "S2";
                if (rank < 9000) GuildRank = "S3";
                if (rank < 8000) GuildRank = "A1";
                if (rank < 7000) GuildRank = "A2";
                if (rank < 6000) GuildRank = "A3";
                if (rank < 5500) GuildRank = "B1";
                if (rank < 5000) GuildRank = "B2";
                if (rank < 4500) GuildRank = "B3";
                if (rank < 4000) GuildRank = "C1";
                if (rank < 3000) GuildRank = "C2";
                if (rank < 2500) GuildRank = "C3";
                if (rank < 2000) GuildRank = "D1";
                if (rank < 1000) GuildRank = "D2";
                if (rank < 500) GuildRank = "D3";

                if (value.Length > 1500)
                {
                    await RankingChannel.SendMessageAsync(value + "```");
                    value = "```";
                }

                value += (countNum.ToString().PadRight(longestCount) + "| " + guild.Name.PadRight(longestName) + "| " + guild.UserIDs[1].ToString().PadRight(5) + "| " + guild.UserIDs[2].ToString().PadRight(7) + "| " +  GuildRank + "\n");
                countNum += 1;

            }
            await RankingChannel.SendMessageAsync(value + "```");
        }
    }
}
