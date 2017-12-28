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
        [Command("add"), Description("Command for admins to create new items"), RequireRoles(RoleCheckMode.Any, "Staff")]
        public async Task Add(CommandContext e, [RemainingText, Description("Use the format name|description|price|availability(-1 if unlimited)|emoji|location(Write 1 for mall or 2 for Avalon Research)")] string addData)
        {
            try
            {
                string[] AddItems = addData.Split('|');

                if (!ItemsList.Any(x => x.Name == AddItems[1]))
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

        [Command("remove"), Description("Command for admins to remove items."), RequireRoles(RoleCheckMode.Any, "Staff")]
        public async Task Remove(CommandContext e, [Description("Use the item ID from the All command")] string removeNum)
        {
            try
            {
                if (int.TryParse(removeNum, out int itemNumParse))
                {
                    await RemoveItem(e, ItemsList.Find(x => x.Id == itemNumParse));
                    ItemsList.Remove(ItemsList.Find(x => x.Id == itemNumParse));
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
                DiscordEmbedBuilder embed = new DiscordEmbedBuilder
                {
                    Color = new DiscordColor("#169E1"),
                    Timestamp = DateTime.UtcNow
                }
                .WithFooter("Heroes & Villains");

                foreach (ShopObject.RootObject item in ItemsList.Where(x => x.Location == 1))
                {
                    if (embed.Fields.Count >= 20)
                    {
                        await e.Channel.SendMessageAsync("", embed: embed);
                        embed.ClearFields();
                    }
                    embed.AddField(item.Emoji + " " + item.Name, "$" + item.Price + "\n\n" + item.Description + "\nItem ID: " + item.Id);
                    
                }
                await e.Channel.SendMessageAsync("", embed: embed);
            }
        }

        [Command("avalon"), Description("Command to list all Avalon custom items.")]
        public async Task Avalon(CommandContext e)
        {
            if (ItemsList.Count > 0)
            {
                DiscordEmbedBuilder embed = new DiscordEmbedBuilder
                {
                    Color = new DiscordColor("4169E1"),
                    Timestamp = DateTime.UtcNow
                }
                .WithFooter("Heroes & Villains");

                foreach (ShopObject.RootObject item in ItemsList.Where(x => x.Location == 2))
                {
                    if (embed.Fields.Count >= 20)
                    {
                        await e.Channel.SendMessageAsync("", embed: embed);
                        embed.ClearFields();
                    }
                    UserObject.RootObject x = Users.FirstOrDefault(y => y.InvData.Items.Contains(item.Id));
                    string owner = "Nobody";
                    if (x != null) owner = x.UserData.Username;
                    embed.AddField(item.Emoji + " " + item.Name, "$" + item.Price + "\n\n" + item.Description + "\nItem ID: " + item.Id + "\nOwner: " + owner);

                }

                await e.Channel.SendMessageAsync("", embed: embed);
            }
        }
        [Command("stock"), Description("Admin item stock command"), RequireRoles(RoleCheckMode.Any, "Staff")]
        public async Task Stock(CommandContext e, [Description("Item ID (from !items all command) of item wanting to be changed.")] int itemID, [Description("How much you wish to change it by.")] int stockChange)
        {
            try
            {
                if (ItemsList.Any(x => x.Id == itemID))
                {
                    ShopObject.RootObject item = ItemsList.First(x => x.Id == itemID);
                    item.Availability += stockChange;

                    if (item.Location == 1)
                    {
                        DiscordChannel ItemChannel = e.Guild.GetChannel(312995249658003553);
                        DiscordMessage itemFromShop = await ItemChannel.GetMessageAsync(item.MessageID);
                        string numLeft = "";
                        if (item.Availability == -1)
                        {
                            numLeft += "∞";
                        }
                        else
                        {
                            numLeft += item.Availability;
                        }
                        DiscordEmbedBuilder embed = new DiscordEmbedBuilder
                        {
                            Color = new DiscordColor("4169E1"),
                            Title = item.Emoji + " " + item.Name,
                            Timestamp = DateTime.UtcNow
                        }
                        .WithFooter("Heroes & Villains")
                        .AddField("$" + item.Price + " - " + numLeft + " left in stock", item.Description);

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
                DiscordEmbedBuilder embed = new DiscordEmbedBuilder
                {
                    Color = new DiscordColor("4169E1"),
                    Timestamp = DateTime.UtcNow
                }
                .WithFooter("Heroes & Villains");

                foreach (UserObject.RootObject userData in Users)
                {
                    if (embed.Fields.Count < 25)
                    {
                        if (userData.InvData.Items.Count > 0)
                        {
                            string userItems = "";
                            foreach (int item in userData.InvData.Items)
                            {
                                ShopObject.RootObject itemInfo = ItemsList.First(x => x.Id == item);
                                userItems += itemInfo.Emoji + " " + itemInfo.Name + "\n\n";
                            }
                            embed.AddField(userData.UserData.Username, userItems);
                        }
                        else
                        {
                            embed.AddField(userData.UserData.Username, "No items");
                        }
                    }
                    else
                    {
                        await e.Channel.SendMessageAsync("", embed: embed);
                        await Task.Delay(500);
                        embed.ClearFields();
                    }
                }

                await e.Channel.SendMessageAsync("", embed: embed);
                await Task.Delay(500);
            }

            else if (!string.IsNullOrWhiteSpace(all))
            {
                if (ulong.TryParse(all.Replace("<", "").Replace(">", "").Replace("@", "").Replace("!", ""), out ulong userNum))
                {
                    try
                    {
                        UserObject.RootObject userData = Users.Find(x => x.UserData.UserID == userNum);
                        DiscordEmbedBuilder embed = new DiscordEmbedBuilder()
                        {
                            Color = new DiscordColor("4169E1"),
                            Timestamp = DateTime.UtcNow
                        }
                        .WithFooter("Heroes & Villains");

                        if (userData.InvData.Items.Count > 0)
                        {
                            string userItems = "";
                            foreach (int item in userData.InvData.Items)
                            {
                                ShopObject.RootObject itemInfo = ItemsList.First(x => x.Id == item);
                                userItems += itemInfo.Emoji + " " + itemInfo.Name + "\n\n";
                            }
                            embed.AddField(userData.UserData.Username, userItems);
                        }
                        else
                        {
                            embed.AddField(userData.UserData.Username, "No items");
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
                DiscordEmbedBuilder embed = new DiscordEmbedBuilder
                {
                    Color = new DiscordColor("4169E1"),
                    Timestamp = DateTime.UtcNow
                }
                .WithFooter("Heroes & Villains");

                UserObject.RootObject userData = Users.Find(x => x.UserData.UserID == e.Member.Id);
                if (userData.InvData.Items.Count > 0)
                {
                    string userItems = "";
                    foreach (int item in userData.InvData.Items)
                    {
                        ShopObject.RootObject itemInfo = ItemsList.First(x => x.Id == item);
                        userItems += itemInfo.Emoji + " " + itemInfo.Name + "\n\n";
                    }
                    embed.AddField(userData.UserData.Username, userItems);
                }
                else
                {
                    embed.AddField(userData.UserData.Username, "No items");
                }

                await e.Channel.SendMessageAsync("", embed: embed);
            }

        }
        [Command("buy"), Description("Command to buy an item.")]
        public async Task Buy(CommandContext e, [Description("Name of item you are buying.")] string itemName)
        {
            UserObject.RootObject user = Users.First(x => x.UserData.UserID == e.User.Id);
            ShopObject.RootObject item = ItemsList.FirstOrDefault(x => x.Name == itemName);
            DiscordMessage msgToDelete = e.Message;
            if (item != null)
            {
                if (user.UserData.Money >= item.Price)
                {
                    if (!user.InvData.Items.Any(x => x == item.Id))
                    {
                        user.UserData.Money -= item.Price;
                        user.InvData.Items.Add(item.Id);
                        if (item.Availability != -1) item.Availability -= 1;

                        msgToDelete = await e.Channel.SendMessageAsync("Item bought! Congratulations " + user.UserData.Username + "!");
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
            UserObject.RootObject user = Users.First(x => x.UserData.UserID == e.User.Id);
            ShopObject.RootObject item = ItemsList.FirstOrDefault(x => x.Name == itemName);
            DiscordMessage msgToDelete = e.Message;
            if (item != null)
            {
                if (user.InvData.Items.Any(x => x == item.Id))
                {
                    user.UserData.Money += item.Price / 2;
                    user.InvData.Items.Remove(user.InvData.Items.First(x => x == item.Id));
                    if (item.Availability != -1) item.Availability += 1;
                    msgToDelete = await e.Channel.SendMessageAsync("Item sold! Congratulations " + user.UserData.Username + "!");
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
            if (itemData.Location == 1)
            {


                string numLeft = "";
                if (itemData.Availability == -1)
                {
                    numLeft += "∞";
                }
                else
                {
                    numLeft += itemData.Availability;
                }
                DiscordEmbedBuilder embed = new DiscordEmbedBuilder
                {
                    Color = new DiscordColor("4169E1"),
                    Title = itemData.Emoji + " " + itemData.Name,
                    Timestamp = DateTime.UtcNow
                }
                .WithFooter("Heroes & Villains")
                .AddField("$" + itemData.Price + " - " + numLeft + " left in stock",itemData.Description);
                DiscordMessage y = await ItemChannel.SendMessageAsync("", embed: embed);
                await Task.Delay(500);
            
                await y.CreateReactionAsync(DiscordEmoji.FromUnicode(e.Client, "📥"));
                await Task.Delay(500);
                await y.CreateReactionAsync(DiscordEmoji.FromUnicode(e.Client, "📤"));
                ItemsList.First(x => x.Name == itemData.Name).MessageID = y.Id;
                SaveData(1);
                SaveData(2);
            }
        }

        public static async Task RemoveItem(CommandContext e, ShopObject.RootObject itemData)
        {
            if (itemData.Location == 1)
            {
                DiscordChannel ItemChannel = e.Guild.GetChannel(312995249658003553);


                DiscordMessage msg = await ItemChannel.GetMessageAsync(itemData.MessageID);
                await msg.DeleteAsync();
            }
        }
    }
}
