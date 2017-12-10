using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
/*
namespace RPBot
{
    [Group("tos")]
    class TOSClass : RPClass
    {
        public static bool active = false;
        
        [Command("create"), Description("Creates a new Town of Salem game.")]
        public async Task Create(CommandContext e)
        {
            if (!active)
            {
                TOSData = new TOSObject.RootObject();
                active = true;
                await e.RespondAsync("Welcome to Town of Salem! New game starting...\n\nTo sign up, please use the command !tos join");
            }
            else
            {
                await e.RespondAsync("Town of Salem game already active.");
            }
        }

        [Command("start"), Description("Starts the Town of Salem game.")]
        public async Task Start(CommandContext e)
        {
            if (active && !TOSData.started && TOSData.players.Count >= 4)
            {
                StartGame(e);
                TOSData.timer = DateTime.Now;
                TOSData.started = true;
                Thread game = new Thread(() => ProcessTurn(e, 2));
                game.Start();
            }

            else if (active && !TOSData.started && TOSData.players.Count < 4)
            {
                await e.RespondAsync("Not enough players to begin the game.");
            }

            else if (active && TOSData.started)
            {
                await e.RespondAsync("Game already started.");
            }
            else
            {
                await e.RespondAsync("There is no active Town of Salem game. Use the command !tos create to make a new game.");
            }
        }

        [Command("stop"), Description("Stops the active Town of Salem game.")]
        public async Task Stop(CommandContext e)
        {
            if (active && !TOSData.started)
            {
                if (e.Member.Roles.Contains(StaffRole))
                {
                    TOSData = new TOSObject.RootObject();
                    await e.RespondAsync("Town of Salem game prematurely ended.");
                }
                else
                {
                    await e.RespondAsync("You do not have permission.");
                }
            }
            else
            {
                await e.RespondAsync("There is no active Town of Salem game. Use the command !tos create to make a new game.");
            }
        }

        [Command("join"), Description("Adds you to the Town of Salem game.")]
        public async Task Join(CommandContext e)
        {
            if (active && !TOSData.started)
            {
                if (TOSData.players.Any(x => x.ID == e.Member.Id))
                {
                    await e.RespondAsync("You are already in the Town of Salem game, " + e.Member.DisplayName);
                }
                else
                {
                    TOSData.players.Add(new TOSObject.UserObject(e.Member.Id, e.Member.DisplayName, -1, 0));
                    await e.RespondAsync("Added " + e.Member.DisplayName + " to the Town of Salem game.");
                }
            }
            else if (TOSData.started)
            {
                await e.RespondAsync("The game has already begun. Sorry!");
            }
            else
            {
                await Create(e);
                TOSData.players.Add(new TOSObject.UserObject(e.Member.Id, e.Member.DisplayName, -1, 0));
                await e.RespondAsync("Added " + e.Member.DisplayName + " to the Town of Salem game.");
            }
        }

        [Command("info"), Description("Prints info on the currently active Town of Salem game.")]
        public async Task Info(CommandContext e)
        {
            if (active || !TOSData.started)
            {
                await TOSData.DisplayData(e, false);
            }
            else
            {
                await e.RespondAsync("No Active game.");
            }
        }

        public static async Task Loop(CommandContext e)
        {
            DiscordChannel channel = e.Channel;
            IReadOnlyList<DiscordChannel> channels = await e.Guild.GetChannelsAsync();
            if (channels.Any(x => x.Name == "bot-spam"))
            {
                channel = channels.First(x => x.Name == "bot-spam");
            }
            Random rnd = new Random();

            var interactivity = e.Client.GetInteractivityModule();
            Dictionary<DiscordUser, int> gameData = new Dictionary<DiscordUser, int>();
            List<string> triviaList = new List<string>(triviaData);
            await Task.Delay(1000);
            for (int i = 1; i < roundNum + 1; i++)
            {
                if (!active) goto l;
                int r = rnd.Next(triviaList.Count);
                List<string> question = triviaList[r].Split('`').ToList();
                await channel.SendMessageAsync("__**Round: " + i + "**__\n" + question[0]);
                question.Remove(question[0]);
                var msg = await interactivity.WaitForMessageAsync(xm => question.Any(x => x.ToLower() == xm.Content.ToLower() || xm.Content.ToLower().Contains("!skip") || xm.Content.ToLower() == "!stop"), TimeSpan.FromSeconds(45));

                if (msg != null)
                {
                    if (msg.Message.Content.ToLower().Contains("skip"))
                    {
                        await channel.SendMessageAsync("Question skipped.");
                        triviaList.Remove(triviaList[r]);

                    }
                    else if (msg.Message.Content == "!stop")
                    {
                        goto l;
                    }
                    else
                    {
                        await channel.SendMessageAsync($"{msg.User.Mention} got it right!");

                        if (!gameData.ContainsKey(msg.User))
                        {
                            gameData.Add(msg.User, 1);
                        }
                        else
                        {
                            gameData[msg.User] += 1;
                        }
                    }
                }
                triviaList.Remove(triviaList[r]);

            }
            l:
            string response = "End of quiz. \n\n__**Leaderboard**__\n```";
            int count = 1;
            foreach (var item in gameData.OrderByDescending(r => r.Value))
            {
                DiscordMember x = await e.Guild.GetMemberAsync(item.Key.Id);
                response += count + ": " + x.DisplayName + ", with " + item.Value + "\n";
                count++;
            }

            await channel.SendMessageAsync(response + "```");
            active = false;

        }
    }
}
*/