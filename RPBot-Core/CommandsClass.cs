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
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Interactivity;
using DSharpPlus.Entities;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;

namespace RPBot
{
    class CommandsClass : BaseCommandModule
    {
        [Command("roll"), Description("Dice roll command!"), IsMuted]
        public async Task Roll(CommandContext e, [Description("Number of sides of the dice")] int numSides = 0, [Description("Number of rolls to do")] int numRolls = 0)
        {
            var test = await e.Guild.GetInvitesAsync();
            var interactivity = e.Client.GetInteractivity();

            if (numSides > 0 && numRolls > 0)
            {
                string name = "";
                if (e.Member.Nickname != null) name = e.Member.Nickname;
                else name = e.User.Username;

                int total = 0;
                string ans = name + " rolled: (";
                for (int i = 0; i < numRolls; i++)
                {
                    int roll = RPClass.Random.Next(1, numSides + 1);
                    total += roll;
                    ans += roll + "+";
                }
                ans = ans.TrimEnd('+') + ") = " + total;
                if (ans.Length > 1000)
                {
                    ans = name + " rolled: " + total;
                }
                await e.RespondAsync(ans);
            }
        }

        [Command("choose"), Description("Command to choose one of the variables given."), IsMuted]
        public async Task Choose(CommandContext e, [Description("List of variables separated by commas.")] string choiceList)
        {
            string[] Choices = choiceList.Split(',');
            int randomChoice = RPClass.Random.Next(0, Choices.Length);
            await e.RespondAsync("Hmm. I choose... " + Choices[randomChoice]);
        }
        
        [Command("hackban"), Description("Admin cases command."), RequireRoles(RoleCheckMode.Any, "Administrator"), IsMuted]
        public async Task HackBan(CommandContext e,[Description("User ID")] ulong userId)
        {

            await e.Guild.BanMemberAsync(userId, 7, "");

            await e.RespondAsync($"Hackbanned ID: {userId}");
        }

        [Command("name"), Description("Command for users to change their RP name temporarily"), RequireRoles(RoleCheckMode.Any, "Staff"), IsMuted]
        public async Task Name(CommandContext e, [Description("What to call yourself")] string name = "")
        {
            DiscordMessage x;
            if (name == "off")
            {
                SpeechObject.RootObject savedName = RPClass.SpeechList.FirstOrDefault(y => y.Id == e.Member.Id);
                if (savedName != null)
                {
                    RPClass.SpeechList.Remove(savedName);
                    x = await e.RespondAsync("Removed from list.");
                }
                else
                {
                    x = await e.RespondAsync("");
                }
                await Task.Delay(2000);
                await e.Message.DeleteAsync();
                await x.DeleteAsync();

            }
            else if (name != "")
            {
                SpeechObject.RootObject savedName = RPClass.SpeechList.FirstOrDefault(y => y.Id == e.Member.Id);
                if (savedName != null)
                {
                    RPClass.SpeechList.Remove(savedName);
                }
                RPClass.SpeechList.Add(new SpeechObject.RootObject(e.Member.Id, name));
                x = await e.RespondAsync("Name changed.");
                await Task.Delay(2000);
                await e.Message.DeleteAsync();
                await x.DeleteAsync();
            }
            else
            {
                x = await e.RespondAsync("Specify a name.");
                await Task.Delay(2000);
                await e.Message.DeleteAsync();
                await x.DeleteAsync();
            }
        }

        [Command("serverinfo"), Description("Gets info about current server"), IsMuted]
        public async Task GuildInfo(CommandContext ctx)
        {
            var members = await ctx.Guild.GetAllMembersAsync();
            var b = new DiscordEmbedBuilder()
            {
                Title = $"{ctx.Guild.Name}",
                Description = $"Created {ctx.Guild.CreationTimestamp.ToString("dddd, d MMMM yyyy")} - over {(DateTime.Now - ctx.Guild.CreationTimestamp.DateTime).Days} days ago!",
                ThumbnailUrl = ctx.Guild.IconUrl,
                Color = new DiscordColor("4169E1")
            }
            .WithFooter($"Server ID: {ctx.Guild.Id.ToString()}")
            .AddField("Text Channels", $"{ctx.Guild.Channels.Where(x => x.Type == ChannelType.Text).Count()}", true)
            .AddField("Voice Channels", $"{ctx.Guild.Channels.Where(x => x.Type == ChannelType.Voice).Count()}", true)
            .AddField("Roles", $"{ctx.Guild.Roles.Count}", true)
            .AddField("Users", $"{members.Count}", true)
            .AddField("Region", $"{ctx.Guild.VoiceRegion.Name}", true)
            .AddField("Owner", $"{ctx.Guild.Owner.DisplayName}#{ctx.Guild.Owner.Discriminator}", true)
            .AddField("Icon Url", ctx.Guild.IconUrl, false);

            await ctx.RespondAsync("", embed: b.Build());
        }

