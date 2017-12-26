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
using DSharpPlus.Entities;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Reflection;

namespace RPBot
{
    class CommandsClass : RPClass
    {
        [Command("roll"), Description("Dice roll command!")]
        public async Task Roll(CommandContext e, [Description("Number of sides of the dice")] int numSides = 0, [Description("Number of rolls to do")] int numRolls = 0)
        {
            var interactivity = e.Client.GetInteractivityModule();

            if (numSides > 0 && numRolls > 0)
            {
                string name = "";
                if (e.Member.Nickname != null) name = e.Member.Nickname;
                else name = e.User.Username;

                int total = 0;
                string ans = name + " rolled: (";
                for (int i = 0; i < numRolls; i++)
                {
                    int roll = random.Next(1, numSides + 1);
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
            int randomChoice = random.Next(0, Choices.Length);
            await e.RespondAsync("Hmm. I choose... " + Choices[randomChoice]);
        }

        [Group("slowmode"), Description("Slowmode commands")]
        class Slowmode
        {
            [Command("on"), Description("Admin command to make OOC chill tf out"), RequireRolesAttribute("Administrator", "Bot-Test")]
            public async Task On(CommandContext e, [Description("Amount of time required between each message (seconds)")] int limitTime)
            {
                slowModeTime = limitTime;
                await e.RespondAsync("Slowmode activated, with " + limitTime + " seconds between each message.");
            }

            [Command("off"), Description("Admin command to disable slow mode"), RequireRolesAttribute("Administrator", "Bot-Test")]
            public async Task Off(CommandContext e)
            {
                slowModeTime = -1;
                await e.RespondAsync("Slowmode disabled.");
            }
        }
        [Command("cases"), Description("Admin cases command."), RequireRolesAttribute("Staff", "Bot-Test")]
        public async Task Cases(CommandContext e, [Description("Select a user.")] DiscordMember user, [Description("Number to increase or decrease cases resolved by")] string caseNum)
        {
            RPClass.Users.First(x => x.UserData.userID == user.Id).UserData.resolvedCases += int.Parse(caseNum);
            if (RPClass.Users.First(x => x.UserData.userID == user.Id).UserData.resolvedCases < 0)
                RPClass.Users.First(x => x.UserData.userID == user.Id).UserData.resolvedCases = 0;

            if (RPClass.Users.First(x => x.UserData.userID == user.Id).UserData.role == 1) await XPClass.UpdatePlayerRanking(e.Guild, 1);
            else if (RPClass.Users.First(x => x.UserData.userID == user.Id).UserData.role == 2) await XPClass.UpdatePlayerRanking(e.Guild, 2);
            await XPClass.UpdateGuildRanking(e.Guild);

            RPClass.SaveData(1);

            await e.RespondAsync("Cases updated.");
        }

        [Command("crimes"), Description("Admin cases command."), RequireRolesAttribute("Staff", "Bot-Test")]
        public async Task Crimes(CommandContext e, [Description("Select a user.")] DiscordMember user, [Description("Number to increase or decrease crimes committed by")] string crimeNum)
        {
            RPClass.Users.First(x => x.UserData.userID == user.Id).UserData.crimesCommitted += int.Parse(crimeNum);
            if (RPClass.Users.First(x => x.UserData.userID == user.Id).UserData.crimesCommitted < 0)
                RPClass.Users.First(x => x.UserData.userID == user.Id).UserData.crimesCommitted = 0;

            if (RPClass.Users.First(x => x.UserData.userID == user.Id).UserData.role == 1) await XPClass.UpdatePlayerRanking(e.Guild, 1);
            else if (RPClass.Users.First(x => x.UserData.userID == user.Id).UserData.role == 2) await XPClass.UpdatePlayerRanking(e.Guild, 2);
            await XPClass.UpdateGuildRanking(e.Guild);

            RPClass.SaveData(1);

            await e.RespondAsync("Crimes updated.");
        }

        [Command("name"), Description("Command for users to change their RP name temporarily"), RequireRolesAttribute("Staff", "Bot-Test")]
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
                await Task.Delay(2000);
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

        [Command("sudo"), Description("Execute a command as if you're another user"), RequireRolesAttribute("Staff", "Bot-Test")]
        public async Task SudoAsync(CommandContext e, [Description("User to Sudo")]DiscordUser user, [RemainingText, Description("Command to execute")] string command = "help")
        {
            await e.CommandsNext.SudoAsync(user, e.Channel, command);
        }

        [Command("serverinfo"), Description("Gets info about current server")]
        public async Task GuildInfo(CommandContext ctx)
        {
            var b = new DiscordEmbedBuilder();
            b.WithTitle($"{ctx.Guild.Name} ({ctx.Guild.Id})")
                .WithDescription($"Guild owned by {ctx.Guild.Owner.Username}#{ctx.Guild.Owner.Discriminator} (ID: {ctx.Guild.Owner.Id})")
                .WithThumbnailUrl(ctx.Guild.IconUrl)
                .AddField("Channel count", $"{ctx.Guild.Channels.Count}", true)
                .AddField("AFK Timeout", $"{ctx.Guild.AfkTimeout}", true)
                .AddField("Region", ctx.Guild.RegionId, true)
                .AddField("Role Count", $"{ctx.Guild.Roles.Count}", true)
                .AddField("Large?", ctx.Guild.IsLarge ? "Yes" : "No", true)
                .AddField("Icon Url", ctx.Guild.IconUrl, false)
                .WithFooter("Creation Date:", ctx.Guild.IconUrl)
                .WithTimestamp(ctx.Guild.CreationTimestamp)
                .WithColor(new DiscordColor("4169E1"));

            await ctx.RespondAsync("", embed: b.Build());
        }

        [Command("userinfo"), Description("Gets info about user")]
        public async Task UserInfo(CommandContext ctx, [Description("User to get info about")] DiscordMember m)
        {
            var b = new DiscordEmbedBuilder();
            string Title = m.Username;
            b.WithTitle(m.Username + " ~ " + m.Nickname)
                .WithDescription("Current Status: " + m.Presence.Status.ToString())
                .WithThumbnailUrl(m.AvatarUrl)
                .AddField("Joined Discord on: ", m.CreationTimestamp.ToString("dd MMMM yyyy H:mm:ss") + "\n(" + DateTimeOffset.Now.Subtract(m.CreationTimestamp).Days + " days ago)", true)
                .AddField("Joined this Server on: ", m.JoinedAt.ToString("dd MMMM yyyy H:mm:ss") + "\n(" + DateTimeOffset.Now.Subtract(m.JoinedAt).Days + " days ago)", true)
                .AddField("Roles: ", string.Join(",", m.Roles.Select(x => x.Name)), true)
                .WithFooter("User ID:" + m.Id)
                .WithColor(new DiscordColor("4169E1"));
            await ctx.RespondAsync("", embed: b.Build());
        }

        [Command("purge"), Aliases("p"), Description("Admin purge command"), RequireRolesAttribute("Staff", "Bot-Test")]
        public async Task PurgeAsync(CommandContext e, [Description("Number of messages to purge (Max 100)")] int limit)
        {
            var i = 0;
            var ms = await e.Channel.GetMessagesAsync(limit, e.Message.Id);
            var deletThis = new List<DiscordMessage>();
            foreach (var m in ms)
            {
                deletThis.Add(m);
            }
            if (deletThis.Any())
                await e.Channel.DeleteMessagesAsync(deletThis, "Purged messages.");
            var resp = await e.RespondAsync($"Latest messages deleted.");
            await Task.Delay(2000);
            await resp.DeleteAsync("Purge command executed.");
            await e.Message.DeleteAsync();

        }

        [Command("clean"), Aliases("c"), Description("Cleans up all commands in channel"), RequireRolesAttribute("Staff", "Bot-Test")]
        public async Task CleanAsync(CommandContext e)
        {
            var prefix = "!";
            var ms = await e.Channel.GetMessagesAsync(100, e.Message.Id);
            var delet_this = new List<DiscordMessage>();
            foreach (var m in ms)
            {
                if (m.Author.Id == e.Client.CurrentUser.Id || m.Content.StartsWith(prefix))
                    delet_this.Add(m);
            }
            if (delet_this.Any())
                await e.Channel.DeleteMessagesAsync(delet_this, "Cleaned up commands");
            var resp = await e.RespondAsync($"Latest messages deleted.");
            await Task.Delay(2000);
            await resp.DeleteAsync("Clean command executed.");
            await e.Message.DeleteAsync();
        }

        [Command("restart"),  Description("Admin restart command"), RequireRolesAttribute("Staff", "Bot-Test")]
        public async Task Restart(CommandContext e)
        {
            SaveData(-1);
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

        [Command("update"), Description("Admin update command"), RequireRolesAttribute("Administrator", "Bot-Test")]
        public async Task Update(CommandContext e)
        {
            await e.RespondAsync("Restarting. Wish me luck!");
            SaveData(-1);
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
            WebClient client = new WebClient();
            String json = client.DownloadString("https://icanhazdadjoke.com/slack");
            JokeObject.RootObject obj = JsonConvert.DeserializeObject<JokeObject.RootObject>(json);
            await e.RespondAsync(obj.attachments[0].text);
            Console.ReadLine();
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

        [Command("sayall"), Description("Makes the bot delete all messages in a channel (the channel the command is used in) and repost them."), RequirePermissions(Permissions.Administrator)]
        public async Task SayAll(CommandContext e, string whattodelete = "", string whattosetto = "")
        {
            await e.Message.DeleteAsync();
            List<DiscordMessage> messageList = new List<DiscordMessage>();

            int iter = 1;
            messageList.AddRange(await e.Channel.GetMessagesAsync(100));
            while (true)
            {
                messageList.AddRange(await e.Channel.GetMessagesAsync(100, before: messageList.Last().Id));

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
                if (ret.Contains(whattodelete))
                {
                    Console.WriteLine("e.e");
                    ret = ret.Replace(whattodelete, whattosetto);
                }
                await e.RespondAsync(ret);
            }
            await e.Channel.DeleteMessagesAsync(messageList);

        }

        [Command("removeuser"), Description("Makes the bot delete a user that has left the server (Name in statsheets, copied exactly)."), RequireRolesAttribute("Staff", "Bot-Test")]
        public async Task RemoveUser(CommandContext e, [RemainingText] string whotodelete = "")
        {
            if (Users.Any(x => x.UserData.username == whotodelete))
            {
                Users.Remove(Users.First(x => x.UserData.username == whotodelete));
                SaveData(-1);
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
            var interactivity = e.Client.GetInteractivityModule();
            List<Page> interactivityPages = new List<Page>();

            var members = await e.Guild.GetAllMembersAsync();
            members = members.OrderBy(x => x.JoinedAt).ToList();
            Page p = new Page();
            DiscordEmbedBuilder b = new DiscordEmbedBuilder()
            .WithColor(new DiscordColor("4169E1"))
            .WithFooter("Heroes & Villains")
            .WithTimestamp(DateTime.UtcNow);

            for (int i = 1; i <= members.Count(); i++)
            {
                b.Description += i + ": " + members[i-1].DisplayName + " - " + members[i-1].JoinedAt.ToString("dd MMMM yyyy (H:mm:ss)") + "\n";
                if (i % 10 == 0 && i != 0)
                {
                    p.Embed = b;
                    interactivityPages.Add(p);
                    p = new Page();
                    b = new DiscordEmbedBuilder()
                        .WithColor(new DiscordColor("4169E1"))
                        .WithFooter("Heroes & Villains")
                        .WithTimestamp(DateTime.UtcNow);
                }
            }

            await interactivity.SendPaginatedMessage(e.Channel, e.Member, interactivityPages, timeoutoverride: TimeSpan.FromSeconds(60));
            
        }

        [Group("approval"), Description("Approval commands")]
        class ApprovalClass
        {
            [Command("add"), Description("Command to create a new approval instance."), RequireRolesAttribute("Staff", "Bot-Test")]
            public async Task AddApproval(CommandContext e, [Description("Mention the user you will be approving.")]DiscordMember m)
            {
                Regex rgx = new Regex("[^a-zA-Z0-9-]");
                string name = rgx.Replace(m.DisplayName, "");
                DiscordChannel c = await e.Guild.CreateChannelAsync(name, ChannelType.Text, parent: ApprovalsCategory);
                await c.AddOverwriteAsync(m, Permissions.SendMessages, Permissions.None);
                await c.AddOverwriteAsync(e.Guild.EveryoneRole, Permissions.ReadMessageHistory, Permissions.SendMessages);

                approvalsList.Add(c.Id, m.Id);

                SaveData(8);
                await e.RespondAsync("Approval instance created.");
            }

            [Command("remove"), Description("Command to remove an approval instance. Execute this command in the instance you wish to remove, or mention the user the approval instance is for."), RequireRolesAttribute("Staff", "Bot-Test")]
            public async Task RemoveApproval(CommandContext e, [Description("Mention the user the approval instance is for, or execute the command in the instance you wish to remove.")] DiscordMember m = null)
            {

                if (m != null)
                {
                    if (approvalsList.ContainsValue(m.Id))
                    {
                        DiscordChannel d = e.Guild.GetChannel(approvalsList.First(x => x.Value == m.Id).Key);
                        approvalsList.Remove(d.Id);
                        await d.DeleteAsync();
                    }
                }
                else
                {
                    if (approvalsList.ContainsKey(e.Channel.Id))
                    {
                        await e.Channel.DeleteAsync();
                        approvalsList.Remove(e.Channel.Id);
                    }
                }
                SaveData(8);
            }
        }
        [Command("emoji"), Aliases("e"), Description("Attempts to print the given emoji.")]
        public async Task Emoji(CommandContext e, string emoji)
        {
            var Emojis = new List<DiscordEmoji>();
            foreach (DiscordGuild g in e.Client.Guilds.Values)
            {
                Emojis.AddRange(g.Emojis);
            }
            DiscordEmoji discordEmoji = Emojis.FirstOrDefault(x => x.GetDiscordName() == emoji);
            if (discordEmoji != null)
            {
                await e.RespondAsync(discordEmoji);
            }
            else
            {
                await e.RespondAsync(emoji);
            }
           // await e.RespondAsync(Emojis.PickRandom());
        }
    }
}
