using DSharpPlus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace SLOBot
{
    public class LatestClass
    {
        public static void LatestStartup()
        {
            using (WebClient SLOParse = new WebClient())
            {
                string htmlCode = SLOParse.DownloadString("https://www.shinobilifeonline.com/index.php?action=forum");
                File.WriteAllText("LatestLog.txt", "");

                Match match = Regex.Match(htmlCode, "<span class.*\n.*<a href=\"(.+)PHPSESSID.*&amp;(.+)\".*<b>(.+)</b>.*\n.*>(.+)<.*\n.*\n.*<strong>(.*)</.*at (.*)", RegexOptions.Multiline);
                string match1 = match.Groups[4].Value + " posted on the thread: \"" + match.Groups[3].Value + "\", " + match.Groups[5].Value.ToLower() + " at " + match.Groups[6].Value + " - " + match.Groups[1].Value + match.Groups[2].Value;
                File.AppendAllText("LatestLog.txt", match1 + Environment.NewLine);

                for (int i = 1; i <= 9; i++)
                {
                    match = match.NextMatch();
                    string matchmore = match.Groups[4].Value + " posted on the thread: \"" + match.Groups[3].Value + "\", " + match.Groups[5].Value.ToLower() + " at " + match.Groups[6].Value + " - " + match.Groups[1].Value + match.Groups[2].Value;
                    File.AppendAllText("LatestLog.txt", matchmore + Environment.NewLine);
                }
            }
        }
        public static void LatestUpdates(DiscordChannel a)
        {

            while (true)
            {
                string[] textreturn = File.ReadAllLines("LatestLog.txt");
                string returnResults = "";
                using (WebClient SLOParse = new WebClient())
                {
                    string htmlCode = SLOParse.DownloadString("https://www.shinobilifeonline.com/index.php?action=forum");
                    Match match = Regex.Match(htmlCode, "<span class.*\n.*<a href=\"(.+)PHPSESSID.*&amp;(.+)\".*<b>(.+)</b>.*\n.*>(.+)<.*\n.*\n.*<strong>(.*)</.*at (.*)", RegexOptions.Multiline);
                    string match1 = match.Groups[4].Value + " posted on the thread: \"" + match.Groups[3].Value + "\", " + match.Groups[5].Value.ToLower() + " at " + match.Groups[6].Value + " - " + match.Groups[1].Value + match.Groups[2].Value;


                    if (!textreturn.Contains(match1))
                    {

                        returnResults += match1 + "\n";
                    }
                    for (int i = 1; i <= 3; i++)
                    {
                        match = match.NextMatch();
                        string matchmore = match.Groups[4].Value + " posted on the thread: \"" + match.Groups[3].Value + "\", " + match.Groups[5].Value.ToLower() + " at " + match.Groups[6].Value + " - " + match.Groups[1].Value + match.Groups[2].Value;
                        if (!textreturn.Contains(matchmore))
                        {
                            returnResults += match1 + "\n";
                        }
                    }
                    LatestStartup();
                }
                a.SendMessageAsync(returnResults);
                Thread.Sleep(30000);
            }
        }
    }
}
