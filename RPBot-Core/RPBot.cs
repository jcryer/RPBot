using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Interactivity;
using DSharpPlus.EventArgs;
using DSharpPlus.Entities;
using DSharpPlus.Net.WebSocket;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.IO;

namespace RPBot
{
    internal sealed class RPBot
    {
        private Config Config { get; }
        public DiscordClient Discord;
        private CommandsNextExtension CommandsNextService { get; }
		private InteractivityExtension InteractivityService { get; }
        private Timer GameGuard { get; set; }

        public RPBot(Config cfg, int shardid)
        {
            // global bot config
            this.Config = cfg;

            // discord instance config and the instance itself
            var dcfg = new DiscordConfiguration
            {
                AutoReconnect = true,
                LargeThreshold = 250,
                LogLevel = LogLevel.Info,
                Token = this.Config.Token,
                TokenType = TokenType.Bot,
                UseInternalLogHandler = false,
                ShardId = shardid,
                ShardCount = this.Config.ShardCount,
            };
            Discord = new DiscordClient(dcfg);
			//Discord.SetWebSocketClient<WebSocketSharpClient>();            
            // events
            Discord.DebugLogger.LogMessageReceived += this.DebugLogger_LogMessageReceived;
            Discord.Ready += this.Discord_Ready;
            Discord.GuildAvailable += this.Discord_GuildAvailable;
            Discord.MessageCreated += this.Discord_MessageCreated;
            Discord.MessageDeleted += this.Discord_MessageDeleted;
            Discord.MessageUpdated += this.Discord_MessageUpdated;

            Discord.MessageReactionAdded += this.Discord_MessageReactionAdd;
            Discord.MessageReactionsCleared += this.Discord_MessageReactionRemoveAll;
            Discord.PresenceUpdated += this.Discord_PresenceUpdate;
            Discord.SocketClosed += this.Discord_SocketClose;
            Discord.GuildMemberAdded += this.Discord_GuildMemberAdded;
            Discord.GuildMemberRemoved += this.Discord_GuildMemberRemoved;
            Discord.SocketErrored += this.Discord_SocketError;
            Discord.VoiceStateUpdated += this.Discord_VoiceStateUpdated;
            Discord.ClientErrored += this.Discord_ClientErrored;

            // commandsnext config and the commandsnext service itself
            var cncfg = new CommandsNextConfiguration
            {
                StringPrefixes = new List<string>() { Config.CommandPrefix, "" },
                EnableDms = true,
                EnableMentionPrefix = true,
                CaseSensitive = false
            };

            this.CommandsNextService = Discord.UseCommandsNext(cncfg);
            this.CommandsNextService.CommandErrored += this.CommandsNextService_CommandErrored;
            this.CommandsNextService.CommandExecuted += this.CommandsNextService_CommandExecuted;

            CommandsNextService.RegisterCommands(typeof(XPClass));
            //    this.CommandsNextService.RegisterCommands<InstanceClass>();
            CommandsNextService.RegisterCommands(typeof(MoneyClass));
            CommandsNextService.RegisterCommands(typeof(CommandsClass));
            CommandsNextService.RegisterCommands(typeof(GuildClass));
            CommandsNextService.RegisterCommands(typeof(BloodClass));
            CommandsNextService.RegisterCommands(typeof(MeritClass));
            CommandsNextService.RegisterCommands(typeof(LogClass));
            CommandsNextService.RegisterCommands(typeof(TriviaClass));
            CommandsNextService.RegisterCommands(typeof(ModClass));
            CommandsNextService.RegisterCommands(typeof(TagClass));
            CommandsNextService.RegisterCommands(typeof(SVClass));
            CommandsNextService.RegisterCommands(typeof(WikiClass));
            CommandsNextService.RegisterCommands(typeof(StatsClass));
            CommandsNextService.RegisterCommands(typeof(SignupClass));
            CommandsNextService.RegisterCommands(typeof(FameClass));
            CommandsNextService.RegisterCommands(typeof(InfamyClass));
            CommandsNextService.RegisterCommands(typeof(CardClass));


            // WikiClass.InitWiki();

            InteractivityConfiguration icfg = new InteractivityConfiguration();
			this.InteractivityService = Discord.UseInteractivity(icfg);
        }

