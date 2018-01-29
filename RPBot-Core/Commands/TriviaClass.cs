using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RPBot
{
    [Group("trivia"), Description("Trivia Commands")]
    public class TriviaClass : BaseCommandModule
    {
        public static List<string> TriviaList;
        public static bool active = false;
        Thread t;

        [Command("list"), Description("Command to list available trivia sheets.")]
        public async Task List(CommandContext e)
        {
            DiscordChannel channel = e.Channel;
            IReadOnlyList<DiscordChannel> channels = await e.Guild.GetChannelsAsync();
            if (channels.Any(x => x.Name == "bot-spam"))
            {
                channel = channels.First(x => x.Name == "bot-spam");
            }
            string response = "__**Available Trivia Question Sheets:**__\n```\n";
            foreach (string item in TriviaList)
            {
                response += item + "\n";
            }
            await channel.SendMessageAsync(response + "```");
            Console.ReadLine();
        }

        [Command("start"), Description("Command to start specific quiz.")]
        public async Task Start(CommandContext e, [Description("Which quiz to start")] string quiz, [Description("OPTIONAL: how many rounds to have")] int roundNum = 10)
        {
            if (roundNum < 1) roundNum = 1;
            if (roundNum > 50) roundNum = 50;
            if (TriviaList.Contains(quiz.ToLower()))
            {
                if (!active)
                {
                    await e.RespondAsync("Game started. Rounds: " + roundNum + ".");
                    string[] triviaData = File.ReadAllLines("trivia/" + quiz.ToLower() + ".txt");
                    t = new Thread(() => Loop(e, roundNum, triviaData));
                    active = true;
                    t.Start();
                }
                else
                {
                    await e.RespondAsync("A quiz is already active.");
                }
            }
            else
            {
                await e.RespondAsync("There is no quiz by that name.");
            }
            Console.ReadLine();
        }

        public static async void Loop(CommandContext e, int roundNum, string[] triviaData)
        {
            DiscordChannel channel = e.Channel;
            IReadOnlyList<DiscordChannel> channels = await e.Guild.GetChannelsAsync();
            if (channels.Any(x => x.Name == "bot-spam"))
            {
                channel = channels.First(x => x.Name == "bot-spam");
            }
            Random rnd = new Random();

            var interactivity = e.Client.GetInteractivity();
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
