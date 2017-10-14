using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RPBot
{
    class ItemClass : RPClass
    { 
        public static async Task Items(CommandContext e, string cmd, string subcmd)
        {
            if (cmd == "add")
            {
                if (e.Member.Roles.Any(x => x.Id == 313845882841858048))
                {
                    if (subcmd != "")
                    {
                        try
                        {
                            string[] AddItems = subcmd.Split('|');

                            if (!ItemsList.Any(x => x.name == AddItems[1]))
                            {
                                if (int.Parse(AddItems[5]) == 1 || int.Parse(AddItems[5]) == 2)
                                {
                                    List<KeyValuePair<int, int>> stats = new List<KeyValuePair<int, int>>();
                                    try
                                    {
                                        string[] statSplit = subcmd.Split('|').Skip(1).ToArray();
                                        foreach (string stat in statSplit)
                                        {
                                            string[] statElement = stat.Split(',');
                                            stats.Add(new KeyValuePair<int, int>(int.Parse(statElement[0]), int.Parse(statElement[1])));
                                        }
                                    }
                                    catch
                                    {
                                        await e.RespondAsync("No stats specified");
                                    }// id, name, desc, price, availability, emoji, location, messageID
                                     // !add "test;an awesome test item|20|1|:P|1|247458920134678093
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
                    else
                    {
                        await e.RespondAsync("Use the format \"name|description|price|availability(-1 if unlimited)|emoji|location(Write 1 for mall or 2 for Avalon Research)\"");
                    }
                }
            }
            else if (cmd == "remove")
            {
                if (e.Member.Roles.Any(x => x.Id == 313845882841858048))
                {
                    if (subcmd != "")
                    {
                        try
                        {
                            int itemNumParse = 0;
                            if (int.TryParse(subcmd, out itemNumParse))
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
                    else
                    {
                        await e.RespondAsync("Use the item ID from the All command.");
                    }
                }

            }
            else if (cmd == "all")
            {
                if (ItemsList.Count > 0)
                {
                    DiscordEmbedBuilder embed = new DiscordEmbedBuilder();

                    embed.Color = new DiscordColor(4589319);
                    embed.WithFooter("Heroes & Villains");
                    embed.WithTimestamp(DateTime.UtcNow);
                    foreach (ShopObject.RootObject item in ItemsList)
                    {
                        embed.AddField(item.emoji + " " + item.name, "$" + item.price + "\n\n" + item.description + "\nItem ID: " + item.id);
                    }
                    await e.Channel.SendMessageAsync("", embed: embed);
                }
            }
            else if (cmd == "avalon")
            {
                if (ItemsList.Count > 0)
                {
                    DiscordEmbedBuilder embed = new DiscordEmbedBuilder();

                    embed.Color = new DiscordColor(4589319);
                    embed.WithFooter("Heroes & Villains");
                    embed.WithTimestamp(DateTime.UtcNow);
                    foreach (ShopObject.RootObject item in ItemsList.Where(x => x.location == 2))
                    {
                        UserObject.RootObject x = Users.FirstOrDefault(y => y.InvData.items.Contains(item.id));
                        string owner = "Nobody";
                        if (x != null) owner = x.UserData.username;
                        embed.AddField(item.emoji + " " + item.name, "$" + item.price + "\n\n" + item.description + "\nItem ID: " + item.id + "\nOwner: " + owner);

                    }

                    await e.Channel.SendMessageAsync("", embed: embed);
                }
            }
            
        }

        public static async Task Stock(CommandContext e, int itemID, int stockChange)
        {
            if (e.Member.Roles.Any(x => x.Id == 313845882841858048))
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

                            embed.Color = new DiscordColor(0x0066FF);
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
                        await e.RespondAsync("Use the item ID from the All command.");
                    }
                }
                catch
                {
                    await e.RespondAsync("Use the item ID from the All command.");
                }
            }
        }

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

                embed.Color = new DiscordColor(0x0066FF);
                embed.WithTitle(itemData.emoji + " " + itemData.name);
                embed.WithFooter("Heroes & Villains");
                embed.WithTimestamp(DateTime.UtcNow);
                embed.AddField("$" + itemData.price + " - " + numLeft + " left in stock",itemData.description);
                DiscordMessage y = await ItemChannel.SendMessageAsync("", embed: embed);
                Thread.Sleep(500);
            
                await y.CreateReactionAsync(DiscordEmoji.FromUnicode(e.Client, "📥"));
                Thread.Sleep(500);
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

        public static async Task Inventory(CommandContext e, string all)
        {
            if (all == "all")
            {
                DiscordEmbedBuilder embed = new DiscordEmbedBuilder();

                embed.Color = new DiscordColor(4589319);
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
                        Thread.Sleep(500);
                        embed.ClearFields();

                        embed.Color = new DiscordColor(4589319);
                        embed.WithFooter("Heroes & Villains");
                        embed.WithTimestamp(DateTime.UtcNow);
                    }
                }

                await e.Channel.SendMessageAsync("", embed: embed);
                Thread.Sleep(500);
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

                        embed.Color = new DiscordColor(4589319);
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

                embed.Color = new DiscordColor(4589319);
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

        public static async Task Buy(CommandContext e, string itemName)
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
                Thread.Sleep(2000);
                await msgToDelete.DeleteAsync();
                SaveData(1);
            }
            else
            {
                await e.RespondAsync("Use the exact item name.");
            }
        }

        public static async Task Sell(CommandContext e, string itemName)
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
                Thread.Sleep(2000);
                await msgToDelete.DeleteAsync();
                SaveData(1);
            }
            else
            {
                await e.RespondAsync("Use the exact item name.");
            }
        }
    }
}
