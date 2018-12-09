using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using SixLabors.Shapes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPBot
{
    [Group("stats"), IsMuted]
    class StatsClass : BaseCommandModule
    {
        public static IReadOnlyCollection<PointF> Points = new List<PointF>() { new PointF(0, -618), new PointF(409, -467), new PointF(629, -88), new PointF(553, 337), new PointF(220, 618), new PointF(-220, 618), new PointF(-553, 337), new PointF(-629, -88), new PointF(-409, -467) };
        public static string[] Names = new string[] { "Melee Attack", "Ranged Attack", "Mobility", "Dodge", "Durability", "Utility", "Healing", "Influence", "Potential" };

        [Command("show")]
        public async Task ShowStats(CommandContext e, DiscordMember d = null)
        {
            if (d == null)
                d = e.Member;

            var user = RPClass.Users.FirstOrDefault(x => x.UserData.UserID == d.Id);

            if (user == null)
            {
                await e.RespondAsync("Fail. This is awkward.");
                return;
            }
            if (user.Stats.Dodge == 0)
            {
                await e.RespondAsync("Fail. No data yet in system.");
                return;
            }
            string response = BuildGraph(user.Stats.GetList(), user.GetRank().First(), user.UserData.Username);
            await e.RespondWithFileAsync(response, $"Your current rank: {user.GetRank()}");
            File.Delete(response);
        }

        [Command("set"), RequireRoles(RoleCheckMode.Any, "Staff")]
        public async Task SetStats(CommandContext e, DiscordMember d, [Description("Give all stats with commas separating them, in the order: " +
        "Melee Attack, Ranged Attack, Mobility, Dodge, Durability, Utility, Healing, Influence, Potential." +
        "\n e.g. 'A+,B-,A-,C+,F+,D,A,S,N/A'."), RemainingText] string stats)
        {
            var user = RPClass.Users.FirstOrDefault(x => x.UserData.UserID == d.Id);

            if (user == null)
            {
                await e.RespondAsync("Fail. This is awkward.");
                return;
            }
            string[] splitStats = stats.Split(',');
            DiscordEmbedBuilder b = new DiscordEmbedBuilder()
            {
                Title = $"Stats for {d.DisplayName}",
                Description = "Stats changed. New values:"
            };

            int[] totalPoints = new int[9];
            for (int i = 0; i < 9; i++)
            {
                int response = ScoreToPoints(splitStats[i].ToUpper());
                if (response == 0)
                {
                    await e.RespondAsync("Invalid response.");
                    return;
                }
                b.AddField(Names[i], $"Points: {response}");
                totalPoints[i] = response;
            }
            user.Stats.SetList(totalPoints);
            await e.RespondAsync(embed: b.Build());
        }

        public static string BuildGraph(int[] stats, char rank, string name)
        {
            using (Image<Rgba32> img = Image.Load("Data/template.png"))
            {
                IPath userGraph = BuildPath(stats);
                var DRanks = RPClass.Users.Where(x => x.GetRank().Contains(rank)).Where(x => x.Stats.Melee != 0).Select(x => x.Stats.GetList());

                int[] total = new int[9];
                int[] data = new int[9];
                foreach (var array in DRanks)
                {
                    for (int i = 0; i < 9; i++)
                    {
                        total[i] += array[i];
                    }
                }
                for (int i = 0; i < 9; i++)
                {
                    data[i] = total[i] / DRanks.Count();
                }
                IPath rankGraph = BuildPath(data);
                img.Mutate(x =>
                    x.Fill(Brushes.Percent20(Rgba32.Red), rankGraph)
                    .Draw(Rgba32.Red, 6, rankGraph)
                    .Fill(Brushes.Percent20(Rgba32.Blue), userGraph)
                    .Draw(Rgba32.Blue, 6, userGraph));

                string fileName = Extensions.RandomString(5);
                img.Save($"Data/{fileName}.png");
                return $"Data/{fileName}.png";
            }
        }

        public static IPath BuildPath(int[] data)
        {
            var points = Points.ToArray();
            PathBuilder pb = new PathBuilder();
            pb.SetOrigin(new PointF(846, 734));
            for (int i = 0; i < 9; i++)
            {
                float enumerator = ((float)1 / 21) * data[i];
                points[i].X *= enumerator;
                points[i].Y *= enumerator;
            }
            for (int i = 0; i < 9; i++)
            {
                if (i < 8)
                    pb.AddLine(points[i], points[i + 1]);
                else
                    pb.AddLine(points[8], points[0]);
            }
            return pb.Build();
        }

        public static int ScoreToPoints(string score)
        {
            int points = 0;
            if (score == "N/A")
            {
                points = 1;
            }
            else
            {
                switch (score[0])
                {
                    case 'S':
                        points = 20;
                        break;
                    case 'A':
                        points = 17;
                        break;
                    case 'B':
                        points = 14;
                        break;
                    case 'C':
                        points = 11;
                        break;
                    case 'D':
                        points = 8;
                        break;
                    case 'E':
                        points = 5;
                        break;
                    case 'F':
                        points = 2;
                        break;
                }
                if (score.Length > 1)
                {
                    if (score[1] == '+')
                        points += 1;
                    else if (score[1] == '-')
                        points -= 1;
                }
            }
            return points;
        }
    }
}