        [Command("userinfo"), Description("Gets info about user"), IsMuted]
        public async Task UserInfo(CommandContext ctx, [Description("User to get info about")] DiscordMember m = null)
        {
            if (m == null)
                m = ctx.Member;
            DateTime JoinDate = m.JoinedAt.DateTime;

            if (m.Id == 126070623855312896)
                JoinDate = new DateTime(2017, 5, 13, 14, 11, 19);
            else if (m.Id == 242720599158423554)
                JoinDate = ctx.Guild.CreationTimestamp.DateTime;
            else if (m.Id == 286222030997684226)
                JoinDate = new DateTime(2017, 10, 11, 19, 12, 32);
            else if (m.Id == 221576298743595009)
                JoinDate = new DateTime(2017, 11, 24, 18, 2, 13);
            var b = new DiscordEmbedBuilder()
            {
                Title = $"{m.DisplayName}#{m.Discriminator}",
                ThumbnailUrl = m.AvatarUrl ?? m.DefaultAvatarUrl,
                Color = new DiscordColor("4169E1")
            }
            .WithFooter($"User ID:{m.Id}")
            .AddField("Joined Discord on: ", $"{m.CreationTimestamp.ToString("dd MMM yyyy H:mm")} \n({DateTimeOffset.Now.Subtract(m.CreationTimestamp).Days} days ago)", true)
            .AddField("Joined this Server on: ", $"{JoinDate.ToString("dd MMM yyyy H:mm")}\n({DateTime.Now.Subtract(JoinDate).Days} days ago)", true)
            .AddField("Roles: ", string.Join(", ", m.Roles.Select(x => x.Name)), true);

            await ctx.RespondAsync("", embed: b.Build());
        }

        [Group("purge", CanInvokeWithoutSubcommand = true), Aliases("p"), RequireRoles(RoleCheckMode.Any, "Staff"), IsMuted]
        class Purge : BaseCommandModule
        {
            [Description("Delete an amount of messages from the current channel.")]
            public async Task ExecuteGroupAsync(CommandContext ctx, [Description("Amount of messages to remove (max 100)")]int limit = -1,
                [Description("Amount of messages to skip")]int skip = 0)
            {
                if (limit == -1)
                {
                    await ctx.RespondAsync("NO. GOD. NO.");
                    return;
                }
                var i = 0;
                var ms = await ctx.Channel.GetMessagesAsync(limit);
                var deletThis = new List<DiscordMessage>();
                foreach (var m in ms)
                {
                    if (i < skip)
                        i++;
                    else
                        deletThis.Add(m);
                }
                if (deletThis.Any())
                    await ctx.Channel.DeleteMessagesAsync(deletThis);
                var resp = await ctx.RespondAsync("Latest messages deleted.");
                await Task.Delay(2000);
                await resp.DeleteAsync();

                string paste = "";
                deletThis.Reverse();
                foreach (var m in deletThis)
                {
                    paste += (m.Author as DiscordMember).DisplayName + ": " + m.Content + " \r\n";
                }
                await PurgeLog(ctx, paste);
            }

            [Command("from"), Description("Delete an amount of messages from a specified message"), Aliases("f", "fr")]
            public async Task PurgeFromAsync(CommandContext ctx, [Description("Message to delete from")]DiscordMessage message,
            [Description("Amount of messages to remove (max 100)")]int limit = 50)
            {
                if (limit == -1)
                {
                    await ctx.RespondAsync("NO. GOD. NO.");
                    return;
                }
                var ms = await ctx.Channel.GetMessagesBeforeAsync(message.Id, limit);
                await ctx.Channel.DeleteMessagesAsync(ms);
                await Task.Delay(2000);
                await ctx.Message.DeleteAsync();
                ms.Reverse();

                string paste = "";
                foreach (var m in ms)
                {
                    paste += (m.Author as DiscordMember).DisplayName + ": " + m.Content + "\r\n";
                }
                await PurgeLog(ctx, paste);
            }

