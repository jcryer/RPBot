﻿using System;
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
using DSharpPlus.Entities;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Reflection;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace RPBot
{
    class CommandsClass : BaseCommandModule
    {
        [Command("roll"), Description("Dice roll command!")]
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

        [Command("choose"), Description("Command to choose one of the variables given.")]
        public async Task Choose(CommandContext e, [Description("List of variables separated by commas.")] string choiceList)
        {
            string[] Choices = choiceList.Split(',');
            int randomChoice = RPClass.Random.Next(0, Choices.Length);
            await e.RespondAsync("Hmm. I choose... " + Choices[randomChoice]);
        }

        [Group("slowmode"), Description("Slowmode commands")]
        class Slowmode
        {
            [Command("on"), Description("Admin command to make OOC chill tf out"), RequireRoles(RoleCheckMode.Any, "Administrator")]
            public async Task On(CommandContext e, [Description("Amount of time required between each message (seconds)")] int limitTime)
            {
                RPClass.slowModeTime = limitTime;
                await e.RespondAsync("Slowmode activated, with " + limitTime + " seconds between each message.");
            }

            [Command("off"), Description("Admin command to disable slow mode"), RequireRoles(RoleCheckMode.Any, "Administrator")]
            public async Task Off(CommandContext e)
            {
                RPClass.slowModeTime = -1;
                await e.RespondAsync("Slowmode disabled.");
            }
        }
        [Command("cases"), Description("Admin cases command."), RequireRoles(RoleCheckMode.Any, "Staff")]
        public async Task Cases(CommandContext e, [Description("Select a user.")] DiscordMember user, [Description("Number to increase or decrease cases resolved by")] string caseNum)
        {
            RPClass.Users.First(x => x.UserData.UserID == user.Id).UserData.ResolvedCases += int.Parse(caseNum);
            if (RPClass.Users.First(x => x.UserData.UserID == user.Id).UserData.ResolvedCases < 0)
                RPClass.Users.First(x => x.UserData.UserID == user.Id).UserData.ResolvedCases = 0;

            if (RPClass.Users.First(x => x.UserData.UserID == user.Id).UserData.Role == 1) await XPClass.UpdatePlayerRanking(e.Guild, 1);
            else if (RPClass.Users.First(x => x.UserData.UserID == user.Id).UserData.Role == 2) await XPClass.UpdatePlayerRanking(e.Guild, 2);
            else if (RPClass.Users.First(x => x.UserData.UserID == user.Id).UserData.Role == 3) await XPClass.UpdatePlayerRanking(e.Guild, 3);
            else if (RPClass.Users.First(x => x.UserData.UserID == user.Id).UserData.Role == 4) await XPClass.UpdatePlayerRanking(e.Guild, 4);

            await XPClass.UpdateGuildRanking(e.Guild);

            RPClass.SaveData(1);

            await e.RespondAsync("Cases updated.");
        }

        [Command("crimes"), Description("Admin cases command."), RequireRoles(RoleCheckMode.Any, "Staff")]
        public async Task Crimes(CommandContext e, [Description("Select a user.")] DiscordMember user, [Description("Number to increase or decrease crimes committed by")] string crimeNum)
        {
            RPClass.Users.First(x => x.UserData.UserID == user.Id).UserData.CrimesCommitted += int.Parse(crimeNum);
            if (RPClass.Users.First(x => x.UserData.UserID == user.Id).UserData.CrimesCommitted < 0)
                RPClass.Users.First(x => x.UserData.UserID == user.Id).UserData.CrimesCommitted = 0;

            if (RPClass.Users.First(x => x.UserData.UserID == user.Id).UserData.Role == 1) await XPClass.UpdatePlayerRanking(e.Guild, 1);
            else if (RPClass.Users.First(x => x.UserData.UserID == user.Id).UserData.Role == 2) await XPClass.UpdatePlayerRanking(e.Guild, 2);
            else if (RPClass.Users.First(x => x.UserData.UserID == user.Id).UserData.Role == 3) await XPClass.UpdatePlayerRanking(e.Guild, 3);
            else if (RPClass.Users.First(x => x.UserData.UserID == user.Id).UserData.Role == 4) await XPClass.UpdatePlayerRanking(e.Guild, 4);

            await XPClass.UpdateGuildRanking(e.Guild);

            RPClass.SaveData(1);

            await e.RespondAsync("Crimes updated.");
        }

        [Command("name"), Description("Command for users to change their RP name temporarily"), RequireRoles(RoleCheckMode.Any, "Staff")]
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
        [Command("json"), Description("Admin json file command")]
        public async Task Json(CommandContext e)
        {
            RPClass.SaveData(1);
            await e.RespondWithFileAsync("UserData.txt", "Json file of all user data!\nRoles: 1 = Hero, 2 = Villain, 3 = Rogue\nStatus: 1 = Alive, 2 = Dead");
        }

        [Command("sudo"), Description("Execute a command as if you're another user"), RequireRoles(RoleCheckMode.Any, "Staff")]
        public async Task SudoAsync(CommandContext e, [Description("User to Sudo")]DiscordUser user, [RemainingText, Description("Command to execute")] string command = "help")
        {
            await e.CommandsNext.SudoAsync(user, e.Channel, command);
        }

        [Command("serverinfo"), Description("Gets info about current server")]
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

        [Command("userinfo"), Description("Gets info about user")]
        public async Task UserInfo(CommandContext ctx, [Description("User to get info about")] DiscordMember m = null)
        {
            if (m == null)
                m = ctx.Member;
            DateTime JoinDate = m.JoinedAt.DateTime;

            if (m.Id == 126070623855312896)
                JoinDate = new DateTime(2017, 5, 13, 14, 11, 19);
            else if (m.Id == 242720599158423554)
                JoinDate = ctx.Guild.CreationTimestamp.DateTime;
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

        [Group("purge", CanInvokeWithoutSubcommand = true), Aliases("p"), RequireRoles(RoleCheckMode.Any, "Staff")]
        class Purge
        {
            [Description("Delete an amount of messages from the current channel.")]
            public async Task ExecuteGroupAsync(CommandContext ctx, [Description("Amount of messages to remove (max 100)")]int limit = 50,
                [Description("Amount of messages to skip")]int skip = 0)
            {
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
                await ctx.Message.DeleteAsync();
            }

            [Command("from"), Description("Delete an amount of messages from a specified message"), Aliases("f", "fr")]
            public async Task PurgeFromAsync(CommandContext ctx, [Description("Message to delete from")]DiscordMessage message,
            [Description("Amount of messages to remove (max 100)")]int limit = 50)
            {
                var ms = await ctx.Channel.GetMessagesBeforeAsync(message.Id, limit);
                await ctx.Channel.DeleteMessagesAsync(ms);
                await Task.Delay(2000);
                await ctx.Message.DeleteAsync();
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
            }

            [Command("user"), Description("Delete an amount of messages by an user."), Aliases("u", "pu")]
            public async Task PurgeUserAsync(CommandContext ctx, [Description("User to delete messages from")]DiscordUser user,
            [Description("Amount of messages to remove (max 100)")]int limit = 50, [Description("Amount of messages to skip")]int skip = 0)
            {
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
                await ctx.Message.DeleteAsync();
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
                await ctx.Message.DeleteAsync();
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
                await ctx.Message.DeleteAsync();
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
                await ctx.Message.DeleteAsync();
            }
        }

        [Command("restart"),  Description("Admin restart command"), RequireRoles(RoleCheckMode.Any, "Staff")]
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

        [Command("update"), Description("Admin update command"), RequireRoles(RoleCheckMode.Any, "Administrator")]
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

        [Command("joke"), Description("Random joke command.")]
        public async Task Joke(CommandContext e)
        {
            using (HttpClient client = new HttpClient())
            {
                string response = await client.GetStringAsync("https://icanhazdadjoke.com/slack");
                dynamic obj = JObject.Parse(response);
                await e.RespondAsync(obj.Attachments[0].Text);
            }
        }

        [Command("space"),  Description("Spaces text out")]
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

        [Command("say"), Description("Tell the bot what to say"), RequirePermissions(Permissions.ManageChannels)]
        public async Task Say(CommandContext e, [RemainingText, Description("What to say?")] string text)
        {
            await e.RespondAsync(text);
            await e.Message.DeleteAsync("test");
        }


        [Command("embed"), Description("Allows you to make the bot say messages in an embed\n**Usage:**\n`!embed <Title>:<Description>:<Field 1 Title>:<Field 1 Description>:` etc."), RequirePermissions(Permissions.ManageChannels)]
        public async Task Embed(CommandContext e, [RemainingText, Description("What to say?")] string text)
        {
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder()
            {
                Color = new DiscordColor("4169E1"),
                Timestamp = DateTime.UtcNow,
                ThumbnailUrl = e.Client.CurrentUser.AvatarUrl
            }
            .WithFooter("Heroes & Villains");

            string[] textSplit = text.Split(':');

            embed.Title = !string.IsNullOrWhiteSpace(textSplit[0]) ? textSplit[0] : "N/A";
            embed.Description = !string.IsNullOrWhiteSpace(textSplit[1]) ? textSplit[1] : "N/A";
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

        [Command("sayall"), Description("Makes the bot delete all messages in a channel (the channel the command is used in) and repost them."), RequirePermissions(Permissions.Administrator)]
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

      

        [Command("removeuser"), Description("Makes the bot delete a user that has left the server (Name in statsheets, copied exactly)."), RequireRoles(RoleCheckMode.Any, "Staff")]
        public async Task RemoveUser(CommandContext e, [RemainingText] string whotodelete = "")
        {
            if (RPClass.Users.Any(x => x.UserData.Username == whotodelete))
            {
                UserObject.RootObject user = RPClass.Users.First(x => x.UserData.Username == whotodelete);
                RPClass.Users.Remove(user);
                RPClass.Guilds.First(x => x.Id == user.UserData.GuildID).UserIDs.Remove(user.UserData.UserID);
                RPClass.SaveData(-1);
                await e.RespondAsync("User removed.");
            }
            else
            {
                await e.RespondAsync("No user with that name found.");
            }
        }

        [Command("joinlist"), Description("List of who joined when.")]
        public async Task JoinList(CommandContext e)
        {
            var interactivity = e.Client.GetInteractivity();
            List<Page> interactivityPages = new List<Page>();

            var members = await e.Guild.GetAllMembersAsync();
            members = members.OrderBy(x => x.JoinedAt).ToList();
            Page p = new Page();
            DiscordEmbedBuilder b = new DiscordEmbedBuilder()
            {
                Color = new DiscordColor("4169E1"),
                Timestamp = DateTime.UtcNow
            }
            .WithFooter("Heroes & Villains");

            for (int i = 1; i <= members.Count(); i++)
            {
                b.Description += i + ": " + members[i-1].DisplayName + " - " + members[i-1].JoinedAt.ToString("dd MMMM yyyy (H:mm:ss)") + "\n";
                if (i % 10 == 0 && i != 0)
                {
                    p.Embed = b;
                    interactivityPages.Add(p);
                    p = new Page();
                    b.ClearFields();
                }
            }

            await interactivity.SendPaginatedMessage(e.Channel, e.Member, interactivityPages, timeoutoverride: TimeSpan.FromSeconds(60));
            
        }

        [Group("approval"), Description("Approval commands")]
        class ApprovalClass
        {
            [Command("add"), Description("Command to create a new approval instance."), RequireRoles(RoleCheckMode.Any, "Staff")]
            public async Task AddApproval(CommandContext e, [Description("Mention the user you will be approving.")]DiscordMember m)
            {
                Regex rgx = new Regex("[^a-zA-Z0-9-]");
                string name = rgx.Replace(m.DisplayName, "");
                DiscordChannel c = await e.Guild.CreateChannelAsync(name, ChannelType.Text, parent: RPClass.ApprovalsCategory);
                await c.AddOverwriteAsync(m, Permissions.SendMessages, Permissions.None);
                await c.AddOverwriteAsync(e.Guild.EveryoneRole, Permissions.ReadMessageHistory, Permissions.SendMessages);

                RPClass.approvalsList.Add(c.Id, m.Id);

                RPClass.SaveData(8);
                await e.RespondAsync("Approval instance created.");
            }

            [Command("remove"), Description("Command to remove an approval instance. Execute this command in the instance you wish to remove, or mention the user the approval instance is for."), RequireRoles(RoleCheckMode.Any, "Staff")]
            public async Task RemoveApproval(CommandContext e, [Description("Mention the user the approval instance is for, or execute the command in the instance you wish to remove.")] DiscordMember m = null)
            {

                if (m != null)
                {
                    if (RPClass.approvalsList.ContainsValue(m.Id))
                    {
                        DiscordChannel d = e.Guild.GetChannel(RPClass.approvalsList.First(x => x.Value == m.Id).Key);
                        RPClass.approvalsList.Remove(d.Id);
                        await d.DeleteAsync();
                    }
                }
                else
                {
                    if (RPClass.approvalsList.ContainsKey(e.Channel.Id))
                    {
                        await e.Channel.DeleteAsync();
                        RPClass.approvalsList.Remove(e.Channel.Id);
                    }
                }
                RPClass.SaveData(8);
            }
        }

        [Group("emoji", CanInvokeWithoutSubcommand = true), Aliases("e"), Description("Approval commands")]
        class EmojiClass
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
        
    }
}
