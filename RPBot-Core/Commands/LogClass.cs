using CommonMark;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RPBot
{
    class LogClass : RPClass
    {
        [Command("log"), Description("Admin log command (testing)"), RequireRolesAttribute("Staff")]
        [Hidden]
        public async Task Log(CommandContext e, [RemainingText, Description("Description of the log")] string desc)
        {
            List<DiscordMessage> messageList = new List<DiscordMessage>();
            List<LogObject.Message> logObjectList = new List<LogObject.Message>();

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

            var allMembers = await e.Guild.GetAllMembersAsync();

            var allChannels = await e.Guild.GetChannelsAsync();

            foreach (DiscordMessage message in messageList)
            {
                if (message.MessageType == MessageType.Default)
                {
                    string content = message.Content;
                    Regex ItemRegex = new Regex(@"(<[#@!]+\d+>)+");
                    foreach (Match ItemMatch in ItemRegex.Matches(content))
                    {
                        ulong ulongNum = 0;
                        Console.WriteLine(ItemMatch.Value);
                        if (ulong.TryParse(ItemMatch.Value.Replace("<", "").Replace(">", "").Replace("@", "").Replace("!", ""), out ulongNum))
                        {
                            try
                            {
                                DiscordMember d = allMembers.First(x => x.Id == ulongNum);
                                content = content.Replace(ItemMatch.Value, "**@" + d.DisplayName + "**");
                            }
                            catch { }
                        }
                        else if (ulong.TryParse(ItemMatch.Value.Replace("<", "").Replace(">", "").Replace("#", ""), out ulongNum))
                        {
                            try
                            {
                                DiscordChannel c = allChannels.First(x => x.Id == ulongNum);
                                content = content.Replace(ItemMatch.Value, "**#" + c.Name + "**");
                            }
                            catch { }
                        }
                        else
                        {
                            content = content.Replace(ItemMatch.Value, "");
                        }
                    }
                        DiscordMember member = allMembers.First(x => x.Id == message.Author.Id);
                        logObjectList.Add(new LogObject.Message(member.DisplayName, member.Color.ToString(), message.Author.AvatarUrl, message.Timestamp, CommonMarkConverter.Convert(content), message.Author.IsBot));
                }
            }
                
            string returnedHTML = CreateMessage(logObjectList);

            string HTMLResponse = File.ReadAllText("Data/template.html");
            HTMLResponse = HTMLResponse.Replace("TOPICHERE", desc).Replace("CHANNELHERE", e.Channel.Name).Replace("CONTENTHERE", returnedHTML);
            string fileName = e.Channel.Name + "_" + DateTime.Now.ToString("dd-mm-yyyy");
            File.WriteAllText("../../var/www/html/logs/" + fileName, HTMLResponse);
            string logList = File.ReadAllText("../../var/www/html/logs/loglist.csv");
            File.WriteAllText("../../var/www/html/logs/loglist.csv", logList + "," + fileName);
            await e.RespondAsync("Done! File name: " + fileName + "\n" + "http://51.15.222.156/" + fileName);
            string output = JsonConvert.SerializeObject(messageList);
            string logFile = "logs/" + DateTime.Now.ToString("MM_dd_yy-H_mm_ss") + "(" + e.Channel.Name + ")" + ".txt";
            File.WriteAllText(logFile, output);
        }

        public static string CreateMessage(List<LogObject.Message> logObjectList)
        {
            string HTMLList = "";
            foreach (LogObject.Message m in logObjectList)
            {
                HTMLList += "<div class=\"message DEFAULT\"> <div class=\"author\"><img class=\"avatar\" src=\"";
                HTMLList += m.avatar + "\"></div><div class=\"content\"><span class=\"name\"><font color = \"";
                HTMLList += m.fontColour + "\" > " + m.username + "</font><span></span><span class=\"timestamp\">" + m.timestamp.ToString() + "</span><span class=\"timestamp-small\">" + m.timestamp.ToString("mm:ss") + "</span></span>";
                HTMLList += "<span class=\"text\"><div> " + m.content.Replace("<p>","").Replace("</p>","") + "</div> </span> </div></div>";
            }
            return HTMLList;
        }
    }
}