            [Command("fromto"), Description("Delete all messages between two specified messages")]
            public async Task PurgeFromToAsync(CommandContext ctx, [Description("Message to delete from (closest message)")]DiscordMessage from,
                [Description("Message to delete to (furthest message)")]DiscordMessage to)
            {
                var deletThis = new List<DiscordMessage>();
                var ms = (await ctx.Channel.GetMessagesBeforeAsync(from.Id, 100));
                bool found = false;
                while (true)
                {
                    foreach (var m in ms)
                    {
                        deletThis.Add(m);
                        if (m == to)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (deletThis.Count > 1000)
                    {
                        break;
                    }
                    if (found == true)
                        break;
                    ms = await ctx.Channel.GetMessagesBeforeAsync(deletThis.Last().Id, 100);

                }
                if (found)
                {
                    var messageListSplit = Extensions.Split(deletThis, 100);
                    foreach (var messageList in messageListSplit)
                    {
                        await ctx.Channel.DeleteMessagesAsync(messageList);
                        await Task.Delay(500);
                    }
                    await ctx.Channel.DeleteMessageAsync(from);
                }
                else
                {
                    await ctx.RespondAsync("Second message not found.");
                }
                await Task.Delay(2000);
                await ctx.Message.DeleteAsync();
                deletThis.Reverse();

                string paste = "";
                foreach (var m in deletThis)
                {
                    paste += (m.Author as DiscordMember).DisplayName + ": " + m.Content + "\r\n";
                }
                await PurgeLog(ctx, paste);
            }

            [Command("user"), Description("Delete an amount of messages by an user."), Aliases("u", "pu")]
            public async Task PurgeUserAsync(CommandContext ctx, [Description("User to delete messages from")]DiscordUser user,
            [Description("Amount of messages to remove (max 100)")]int limit = 50, [Description("Amount of messages to skip")]int skip = 0)
            {
                if (limit == -1)
                {
                    await ctx.RespondAsync("NO. GOD. NO.");
                    return;
                }
                var i = 0;
                var ms = await ctx.Channel.GetMessagesAsync(limit);
                var deletThis = new List<DiscordMessage>();
                foreach (var m in ms)
                {
                    if (user != null && m.Author.Id != user.Id) continue;
                    if (i < skip)
                        i++;
                    else
                        deletThis.Add(m);
                }
                if (deletThis.Any())
                    await ctx.Channel.DeleteMessagesAsync(deletThis);
                var resp = await ctx.RespondAsync($"Latest messages by {user?.Mention} (ID:{user?.Id}) deleted.");
                await Task.Delay(2000);
                await resp.DeleteAsync();
                deletThis.Reverse();

                string paste = "";
                foreach (var m in deletThis)
                {
                    paste += (m.Author as DiscordMember).DisplayName + ": " + m.Content + "\r\n";
                }
                await PurgeLog(ctx, paste);
            }

            [Command("commands"), Description("Purge RPBot's messages."), Aliases("c", "self", "own", "clean")]
            public async Task CleanAsync(CommandContext ctx)
            {
                var ms = await ctx.Channel.GetMessagesAsync();
                var deletThis = ms.Where(m => m.Author.Id == ctx.Client.CurrentUser.Id || m.Content.StartsWith("!"))
                    .ToList();
                if (deletThis.Any())
                    await ctx.Channel.DeleteMessagesAsync(deletThis);
                var resp = await ctx.RespondAsync("Latest messages deleted.");
                await Task.Delay(2000);
                await resp.DeleteAsync();
                deletThis.Reverse();

                string paste = "";
                foreach (var m in deletThis)
                {
                    paste += (m.Author as DiscordMember).DisplayName + ": " + m.Content + "\r\n";
                }
                await PurgeLog(ctx, paste);
            }

            [Command("bots"), Description("Purge messages from all bots in this channel"), Aliases("b", "bot")]
            public async Task PurgeBotsAsync(CommandContext ctx)
            {
                var ms = await ctx.Channel.GetMessagesAsync();
                var deletThis = ms.Where(m => m.Author.IsBot || m.Content.StartsWith("!"))
                    .ToList();
                if (deletThis.Any())
                    await ctx.Channel.DeleteMessagesAsync(deletThis);
                var resp = await ctx.RespondAsync("Latest messages deleted.");
                await Task.Delay(2000);
                await resp.DeleteAsync();
                deletThis.Reverse();

                string paste = "";
                foreach (var m in deletThis)
                {
                    paste += (m.Author as DiscordMember).DisplayName + ": " + m.Content + "\r\n";
                }
                await PurgeLog(ctx, paste);
            }

            [Command("images"), Description("Purge messages with images or attachments on them."), Aliases("i", "imgs", "img")]
            public async Task PurgeImagesAsync(CommandContext ctx)
            {
                var ms = await ctx.Channel.GetMessagesAsync();
                Regex ImageRegex = new Regex(@"\.(png|gif|jpg|jpeg|tiff|webp)");
                var deleteThis = ms.Where(m => ImageRegex.IsMatch(m.Content) || m.Attachments.Any()).ToList();
                if (deleteThis.Any())
                    await ctx.Channel.DeleteMessagesAsync(deleteThis);
                var resp = await ctx.RespondAsync("Latest messages deleted.");
                await Task.Delay(2000);
                await resp.DeleteAsync();
                deleteThis.Reverse();

                string paste = "";
                foreach (var m in deleteThis)
                {
                    paste += (m.Author as DiscordMember).DisplayName + ": " + m.Content + "\r\n";
                }
                await PurgeLog(ctx, paste);
            }
        }

