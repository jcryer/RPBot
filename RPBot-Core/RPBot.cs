﻿using System;
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

namespace RPBot
{
    internal sealed class RPBot : RPClass
    {
        private Config Config { get; }
        public DiscordClient Discord;
        private CommandsNextModule CommandsNextService { get; }
		private InteractivityModule InteractivityService { get; }
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
            Discord.MessageReactionAdded += this.Discord_MessageReactionAdd;
            Discord.MessageReactionsCleared += this.Discord_MessageReactionRemoveAll;
            Discord.PresenceUpdated += this.Discord_PresenceUpdate;
            Discord.SocketClosed += this.Discord_SocketClose;
            Discord.GuildMemberAdded += this.Discord_GuildMemberAdded;
            Discord.GuildMemberRemoved += this.Discord_GuildMemberRemoved;
            Discord.SocketErrored += this.Discord_SocketError;

            // commandsnext config and the commandsnext service itself
            var cncfg = new CommandsNextConfiguration
            {
                StringPrefix = this.Config.CommandPrefix,
                EnableDms = true,
                EnableMentionPrefix = true
            };
            this.CommandsNextService = Discord.UseCommandsNext(cncfg);
            this.CommandsNextService.CommandErrored += this.CommandsNextService_CommandErrored;
            this.CommandsNextService.CommandExecuted += this.CommandsNextService_CommandExecuted;

            this.CommandsNextService.RegisterCommands<MoneyClass>();
            this.CommandsNextService.RegisterCommands<CommandsClass>();

            this.CommandsNextService.RegisterCommands<ItemClass>();
            this.CommandsNextService.RegisterCommands<XPClass>();
            this.CommandsNextService.RegisterCommands<InstanceClass>();
            this.CommandsNextService.RegisterCommands<GuildClass>();

            this.CommandsNextService.RegisterCommands<LogClass>();
            this.CommandsNextService.RegisterCommands<TriviaClass>();
            this.CommandsNextService.RegisterCommands<ModClass>();

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
            await Discord.UpdateStatusAsync(new DiscordGame("God"));


        }

        private async Task Discord_SocketClose(SocketCloseEventArgs e)
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

