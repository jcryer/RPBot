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

namespace SLOBot
{
    internal sealed class SLOBot
    {
        private Config Config { get; }
        public DiscordClient Discord;
        private Commands Commands { get; }
        //private VoiceNextClient VoiceService { get; }
        private CommandsNextModule CommandsNextService { get; }
        //private InteractivityModule InteractivityService { get; }
        private Timer GameGuard { get; set; }

        public SLOBot(Config cfg, int shardid)
        {
            // global bot config
            this.Config = cfg;

            // discord instance config and the instance itself
            var dcfg = new DiscordConfig
            {
                AutoReconnect = true,
                DiscordBranch = Branch.Stable,
                LargeThreshold = 250,
                LogLevel = LogLevel.Unnecessary,
                Token = this.Config.Token,
                TokenType = TokenType.Bot,
                UseInternalLogHandler = false,
                ShardId = shardid,
                ShardCount = this.Config.ShardCount,
                GatewayVersion = 5
            };
            Discord = new DiscordClient(dcfg);

            // events
            Discord.DebugLogger.LogMessageReceived += this.DebugLogger_LogMessageReceived;
            Discord.Ready += this.Discord_Ready;
            Discord.GuildAvailable += this.Discord_GuildAvailable;
            Discord.MessageCreated += this.Discord_MessageCreated;
            Discord.MessageReactionAdd += this.Discord_MessageReactionAdd;
            Discord.MessageReactionRemoveAll += this.Discord_MessageReactionRemoveAll;
            Discord.PresenceUpdate += this.Discord_PresenceUpdate;

            // commandsnext config and the commandsnext service itself
            var cncfg = new CommandsNextConfiguration
            {
                Prefix = this.Config.CommandPrefix,
                EnableDms = false,
                EnableMentionPrefix = true
            };
            this.CommandsNextService = Discord.UseCommandsNext(cncfg);
            this.CommandsNextService.CommandErrored += this.CommandsNextService_CommandErrored;
            this.CommandsNextService.CommandExecuted += this.CommandsNextService_CommandExecuted;
            this.CommandsNextService.RegisterCommands<Commands>();


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

                case LogLevel.Unnecessary:
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
            await Discord.UpdateStatusAsync("Testing with Chell!");
            foreach (DiscordGuild a in Discord.Guilds.Values)
            {
                Console.WriteLine(a.OwnerID);
            }
        }

        private Task Discord_GuildAvailable(GuildCreateEventArgs e)
        {
            this.GameGuard = new Timer(TimerCallback, null, TimeSpan.FromMinutes(0), TimeSpan.FromMinutes(15));

            this.Discord.DebugLogger.LogMessage(LogLevel.Info, "DSPlus Test", $"Guild available: {e.Guild.Name}", DateTime.Now);
            return Task.Delay(0);
        }

        private Task Discord_PresenceUpdate(PresenceUpdateEventArgs e)
        {
            //if (e.User != null)
            //    this.Discord.DebugLogger.LogMessage(LogLevel.Unnecessary, "DSPlus Test", $"{e.User.Username}#{e.User.Discriminator} ({e.UserID}): {e.Status ?? "<unknown>"} playing {e.Game ?? "<nothing>"}", DateTime.Now);

            return Task.Delay(0);
        }
        