        public async Task RunAsync()
        {
            await Discord.ConnectAsync();
            await Task.Delay(-1);
        }

        private void DebugLogger_LogMessageReceived(object sender, DebugLogMessageEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("[{0:yyyy-MM-dd HH:mm:ss zzz}] ", e.Timestamp.ToLocalTime());

            var tag = e.Application;
            if (tag.Length > 12)
                tag = tag.Substring(0, 12);
            if (tag.Length < 12)
                tag = tag.PadLeft(12, ' ');
            Console.Write("[{0}] ", tag);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("[{0}] ", string.Concat("SHARD ", this.Discord.ShardId.ToString("00")));

            switch (e.Level)
            {
                case LogLevel.Critical:
                case LogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;

                case LogLevel.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;

                case LogLevel.Info:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;

                case LogLevel.Debug:
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    break;
            }
            Console.Write("[{0}] ", e.Level.ToString().PadLeft(11));

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(e.Message);
        }

        private async Task Discord_Ready(ReadyEventArgs e)
        {
            await Task.Delay(0);

        }

        private async Task Discord_VoiceStateUpdated(VoiceStateUpdateEventArgs e)
        {
            
            await Task.Delay(0);

        }

        private async Task Discord_SocketClose(SocketCloseEventArgs e)
        {
            try
            {
                await (await e.Client.GetChannelAsync(392429153909080065)).SendMessageAsync("Not Restarting(test): Socket Close");
            }
            catch
            {
            }

            RPClass.SaveData(-1);
            /*
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
            */
        }

        private async Task Discord_ClientErrored(ClientErrorEventArgs e)
        {
            try
            {
                await (await e.Client.GetChannelAsync(392429153909080065)).SendMessageAsync("Not Restarting (test): Client Error");
            }
            catch
            {
            }
            
            RPClass.SaveData(-1);
            /*
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
            */
        }

        private async Task Discord_SocketError(SocketErrorEventArgs e)
        {
            try
            {
                await (await e.Client.GetChannelAsync(392429153909080065)).SendMessageAsync("Not Restarting(test): Socket Error");
            }
            catch
            {
            }
            RPClass.SaveData(-1);
            /*
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
            */
        }