        [Command("restart"),  Description("Admin restart command"), RequireRoles(RoleCheckMode.Any, "Staff"), IsMuted]
        public async Task Restart(CommandContext e)
        {
            RPClass.SaveData(-1);
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"dotnet RPBot-Core.dll\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            Environment.Exit(-1);
            await Task.Delay(0);
        }

        [Command("update"), Description("Admin update command"), RequireOwner]
        public async Task Update(CommandContext e)
        {
            await e.RespondAsync("Restarting. Wish me luck!");
            RPClass.SaveData(-1);
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"bash update.sh\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            Environment.Exit(-1);
            await Task.Delay(0);
        }

        [Command("joke"), Description("Random joke command."), IsMuted]
        public async Task Joke(CommandContext e)
        {
            using (HttpClient client = new HttpClient())
            {
                string response = await client.GetStringAsync("https://icanhazdadjoke.com/slack");
                dynamic obj = JObject.Parse(response);
                await e.RespondAsync(obj.Attachments[0].Text);
            }
        }

        [Command("space"),  Description("Spaces text out"), IsMuted]
        public async Task Space(CommandContext e, [Description("How many spaces between each char")] int space, [RemainingText, Description("What to say?")] string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                await e.RespondAsync("You didn't give anything to say!");
                return;
            }
            if (space < 1 || space > 10)
            {
                await e.RespondAsync("Cancerous amount of spaces, idiot.");
                return;
            } 
            string retVal = "";
            text = text.Replace(" ", "");
            foreach (char c in text)
            {
                retVal += c;
                for (int i = 0; i < space; i++)
                {
                    retVal += " ";
                }
            }
            await e.RespondAsync(retVal);
        }

        [Command("say"), Description("Tell the bot what to say"), RequireRoles(RoleCheckMode.Any, "Staff", "Game Masters"), IsMuted]
        public async Task Say(CommandContext e, [RemainingText, Description("What to say?")] string text)
        {
            await e.RespondAsync(text);

            DiscordEmbedBuilder b = new DiscordEmbedBuilder
            {
                Title = "Say Executed",
                Color = DiscordColor.CornflowerBlue
            }
            .AddField("By", e.Message.Author.Username + "#" + e.Message.Author.Discriminator + " (" + e.Message.Author.Id + ")", true)
            .AddField("Channel", e.Message.Channel.Name, true)
            .AddField("Message", e.Message.Content, false);

            await e.Guild.GetChannel(392429153909080065).SendMessageAsync(embed: b.Build());
            await e.Message.DeleteAsync();
        }

        [Command("edit"), Description("Tell the bot what to say"), RequireOwner, IsMuted]
        public async Task Edit(CommandContext e, string messageID, [RemainingText, Description("What to say?")] string text)
        {
            var message = await e.Channel.GetMessageAsync(ulong.Parse(messageID));
            await message.ModifyAsync(text);

            DiscordEmbedBuilder b = new DiscordEmbedBuilder
            {
                Title = "Edit Executed",
                Color = DiscordColor.CornflowerBlue
            }
            .AddField("By", e.Message.Author.Username + "#" + e.Message.Author.Discriminator + " (" + e.Message.Author.Id + ")", true)
            .AddField("Channel", e.Message.Channel.Name, true)
            .AddField("Message", message.Content, false);

            await e.Guild.GetChannel(392429153909080065).SendMessageAsync(embed: b.Build());
            await e.Message.DeleteAsync();
        }