        public async Task Discord_MessageCreated(MessageCreateEventArgs e)
        {
            //List<DiscordChannel> Channels = await Discord.Guilds.First(x => x.Key == 187979961729024001).Value.GetChannels();

            await Task.Delay(0);
            
            if (Program.firstRun)
            {
                try
                {
                    List<DiscordChannel> Channels = await Discord.Guilds.First(x => x.Key == 187979961729024001).Value.GetChannelsAsync();

                    foreach (DiscordChannel Channel in Channels)
                    {
                        if (Channel.Name == "recent-threads")
                        {
                            Thread Latest = new Thread(() => LatestClass.LatestUpdates(Channel));
                            Latest.Start();
                            Program.firstRun = false;
                            break;
                        }
                        /*
                        else if (Channel.Name == "general")
                        {
							Console.WriteLine(SAClass.started);
							if (SAClass.started && !Program.processing)
                            {
								Thread game = new Thread(() => SAClass.ProcessTurn(e)); // TOTALLY BROKEN NEED TO REDO
                                game.Start();
                            }

                        }
                        */
                    }
                }
                catch { }
            }
            if (!e.Message.Content.StartsWith("!"))
            {

                if (e.Guild.Id == 301631649978777610)
                {
                    Random r = new Random();
                    List<DiscordRole> b = e.Guild.Roles;
                    DiscordRole x = b.FirstOrDefault(y => y.Id == 303852997656444929);
                    if (x != null)
                    {
                        x.Color = Program.Colours[r.Next(Program.Colours.Count)];
                        Console.WriteLine(x.Color);
                        await e.Guild.UpdateRoleAsync(x);
                    }
                }

                AutoTranslate at = Program.UsersTranslated.FirstOrDefault(y => y.userID == e.Message.Author.Id);

                if (at != null)
                {
                    Translator t = new Translator();
                    string c = t.Translate(e.Message.Content, at.from, at.to);
                    if (t.Error == null)
                    {
                        await e.Channel.SendMessageAsync(e.Message.Author.Username + " said: " + c);

                        await e.Message.DeleteAsync();
                    }
                    else
                    {
                        await e.Channel.SendMessageAsync("Translation failed. I'm a terrible bot.");
                    }
                }
            }
            
        }

        private /*async*/ Task Discord_MessageReactionAdd(MessageReactionAddEventArgs e)
        {
            return Task.Delay(0);

            //await e.Message.DeleteAllReactions();
        }

        private /*async*/ Task Discord_MessageReactionRemoveAll(MessageReactionRemoveAllEventArgs e)
        {
            return Task.Delay(0);

            //await e.Message.DeleteAllReactions();
        }

        private async Task CommandService_CommandError(CommandErrorEventArgs e)
        {
            var ms = e.Exception.Message;
            var st = e.Exception.StackTrace;

            ms = ms.Length > 1000 ? ms.Substring(0, 1000) : ms;
            st = st.Length > 1000 ? st.Substring(0, 1000) : st;

            var embed = new DiscordEmbed
            {
                Color = 0xFF0000,
                Title = "An exception occured when executing a command",
                Description = $"`{e.Exception.GetType()}` occured when executing `{e.Command.Name}`.",
                Footer = new DiscordEmbedFooter
                {
                    IconUrl = this.Discord.Me.AvatarUrl,
                    Text = this.Discord.Me.Username
                },
                Timestamp = DateTime.UtcNow,
                Fields = new List<DiscordEmbedField>()
                {
                    new DiscordEmbedField
                    {
                        Name = "Message",
                        Value = ms,
                        Inline = false
                    },
                    new DiscordEmbedField
                    {
                        Name = "Stack trace",
                        Value = $"```cs\n{st}\n```",
                        Inline = false
                    }
                }
            };
            await e.Context.Message.RespondAsync("\u200b", embed: embed);
        }

        private void TimerCallback(object _)
        {
            try
            {
                this.Discord.UpdateStatusAsync("testing with Chell").GetAwaiter().GetResult();
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

            var embed = new DiscordEmbed
            {
                Color = 0xFF0000,
                Title = "An exception occured when executing a command",
                Description = $"`{e.Exception.GetType()}` occured when executing `{e.Command.Name}`.",
                Footer = new DiscordEmbedFooter
                {
                    IconUrl = Discord.Me.AvatarUrl,
                    Text = Discord.Me.Username
                },
                Timestamp = DateTime.UtcNow,
                Fields = new List<DiscordEmbedField>()
                {
                    new DiscordEmbedField
                    {
                        Name = "Message",
                        Value = ms,
                        Inline = false
                    },
                    new DiscordEmbedField
                    {
                        Name = "Stack trace",
                        Value = $"```cs\n{st}\n```",
                        Inline = false
                    }
                }
            };
            await e.Context.Channel.SendMessageAsync("\u200b", embed: embed);
        }

        private Task CommandsNextService_CommandExecuted(CommandExecutedEventArgs e)
        {
            Discord.DebugLogger.LogMessage(LogLevel.Info, "CommandsNext", $"{e.Context.User.Username} executed {e.Command.Name} in {e.Context.Channel.Name}", DateTime.Now);
            return Task.Delay(0);
        }
    }
}
