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
    [Group("money")]
    [Description("All Money Commands")]
    class MoneyClass : RPClass
    { 
        [Command("give"), Description("Command for admins to give out currency to users."), RequireRolesAttribute("Staff", "Bot-Test")]
        public async Task Give(CommandContext e, [Description("Who to award the money to")] DiscordMember user, [Description("Amount of money to award")] int money = -1)
        {
            if (money > 0)
            {
                RPClass.Users.Find(x => x.UserData.userID == user.Id).UserData.money += money;
                UserObject.RootObject a = RPClass.Users.Find(x => x.UserData.userID == user.Id);
                await e.RespondAsync("User: " + a.UserData.username + " now has $" + a.UserData.money);
                RPClass.SaveData(1);
            }
        }
        
        [Command("take"), Description("Command for admins to take currency from users."), RequireRolesAttribute("Staff", "Bot-Test")]
        public async Task Take(CommandContext e, [Description("Who to take the money from")] DiscordMember user, [Description("Amount of money to take")] int money = -1)
        {
            if (money > 0)
            {
                UserObject.RootObject userData = RPClass.Users.Find(x => x.UserData.userID == user.Id);
                userData.UserData.money -= money;
                if (userData.UserData.money < 0) userData.UserData.money = 0;
                await e.RespondAsync("User: " + userData.UserData.username + " now has $" + userData.UserData.money);
                RPClass.SaveData(1);
            }
        }
        [Command("transfer"), Description("Command for users to transfer money to each other.")]
        public async Task Transfer(CommandContext e, [Description("Who to send the money to")] DiscordMember user, [Description("Amount of money to award")] int money = -1)
        {

            if (money > 0 )
            {
                if (RPClass.Users.Find(x => x.UserData.userID == e.Message.Author.Id).UserData.money >= money)
                {
                    RPClass.Users.Find(x => x.UserData.userID == user.Id).UserData.money += money;
                    RPClass.Users.Find(x => x.UserData.userID == e.Message.Author.Id).UserData.money -= money;
                    UserObject.RootObject a = RPClass.Users.Find(x => x.UserData.userID == user.Id);
                    UserObject.RootObject b = RPClass.Users.Find(x => x.UserData.userID == e.Message.Author.Id);

                    await e.RespondAsync("User: " + a.UserData.username + " now has $" + a.UserData.money);
                    await e.RespondAsync("User: " + b.UserData.username + " now has $" + b.UserData.money);

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
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder();

            embed.Color = new DiscordColor("4169E1");
            embed.WithFooter("Heroes & Villains");
            embed.WithTimestamp(DateTime.UtcNow);

            if (all == "all" && e.Member.Roles.Any(x => x == StaffRole))
            {
                foreach (UserObject.RootObject userData in RPClass.Users)
                {
                    if (embed.Fields.Count < 25)
                    {
                        embed.AddField(userData.UserData.username, "Money: $" + userData.UserData.money);
                    }
                    else
                    {
                        await e.Channel.SendMessageAsync("", embed: embed);
                        await Task.Delay(500);
                        embed.ClearFields();

                        embed.Color = new DiscordColor("4169E1");
                        embed.WithFooter("Heroes & Villains");
                        embed.WithTimestamp(DateTime.UtcNow);
                    }
                }
            }
            else if (!string.IsNullOrWhiteSpace(all))
            {
                all = all.Replace("<", "").Replace(">", "").Replace("@", "").Replace("!", "");
                ulong userNum = 0;
                if (ulong.TryParse(all, out userNum))
                {
                    embed.AddField(RPClass.Users.First(x => x.UserData.userID == userNum).UserData.username, "Money: $" + RPClass.Users.First(x => x.UserData.userID == userNum).UserData.money);
                }
                else
                {
                    await e.RespondAsync("Mention a user to select them.");
                }
            }
            else
            {
                UserObject.RootObject userData = RPClass.Users.Find(x => x.UserData.userID == e.Member.Id);

                embed.AddField(userData.UserData.username,"Money: $" + userData.UserData.money);
            }
            await e.Channel.SendMessageAsync("", embed: embed);

        }
        
    }
}