        private async Task Discord_GuildMemberAdded(GuildMemberAddEventArgs e)
        {
            if (e.Guild == RPClass.RPGuild)
            {
                DiscordEmbedBuilder b = new DiscordEmbedBuilder()
                .AddField($"Welcome, {e.Member.Username}!",
$@"Welcome to the Heroes & Villains Discord Server, {e.Member.Mention}!
First things first, please read the <#511221485004783618> to see how we run things here.
Once you've done that, then head over to <#463498007514710016> to get some help for your OC and ask for an approval channel.
To learn more about the lore and the Server, then browse through <#366574305418543105> or head over to our Wiki and have a look through the pages!

https://roleplay-heroes-and-villains.wikia.com/wiki/Role-Play:_Heroes_and_Villains_Wiki

We hope you enjoy your stay!")
            .WithFooter("Heroes & Villains Discord Server").WithColor(DiscordColor.CornflowerBlue);

                await e.Member.SendMessageAsync("", embed: b);
                await e.Guild.Channels.First(x => x.Id == 312918289988976653).SendMessageAsync("", embed: b);

                DiscordEmbedBuilder c = new DiscordEmbedBuilder
                {
                    Title = "Member Joined",
                    Color = DiscordColor.Green
                }
                .AddField("Member", e.Member.DisplayName + "#" + e.Member.Discriminator + " (" + e.Member.Id + ")", true)
                .AddField("Timestamp", e.Member.JoinedAt.ToString(), true);

                await e.Guild.GetChannel(392429153909080065).SendMessageAsync(embed: c);
            }
        }

        private async Task Discord_GuildMemberRemoved(GuildMemberRemoveEventArgs e)
        {
            if (e.Guild == RPClass.RPGuild)
            {
                DiscordEmbedBuilder b = new DiscordEmbedBuilder
                {
                    Title = "Goodbye!"
                }
                .AddField("Bye " + e.Member.DisplayName, "We didn't like them anyway.");
                await e.Guild.Channels.First(x => x.Id == 312918289988976653).SendMessageAsync("", embed: b);

                DiscordEmbedBuilder c = new DiscordEmbedBuilder
                {
                    Title = "Member Left",
                    Color = DiscordColor.Red
                }
                .AddField("Member", e.Member.DisplayName + "#" + e.Member.Discriminator + " (" + e.Member.Id + ")", true)
                .AddField("Timestamp", DateTime.UtcNow.ToString(), true);

                await e.Guild.GetChannel(392429153909080065).SendMessageAsync(embed: c);
            }
        }

        public async Task Discord_GuildAvailable(GuildCreateEventArgs e)
        {
            this.GameGuard = new Timer(TimerCallback, null, TimeSpan.FromMinutes(0), TimeSpan.FromMinutes(15));

            this.Discord.DebugLogger.LogMessage(LogLevel.Info, "DSPlus", $"Guild available: {e.Guild.Name}", DateTime.UtcNow);

            if (e.Guild.Id == 312918289988976653)
            {
                RPClass.GuildRankingChannel = e.Guild.GetChannel(312964153197330433);
                RPClass.HeroRankingChannel = e.Guild.GetChannel(315048564525105153);
                RPClass.VillainRankingChannel = e.Guild.GetChannel(315048584007385093);
                RPClass.RogueRankingChannel = e.Guild.GetChannel(371782656716832769);
                RPClass.AcademyRankingChannel = e.Guild.GetChannel(402966763022712843);
                RPClass.ApprovalsCategory = e.Guild.GetChannel(510382933652144138);
                RPClass.InstanceCategory = e.Guild.GetChannel(513361784955207701);
                RPClass.StatsChannel = e.Guild.GetChannel(312964092748890114);
                RPClass.FameChannel = e.Guild.GetChannel(465218787768270848);
                RPClass.GameChannel = e.Guild.GetChannel(378770557757423617);
                RPClass.StaffRole = e.Guild.GetRole(415936404598685721); 
                RPClass.HelpfulRole = e.Guild.GetRole(312979390516559885);
                RPClass.MuteRole = e.Guild.GetRole(379163684276142091);
                RPClass.AdminRole = e.Guild.GetRole(312961839359328266);
                RPClass.RPLockRole = e.Guild.GetRole(516706421316780034);
                FameRoles.Init(e.Guild);
                RPClass.RPGuild = e.Guild;

                await RPClass.AddOrUpdateUsers(RPClass.RPGuild, true);

                if (RPClass.Restarted)
                {
                    DiscordChannel c = e.Guild.GetChannel(404108476835430401);
                    DiscordMember me = await e.Guild.GetMemberAsync(126070623855312896);
                    await c.SendMessageAsync("Restarted successfully, " + me.Mention + "!");
                }
            }
            
            
        }

        private Task Discord_PresenceUpdate(PresenceUpdateEventArgs e)
        {
            return Task.Delay(0);
        }

        private async Task Discord_MessageDeleted(MessageDeleteEventArgs e)
        {
            if (e.Guild == RPClass.RPGuild) {
                if (e.Message.Author != e.Client.CurrentUser)
                {
                    try
                    {
                        if (e.Channel.Id != 419128665549570049)
                        {
                            if (!e.Message.Content.StartsWith("!"))
                            {
                                DiscordEmbedBuilder b = new DiscordEmbedBuilder
                                {
                                    Title = "Message Deleted",
                                    Color = DiscordColor.Red
                                }
                                .AddField("Member", e.Message.Author.Username + "#" + e.Message.Author.Discriminator + " (" + e.Message.Author.Id + ")", true)
                                .AddField("Channel", e.Message.Channel.Name, true)
                                .AddField("Creation Timestamp", e.Message.CreationTimestamp.ToString(), true)
                                .AddField("Deletion Timestamp", e.Message.Timestamp.ToString(), true)
                                .AddField("Message", e.Message.Content.Any() ? e.Message.Content : "-", false)
                                .AddField("Attachments", e.Message.Attachments.Any() ? string.Join("\n", e.Message.Attachments.Select(x => x.Url)) : "-", false);

                                await e.Guild.GetChannel(392429153909080065).SendMessageAsync(embed: b);
                            }
                        }
                    }
                    catch { }
                }
            }
        }

        private async Task Discord_MessageUpdated(MessageUpdateEventArgs e)
        {
            if (e.Guild == RPClass.RPGuild)
            {
                if (e.Message.Author != e.Client.CurrentUser)
                {
                    try
                    {
                        if (!e.Message.Content.StartsWith("!"))
                        {
                            if (e.Channel.Id != 419128665549570049)
                            {
                                DiscordEmbedBuilder embed = new DiscordEmbedBuilder
                                {
                                    Title = "Message Edited",
                                    Color = DiscordColor.Orange
                                }
                                .AddField("Member", e.Message.Author.Username + "#" + e.Message.Author.Discriminator + " (" + e.Message.Author.Id + ")", true)
                                .AddField("Channel", e.Message.Channel.Name, true)
                                .AddField("Creation Timestamp", e.Message.CreationTimestamp.ToString(), true)
                                .AddField("Edit Timestamp", e.Message.EditedTimestamp.ToString(), true)
                                .AddField("Old Message", e.MessageBefore.Content.Any() ? e.MessageBefore.Content : "-", false)
                                .AddField("New Message", e.Message.Content.Any() ? e.Message.Content : "-", false)
                                .AddField("Attachments", e.Message.Attachments.Any() ? string.Join("\n", e.Message.Attachments.Select(x => x.Url)) : "-", false);

                                await e.Guild.GetChannel(392429153909080065).SendMessageAsync(embed: embed);
                            }
                        }
                    }
                    catch { }
                }
            }
        }

        public async Task Discord_MessageCreated(MessageCreateEventArgs e)
        {
            if (e.Guild == RPClass.RPGuild)
            { 
                if (RPClass.FirstRun)
                {
                    try
                    {
                        RPClass.FirstRun = false;
                        Thread myNewThread = new Thread(async () => await RPClass.UpdateClock(e, Discord));
                        myNewThread.Start();
                    }
                    catch { }
                }

                if (!e.Message.Content.StartsWith("!"))
                {
                    if (e.Channel.ParentId.HasValue)
                    {
                        if (RPClass.RPCategories.Contains(e.Channel.ParentId.Value))
                        {
                            var user = RPClass.Users.FirstOrDefault(x => x.UserData.UserID == e.Author.Id);
                            if (user != null)
                            {
                                user.Activity.CharacterCount += e.Message.Content.Length;
                                user.Activity.MessageCount += 1;
                                user.Activity.WordCount += CountWords(e.Message.Content);
                            }
                        }
                    } 
                    if ((e.Author as DiscordMember).Roles.Any(x => x == RPClass.StaffRole)) 
                    {
                        MatchCollection matchList = Regex.Matches(e.Message.Content, "`{0,3}{{(.+?)}}`{0,3}");
                        var list = matchList.Cast<Match>().Select(match => match.Value).ToList();
                        int changes = 0;
                        if (list.Any())
                        {
                            string content = e.Message.Content;
                            var roles = new List<DiscordRole>();
                            foreach (var ping in list)
                            {
                                if (ping.Contains("`"))
                                {
                                    continue;
                                }
                                var pingCut = ping.Substring(2, ping.Length - 4);

                                if (e.Guild.Roles.Any(x => x.Name == pingCut))
                                {
                                    var role = e.Guild.Roles.First(x => x.Name == pingCut);
                                    if (!role.IsMentionable)
                                    {
                                        try
                                        {
                                            await role.UpdateAsync(mentionable: true);
                                            roles.Add(role);
                                        }
                                        catch (Exception exception) { }
                                    }
                                    content = content.Replace(ping, role.Mention);
                                    changes++;
                                }
                            }
                            if (changes != 0)
                            {
                                await e.Channel.SendMessageAsync(content);
                                await e.Message.DeleteAsync();
                                foreach (var role in roles)
                                {
                                    await role.UpdateAsync(mentionable: false);
                                }
                            }
                        }
                    }

                    if (RPClass.SpeechList.Any(x => x.Id == e.Author.Id) && !e.Message.Content.StartsWith("*"))
                    {
                        SpeechObject.RootObject savedName = RPClass.SpeechList.First(x => x.Id == e.Author.Id);
                        await e.Message.DeleteAsync();
                        await e.Channel.SendMessageAsync("__**" + savedName.Name + "**__: " + e.Message.Content);
                    }
                    else if (e.Message.Content.StartsWith("~") && !(e.Message.Content[1] == '~'))
                    {
                        try
                        {
                            string[] msg = e.Message.Content.Split(new char[] { ' ' }, 2);
                            await e.Message.RespondAsync("__**" + msg[0].Replace("~", "") + "**__: " + msg[1]);
                            await e.Message.DeleteAsync();
                        }
                        catch
                        {

                        }
                    }
                }
            }
        }

        public static int CountWords(string s)
        {
            MatchCollection collection = Regex.Matches(s, @"[\S]+");
            return collection.Count;
        }

        private Task Discord_MessageReactionAdd(MessageReactionAddEventArgs e)
        {
            return Task.Delay(0);
        }

        private Task Discord_MessageReactionRemoveAll(MessageReactionsClearEventArgs e)
        {
            return Task.Delay(0);
        }

        private void TimerCallback(object _)
        {

        }

        private bool IsCommandMethod(MethodInfo method, Type return_type, params Type[] arg_types)
        {
            if (method.ReturnType != return_type)
                return false;

            var prms = method.GetParameters();
            if (prms.Length != arg_types.Length)
                return false;

            for (var i = 0; i < arg_types.Length; i++)
                if (prms[i].ParameterType != arg_types[i])
                    return false;

            return true;
        }

        private async Task CommandsNextService_CommandErrored(CommandErrorEventArgs e)
        {
            if (e.Exception is CommandNotFoundException)
                return;

            Discord.DebugLogger.LogMessage(LogLevel.Error, "CommandsNext", $"{e.Exception.GetType()}: {e.Exception.Message}", DateTime.UtcNow);
            
            var ms = e.Exception.Message;
            var st = e.Exception.StackTrace;

            ms = ms.Length > 1000 ? ms.Substring(0, 1000) : ms;
            st = st.Length > 1000 ? st.Substring(0, 1000) : st;

            DiscordEmbedBuilder embed = new DiscordEmbedBuilder()
            {
                Title = "You failure at life.",
                Description = "You fucked it up, didn't you. You are a mess. You are a disgrace. There's a door, use it and never come back,you miserable piece of shit.",
                Color = new DiscordColor(0xFF0000),
                Timestamp = DateTime.UtcNow
            }
            .WithFooter("Heroes & Villains")
            .AddField("Command errored", $"```{e.Exception.GetType()} occured when executing `{ e.Command.Name }`.\n" + ms + "```")
            .AddField("Stack trace", $"```cs\n{st}\n```")
            .AddField("Source", e.Exception.Source)
            .AddField("Message", e.Exception.Message);
            await e.Context.RespondAsync("Message errored. Go bug J.C.");
            await e.Context.Guild.Channels.First(x => x.Id == 392429153909080065).SendMessageAsync("", embed: embed);
        }

        private async Task CommandsNextService_CommandExecuted(CommandExecutionEventArgs e)
        {
            await RPClass.AddOrUpdateUsers(e.Context.Guild, false);
            Discord.DebugLogger.LogMessage(LogLevel.Info, "CommandsNext", $"{e.Context.User.Username} executed {e.Command.Name} in {e.Context.Channel.Name}", DateTime.UtcNow);
        }
    }
}