        private async Task Discord_SocketError(SocketErrorEventArgs e)
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
            await Task.Delay(0);

        }

        private async Task Discord_GuildMemberAdded(GuildMemberAddEventArgs e)
        {
            DiscordEmbedBuilder b = new DiscordEmbedBuilder();
            b.Title = "Welcome!";
            b.AddField("Welcome to the Heroes & Villains RP Server, " + e.Member.Username + "!",@"
Please read " + e.Guild.Channels.First(x => x.Id == 345307678261772289).Mention + @" then go for " + e.Guild.Channels.First(x => x.Id == 386672328672804864).Mention + @", followed by " + e.Guild.Channels.First(x => x.Id == 314972495432384513).Mention + @". 
Then, once you have decided the character(and filled out the template), ask for the Unapproved role in " + e.Guild.Channels.First(x => x.Id == 314972495432384513).Mention + @" and post in  " + e.Guild.Channels.First(x => x.Id == 314849025968832523).Mention + @". If you have any questions ask in " + e.Guild.Channels.First(x => x.Id == 315272713570615296).Mention + @"
Be sure to checkout the " + e.Guild.Channels.First(x => x.Id == 346944357351555072).Mention + @"!

Hope you enjoy your time here " + e.Member.Mention + "!");
            await e.Member.SendMessageAsync("", embed: b);
            await e.Guild.Channels.First(x => x.Id == 312918289988976653).SendMessageAsync("", embed: b);

        }

        private async Task Discord_GuildMemberRemoved(GuildMemberRemoveEventArgs e)
        {
            DiscordEmbedBuilder b = new DiscordEmbedBuilder();
            b.Title = "Goodbye!";
            b.AddField("Bye " + e.Member.DisplayName, "We didn't like them anyway.");
            await e.Guild.Channels.First(x => x.Id == 312918289988976653).SendMessageAsync("", embed: b);

        }

        private Task Discord_GuildAvailable(GuildCreateEventArgs e)
        {
            this.GameGuard = new Timer(TimerCallback, null, TimeSpan.FromMinutes(0), TimeSpan.FromMinutes(15));

            this.Discord.DebugLogger.LogMessage(LogLevel.Info, "DSPlus Test", $"Guild available: {e.Guild.Name}", DateTime.Now);
            return Task.Delay(0);
        }

        private Task Discord_PresenceUpdate(PresenceUpdateEventArgs e)
        {
            return Task.Delay(0);
        }
        
        public async Task Discord_MessageCreated(MessageCreateEventArgs e)
        {
            if (Program.firstRun)
            {
                try
                {
                    DiscordGuild RPGuild = Discord.Guilds.First(x => x.Key == 312918289988976653).Value;
                    firstRun = false;
                    Thread myNewThread = new Thread(async () => await UpdateClock(e, Discord));
                    myNewThread.Start();
                    GuildRankingChannel = e.Guild.GetChannel(312964153197330433);
                    PlayerRankingChannel = e.Guild.GetChannel(315048564525105153);
                    VillainRankingChannel = e.Guild.GetChannel(315048584007385093);
                    RogueRankingChannel = e.Guild.GetChannel(371782656716832769);
                    ApprovalsCategory = e.Guild.GetChannel(389160226944712705);
                    StatsChannel = e.Guild.GetChannel(312964092748890114);
                    StaffRole = e.Guild.GetRole(313845882841858048);
                    HelpfulRole = e.Guild.GetRole(312979390516559885);
                    PunishedRole = e.Guild.GetRole(379163684276142091);

                    await AddOrUpdateUsers(RPGuild, true);
                }
                catch { }
            }
            if (!e.Message.Content.StartsWith("!"))
            {
                if (SpeechList.Any(x => x.id == e.Author.Id) && !e.Message.Content.StartsWith("*"))
                {
                    SpeechObject.RootObject savedName = SpeechList.First(x => x.id == e.Author.Id);
                    await e.Message.DeleteAsync();
                    await e.Channel.SendMessageAsync("__**" + savedName.name + "**__: " + e.Message.Content);
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

        private async Task Discord_MessageReactionAdd(MessageReactionAddEventArgs e)
        {
            if (!e.User.IsBot && ItemsList.Any(x => x.messageID == e.Message.Id))
            {
                DiscordMessage msgToDelete = e.Message;
                if (e.Emoji.Name == "📥")
                {
                    DiscordMember l = await e.Channel.Guild.GetMemberAsync(e.User.Id);
                     
                    DiscordMessage test = await e.Channel.GetMessageAsync(e.Message.Id);
                    await test.DeleteReactionAsync(DiscordEmoji.FromName(e.Client as DiscordClient, ":inbox_tray:"), l);

                    UserObject.RootObject user = Users.First(x => x.UserData.userID == e.User.Id);
                    ShopObject.RootObject item = ItemsList.First(x => x.messageID == e.Message.Id);
                    if (user.UserData.money >= item.price)
                    {
                        if (!user.InvData.items.Any(x => x == item.id))
                        {

                            if (item.availability == -1 || item.availability > 0)
                            {
                                msgToDelete = await e.Channel.SendMessageAsync("Item bought! Congratulations " + user.UserData.username + "!");
                                if (item.availability != -1)
                                {
                                    item.availability -= 1;
                                }
                                user.UserData.money -= item.price;
                                user.InvData.items.Add(item.id);

                                DiscordEmbedBuilder embed = new DiscordEmbedBuilder();
                                
                                if (item.availability != -1) embed.AddField("$" + item.price + " - " + item.availability + " left in stock",item.description);
                                else embed.AddField("$" + item.price + " - ∞ left in stock", item.description);
                                embed.WithTitle(item.emoji + " " + item.name);

                                embed.Color = new DiscordColor("4169E1");
                                embed.WithFooter("Heroes & Villains");
                                embed.WithTimestamp(DateTime.UtcNow);

                                await test.ModifyAsync("", embed: embed);
                            }
                            else
                            {
                                msgToDelete = await e.Channel.SendMessageAsync("This item is out of stock.");
                            }
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
                }
                else if (e.Emoji.Name == "📤")
                {
                    await e.Message.DeleteReactionAsync(DiscordEmoji.FromUnicode(e.Client as DiscordClient, "📤"), await e.Channel.Guild.GetMemberAsync(e.User.Id));

                    UserObject.RootObject user = Users.First(x => x.UserData.userID == e.User.Id);
                    ShopObject.RootObject item = ItemsList.First(x => x.messageID == e.Message.Id);
                    if (user.InvData.items.Any(x => x == item.id))
                    {
                        user.UserData.money += item.price / 2;
                        user.InvData.items.Remove(user.InvData.items.First(y => y == item.id));
                        if (item.availability != -1) item.availability += 1;
                        msgToDelete = await e.Channel.SendMessageAsync("Item sold! Congratulations " + user.UserData.username + "!");

                        DiscordEmbedBuilder embed = new DiscordEmbedBuilder();

                        embed.AddField("$" + item.price + " - " + item.availability + " left in stock", item.description);


                        embed.WithTitle(item.emoji + " " + item.name);

                        embed.Color = new DiscordColor("4169E1");
                        embed.WithFooter("Heroes & Villains");
                        embed.WithTimestamp(DateTime.UtcNow);

                        await e.Message.ModifyAsync("", embed: embed);
                    }
                    else
                    {
                        msgToDelete = await e.Channel.SendMessageAsync("You do not own that item.");
                    }
                }
                await Task.Delay(3500);
                await msgToDelete.DeleteAsync();
                SaveData(1);
            }
        }

        private /*async*/ Task Discord_MessageReactionRemoveAll(MessageReactionsClearEventArgs e)
        {
            return Task.Delay(0);
        }

        private void TimerCallback(object _)
        {
            try
            {
                this.Discord.UpdateStatusAsync(new DiscordGame("God")).GetAwaiter().GetResult();
            }
            catch (Exception) { }
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

            Discord.DebugLogger.LogMessage(LogLevel.Error, "CommandsNext", $"{e.Exception.GetType()}: {e.Exception.Message}", DateTime.Now);
            
            var ms = e.Exception.Message;
            var st = e.Exception.StackTrace;

            ms = ms.Length > 1000 ? ms.Substring(0, 1000) : ms;
            st = st.Length > 1000 ? st.Substring(0, 1000) : st;

            DiscordEmbedBuilder embed = new DiscordEmbedBuilder();

            embed.WithTitle("You failure at life");
            embed.WithDescription("You fucked it up, didn't you. You are a mess. You are a disgrace. There's a door, use it and never come back,you miserable piece of shit.");
            embed.Color = new DiscordColor(0xFF0000);
            embed.WithFooter("Heroes & Villains");
            embed.WithTimestamp(DateTime.UtcNow);

            embed.AddField("Command errored", $"```{e.Exception.GetType()} occured when executing `{ e.Command.Name }`.\n" + ms + "```");
            embed.AddField("Stack trace", $"```cs\n{st}\n```");
            embed.AddField("Source", e.Exception.Source);
            embed.AddField("Message", e.Exception.Message);
           // await e.Context.RespondAsync("", embed: embed);
        }

        private async Task CommandsNextService_CommandExecuted(CommandExecutionEventArgs e)
        {
            await RPClass.AddOrUpdateUsers(e.Context.Guild, false);
            Discord.DebugLogger.LogMessage(LogLevel.Info, "CommandsNext", $"{e.Context.User.Username} executed {e.Command.Name} in {e.Context.Channel.Name}", DateTime.Now);
        }
    }
}