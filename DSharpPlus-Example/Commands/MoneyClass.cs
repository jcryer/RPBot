using DSharpPlus;
using DSharpPlus.CommandsNext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RPBot
{
    class MoneyClass : RPClass
    {

        public static async Task Give(CommandContext e, string user, int money)
        {
            ulong userNum = 0;
            if (money > 0 && ulong.TryParse(user, out userNum))
            {
                if (e.Member.Roles.Any(x => x.Id == 313845882841858048))
                {
                    {
                        await AddUsers(e.Guild, false);
                        try
                        {
                            Users.Find(x => x.UserData.userID == userNum).UserData.money += money;
                            UserObject.RootObject a = Users.Find(x => x.UserData.userID == userNum);
                            await e.RespondAsync("User: " + a.UserData.username + " now has $" + a.UserData.money);
                            SaveData(1);
                        }
                        catch
                        {
                            await e.RespondAsync("Mention a user to select them.");
                        }
                    }
                }
            }

        }

        public static async Task Take(CommandContext e, string user, int money)
        {
            ulong userNum = 0;
            if (money > 0 && ulong.TryParse(user, out userNum))
            {
                if (e.Member.Roles.Any(x => x.Id == 313845882841858048))
                {
                    await AddUsers(e.Guild, false);
                    try
                    {
                        UserObject.RootObject userData = Users.Find(x => x.UserData.userID == userNum);
                        userData.UserData.money -= money;
                        if (userData.UserData.money < 0) userData.UserData.money = 0;
                        await e.RespondAsync("User: " + userData.UserData.username + " now has $" + userData.UserData.money);
                        SaveData(1);
                    }
                    catch
                    {
                        await e.RespondAsync("Mention a user to select them.");
                    }
                }
            }
        }

        public static async Task Transfer(CommandContext e, string user, int money)
        {
            ulong userNum = 0;
            if (money > 0 && ulong.TryParse(user, out userNum))
            {
                await AddUsers(e.Guild, false);
                if (Users.Find(x => x.UserData.userID == e.Message.Author.Id).UserData.money >= money)
                {
                    try
                    {

                        Users.Find(x => x.UserData.userID == userNum).UserData.money += money;
                        Users.Find(x => x.UserData.userID == e.Message.Author.Id).UserData.money -= money;
                        UserObject.RootObject a = Users.Find(x => x.UserData.userID == userNum);
                        UserObject.RootObject b = Users.Find(x => x.UserData.userID == e.Message.Author.Id);

                        await e.RespondAsync("User: " + a.UserData.username + " now has $" + a.UserData.money);
                        await e.RespondAsync("User: " + b.UserData.username + " now has $" + b.UserData.money);

                        SaveData(1);
                    }
                    catch
                    {
                        await e.RespondAsync("Mention a user to select them.");
                    }
                }
                else
                {
                    await e.RespondAsync("You don't have enough money to do that.");
                }
            }
        }

        public static async Task Balance(CommandContext e, string all)
        {
            if (e.Guild.Id == 312918289988976653)
            {
                DiscordEmbedBuilder embed = new DiscordEmbedBuilder();

                embed.Color = new DiscordColor(4589319);
                embed.WithFooter("Heroes & Villains");
                embed.WithTimestamp(DateTime.UtcNow);


                if (all == "all" && (e.Member.Roles.Any(x => x.Id == 312961839359328266) || e.Member.Roles.Any(x => x.Id == 312961821063512065) || e.Member.Roles.Any(x => x.Id == 312979390516559885)))
                {
                    foreach (UserObject.RootObject userData in Users)
                    {
                        if (embed.Fields.Count < 25)
                        {
                            embed.AddField(userData.UserData.username, "Money: $" + userData.UserData.money);
                        }
                        else
                        {
                            await e.Channel.SendMessageAsync("", embed: embed);
                            Thread.Sleep(500);
                            embed.ClearFields();

                            embed.Color = new DiscordColor(4589319);
                            embed.WithFooter("Heroes & Villains");
                            embed.WithTimestamp(DateTime.UtcNow);
                        }
                    }
                }
                else
                {
                    UserObject.RootObject userData = Users.Find(x => x.UserData.userID == e.Member.Id);

                    embed.AddField(userData.UserData.username,"Money: $" + userData.UserData.money);
                }
                await e.Channel.SendMessageAsync("", embed: embed);
            }

        }
    }
}