        [Command("embed"), Description("Allows you to make the bot say messages in an embed\n**Usage:**\n`!embed <Title>:<Description>:<Field 1 Title>:<Field 1 Description>:` etc."), RequireRoles(RoleCheckMode.Any, "Staff"), IsMuted]
        public async Task Embed(CommandContext e, [RemainingText, Description("What to say?")] string text)
        {
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder()
            {
                Color = DiscordColor.CornflowerBlue,
                Timestamp = DateTime.Now
            };
            string[] textSplit = text.Split(':');

            embed.Title = !string.IsNullOrWhiteSpace(textSplit[0]) ? textSplit[0] : "N/A";
            embed.Description = !string.IsNullOrWhiteSpace(textSplit[1]) ? textSplit[0] : "N/A";

            bool first = true;
            foreach (string embedInfo in textSplit.Skip(2))
            {
                if (first) 
                    embed.AddField(embedInfo, "N/A");
                else
                    embed.Fields.Last().Value = embedInfo;
                first = !first;
            }
            await e.RespondAsync(embed: embed);
            await e.Message.DeleteAsync();
        }

        [Command("sayall"), Description("Makes the bot delete all messages in a channel (the channel the command is used in) and repost them."), RequireRoles(RoleCheckMode.Any, "Administrator"), IsMuted]
        public async Task SayAll(CommandContext e, string whattodelete = "", string whattosetto = "")
        {
            await e.Message.DeleteAsync();
            List<DiscordMessage> messageList = new List<DiscordMessage>();

            int iter = 1;
            messageList.AddRange(await e.Channel.GetMessagesAsync());
            while (true)
            {
                messageList.AddRange(await e.Channel.GetMessagesBeforeAsync(messageList.Last().Id, 100));

                if (messageList.Count != (100 * iter))
                {
                    break;
                }
                iter++;
            }
            messageList.Reverse();
            
            foreach (DiscordMessage m in messageList)
            {
                string ret = m.Content;

                if (!(whattodelete == "" && whattosetto == ""))
                {
                    if (ret.Contains(whattodelete))
                    {
                        ret = ret.Replace(whattodelete, whattosetto);
                    }
                }
                await e.RespondAsync(ret);
            }
            await e.Channel.DeleteMessagesAsync(messageList);

        }

        [Command("removeuser"), Description("Makes the bot delete a user that has left the server (Name in statsheets, copied exactly)."), RequireRoles(RoleCheckMode.Any, "Staff"), IsMuted]
        public async Task RemoveUser(CommandContext e, [RemainingText] string whotodelete = "")
        {
            if (RPClass.Users.Any(x => x.UserData.Username == whotodelete))
            {
                UserObject.RootObject user = RPClass.Users.First(x => x.UserData.Username == whotodelete);

                if (RPClass.Guilds.Any(x => x.Id == user.UserData.GuildID))
                {
                    RPClass.Guilds.First(x => x.Id == user.UserData.GuildID).UserIDs.Remove(user.UserData.UserID);
                }
                RPClass.Users.Remove(user);
                RPClass.SaveData(-1);
                await e.RespondAsync("User removed.");
            }
            else
            {
                await e.RespondAsync("No user with that name found.");
            }
        }

        [Command("giverole"), RequireOwner]
        public async Task GiveRole(CommandContext e, DiscordMember member, DiscordRole role)
        {
            await member.GrantRoleAsync(role);
        }

        [Command("takerole"), RequireOwner]
        public async Task TakeRole(CommandContext e, DiscordMember member, DiscordRole role)
        {
            await member.RevokeRoleAsync(role);
        }

        [Command("rip"), RequireOwner] 
        public async Task RIP(CommandContext e)
        {
            List<DiscordMessage> messageList = new List<DiscordMessage>();

            int iter = 1;
            messageList.AddRange(await e.Channel.GetMessagesAsync(100));
            while (true)
            {
                if (iter % 10 == 0)
                {
                    await e.Guild.GetChannel(404108476835430401).SendMessageAsync($"iter: {iter}");
                }

                messageList.AddRange(await e.Channel.GetMessagesBeforeAsync(messageList.Last().Id, 100));
                iter++;

                if (messageList.Count != (100 * iter))
                {
                    break;
                }
            }

            messageList.Reverse();
            File.WriteAllText($"stuff/{e.Channel.Name}.json", JsonConvert.SerializeObject(messageList));
            await e.RespondAsync("lol done.");
        }
        
