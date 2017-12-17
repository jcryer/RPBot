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
    [Group("items"), Description("Item Commands")]
    class ItemClass : RPClass
    {
        [Command("add"), Description("Command for admins to create new items"), RequireRolesAttribute("Staff", "Bot-Test")]
        public async Task Add(CommandContext e, [RemainingText, Description("Use the format name|description|price|availability(-1 if unlimited)|emoji|location(Write 1 for mall or 2 for Avalon Research)")] string addData)
        {
            try
            {
                string[] AddItems = addData.Split('|');

                if (!ItemsList.Any(x => x.name == AddItems[1]))
                {
                    if (int.Parse(AddItems[5]) == 1 || int.Parse(AddItems[5]) == 2)
                    {
                        List<KeyValuePair<int, int>> stats = new List<KeyValuePair<int, int>>();
                        string[] statSplit = addData.Split('|').Skip(1).ToArray();
                        foreach (string stat in statSplit)
                        {
                            string[] statElement = stat.Split(',');
                            stats.Add(new KeyValuePair<int, int>(int.Parse(statElement[0]), int.Parse(statElement[1])));
                        }
                        ItemsList.Add(new ShopObject.RootObject(1 + ItemsList.Count, AddItems[0], AddItems[1], int.Parse(AddItems[2]), int.Parse(AddItems[3]), AddItems[4], int.Parse(AddItems[5]), ulong.Parse("0")));
                        await e.RespondAsync("Item added!");
                        await AddItem(e, ItemsList.Last());
                        SaveData(2);
                    }
                    else
                    {
                        await e.RespondAsync("Incorrect location. Use 1 for the Mall or 2 for Avalon Research.");
                    }
                }
                else
                {
                    await e.RespondAsync("Item with the same name already exists.");
                }
            }
            catch
            {
                await e.RespondAsync("Use the format \"name|description|price|availability(-1 if unlimited)|emoji|location(Write 1 for mall or 2 for Avalon Research)\"");
            }
        }

        [Command("remove"), Description("Command for admins to remove items."), RequireRolesAttribute("Staff", "Bot-Test")]
        public async Task Remove(CommandContext e, [Description("Use the item ID from the All command")] string removeNum)
        {
            try
            {
                int itemNumParse = 0;
                if (int.TryParse(removeNum, out itemNumParse))
                {
                    await RemoveItem(e, ItemsList.Find(x => x.id == itemNumParse));
                    ItemsList.Remove(ItemsList.Find(x => x.id == itemNumParse));
                    SaveData(1);
                    SaveData(2);
                    await e.RespondAsync("Item removed.");
                }
                else
                {
                    await e.RespondAsync("Use the item ID from the All command.");
                }
            }
            catch
            {
                await e.RespondAsync("Use the item ID from the All command.");
            }
        }

        [Command("all"), Description("Command to list all Mall items.")]
        public async Task All(CommandContext e)
        {
            if (ItemsList.Count > 0)
            {
                DiscordEmbedBuilder embed = new DiscordEmbedBuilder();

                embed.Color = new DiscordColor("#169E1");
                embed.WithFooter("Heroes & Villains");
                embed.WithTimestamp(DateTime.UtcNow);
                foreach (ShopObject.RootObject item in ItemsList.Where(x => x.location == 1))
                {
                    if (embed.Fields.Count >= 20)
                    {
                        await e.Channel.SendMessageAsync("", embed: embed);
                        embed.ClearFields();
                    }
                    embed.AddField(item.emoji + " " + item.name, "$" + item.price + "\n\n" + item.description + "\nItem ID: " + item.id);
                    
                }
                await e.Channel.SendMessageAsync("", embed: embed);
            }
        }

        [Command("avalon"), Description("Command to list all Avalon custom items.")]
        public async Task Avalon(CommandContext e)
        {
            if (ItemsList.Count > 0)
            {
                DiscordEmbedBuilder embed = new DiscordEmbedBuilder();

                embed.Color = new DiscordColor("4169E1");
                embed.WithFooter("Heroes & Villains");
                embed.WithTimestamp(DateTime.UtcNow);
                foreach (ShopObject.RootObject item in ItemsList.Where(x => x.location == 2))
                {
                    if (embed.Fields.Count >= 20)
                    {
                        await e.Channel.SendMessageAsync("", embed: embed);
                        embed.ClearFields();
                    }
                    UserObject.RootObject x = Users.FirstOrDefault(y => y.InvData.items.Contains(item.id));
                    string owner = "Nobody";
                    if (x != null) owner = x.UserData.username;
                    embed.AddField(item.emoji + " " + item.name, "$" + item.price + "\n\n" + item.description + "\nItem ID: " + item.id + "\nOwner: " + owner);

                }

                await e.Channel.SendMessageAsync("", embed: embed);
            }
        }
        [Command("stock"), Description("Admin item stock command"), RequireRolesAttribute("Staff", "Bot-Test")]
        public async Task Stock(CommandContext e, [Description("Item ID (from !items all command) of item wanting to be changed.")] int itemID, [Description("How much you wish to change it by.")] int stockChange)
        {
            try
            {
                if (ItemsList.Any(x => x.id == itemID))
                {
                    ShopObject.RootObject item = ItemsList.First(x => x.id == itemID);
                    item.availability += stockChange;

                    if (item.location == 1)
                    {
                        DiscordChannel ItemChannel = e.Guild.GetChannel(312995249658003553);
                        DiscordMessage itemFromShop = await ItemChannel.GetMessageAsync(item.messageID);
                        string numLeft = "";
                        if (item.availability == -1)
                        {
                            numLeft += "∞";
                        }
                        else
                        {
                            numLeft += item.availability;
                        }
                        DiscordEmbedBuilder embed = new DiscordEmbedBuilder();

                        embed.Color = new DiscordColor("4169E1");
                        embed.WithTitle(item.emoji + " " + item.name);
                        embed.WithFooter("Heroes & Villains");
                        embed.WithTimestamp(DateTime.UtcNow);
                        embed.AddField("$" + item.price + " - " + numLeft + " left in stock", item.description);
                        DiscordMessage y = await itemFromShop.ModifyAsync("", embed: embed);
                    }
                    SaveData(1);
                    SaveData(2);
                    await e.RespondAsync("Stock changed.");
                }
                else
                {
                    await e.RespondAsync("Use the item ID from the !items all command.");
                }
            }
            catch
            {
                await e.RespondAsync("Use the item ID from the !items all command.");
            }
        }

        [Command("inventory"), Aliases("inv"), Description("Command to view your inventory.")]
        public async Task Inventory(CommandContext e, [Description("Use all keyword to see everyone's inventory, or mention a specific person to view their inventory.")] string all = "")
        {
            if (all == "all")
            {
                DiscordEmbedBuilder embed = new DiscordEmbedBuilder();

                embed.Color = new DiscordColor("4169E1");
                embed.WithFooter("Heroes & Villains");
                embed.WithTimestamp(DateTime.UtcNow);

                foreach (UserObject.RootObject userData in Users)
                {
                    if (embed.Fields.Count < 25)
                    {
                        if (userData.InvData.items.Count > 0)
                        {
                            string userItems = "";
                            foreach (int item in userData.InvData.items)
                            {
                                ShopObject.RootObject itemInfo = ItemsList.First(x => x.id == item);
                                userItems += itemInfo.emoji + " " + itemInfo.name + "\n\n";
                            }
                            embed.AddField(userData.UserData.username, userItems);
                        }
                        else
                        {
                            embed.AddField(userData.UserData.username, "No items");
                        }
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

                await e.Channel.SendMessageAsync("", embed: embed);
                await Task.Delay(500);
            }

            else if (!string.IsNullOrWhiteSpace(all))
            {
                ulong userNum = 0;
                if (ulong.TryParse(all.Replace("<", "").Replace(">", "").Replace("@", "").Replace("!", ""), out userNum))
                {
                    try
                    {
                        UserObject.RootObject userData = Users.Find(x => x.UserData.userID == userNum);
                        DiscordEmbedBuilder embed = new DiscordEmbedBuilder();

                        embed.Color = new DiscordColor("4169E1");
                        embed.WithFooter("Heroes & Villains");
                        embed.WithTimestamp(DateTime.UtcNow); if (userData.InvData.items.Count > 0)
                        {
                            string userItems = "";
                            foreach (int item in userData.InvData.items)
                            {
                                ShopObject.RootObject itemInfo = ItemsList.First(x => x.id == item);
                                userItems += itemInfo.emoji + " " + itemInfo.name + "\n\n";
                            }
                            embed.AddField(userData.UserData.username, userItems);
                        }
                        else
                        {
                            embed.AddField(userData.UserData.username, "No items");
                        }

                        await e.Channel.SendMessageAsync("", embed: embed);

                    }
                    catch
                    {
                        await e.RespondAsync("Mention a user to select them.");
                    }
                }
            }
            else
            {
                DiscordEmbedBuilder embed = new DiscordEmbedBuilder();

                embed.Color = new DiscordColor("4169E1");
                embed.WithFooter("Heroes & Villains");
                embed.WithTimestamp(DateTime.UtcNow); UserObject.RootObject userData = Users.Find(x => x.UserData.userID == e.Member.Id);
                if (userData.InvData.items.Count > 0)
                {
                    string userItems = "";
                    foreach (int item in userData.InvData.items)
                    {
                        ShopObject.RootObject itemInfo = ItemsList.First(x => x.id == item);
                        userItems += itemInfo.emoji + " " + itemInfo.name + "\n\n";
                    }
                    embed.AddField(userData.UserData.username, userItems);
                }
                else
                {
                    embed.AddField(userData.UserData.username, "No items");
                }

                await e.Channel.SendMessageAsync("", embed: embed);
            }

        }
        [Command("buy"), Description("Command to buy an item.")]
        public async Task Buy(CommandContext e, [Description("Name of item you are buying.")] string itemName)
        {
            UserObject.RootObject user = Users.First(x => x.UserData.userID == e.User.Id);
            ShopObject.RootObject item = ItemsList.FirstOrDefault(x => x.name == itemName);
            DiscordMessage msgToDelete = e.Message;
            if (item != null)
            {
                if (user.UserData.money >= item.price)
                {
                    if (!user.InvData.items.Any(x => x == item.id))
                    {
                        user.UserData.money -= item.price;
                        user.InvData.items.Add(item.id);
                        if (item.availability != -1) item.availability -= 1;

                        msgToDelete = await e.Channel.SendMessageAsync("Item bought! Congratulations " + user.UserData.username + "!");
                    }
                    else
                    {
                        msgToDelete = await e.Channel.SendMessageAsync("You already own this item.");
                    }
                }
                else
                {
                    msgToDelete = await e.Channel.SendMessageAsync("You do not have enough money.");
                }
                await Task.Delay(2000);
                await msgToDelete.DeleteAsync();
                SaveData(1);
            }
            else
            {
                await e.RespondAsync("Use the exact item name.");
            }
        }

        [Command("sell"), Description("Command to sell an item.")]
        public async Task Sell(CommandContext e, [Description("Name of item you are selling")] string itemName)
        {
            UserObject.RootObject user = Users.First(x => x.UserData.userID == e.User.Id);
            ShopObject.RootObject item = ItemsList.FirstOrDefault(x => x.name == itemName);
            DiscordMessage msgToDelete = e.Message;
            if (item != null)
            {
                if (user.InvData.items.Any(x => x == item.id))
                {
                    user.UserData.money += item.price / 2;
                    user.InvData.items.Remove(user.InvData.items.First(x => x == item.id));
                    if (item.availability != -1) item.availability += 1;
                    msgToDelete = await e.Channel.SendMessageAsync("Item sold! Congratulations " + user.UserData.username + "!");
                }
                else
                {
                    msgToDelete = await e.Channel.SendMessageAsync("You do not own that item.");
                }
                await Task.Delay(2000);
                await msgToDelete.DeleteAsync();
                SaveData(1);
            }
            else
            {
                await e.RespondAsync("Use the exact item name.");
            }
        }

        //  Non-Command Methods
        public static async Task AddItem(CommandContext e, ShopObject.RootObject itemData)
        {
            List<DiscordChannel> ItemChannels = new List<DiscordChannel>(await e.Guild.GetChannelsAsync());
            DiscordChannel ItemChannel = ItemChannels.Find(x => x.Id == 312995249658003553);
            if (itemData.location == 1)
            {


                string numLeft = "";
                if (itemData.availability == -1)
                {
                    numLeft += "∞";
                }
                else
                {
                    numLeft += itemData.availability;
                }
                DiscordEmbedBuilder embed = new DiscordEmbedBuilder();

                embed.Color = new DiscordColor("4169E1");
                embed.WithTitle(itemData.emoji + " " + itemData.name);
                embed.WithFooter("Heroes & Villains");
                embed.WithTimestamp(DateTime.UtcNow);
                embed.AddField("$" + itemData.price + " - " + numLeft + " left in stock",itemData.description);
                DiscordMessage y = await ItemChannel.SendMessageAsync("", embed: embed);
                await Task.Delay(500);
            
                await y.CreateReactionAsync(DiscordEmoji.FromUnicode(e.Client, "📥"));
                await Task.Delay(500);
                await y.CreateReactionAsync(DiscordEmoji.FromUnicode(e.Client, "📤"));
                ItemsList.First(x => x.name == itemData.name).messageID = y.Id;
                SaveData(1);
                SaveData(2);
            }
        }

        public static async Task RemoveItem(CommandContext e, ShopObject.RootObject itemData)
        {
            if (itemData.location == 1)
            {
                DiscordChannel ItemChannel = e.Guild.GetChannel(312995249658003553);


                DiscordMessage msg = await ItemChannel.GetMessageAsync(itemData.messageID);
                await msg.DeleteAsync();
            }
        }
    }
}
