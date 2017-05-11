using System.IO;
using System.Drawing;
using System.Linq;
using DSharpPlus;
using System;
using System.Collections.Generic;

namespace SLOBot
{
    class Reactions : Program
    {
        public static bool active = false;
        public static Dictionary<DiscordUser, int> ReactionsScores = new Dictionary<DiscordUser, int>();
        public static void ReactionTest(DiscordChannel channel, string cmd)
        {
            string cmdEdited = "";
            try
            {
                cmdEdited = cmd.Substring(10);
            }
            catch
            {}

            if (!string.IsNullOrWhiteSpace(cmdEdited)) {
                if (cmdEdited == "start")
                {
                    if (active == false)
                    {
                        channel.SendMessageAsync("Welcome to the Reactions Test Game, written by jcryer and inspired by Mars.");
                        channel.SendMessageAsync("Game will begin shortly.");
                        RandomStringImage();

                        channel.SendFileAsync(new FileStream("text1.png", FileMode.Open), "text1.png");

                        active = true;
                    }
                    else 
                    {
                        channel.SendMessageAsync("Reactions Test Game is already active.");
                    }
                }
                else if (cmdEdited == "stop")
                {
                    if (active == true)
                    {
                        channel.SendMessageAsync("Reactions Test Game ended.");
                        active = false;
                    }
                    else
                    {
                        channel.SendMessageAsync("There is no active Reactions Test Game.");
                    }
                }
                
            }

        }

        public static void RandomStringImage()
        {
            Font arialFont = new Font("Arial", 20);

            string CurrentString = RandomString(10);
            Image img = new Bitmap(1, 1);

            Graphics drawing = Graphics.FromImage(img);
            SizeF textSize = drawing.MeasureString(CurrentString, arialFont);
            img.Dispose();
            drawing.Dispose();
            img = new Bitmap((int)textSize.Width, (int)textSize.Height);
            drawing = Graphics.FromImage(img);
            drawing.Clear(Color.White);

            Brush textBrush = new SolidBrush(Color.Black);
            drawing.DrawString(CurrentString, arialFont, textBrush, 0, 0);
            drawing.Save();

            textBrush.Dispose();
            drawing.Dispose();
            img.Save("text1.png", System.Drawing.Imaging.ImageFormat.Png);
        }
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }


       /* public static string Scoreboard()
        {
            int ScoreCount = 0;
            Dictionary<DiscordUser, int> ScoresOrdered = new Dictionary<DiscordUser, int>();
            foreach (KeyValuePair<DiscordUser, int> a in ReactionsScores.OrderBy(key => key.Value)) 
            {
                if (ScoreCount < 10)
                {
                    ScoresOrdered.Add(a.Key, a.Value);
                }

                ScoreCount++;
            }
            string FullMessage = "```Reactions Test Game Scoreboard:" + Environment.NewLine;

            if (ScoreCount > 10) ScoreCount = 10;
            for (int i = 1; i <= ScoreCount; i++)
            {
                FullMessage += i + ": " + ScoresOrdered. +" - Level " + LevelsSorted[LevelsSorted.Count - i].Value + Environment.NewLine;
            }
            e.Channel.SendMessage(FullMessage + "```");
        }*/
    }
}
