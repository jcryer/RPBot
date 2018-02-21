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
    [Group("money"), IsMuted]
    [Description("All Money Commands")]
    class MoneyClass : BaseCommandModule
    {
        [Command("give"), Description("Command for admins to give out currency to users."), RequireRoles(RoleCheckMode.Any, "Staff")]
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

        [Command("take"), Description("Command for admins to take currency from users."), RequireRoles(RoleCheckMode.Any, "Staff")]
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
            .WithFooter("Heroes & Villains");

            if (all == "all" && e.Member.Roles.Any(x => x == RPClass.StaffRole))
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

    [Group("blood"), IsMuted]
    [Description("All Blood Point Commands")]
    class BloodClass : BaseCommandModule
    {
        [Command("give"), Description("Command for admins to give out blood points to users."), RequireRoles(RoleCheckMode.Any, "Staff")]
        public async Task Give(CommandContext e, [Description("Who to award the blood points to")] DiscordMember user, [Description("Amount of blood points to award")] int bloodPoints = -1)
        {
            if (bloodPoints > 0)
            {
                RPClass.Users.Find(x => x.UserData.UserID == user.Id).UserData.BloodPoints += bloodPoints;
                UserObject.RootObject a = RPClass.Users.Find(x => x.UserData.UserID == user.Id);
                await e.RespondAsync("User: " + a.UserData.Username + " now has " + a.UserData.BloodPoints + " blood points.");
                RPClass.SaveData(1);
            }
        }

        [Command("take"), Description("Command for admins to take blood points from users."), RequireRoles(RoleCheckMode.Any, "Staff")]
        public async Task Take(CommandContext e, [Description("Who to take the blood points from")] DiscordMember user, [Description("Amount of blood points to take")] int bloodPoints = -1)
        {
            if (bloodPoints > 0)
            {
                UserObject.RootObject userData = RPClass.Users.Find(x => x.UserData.UserID == user.Id);
                userData.UserData.BloodPoints -= bloodPoints;
                if (userData.UserData.BloodPoints < 0) userData.UserData.BloodPoints = 0;
                await e.RespondAsync("User: " + userData.UserData.Username + " now has " + userData.UserData.BloodPoints + " blood points.");
                RPClass.SaveData(1);
            }
        }
        [Command("transfer"), Description("Command for users to transfer blood points to each other.")]
        public async Task Transfer(CommandContext e, [Description("Who to send the blood points to")] DiscordMember user, [Description("Amount of blood points to award")] int bloodPoints = -1)
        {

            if (bloodPoints > 0)
            {
                if (RPClass.Users.Find(x => x.UserData.UserID == e.Message.Author.Id).UserData.BloodPoints >= bloodPoints)
                {
                    RPClass.Users.Find(x => x.UserData.UserID == user.Id).UserData.BloodPoints += bloodPoints;
                    RPClass.Users.Find(x => x.UserData.UserID == e.Message.Author.Id).UserData.BloodPoints -= bloodPoints;
                    UserObject.RootObject a = RPClass.Users.Find(x => x.UserData.UserID == user.Id);
                    UserObject.RootObject b = RPClass.Users.Find(x => x.UserData.UserID == e.Message.Author.Id);

                    await e.RespondAsync("User: " + a.UserData.Username + "now has " + a.UserData.BloodPoints + " blood points.");
                    await e.RespondAsync("User: " + b.UserData.Username + " now has " + b.UserData.BloodPoints + " blood points.");

                    RPClass.SaveData(1);
                }
                else
                {
                    await e.RespondAsync("You don't have enough points to do that.");
                }
            }
        }
        [Command("balance"), Aliases("bal"), Description("Prints the user's current blood point total.")]
        public async Task Balance(CommandContext e, [Description("Use all keyword to see everyone's blood point total (Admin only), or @mention someone to view their blood point total.")] string all = "")
        {
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder()
            {
                Color = new DiscordColor("4169E1"),
                Timestamp = DateTime.UtcNow
            }
            .WithFooter("Heroes & Villains");

            if (all == "all" && e.Member.Roles.Any(x => x == RPClass.StaffRole))
            {
                foreach (UserObject.RootObject userData in RPClass.Users)
                {
                    if (embed.Fields.Count < 25)
                    {
                        embed.AddField(userData.UserData.Username, "Blood Points: " + userData.UserData.BloodPoints);
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
                    embed.AddField(RPClass.Users.First(x => x.UserData.UserID == userNum).UserData.Username, "Blood Points: " + RPClass.Users.First(x => x.UserData.UserID == userNum).UserData.BloodPoints);
                }
                else
                {
                    await e.RespondAsync("Mention a user to select them.");
                }
            }
            else
            {
                UserObject.RootObject userData = RPClass.Users.Find(x => x.UserData.UserID == e.Member.Id);

                embed.AddField(userData.UserData.Username, "Blood Points: " + userData.UserData.BloodPoints);
            }
            await e.RespondAsync("", embed: embed);

        }

    }
    [Group("merit"), IsMuted]
    [Description("All Merit Point Commands")]
    class MeritClass : BaseCommandModule
    {
        [Command("give"), Description("Command for admins to give out merit points to users."), RequireRoles(RoleCheckMode.Any, "Staff")]
        public async Task Give(CommandContext e, [Description("Who to award the merit points to")] DiscordMember user, [Description("Amount of merit points to award")] int meritPoints = -1)
        {
            if (meritPoints > 0)
            {
                RPClass.Users.Find(x => x.UserData.UserID == user.Id).UserData.MeritPoints += meritPoints;
                UserObject.RootObject a = RPClass.Users.Find(x => x.UserData.UserID == user.Id);
                await e.RespondAsync("User: " + a.UserData.Username + " now has " + a.UserData.MeritPoints + " merit points.");
                RPClass.SaveData(1);
            }
        }

        [Command("take"), Description("Command for admins to take merit points from users."), RequireRoles(RoleCheckMode.Any, "Staff")]
        public async Task Take(CommandContext e, [Description("Who to take the merit points from")] DiscordMember user, [Description("Amount of merit points to take")] int meritPoints = -1)
        {
            if (meritPoints > 0)
            {
                UserObject.RootObject userData = RPClass.Users.Find(x => x.UserData.UserID == user.Id);
                userData.UserData.MeritPoints -= meritPoints;
                if (userData.UserData.MeritPoints < 0) userData.UserData.MeritPoints = 0;
                await e.RespondAsync("User: " + userData.UserData.Username + " now has " + userData.UserData.MeritPoints + " merit points.");
                RPClass.SaveData(1);
            }
        }
        [Command("transfer"), Description("Command for users to transfer merit points to each other.")]
        public async Task Transfer(CommandContext e, [Description("Who to send the merit points to")] DiscordMember user, [Description("Amount of merit points to award")] int meritPoints = -1)
        {

            if (meritPoints > 0)
            {
                if (RPClass.Users.Find(x => x.UserData.UserID == e.Message.Author.Id).UserData.MeritPoints >= meritPoints)
                {
                    RPClass.Users.Find(x => x.UserData.UserID == user.Id).UserData.MeritPoints += meritPoints;
                    RPClass.Users.Find(x => x.UserData.UserID == e.Message.Author.Id).UserData.MeritPoints -= meritPoints;
                    UserObject.RootObject a = RPClass.Users.Find(x => x.UserData.UserID == user.Id);
                    UserObject.RootObject b = RPClass.Users.Find(x => x.UserData.UserID == e.Message.Author.Id);

                    await e.RespondAsync("User: " + a.UserData.Username + "now has " + a.UserData.MeritPoints + " merit points.");
                    await e.RespondAsync("User: " + b.UserData.Username + " now has " + b.UserData.MeritPoints + " merit points.");

                    RPClass.SaveData(1);
                }
                else
                {
                    await e.RespondAsync("You don't have enough points to do that.");
                }
            }
        }
        [Command("balance"), Aliases("bal"), Description("Prints the user's current merit point total.")]
        public async Task Balance(CommandContext e, [Description("Use all keyword to see everyone's merit point total (Admin only), or @mention someone to view their merit point total.")] string all = "")
        {
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder()
            {
                Color = new DiscordColor("4169E1"),
                Timestamp = DateTime.UtcNow
            }
            .WithFooter("Heroes & Villains");

            if (all == "all" && e.Member.Roles.Any(x => x == RPClass.StaffRole))
            {
                foreach (UserObject.RootObject userData in RPClass.Users)
                {
                    if (embed.Fields.Count < 25)
                    {
                        embed.AddField(userData.UserData.Username, "Merit Points: " + userData.UserData.MeritPoints);
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
                    embed.AddField(RPClass.Users.First(x => x.UserData.UserID == userNum).UserData.Username, "Merit Points: " + RPClass.Users.First(x => x.UserData.UserID == userNum).UserData.MeritPoints);
                }
                else
                {
                    await e.RespondAsync("Mention a user to select them.");
                }
            }
            else
            {
                UserObject.RootObject userData = RPClass.Users.Find(x => x.UserData.UserID == e.Member.Id);

                embed.AddField(userData.UserData.Username, "Merit Points: " + userData.UserData.MeritPoints);
            }
            await e.RespondAsync("", embed: embed);

        }

    }
}