        [Group("approval"), Description("Approval commands"), IsMuted]
        class ApprovalClass : BaseCommandModule
        {
            [Command("add"), Description("Command to create a new approval instance."), RequireRoles(RoleCheckMode.Any, "Staff")]
            public async Task AddApproval(CommandContext e, [Description("Mention the user you will be approving.")]DiscordMember m)
            {
                Regex rgx = new Regex("[^a-zA-Z0-9-]");
                string name = rgx.Replace(m.DisplayName, "");
                DiscordChannel c = await e.Guild.CreateChannelAsync(name, ChannelType.Text, parent: RPClass.ApprovalsCategory);
                await c.AddOverwriteAsync(m, Permissions.SendMessages, Permissions.None);
                await c.AddOverwriteAsync(e.Guild.EveryoneRole, Permissions.ReadMessageHistory, Permissions.SendMessages);


                var role = e.Guild.GetRole(312961821063512065);
                await c.AddOverwriteAsync(role, Permissions.SendMessages | Permissions.ManageMessages, Permissions.None);

                await e.RespondAsync("Channel created!\n" + c.Mention);
            }
        }

        [Group("emoji", CanInvokeWithoutSubcommand = true), Aliases("e"), Description("Emoji commands"), IsMuted]
        class EmojiClass : BaseCommandModule
        {
            public async Task ExecuteGroupAsync(CommandContext e, [RemainingText] string emoji)
            {
                
                await e.RespondAsync(DiscordEmoji.FromName(e.Client, ":" + emoji + ":"));
            }

            [Command("bee"), Description("BEE MOVIE!"), RequireRoles(RoleCheckMode.Any, "Administrator")]
            public async Task Bee(CommandContext e, [RemainingText]string emoji)
            {
                string response = "";
                string[] emojiLines = emoji.Split("\u000A");
                foreach (string emojiLine in emojiLines)
                {
                    Console.WriteLine(emojiLine);
                    string emojiLineEdited = emojiLine.Replace("::", ":").TrimStart(':').TrimEnd(':');
                    string[] emojis = emojiLineEdited.Split(':');
                    foreach (string emote in emojis)
                    {
                        Console.WriteLine(emote);
                        response += DiscordEmoji.FromName(e.Client, ":" + emote + ":");
                    }
                    response += "\n";
                }
                await e.RespondAsync(response);
            }
            [Command("list"), Description("List of all emoji!")]
            public async Task JoinList(CommandContext e)
            {
                var interactivity = e.Client.GetInteractivity();
                List<Page> interactivityPages = new List<Page>();

                Page p = new Page();

                DiscordEmbedBuilder b = new DiscordEmbedBuilder()
                {
                    Color = new DiscordColor("4169E1"),
                    Timestamp = DateTime.UtcNow
                }
                .WithFooter("Heroes & Villains");
                bool even = false;
                foreach (DiscordGuild g in e.Client.Guilds.Values)
                {
                    even = false;
                    b.Title = g.Name;
                    foreach (DiscordEmoji d in g.Emojis)
                    {
                        if (!even)
                        {
                            b.AddField(DiscordEmoji.FromName(e.Client, d.GetDiscordName()) + " - " + d.Name, "-");
                        }
                        else
                        {
                            b.Fields.Last().Value = DiscordEmoji.FromName(e.Client, d.GetDiscordName()) + " - " + d.Name;
                        }
                        even = !even;
                        if (b.Fields.Count >= 20)
                        {
                            p.Embed = b;
                            interactivityPages.Add(p);
                            p = new Page();
                            b.ClearFields();
                            even = false;
                        }
                    }
                    p.Embed = b;
                    interactivityPages.Add(p);
                    p = new Page();
                    b.ClearFields();
                }

                await interactivity.SendPaginatedMessage(e.Channel, e.Member, interactivityPages, timeoutoverride: TimeSpan.FromSeconds(60));

            }

        }

        public static async Task PurgeLog(CommandContext e, string messages)
        {
            DiscordEmbedBuilder b = new DiscordEmbedBuilder
            {
                Title = $"Messages Purged",
                Color = DiscordColor.Red
            }
            .AddField("By", e.Message.Author.Username + "#" + e.Message.Author.Discriminator + " (" + e.Message.Author.Id + ")", true)
            .AddField("Channel", e.Message.Channel.Name, true);
            string fileName = DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss");
            File.WriteAllText($"{fileName}.txt", messages);

            await e.Guild.GetChannel(392429153909080065).SendFileAsync($"{fileName}.txt", embed: b.Build());
            File.Delete(fileName + ".txt");
        }

    }
}
