﻿using DSharpPlus;
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

        public static List<string> CreateRankingTable(List<RankingRow> tableData)
        {
            List<string> tableStrings = new List<string>();
            int positionMax = 4;
            int nameMax = tableData.Select(x => x.Name.Length).Max() + 2;
            int fameMax = 6;
            int infamyMax = 8;
            int guildMax = tableData.Select(x => x.Guild.Length).Max() + 1;
            int rankMax = 6;

            if (nameMax < 6) nameMax = 6;
            if (guildMax < 7) guildMax = 7;


            string table = $"╔{new string('═', positionMax)}╤{new string('═', nameMax)}╤{new string('═', fameMax)}╤{new string('═', infamyMax)}╤{new string('═', guildMax)}╤{new string('═', rankMax)}╗\n";
            table += $"║{"Pos".PadRight(positionMax)}│{" Name".PadRight(nameMax)}│{" Fame".PadRight(fameMax)}│{" Infamy".PadRight(infamyMax)}│{" Guild".PadRight(guildMax)}│{" Rank".PadRight(rankMax)}║\n";
            table += $"╠{new string('═', positionMax)}╪{new string('═', nameMax)}╪{new string('═', fameMax)}╪{new string('═', infamyMax)}╪{new string('═', guildMax)}╪{new string('═', rankMax)}╣\n";

            foreach (var row in tableData)
            {
                table += $"║{row.Position.PadRight(positionMax)}│ {row.Name.PadRight(nameMax-1)}│ {row.Fame.PadRight(fameMax-1)}│ {row.Infamy.PadRight(infamyMax-1)}│ {row.Guild.PadRight(guildMax-1)}│ {row.Rank.PadRight(rankMax-1)}║\n";
                if (table.Length > 1500)
                {
                    table += $"╚{new string('═', positionMax)}╧{new string('═', nameMax)}╧{new string('═', fameMax)}╧{new string('═', infamyMax)}╧{new string('═', guildMax)}╧{new string('═', rankMax)}╝";
                    tableStrings.Add(table);
                    table = "";
                    if (row != tableData.Last())
                    {
                        table = $"╔{new string('═', positionMax)}╤{new string('═', nameMax)}╤{new string('═', fameMax)}╤{new string('═', infamyMax)}╤{new string('═', guildMax)}╤{new string('═', rankMax)}╗\n";
                    }
                }
            }
            if (table != "")
            {
                table += $"╚{new string('═', positionMax)}╧{new string('═', nameMax)}╧{new string('═', fameMax)}╧{new string('═', infamyMax)}╧{new string('═', guildMax)}╧{new string('═', rankMax)}╝";
                tableStrings.Add(table);
            }
            return tableStrings;
        }

        public class RankingRow
        {
            public string Position;
            public string Name;
            public string Fame;
            public string Infamy;
            public string Guild;
            public string Rank;

            public RankingRow(string position, string name, string fame, string infamy, string guild, string rank)
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
            if (!RPClass.Users.Where(x => x.Xp > 0).Any())
            {
                try
                {
                    await c.DeleteMessagesAsync(await c.GetMessagesAsync(100));
                }
                catch {
                    var msgs = await c.GetMessagesAsync(100);

                    foreach (var msg in msgs)
                    {
                        await msg.DeleteAsync();
                    }
                }
                return;
            }
            try
            {
                await c.DeleteMessagesAsync(await c.GetMessagesAsync(100));
            }
            catch {
                var msgs = await c.GetMessagesAsync(100);

                foreach (var msg in msgs)
                {
                    await msg.DeleteAsync();
                }
            }

            int longestName = 1;
            if (RPClass.Users.Any()) longestName = RPClass.Users.Where(x => x.Xp > 0).Max(x => x.UserData.Username.Length) + 2;
            int longestXP = 7;
            var tables = new List<string>();
            string table = $"╔{new string('═', longestName)}╤{new string('═', longestXP)}╗\n";
            table += $"║{" Name".PadRight(longestName)}│{" XP".PadRight(longestXP)}║\n";
            table += $"╠{new string('═', longestName)}╪{new string('═', longestXP)}╣\n";

            List<UserObject.RootObject> SortedUsers = new List<UserObject.RootObject>();

            SortedUsers = RPClass.Users.OrderByDescending(x => x.Xp).ToList();

            foreach (UserObject.RootObject user in SortedUsers)
            {
                if (user.Xp > 0)
                {
                    table += $"║ {user.UserData.Username.PadRight(longestName-1)}│ {user.Xp.ToString().PadRight(longestXP-1)}║\n";

                    if (table.Length > 1500)
                    {
                        table += $"╚{new string('═', longestName)}╧{new string('═', longestXP)}╝\n";
                        tables.Add(table);
                        table = "";
                        if (user != SortedUsers.Last())
                        {
                            table += $"╔{new string('═', longestName)}╤{new string('═', longestXP)}╗\n";
                        }
                    }
                }
            }

            if (table != "")
            {
                table += $"╚{new string('═', longestName)}╧{new string('═', longestXP)}╝\n";
                tables.Add(table);
            }
            foreach (var tableString in tables)
                await c.SendMessageAsync("```" + tableString + "```");
        }

        public static async Task UpdatePlayerRanking(DiscordGuild e, int type)
		{

			DiscordChannel RankingChannel = RPClass.HeroRankingChannel;

            if (type == 2) RankingChannel = RPClass.VillainRankingChannel;
            else if (type == 3) RankingChannel = RPClass.RogueRankingChannel;
            else if (type == 4) RankingChannel = RPClass.AcademyRankingChannel;

            if (!RPClass.Users.Where(x => x.UserData.Role == type && x.Xp > 0).Any())
            {
                try
                {
                    await RankingChannel.DeleteMessagesAsync(await RankingChannel.GetMessagesAsync(100));
                }
                catch {

                    var msgs = await RankingChannel.GetMessagesAsync(100);

                    foreach (var msg in msgs)
                    {
                        await msg.DeleteAsync();
                    }
                }
                return;
            }
            try
            {
                await RankingChannel.DeleteMessagesAsync(await RankingChannel.GetMessagesAsync(100));
            }
            catch {

                var msgs = await RankingChannel.GetMessagesAsync(100);

                foreach (var msg in msgs)
                {
                    await msg.DeleteAsync();
                }
            }
            List<UserObject.RootObject> SortedUsers = new List<UserObject.RootObject>();

			SortedUsers = RPClass.Users.Where(x => x.UserData.Role == type).OrderByDescending(x => (x.Xp)).ToList();
            int countNum = 1;
            var tableData = new List<RankingRow>();
			foreach (UserObject.RootObject user in SortedUsers)
			{
                string userGuild = "";
                if (user.UserData.GuildID == 0) userGuild += "N/A";
                else userGuild += RPClass.Guilds.First(x => x.Id == user.UserData.GuildID && x.UserIDs.Contains(user.UserData.UserID)).Name;

                tableData.Add(new RankingRow(countNum.ToString(), user.UserData.Username, user.UserData.Fame.ToString(), user.UserData.Infamy.ToString(), userGuild, user.GetRank()));

				countNum += 1;
            }
            foreach (var table in CreateRankingTable(tableData))
                await RankingChannel.SendMessageAsync("```" + table + "```");
		}

        public static async Task UpdateGuildRanking(DiscordGuild e)
        {

            DiscordChannel RankingChannel = RPClass.GuildRankingChannel;

            if (!RPClass.Guilds.Any())
            {
                try
                {
                    await RankingChannel.DeleteMessagesAsync(await RankingChannel.GetMessagesAsync(100));
                }
                catch {

                    var msgs = await RankingChannel.GetMessagesAsync(100);

                    foreach (var msg in msgs)
                    {
                        await msg.DeleteAsync();
                    }
                }
                return;
            }
            try
            {
                await RankingChannel.DeleteMessagesAsync(await RankingChannel.GetMessagesAsync(100));
            }
            catch {

                var msgs = await RankingChannel.GetMessagesAsync(100);

                foreach (var msg in msgs)
                {
                    await msg.DeleteAsync();
                }
            }
            List<string> tableStrings = new List<string>();
            int positionMax = 4;
            int nameMax = RPClass.Guilds.Max(x => x.Name.Length) + 2;
            int fameMax = 6;
            int infamyMax = 8;
            int rankMax = 6;

            if (nameMax < 6) nameMax = 6;

            string table = $"╔{new string('═', positionMax)}╤{new string('═', nameMax)}╤{new string('═', fameMax)}╤{new string('═', infamyMax)}╤{new string('═', rankMax)}╗\n";
            table += $"║{"Pos".PadRight(positionMax)}│{" Name".PadRight(nameMax)}│{" Fame".PadRight(fameMax)}│{" Infamy".PadRight(infamyMax)}│{" Rank".PadRight(rankMax)}║\n";
            table += $"╠{new string('═', positionMax)}╪{new string('═', nameMax)}╪{new string('═', fameMax)}╪{new string('═', infamyMax)}╪{new string('═', rankMax)}╣\n";


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
                    GuildsNew.Add(new GuildObject.RootObject(0, guild.Name, new List<ulong>() { (ulong)xp, (ulong)fame, (ulong)infamy })); // xp, fame, infamy
                }
            }
            List<GuildObject.RootObject> SortedGuilds = GuildsNew.OrderByDescending(x => x.UserIDs[0]).ToList();

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

                table += $"║{countNum.ToString().PadRight(positionMax)}│ {guild.Name.PadRight(nameMax - 1)}│ {guild.UserIDs[1].ToString().PadRight(fameMax - 1)}│ {guild.UserIDs[2].ToString().PadRight(infamyMax - 1)}│ {GuildRank.PadRight(rankMax - 1)}║\n";


                if (table.Length > 1500)
                {
                    table += $"╚{new string('═', positionMax)}╧{new string('═', nameMax)}╧{new string('═', fameMax)}╧{new string('═', infamyMax)}╧{new string('═', rankMax)}╝";
                    tableStrings.Add(table);
                    table = "";
                    if (guild != SortedGuilds.Last())
                    {
                        table = $"╔{new string('═', positionMax)}╤{new string('═', nameMax)}╤{new string('═', fameMax)}╤{new string('═', infamyMax)}╤{new string('═', rankMax)}╗\n";
                    }
                }

                countNum += 1;

            }

            if (table != "")
            {
                table += $"╚{new string('═', positionMax)}╧{new string('═', nameMax)}╧{new string('═', fameMax)}╧{new string('═', infamyMax)}╧{new string('═', rankMax)}╝";
                tableStrings.Add(table);
            }
            foreach (var tableString in tableStrings)
                await RankingChannel.SendMessageAsync("```" + tableString + "```");
        }
    }
}
