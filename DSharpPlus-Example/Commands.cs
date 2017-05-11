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

namespace SLOBot
{
    public sealed class Commands : Program 
    {
        [Command("sa"), Description("All Secret Assassin commands")]
        public Task SA(CommandContext e, [Description("Use Create, Stop, Start, Join, Info or Vote")] string keyword, [Description("Type the player's number as shown using !sa info")] int vote = -1) => SAClass.SAGame(e, keyword, vote);

        [Command("test"), Description("Test Command"), Hidden()]
        public async Task Test(CommandContext e) =>
            await e.Channel.SendMessageAsync("u w0t m8");

        [Command("kill"), Description("Command for Assassins to use in PM in night phase.")]
        public async Task Kill(CommandContext e, [Description("Type the player's number as shown using !sa info")] int vote = -1)
        {
            if (e.Channel.IsPrivate)
				await SAClass.PMTasks(e, 1, vote);
        }

        [Command("protect"), Description("Command for Medics to use in PM in night phase.")]
        public async Task Protect(CommandContext e, [Description("Type the player's number as shown using !sa info")] int vote = -1)
        {
            if (e.Channel.IsPrivate)
				await SAClass.PMTasks(e, 2, vote);
        }

        [Command("find"), Description("Command for ANBU to use in PM in night phase.")]
        public async Task Find(CommandContext e, [Description("Type the player's number as shown using !sa info")] int vote = -1)
        {
            if (e.Channel.IsPrivate)
				await SAClass.PMTasks(e, 3, vote);
        }

        [Command("ping")]
        public async Task Ping(CommandContext e) =>
            await e.Message.RespondAsync("Pong!");

        [Command("cat"), Description("Displays a cat photo.")]
        public async Task Cat(CommandContext e)
        {

            string s;
            using (WebClient webclient = new WebClient())
            {
                s = webclient.DownloadString("http://random.cat/meow");
                int pFrom = s.IndexOf("\\/i\\/", StringComparison.Ordinal) + "\\/i\\/".Length;
                int pTo = s.LastIndexOf("\"}", StringComparison.Ordinal);
                string cat = s.Substring(pFrom, pTo - pFrom);
                webclient.DownloadFile("http://random.cat/i/" + cat, "cat.png");
                await e.Channel.SendFileAsync(new FileStream("cat.png", FileMode.Open), "cat.png", "meow!");
            }
        }

        [Command("removeSA"), Hidden()]
        public async Task RemoveSA(CommandContext e)
        {
            DiscordChannel Channel = e.Guild.Channels.First(x => x.Name == "sa");

            foreach (DiscordOverwrite a in Channel.PermissionOverwrites)
            {
                Console.WriteLine(a.Type);
                if (a.Id == 302577136148021248)
                {
                    Console.WriteLine("SA!");
                    a.AllowPermission(DSharpPlus.Permission.ReadMessages);
                    await Channel.UpdateOverwriteAsync(a);
                }
            }
        }

        [Command("serverstatus"), Description("Pings the SLO game server directly to find out whether it is on.")]
        public async Task ServerStatus (CommandContext e)
        {
            var client = new TcpClient();
            if (!client.ConnectAsync("52.9.109.21", 443).Wait(1000))
            {
                await e.Channel.SendMessageAsync("Server appears to be down.");
            }
            else
            {
                await e.Channel.SendMessageAsync("Server is up!");

            }
        }
        [Command("translate")]
        public async Task Translate(CommandContext e) => await Translator.Translate(e);

        [Command("throwback"), Description("Returns either a random image or a specified image from throwback database.")]
        public async Task Throwback(CommandContext e, [Description("Search for a keyword in the images database.")] string search = "") => await ThrowbackClass.Throwback(e, search);


    }
}/*

                        
                        else if (cmd.StartsWith("latest", StringComparison.Ordinal))
                        {
                            string htmlCode;
                            int numResults;

                            string returnResults = "";
                            using (WebClient SLOParse = new WebClient())
                            {
                                htmlCode = SLOParse.DownloadString("https://www.shinobilifeonline.com/index.php?action=forum");

                            }

                            Match match = Regex.Match(htmlCode, "<span class.*\n.*<a href=\"(.+)PHPSESSID.*&amp;(.+)\".*<b>(.+)</b>.*\n.*>(.+)<.*\n.*\n.*<strong>(.*)</.*at (.*)", RegexOptions.Multiline);
                            try
                            {
                                string cmdEdited = cmd.Substring(7);
                                if (int.TryParse(cmdEdited, out numResults) && numResults <= 10 && numResults > 0)
                                {
                                    returnResults = match.Groups[4].Value + " posted on the thread: \"" + match.Groups[3].Value + "\", " + match.Groups[5].Value.ToLower() + " at "
                                                        + match.Groups[6].Value + "\n" + match.Groups[1].Value + match.Groups[2].Value + "\n";
                                    for (int i = 1; i <= numResults - 1; i++)
                                    {
                                        match = match.NextMatch();
                                        returnResults += match.Groups[4].Value + " posted on the thread: \"" + match.Groups[3].Value + "\", " + match.Groups[5].Value.ToLower() + " at "
                                                        + match.Groups[6].Value + "\n" + match.Groups[1].Value + match.Groups[2].Value + "\n";

                                    }
                                }
                                else
                                {
                                    await e.Message.Respond("Incorrect format. Posts requested must be an integer between 1 and 10");
                                }
                            }
                            catch
                            {
                                returnResults += match.Groups[4].Value + " posted on the thread: \"" + match.Groups[3].Value + "\", " + match.Groups[5].Value.ToLower() + " at "
                                                    + match.Groups[6].Value + "\n" + match.Groups[1].Value + match.Groups[2].Value + "\n";
                            }
                            await e.Message.Respond(returnResults);


                        }

                        else if (cmd.StartsWith("upload"))
                        {
                            string cmdEdited = "";
                            try
                            {
                                cmdEdited = cmd.Substring(8);
                            }
                            catch { }
                            if (!string.IsNullOrWhiteSpace(cmdEdited))
                            {
                                using (WebClient client = new WebClient())
                                {
                                    string tempname = Reactions.RandomString(5) + "." + cmdEdited.Substring(cmdEdited.LastIndexOf('.') + 1);
                                    try
                                    {
                                        client.DownloadFile(new Uri(cmdEdited), @"SLO Shit/Temp/" + tempname);
                                        await e.Channel.SendMessage("Uploaded successfully!");
                                    }
                                    catch
                                    {
                                        await e.Channel.SendMessage("An image URL requires an ending of .png, .jpg etc.");
                                    }
                                }
                            }
                        }
					}
                    
				}
			}
		};
            

	}
}
*/