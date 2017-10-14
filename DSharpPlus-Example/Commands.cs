using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using DSharpPlus;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using System.Net.Sockets;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity;

namespace RPBot
{
    public sealed class Commands : Program 
    {
        [Command("give"), Description("Command for admins to give out currency to users.")]
        public async Task Give(CommandContext e, [Description("Who to award the money to (use a mention)")] string user = "", [Description("Amount of money to award")] int money = -1) => await MoneyClass.Give(e, user.Replace("<", "").Replace(">", "").Replace("@", "").Replace("!", ""), money);

        [Command("transfer"), Description("Command for users to transfer money to each other.")]
        public async Task Transfer(CommandContext e, [Description("Who to send the money to (use a mention)")] string user = "", [Description("Amount of money to award")] int money = -1) => await MoneyClass.Transfer(e, user.Replace("<", "").Replace(">", "").Replace("@", "").Replace("!", ""), money);
        
        [Command("balance"), Aliases("bal"), Description("Sends the user a PM with their current balance.")]
        public async Task Balance(CommandContext e, [Description("Use all keyword to see everyone's balance (Admin only)")] string all = "") => await MoneyClass.Balance(e, all);

        [Command("take"), Description("Command for admins to take currency from users.")]
        public async Task Take(CommandContext e, [Description("Who to take the money from (use a mention)")] string user = "", [Description("Amount of money to take")] int money = -1) => await MoneyClass.Take(e, user.Replace("<", "").Replace(">", "").Replace("@", "").Replace("!", ""), money);
        
        [Command("roll"), Description("Dice roll command!")]
        public async Task Roll(CommandContext e, [Description("Number of sides of the dice")] int numSides = 0, [Description("Number of rolls to do")] int numRolls = 0) => await RPClass.Roll(e, numSides, numRolls);
    
        [Command("xp"), Description("Admin xp command")]
        public async Task Stats(CommandContext e, [Description("User to change stats of (Use mention)")] string user, [Description("How much you wish to change it by (-5 to 5)")] int xpNum) => await XPClass.XP(e, user.Replace("<", "").Replace(">", "").Replace("@", "").Replace("!", ""), xpNum);

        [Command("stock"), Description("Admin item stock command")]
        public async Task Stock(CommandContext e, [Description("Item ID (from !items all command) of item wanting to be changed.")] int itemID, [Description("How much you wish to change it by (-5 to 5)")] int stockChange) => await ItemClass.Stock(e, itemID, stockChange);

        [Command("items"), Description("Admin item command.")]
        public async Task Items(CommandContext e, [Description("Either Add, Remove, All or Avalon.")] string cmd, [Description("If Adding, use the format \"name|description|price|availability(-1 if unlimited)|emoji|location(Write 1 for mall or 2 for Avalon Research)\". If removing, use the item ID from the All command.")] string subcmd = "") => await ItemClass.Items(e, cmd, subcmd);

        [Command("inventory"), Aliases("inv"), Description("Command to view your inventory.")]
        public async Task Inventory(CommandContext e, [Description("Use all keyword to see everyone's inventory, or mention a specific person to view their inventory. Do not use a sub command if you wish to view your own inventory.")] string all = "") => await ItemClass.Inventory(e, all);

        [Command("buy"), Description("Command to buy an item.")]
        public async Task Buy(CommandContext e, [Description("Name of item you are buying.")] string itemName) => await ItemClass.Buy(e, itemName);

        [Command("sell"), Description("Command to sell an item.")]
        public async Task Sell(CommandContext e, [Description("Name of item you are selling")] string itemName) => await ItemClass.Sell(e, itemName);

		[Command("choose"), Description("Command to choose one of the variables given.")]
		public async Task Choose(CommandContext e, [Description("List of variables seperated by commas.")] string choiceList) => await RPClass.Choose(e, choiceList);

        [Command("guild"), Description("Admin guild command.")]
        public async Task Guild(CommandContext e, [Description("Either Create, Destroy, ChangeStatus, AddUser or RemoveUser")] string cmd, [Description("Give name of Guild.")] string subcmd = "", [Description("If Create or ChangeStatus, use 1 for Active or 2 for Retired. If AddUser or RemoveUser, @mention the person.")] string subsubcmd = "") => await XPClass.Guild(e, cmd, subcmd, subsubcmd);

