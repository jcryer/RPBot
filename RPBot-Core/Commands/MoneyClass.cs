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
    [Group("money"), IsMuted,]
    [Description("All Money Commands")]
    class MoneyClass : BaseCommandModule
    {
        [Command("give"), Description("Command for admins to give out currency to users."), IsStaff]
        public async Task Give(CommandContext e, [Description("Who to award the money to")] DiscordMember user, [Description("Amount of money to award")] int money = -1)
        {
            if (money > 0)
            {
                RPClass.Users.Find(x => x.UserData.UserID == user.Id).UserData.Money += money;
                UserObject.RootObject a = RPClass.Users.Find(x => x.UserData.UserID == user.Id);
                await e.RespondAsync("User: " + a.UserData.Username + " now has $" + a.UserData.Money);
                RPClass.SaveData(1);
            }
        }

        [Command("take"), Description("Command for admins to take currency from users."), IsStaff]
        public async Task Take(CommandContext e, [Description("Who to take the money from")] DiscordMember user, [Description("Amount of money to take")] int money = -1)
        {
            if (money > 0)
            {
                UserObject.RootObject userData = RPClass.Users.Find(x => x.UserData.UserID == user.Id);
                userData.UserData.Money -= money;
                if (userData.UserData.Money < 0) userData.UserData.Money = 0;
                await e.RespondAsync("User: " + userData.UserData.Username + " now has $" + userData.UserData.Money);
                RPClass.SaveData(1);
            }
        }
        [Command("transfer"), Description("Command for users to transfer money to each other.")]
        public async Task Transfer(CommandContext e, [Description("Who to send the money to")] DiscordMember user, [Description("Amount of money to award")] int money = -1)
        {

            if (money > 0)
            {
                if (RPClass.Users.Find(x => x.UserData.UserID == e.Message.Author.Id).UserData.Money >= money)
                {
                    RPClass.Users.Find(x => x.UserData.UserID == user.Id).UserData.Money += money;
                    RPClass.Users.Find(x => x.UserData.UserID == e.Message.Author.Id).UserData.Money -= money;
                    UserObject.RootObject a = RPClass.Users.Find(x => x.UserData.UserID == user.Id);
                    UserObject.RootObject b = RPClass.Users.Find(x => x.UserData.UserID == e.Message.Author.Id);

                    await e.RespondAsync("User: " + a.UserData.Username + " now has $" + a.UserData.Money);
                    await e.RespondAsync("User: " + b.UserData.Username + " now has $" + b.UserData.Money);

                    RPClass.SaveData(1);
                }
                else
                {
                    await e.RespondAsync("You don't have enough money to do that.");
                }
            }
        }
        [Command("balance"), Aliases("bal"), Description("Prints the user's current balance.")]
        public async Task Balance(CommandContext e, [Description("Use all keyword to see everyone's balance (Admin only), or @mention someone to view their balance")] string all = "")
        {
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder()
            {
                Color = new DiscordColor("4169E1"),
                Timestamp = DateTime.UtcNow
            }
            .WithFooter("Mournstead");

            if (all == "all" && e.Member.Roles.Any(x => x == RPClass.AdminRole))
            {
                foreach (UserObject.RootObject userData in RPClass.Users)
                {
                    if (embed.Fields.Count < 25)
                    {
                        embed.AddField(userData.UserData.Username, "Money: $" + userData.UserData.Money);
                    }
                    else
                    {
                        await e.RespondAsync("", embed: embed);
                        await Task.Delay(500);
                        embed.ClearFields();
                    }
                }
            }
            else if (!string.IsNullOrWhiteSpace(all))
            {
                all = all.Replace("<", "").Replace(">", "").Replace("@", "").Replace("!", "");
                if (ulong.TryParse(all, out ulong userNum))
                {
                    embed.AddField(RPClass.Users.First(x => x.UserData.UserID == userNum).UserData.Username, "Money: $" + RPClass.Users.First(x => x.UserData.UserID == userNum).UserData.Money);
                }
                else
                {
                    await e.RespondAsync("Mention a user to select them.");
                }
            }
            else
            {
                UserObject.RootObject userData = RPClass.Users.Find(x => x.UserData.UserID == e.Member.Id);

                embed.AddField(userData.UserData.Username, "Money: $" + userData.UserData.Money);
            }
            await e.RespondAsync("", embed: embed);

        }

    }

    [Group("ss"), IsMuted]
    [Description("All Soul Shard Commands")]
    class BloodClass : BaseCommandModule
    {
        [Command("give"), Description("Command for admins to give out soul shards to users."), IsStaff]
        public async Task Give(CommandContext e, [Description("Who to award the soul shards to")] DiscordMember user, [Description("Amount of soul shards to award")] int soulShards = -1)
        {
            if (soulShards > 0)
            {
                RPClass.Users.Find(x => x.UserData.UserID == user.Id).UserData.SoulShards += soulShards;
                UserObject.RootObject a = RPClass.Users.Find(x => x.UserData.UserID == user.Id);
                await e.RespondAsync("User: " + a.UserData.Username + " now has " + a.UserData.SoulShards + " soul shards.");
                RPClass.SaveData(1);
            }
        }

        [Command("take"), Description("Command for admins to take soul shards from users."), IsStaff]
        public async Task Take(CommandContext e, [Description("Who to take the soul shards from")] DiscordMember user, [Description("Amount of soul shards to take")] int soulShards = -1)
        {
            if (soulShards > 0)
            {
                UserObject.RootObject userData = RPClass.Users.Find(x => x.UserData.UserID == user.Id);
                userData.UserData.SoulShards -= soulShards;
                if (userData.UserData.SoulShards < 0) userData.UserData.SoulShards = 0;
                await e.RespondAsync("User: " + userData.UserData.Username + " now has " + userData.UserData.SoulShards + " soul shards.");
                RPClass.SaveData(1);
            }
        }
        [Command("transfer"), Description("Command for users to transfer soul shards to each other.")]
        public async Task Transfer(CommandContext e, [Description("Who to send the soul shards to")] DiscordMember user, [Description("Amount of soul shards to award")] int soulShards = -1)
        {

            if (soulShards > 0)
            {
                if (RPClass.Users.Find(x => x.UserData.UserID == e.Message.Author.Id).UserData.SoulShards >= soulShards)
                {
                    RPClass.Users.Find(x => x.UserData.UserID == user.Id).UserData.SoulShards += soulShards;
                    RPClass.Users.Find(x => x.UserData.UserID == e.Message.Author.Id).UserData.SoulShards -= soulShards;
                    UserObject.RootObject a = RPClass.Users.Find(x => x.UserData.UserID == user.Id);
                    UserObject.RootObject b = RPClass.Users.Find(x => x.UserData.UserID == e.Message.Author.Id);

                    await e.RespondAsync("User: " + a.UserData.Username + "now has " + a.UserData.SoulShards + " soul shards.");
                    await e.RespondAsync("User: " + b.UserData.Username + " now has " + b.UserData.SoulShards + " soul shards.");

                    RPClass.SaveData(1);
                }
                else
                {
                    await e.RespondAsync("You don't have enough shards to do that.");
                }
            }
        }
        [Command("balance"), Aliases("bal"), Description("Prints the user's current soul shard total.")]
        public async Task Balance(CommandContext e, [Description("Use all keyword to see everyone's soul shard total (Admin only), or @mention someone to view their soul shards total.")] string all = "")
        {
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder()
            {
                Color = new DiscordColor("4169E1"),
                Timestamp = DateTime.UtcNow
            }
            .WithFooter("Mournstead");

            if (all == "all" && e.Member.Roles.Any(x => x == RPClass.AdminRole))
            {
                foreach (UserObject.RootObject userData in RPClass.Users)
                {
                    if (embed.Fields.Count < 25)
                    {
                        embed.AddField(userData.UserData.Username, "Soul Shards: " + userData.UserData.SoulShards);
                    }
                    else
                    {
                        await e.RespondAsync("", embed: embed);
                        await Task.Delay(500);
                        embed.ClearFields();
                    }
                }
            }
            else if (!string.IsNullOrWhiteSpace(all))
            {
                all = all.Replace("<", "").Replace(">", "").Replace("@", "").Replace("!", "");
                if (ulong.TryParse(all, out ulong userNum))
                {
                    embed.AddField(RPClass.Users.First(x => x.UserData.UserID == userNum).UserData.Username, "Soul Shards: " + RPClass.Users.First(x => x.UserData.UserID == userNum).UserData.SoulShards);
                }
                else
                {
                    await e.RespondAsync("Mention a user to select them.");
                }
            }
            else
            {
                UserObject.RootObject userData = RPClass.Users.Find(x => x.UserData.UserID == e.Member.Id);

                embed.AddField(userData.UserData.Username, "Soul Shards: " + userData.UserData.SoulShards);
            }
            await e.RespondAsync("", embed: embed);

        }
    }
}
