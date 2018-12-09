using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RPBot
{
    [Group("card"), IsMuted, RequireRoles(RoleCheckMode.Any, "Staff")]
    class CardClass : BaseCommandModule
    {
        [Command("create")]
        public async Task ShowCard(CommandContext e, [Description("Name of the character")]string characterName, [Description("Role - 'Hero', 'Villain' or 'Rogue'")]string role,
            [Description("Image for the front - URL")]string frontImage,
            [Description("Icon for the back - URL")]string backImage, [Description("Hex colour of background")]string hex, [Description("Location of birth")]string born, [Description("Height (feet & inches)")]string height, [Description("Weight (kg)")]string weight,
            [Description("Quirk name")]string quirk, [Description("Power (/5)")]string power, [Description("Intelligence (/5)")]string intelligence, [Description("Speed (/5)")]string speed,
            [Description("Agility (/5)")]string agility, [Description("Technique (/5)")]string technique, [Description("Precision (/5)")]string precision, [Description("Quote title")]string quoteTitle,
            [Description("Quote content")]string quote)
        {

            if (File.Exists($"Cards/Done/front-{characterName}.png"))
            {
                await e.RespondAsync("Card already exists. Use the command `!card delete NAME`.");
                return;
            }

            Roles r = Roles.Hero;
            if (role == "Hero") r = Roles.Hero;
            else if (role == "Villain") r = Roles.Villain;
            else if (role == "Rogue") r = Roles.Rogue;
            else
            {
                await e.RespondAsync("Invalid role type.");
                return;
            }
            string fileStamp = DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss");

            using (WebClient webClient = new WebClient())
            {
                webClient.DownloadFile(frontImage, $"Cards/frontimage-{fileStamp}.png");
            }
            string front = GenerateFront(characterName, r, $"Cards/frontimage-{fileStamp}.png");

            using (WebClient webClient = new WebClient())
            {
                webClient.DownloadFile(backImage, $"Cards/backimage-{fileStamp}.png");
            }

            string back = GenerateBack($"Cards/backimage-{fileStamp}.png", hex, r, characterName, born, height, weight, quirk, power, intelligence, speed, agility, technique, precision, quoteTitle, quote);

            RPClass.CardList.Add(characterName, $"{front}¬{back}");
            await e.RespondWithFileAsync(front);
            await e.RespondWithFileAsync(back);
            await e.Message.DeleteAsync();

            File.Delete($"Cards/frontimage-{fileStamp}.png");
            File.Delete($"Cards/backimage-{fileStamp}.png");
            RPClass.SaveData(5);
        }

        [Command("show")]
        public async Task ShowCard(CommandContext e, [Description("Name of the character")]string characterName)
        {
            if (RPClass.CardList.Any(x => x.Key == characterName))
            {
                string[] vals = RPClass.CardList[characterName].Split('¬');
                await e.RespondWithFileAsync(vals[0]);
                await e.RespondWithFileAsync(vals[1]);

            }
        }

        [Command("add")]
        public async Task AddCard(CommandContext e, [Description("Name of the character")]string characterName, [Description("Front Image")]string front, [Description("Back image")]string back)
        {
            if (File.Exists($"Cards/Done/front-{characterName}.png"))
            {
                await e.RespondAsync("Card already exists. Use the command `!card delete NAME`.");
                return;
            }
            using (WebClient webClient = new WebClient())
            {
                webClient.DownloadFile(front, $"Cards/Done/front-{characterName}.png");
            }

            using (WebClient webClient = new WebClient())
            {
                webClient.DownloadFile(back, $"Cards/Done/back-{characterName}.png");
            }

            RPClass.CardList.Add(characterName, $"Cards/Done/front-{characterName}.png¬Cards/Done/back-{characterName}.png");
            RPClass.SaveData(5);
            await e.RespondAsync("Added!");
        }
        [Command("list"), Description("Lists all cards.")]
        public async Task List(CommandContext e)
        {
            var interactivity = e.Client.GetInteractivity();
            List<Page> interactivityPages = new List<Page>();

            Page p = new Page();

            DiscordEmbedBuilder b = new DiscordEmbedBuilder()
            {
                Color = new DiscordColor("4169E1"),
                Timestamp = DateTime.UtcNow
            }
            .WithFooter("Heroes & Villains");
            bool even = false;
            foreach (string name in RPClass.CardList.Keys)
            {
                if (!even)
                {
                    b.AddField(name, "-");
                }
                else
                {
                    b.Fields.Last().Value = name;
                }
                even = !even;
                if (b.Fields.Count >= 10 && !even)
                {
                    p.Embed = b;
                    interactivityPages.Add(p);
                    p = new Page();
                    b.ClearFields();
                    even = false;
                }
            }
            p.Embed = b;
            interactivityPages.Add(p);
            p = new Page();
            b.ClearFields();
            await interactivity.SendPaginatedMessage(e.Channel, e.Member, interactivityPages, timeoutoverride: TimeSpan.FromSeconds(60));
        }

        [Command("delete")]
        public async Task DeleteCard(CommandContext e, [Description("Name of the character")]string characterName)
        {
            if (File.Exists($"Cards/Done/front-{characterName}.png"))
            {
                File.Delete($"Cards/Done/front-{characterName}.png");
            }
            if (File.Exists($"Cards/Done/back-{characterName}.png"))
            {
                File.Delete($"Cards/Done/back-{characterName}.png");
            }
            if (RPClass.CardList.Any(x => x.Key == characterName))
            {
                RPClass.CardList.Remove(characterName);
            }
            RPClass.SaveData(5);
            await e.RespondAsync("Done!");
        }

        public static string GenerateFront(string name, Roles role, string filePath)
        {
            string hexcode = "#0B1133";
            string border = "Cards/hero_frontoverlay.png";

            if (role == Roles.Villain)
            {
                border = "Cards/villain_frontoverlay.png";
                hexcode = "#670000";
            }
            else if (role == Roles.Rogue)
            {
                border = "Cards/rogue_frontoverlay.png";
                hexcode = "#350022";
            }

            var fontCollection = new FontCollection();
            var parentFont = fontCollection.Install("Cards/front.tff").CreateFont(50, FontStyle.Bold);

            var childFont = parentFont;
            using (Image<Rgba32> backgroundImage = Image.Load(filePath))
            {
                using (Image<Rgba32> borderImage = Image.Load(border))
                {
                    int fontSize = 50;
                    float textOffset = 0;
                    while (true)
                    {
                        var size = TextMeasurer.Measure(name, new RendererOptions(childFont));
                        if (size.Width > 350)
                        {
                            fontSize -= 5;
                            childFont = new Font(parentFont, fontSize);
                            continue;
                        }

                        textOffset = 250 - size.Width / 2;
                        break;
                    }
                    backgroundImage.Mutate(x => x.Resize(new ResizeOptions() { Mode = ResizeMode.Crop, Size = new Size(500, 700), Position = AnchorPositionMode.Top }).DrawImage(borderImage, 1).DrawText(name, childFont, Brushes.Solid(Rgba32.FromHex("#FFFFFF")), Pens.Solid(Rgba32.FromHex(hexcode), 8), new PointF(textOffset, 630)).DrawText(name, childFont, Brushes.Solid(Rgba32.FromHex("#FFFFFF")), new PointF(textOffset, 630)));

                    backgroundImage.Save($"Cards/Done/front-{name}.png");
                    return $"Cards/Done/front-{name}.png";
                }
            }
        }

        public static string GenerateBack(string filePath, string backgroundColour, Roles role, string name, string born, string height, string weight, string quirk, string power, string intelligence, string speed, string agility, string technique, string precision, string quoteTitle, string quoteContent)
        {
            string border = "Cards/hero_backoverlay.png";
            string boxes = "Cards/hero_boxes.png";
            string background = "Cards/background.png";

            if (role == Roles.Villain)
            {
                border = "Cards/villain_backoverlay.png";
                boxes = "Cards/villain_boxes.png";
            }
            else if (role == Roles.Rogue)
            {
                border = "Cards/rogue_backoverlay.png";
                boxes = "Cards/rogue_boxes.png";
            }
            var fontCollection = new FontCollection();
            var parentFont = fontCollection.Install("Cards/back.ttf").CreateFont(25);
            using (Image<Rgba32> backgroundImage = Image.Load(background))
            using (Image<Rgba32> boxesImage = Image.Load(boxes))
            using (Image<Rgba32> iconImage = Image.Load(filePath))
            using (Image<Rgba32> borderImage = Image.Load(border))
            {
                int fontSize = 23;
                while (true)
                {
                    SizeF nameSize = TextMeasurer.Measure(name, new RendererOptions(new Font(parentFont, fontSize)));
                    SizeF bornSize = TextMeasurer.Measure(born, new RendererOptions(new Font(parentFont, fontSize)));
                    SizeF heightSize = TextMeasurer.Measure(height, new RendererOptions(new Font(parentFont, fontSize)));
                    SizeF weightSize = TextMeasurer.Measure(weight, new RendererOptions(new Font(parentFont, fontSize)));
                    SizeF quirkSize = TextMeasurer.Measure(quirk, new RendererOptions(new Font(parentFont, fontSize)));

                    float maxWidth = nameSize.Width;
                    if (bornSize.Width > maxWidth) maxWidth = bornSize.Width;
                    if (heightSize.Width > maxWidth) maxWidth = heightSize.Width;
                    if (weightSize.Width > maxWidth) maxWidth = weightSize.Width;
                    if (quirkSize.Width > maxWidth) maxWidth = quirkSize.Width;

                    if (maxWidth > 111)
                    {
                        fontSize -= 1;
                        continue;
                    }

                    //textOffset = 250 - size.Width / 2;
                    break;
                }
                iconImage.Mutate(x => x.Resize(new ResizeOptions() { Mode = ResizeMode.Crop, Size = new Size(187, 185), Position = AnchorPositionMode.Top }));
                backgroundImage.Mutate(x => x.BackgroundColor(Rgba32.FromHex(backgroundColour)).DrawImage(iconImage, 1, new Point(29, 119)).DrawImage(boxesImage, 1).DrawImage(borderImage, 1));

                backgroundImage.Mutate(x => x.DrawText("NAME: ", parentFont, Rgba32.White, new PointF(246, 130)).DrawText(name, new Font(parentFont, fontSize), Rgba32.White, new PointF(300, 131)));
                backgroundImage.Mutate(x => x.DrawText("BORN IN: ", parentFont, Rgba32.White, new PointF(246, 155)).DrawText(born, new Font(parentFont, fontSize), Rgba32.White, new PointF(325, 156)));
                backgroundImage.Mutate(x => x.DrawText("HEIGHT: ", parentFont, Rgba32.White, new PointF(246, 180)).DrawText(height, new Font(parentFont, fontSize), Rgba32.White, new PointF(315, 181)));
                backgroundImage.Mutate(x => x.DrawText("WEIGHT: ", parentFont, Rgba32.White, new PointF(246, 205)).DrawText(weight, new Font(parentFont, fontSize), Rgba32.White, new PointF(325, 206)));
                backgroundImage.Mutate(x => x.DrawText("QUIRK: ", parentFont, Rgba32.White, new PointF(246, 230)).DrawText(quirk, new Font(parentFont, fontSize), Rgba32.White, new PointF(305, 231)));

                backgroundImage.Mutate(x => x.DrawText("POWER:  " + power + " / 5", parentFont, Rgba32.White, new PointF(246, 290)));
                backgroundImage.Mutate(x => x.DrawText("INTELLIGENCE:  " + intelligence + " / 5", parentFont, Rgba32.White, new PointF(246, 315)));
                backgroundImage.Mutate(x => x.DrawText("SPEED:  " + speed + " / 5", parentFont, Rgba32.White, new PointF(246, 340)));
                backgroundImage.Mutate(x => x.DrawText("AGILITY:  " + agility + " / 5", parentFont, Rgba32.White, new PointF(246, 365)));
                backgroundImage.Mutate(x => x.DrawText("TECHNIQUE:  " + technique + " / 5", parentFont, Rgba32.White, new PointF(246, 390)));
                backgroundImage.Mutate(x => x.DrawText("PRECISION:  " + precision + " / 5", parentFont, Rgba32.White, new PointF(246, 415)));

                int quoteTitleFontSize = 50;
                int rows = 1;
                string firstQuoteTitle = quoteTitle;
                string secondQuoteTitle = "";

                int response = GetFontSize(quoteTitle, parentFont);

                if (response == -1)
                {
                    var resp = GetNLines(quoteTitle, 2);
                    firstQuoteTitle = resp[0];
                    secondQuoteTitle = resp[1];
                    rows = 2;
                    if (resp[0].Length > resp[1].Length)
                        quoteTitleFontSize = GetFontSize(resp[0], parentFont, true);
                    else
                        quoteTitleFontSize = GetFontSize(resp[1], parentFont, true);

                }
                else
                {
                    quoteTitleFontSize = response;
                }
                var firstQuoteTextSize = TextMeasurer.Measure(firstQuoteTitle, new RendererOptions(new Font(parentFont, quoteTitleFontSize)));
                SizeF secondQuoteTextSize = new SizeF();
                backgroundImage.Mutate(x => x.DrawText(firstQuoteTitle, new Font(parentFont, quoteTitleFontSize), Rgba32.White, new PointF(250 - firstQuoteTextSize.Width / 2, 498)));
                if (rows == 2)
                {
                    secondQuoteTextSize = TextMeasurer.Measure(secondQuoteTitle, new RendererOptions(new Font(parentFont, quoteTitleFontSize)));

                    backgroundImage.Mutate(x => x.DrawText(secondQuoteTitle, new Font(parentFont, quoteTitleFontSize), Rgba32.White, new PointF(250 - secondQuoteTextSize.Width / 2, 498 + firstQuoteTextSize.Height + 3)));
                }

                var quoteContentObj = GetLines(quoteContent, parentFont, quoteTitleFontSize - 2, 675 - (503 + (int)firstQuoteTextSize.Height + (int)secondQuoteTextSize.Height));

                int test = 503 + (int)firstQuoteTextSize.Height + (int)secondQuoteTextSize.Height;
                foreach (var line in quoteContentObj.Lines)
                {
                    var lineSize = TextMeasurer.Measure(line, new RendererOptions(new Font(parentFont, quoteContentObj.FontSize)));

                    backgroundImage.Mutate(x => x.DrawText(line, new Font(parentFont, quoteContentObj.FontSize), Rgba32.White, new PointF(250 - lineSize.Width / 2, test)));
                    test += 3 + (int)lineSize.Height;
                }

                backgroundImage.Save($"Cards/Done/back-{name}.png");
                return $"Cards/Done/back-{name}.png";
            }
        }
        public static List<string> GetNLines(string input, int num)
        {
            var wordList = new List<string>();

            var wordslists = input.Split(' ').ToList().Partition(num);
            foreach (var x in wordslists)
            {
                string response = string.Join(" ", x.ToArray());
                wordList.Add(response);
            }
            return wordList;
        }

        public static int GetFontSize(string input, Font f, bool bypassMinimum = false)
        {
            int fontSize = 50;
            while (true)
            {
                var size = TextMeasurer.Measure(input, new RendererOptions(new Font(f, fontSize)));
                if (size.Width > 400)
                {
                    fontSize -= 1;

                    if (fontSize <= 35 && bypassMinimum == false)
                    {
                        return -1;
                    }
                    continue;
                }
                break;
            }
            return fontSize;
        }

        public static int GetRows(Font f, int fontSize, int maxSize)
        {
            int maxRows = 0;
            float totalHeight = 0;

            while (true)
            {
                var size = TextMeasurer.Measure("p", new RendererOptions(new Font(f, fontSize)));
                totalHeight += size.Height + (fontSize / 5);
                if (totalHeight > maxSize)
                {
                    return maxRows;
                }
                else
                {
                    maxRows += 1;
                }
            }
        }

        public static bool CheckWidth(string input, Font f, int fontSize, int maxWidth)
        {
            var size = TextMeasurer.Measure(input, new RendererOptions(new Font(f, fontSize)));
            if (size.Width < maxWidth) return true;
            return false;
        }
        public static Quote GetLines(string input, Font f, int maxFont, int maxHeight)
        {
            int fontSize = maxFont;
            for (int i = fontSize; i > 2; i--)
            {
                int rows = GetRows(f, i, maxHeight);
                for (int j = 1; j <= rows; j++)
                {
                    int successes = 0;
                    var lines = GetNLines(input, j);
                    foreach (var line in lines)
                    {
                        if (CheckWidth(line, f, i, 440))
                        {
                            successes += 1;
                            continue;
                        }
                    }
                    if (successes != j && j == rows)
                    {
                        break;
                    }
                    else if (successes == j && j == rows)
                    {
                        return new Quote(i, lines);
                    }
                }
            }
            return null;
        }

    }
    
    public class Quote
    {
        public int FontSize;
        public List<string> Lines;

        public Quote(int fontSize, List<string> lines)
        {
            FontSize = fontSize;
            Lines = lines;
        }
    }

    public class Row
    {
        public int RowNum;
        public int FontSize;

        public Row(int rowNum, int fontSize)
        {
            FontSize = fontSize;
            RowNum = rowNum;
        }
    }

    public class CardObject
    {
        public string Name;
        public string FilePath;
    }
    enum Roles
    {
        Hero,
        Villain,
        Rogue
    }
}