        [Command("cases"), Description("Admin cases command.")]
        public async Task Cases(CommandContext e, [Description("Mention a user.")] string user, [Description("Number to increase or decrease cases resolved by")] string caseNum)
        {
            user = user.Replace("<", "").Replace(">", "").Replace("@", "").Replace("!", "");
            try
            {
                RPClass.Users.First(x => x.UserData.userID == ulong.Parse(user)).UserData.resolvedCases += int.Parse(caseNum);
                if (RPClass.Users.First(x => x.UserData.userID == ulong.Parse(user)).UserData.resolvedCases < 0)
                    RPClass.Users.First(x => x.UserData.userID == ulong.Parse(user)).UserData.resolvedCases = 0;

                if (RPClass.Users.First(x => x.UserData.userID == ulong.Parse(user)).UserData.role == 1) await XPClass.UpdatePlayerRanking(e.Guild, 1);
                else if (RPClass.Users.First(x => x.UserData.userID == ulong.Parse(user)).UserData.role == 2) await XPClass.UpdatePlayerRanking(e.Guild, 2);
                await XPClass.UpdateGuildRanking(e.Guild);

                RPClass.SaveData(1);

                await e.RespondAsync("Cases updated.");
            }
            catch
            {
                await e.RespondAsync("Mention the user to select them, and use an integer to choose the number of cases.");
            }
        }

        [Command("crimes"), Description("Admin cases command.")]
        public async Task Crimes(CommandContext e, [Description("Mention a user.")] string user, [Description("Number to increase or decrease crimes committed by")] string crimeNum)
        {
            user = user.Replace("<", "").Replace(">", "").Replace("@", "").Replace("!", "");
            try
            {
                RPClass.Users.First(x => x.UserData.userID == ulong.Parse(user)).UserData.crimesCommitted += int.Parse(crimeNum);
                if (RPClass.Users.First(x => x.UserData.userID == ulong.Parse(user)).UserData.crimesCommitted < 0)
                    RPClass.Users.First(x => x.UserData.userID == ulong.Parse(user)).UserData.crimesCommitted = 0;

                if (RPClass.Users.First(x => x.UserData.userID == ulong.Parse(user)).UserData.role == 1) await XPClass.UpdatePlayerRanking(e.Guild, 1);
                else if (RPClass.Users.First(x => x.UserData.userID == ulong.Parse(user)).UserData.role == 2) await XPClass.UpdatePlayerRanking(e.Guild, 2);
                await XPClass.UpdateGuildRanking(e.Guild);

                RPClass.SaveData(1);

                await e.RespondAsync("Crimes updated.");
            }
            catch
            {
                await e.RespondAsync("Mention the user to select them, and use an integer to choose the number of crimes.");
            }
        }

        [Command("name"), Description("Command for users to change their RP name temporarily")]
        public async Task Name(CommandContext e, [Description("What to call yourself")] string name = "")
        {
            DiscordMessage x;
            if (name == "off")
            {
                SpeechObject.RootObject savedName = RPClass.SpeechList.FirstOrDefault(y => y.id == e.Member.Id);
                if (savedName != null)
                {
                    RPClass.SpeechList.Remove(savedName);
                    x = await e.RespondAsync("Removed from list.");
                }
                else
                {
                    x = await e.RespondAsync("");
                }
                Thread.Sleep(2000);
                await e.Message.DeleteAsync();
                await x.DeleteAsync();

            }
            else if (name != "")
            {
                SpeechObject.RootObject savedName = RPClass.SpeechList.FirstOrDefault(y => y.id == e.Member.Id);
                if (savedName != null)
                {
                    RPClass.SpeechList.Remove(savedName);
                }
                RPClass.SpeechList.Add(new SpeechObject.RootObject(e.Member.Id, name));
                x = await e.RespondAsync("Name changed.");
                Thread.Sleep(2000);
                await e.Message.DeleteAsync();
                await x.DeleteAsync();
            }
            else
            {
                x = await e.RespondAsync("Specify a name.");
                Thread.Sleep(2000);
                await e.Message.DeleteAsync();
                await x.DeleteAsync();
            }
        }

    }
}
