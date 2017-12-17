using CommonMark;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
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
        [Command("log"), Description("Admin log command (testing)"), RequireRolesAttribute("Staff", "Bot-Test")]
        [Hidden]
        public async Task Log(CommandContext e)
        {
            List<DiscordMessage> messageList = new List<DiscordMessage>();
            List<LogObject.Message> logObjectList = new List<LogObject.Message>();

            int iter = 1;
            messageList.AddRange(await e.Channel.GetMessagesAsync(100));
            while (true)
            {
                if (messageList.Count == (100 * iter))
                {
                    messageList.AddRange(await e.Channel.GetMessagesAsync(100, before: messageList.Last().Id));
                }
                else
                {
                    messageList.AddRange(await e.Channel.GetMessagesAsync(100, before: messageList.Last().Id));
                    break;
                }
                if (iter % 100 == 0)
                {
                    await e.RespondAsync("Messages so far: " + iter * 100);
                }
                iter++;
            }
            messageList.Reverse();
            /*
                            foreach (DiscordMessage m in msgList)
                            {
                                logArray.Add(new LogObject.RootObject(m.Id, m.Author.Username, m.Content, m.ChannelId));
                            }
                            string output = JsonConvert.SerializeObject(logArray);
                            string logFile = "logs/" + DateTime.Now.ToString("MM_dd_yy-H_mm_ss") + "(" + e.Channel.Id + ")" + ".txt";
                            File.WriteAllText(logFile, output);
                            await e.RespondWithFileAsync(logFile, "Log dump complete! Messages: " + logArray.Count());
            */
            List<DiscordMember> allMembers = new List<DiscordMember>();
            allMembers.AddRange(await e.Guild.GetAllMembersAsync());

            List<DiscordChannel> allChannels = new List<DiscordChannel>();
            allChannels.AddRange(await e.Guild.GetChannelsAsync());

            foreach (DiscordMessage dm in messageList)
            {
                if (dm.MessageType == MessageType.Default)
                {
                    string content = dm.Content;
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
                                content = content.Replace(ItemMatch.Value, "**@" + d.Nickname + "**");
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
                    try
                    {
                        DiscordMember member = allMembers.First(x => x.Id == dm.Author.Id);
                        logObjectList.Add(new LogObject.Message(member.Nickname, member.Color.ToString(), dm.Author.AvatarUrl, dm.Timestamp, CommonMarkConverter.Convert(content), dm.Author.IsBot));
                    }
                    catch
                    {
                        logObjectList.Add(new LogObject.Message(dm.Author.Username, DiscordColor.Gray.ToString(), dm.Author.AvatarUrl, dm.Timestamp, CommonMarkConverter.Convert(content), dm.Author.IsBot));
                    }
                }
            }
                
            string returnedHTML = CreateMessage(logObjectList);

            string HTMLResponse = File.ReadAllText("template.html");
            HTMLResponse = HTMLResponse.Replace("TOPICHERE", "test").Replace("CHANNELHERE", e.Channel.Name).Replace("CONTENTHERE", returnedHTML);

            File.WriteAllText("resp.html", HTMLResponse);
            await e.RespondWithFileAsync("resp.html", "Data dump complete!");
            //string output = JsonConvert.SerializeObject(htmlArray);
            //string logFile = "logs/" + DateTime.Now.ToString("MM_dd_yy-H_mm_ss") + "(" + e.Channel.Id + ")" + ".txt";
            //File.WriteAllText(logFile, output);
            //await e.RespondWithFileAsync(logFile, "Log dump complete! Messages: " + htmlArray.Count());
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
