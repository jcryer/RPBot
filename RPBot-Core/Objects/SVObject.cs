using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPBot
{
    public static class EnumerableExtension
    {
        public static T PickRandom<T>(this IEnumerable<T> source)
        {
            return source.PickRandom(1).Single();
        }

        public static IEnumerable<T> PickRandom<T>(this IEnumerable<T> source, int count)
        {
            return source.Shuffle().Take(count);
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            return source.OrderBy(x => Guid.NewGuid());
        }
    }

    class SVObject
    {
        public class VoteObject
        {
            public SVObject.UserObject user;
            public int voteNum;

            public VoteObject(SVObject.UserObject user, int voteNum)
            {
                this.user = user;
                this.voteNum = voteNum;
            }
        }
        public class RootObject
        {
            public List<UserObject> Players { get; set; }
            public bool Active { get; set; }
            public bool Started { get; set; }
            public DateTime Timer { get; set; }

            public RootObject()
            {
                Players = new List<UserObject>();
            }
            public async Task DisplayData(CommandContext e, bool Roles)
            {
                DiscordEmbedBuilder embed = new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Blue,
                    Title = "Secret Villain™"
                }
                .WithFooter("Mournstead");

                foreach (UserObject user in Players)
                {
                    string statusString = user.Status == 0 ? "Alive" : user.Status == 1 ? "Dead" : "ERROR, Please report this to jcryer.";
                    string playerNumString = user.PlayerNum == -1 ? "N/A" : user.Status >= 0 ? user.PlayerNum.ToString() : "ERROR, Please report this to jcryer.";
                    string roleType = user.Role == 1 ? "Villain" : user.Role == 2 ? "Medic" : user.Role == 3 ? "Spy" : user.Role == 4 ? "Joker" : user.Role == 5 ? "Hero" : "ERROR, Please report this to jcryer.";

                    if (Roles)
                    {
                        embed.AddField(user.Username, "Role: " + roleType);
                    }
                    else
                    {
                        embed.AddField(user.Username, "Player number: " + playerNumString + Environment.NewLine + "Status: " + statusString);
                    }
                }
                
                await e.RespondAsync("", embed: embed);
            }

            public async Task DisplayData(DiscordDmChannel d, bool Roles)
            {
                DiscordEmbedBuilder embed = new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Blue,
                    Title = "Secret Villain™"
                }
                .WithFooter("Mournstead");

                foreach (UserObject user in Players)
                {
                    string statusString = user.Status == 0 ? "Alive" : user.Status == 1 ? "Dead" : "ERROR, Please report this to Jcryer.";
                    string playerNumString = user.PlayerNum == -1 ? "N/A" : user.Status >= 0 ? user.PlayerNum.ToString() : "ERROR, Please report this to Jcryer.";
                    string roleType = user.Role == 1 ? "Villain" : user.Role == 2 ? "Medic" : user.Role == 3 ? "Spy" : user.Role == 4 ? "Joker" : user.Role == 5 ? "Hero" : "ERROR, Please report this to Jcryer.";

                    if (Roles)
                    {
                        embed.AddField(user.Username,"Role: " + roleType);
                    }
                    else
                    {
                        embed.AddField(user.Username, "Player number: " + playerNumString + Environment.NewLine + "Status: " + statusString);
                    }
                }

                await d.SendMessageAsync("", embed: embed);
            }

            public async Task StartGame(CommandContext e)
            {

                await e.RespondAsync("Game started.");
                Started = true;
                int Total = Players.Count;
                int playerNumIncrement = Total;

                int Villains = Convert.ToInt32(Math.Floor((float)(Total / 4)));
                int Medics = Convert.ToInt32(Math.Floor((float)(Villains / 2)));
                int Spies = Convert.ToInt32(Math.Floor((float)(Villains / 2)));
                int Joker = Convert.ToInt32(Math.Floor((float)(Total / 6)));
                int Heroes = Total - Villains - Medics - Spies - Joker;
                await e.RespondAsync("```There are " + Total + " players in this game, meaning there will be:\n" + Villains + " Villain(s),\n" + Medics + " Medic(s),\n" + Spies + " Spies,\n" + Joker + " Joker(s),\n" + Heroes + " Heroes.```");

                for (int i = 0; i < Villains; i++)
                {
                    List<UserObject> usersLeft = Players.Where(x => x.Role < 0).ToList();
                    int r = Program.random.Next(usersLeft.Count(x => x.Role < 0));
                    usersLeft[r].Role = 1;
                }
                for (int i = 0; i < Medics; i++)
                {
                    List<UserObject> usersLeft = Players.Where(x => x.Role < 0).ToList();
                    int r = Program.random.Next(usersLeft.Count(x => x.Role < 0));
                    usersLeft[r].Role = 2;
                }
                for (int i = 0; i < Spies; i++)
                {
                    List<UserObject> usersLeft = Players.Where(x => x.Role < 0).ToList();
                    int r = Program.random.Next(usersLeft.Count(x => x.Role < 0));
                    usersLeft[r].Role = 3;
                }
                for (int i = 0; i < Joker; i++)
                {
                    List<UserObject> usersLeft = Players.Where(x => x.Role < 0).ToList();
                    int r = Program.random.Next(usersLeft.Count(x => x.Role < 0));
                    usersLeft[r].Role = 4;
                }
                for (int i = 0; i < Heroes; i++)
                {
                    List<UserObject> usersLeft = Players.Where(x => x.Role < 0).ToList();
                    int r = Program.random.Next(usersLeft.Count(x => x.Role < 0));
                    usersLeft[r].Role = 5;
                }
                List<int> numberList = Enumerable.Range(0, Total).ToList();
                for (int i = 0; i < Total; i++)
                {
                    int rnd = numberList.PickRandom();
                    Players[i].PlayerNum = rnd;
                    numberList.Remove(rnd);
                }
                Players = Players.OrderBy(x => x.PlayerNum).ToList();

                try
                {
                    foreach (UserObject userData in Players)
                    {
                        DiscordMember user = await e.Guild.GetMemberAsync(userData.ID);
                        DiscordDmChannel dm = await user.CreateDmChannelAsync();
                        string roleType = userData.Role == 1 ? "Villain" : userData.Role == 2 ? "Medic" : userData.Role == 3 ? "Spy" : userData.Role == 4 ? "Joker" : userData.Role == 5 ? "Hero" : "ERROR, Please report this to Jcryer.";
                        if (userData.Role == 1)
                        {
                            if (Players.Count(x => x.Role == 1) > 1)
                            {
                                await dm.SendMessageAsync("Hi! your role is: Villain, and the other Villains are: ");
                                foreach (UserObject a in Players.FindAll(x => x.Role == 1 && x.ID != user.Id))
                                {
                                    await dm.SendMessageAsync(a.Username);
                                }
                            }
                            else
                            {
                                await dm.SendMessageAsync("Hi! your role is: Villain.");
                            }
                        }
                        else
                        {
                            await dm.SendMessageAsync("Hi! Your role is: " + roleType);
                        }
                        await DisplayData(dm, false);
                    }
                }
                catch { }
            }
        }
        
        public class UserObject
        {
            public ulong ID { get; set; }
            public string Username { get; set; }
            public int Role { get; set; }
            public int Status { get; set; }
            public int PlayerNum { get; set; }

            public UserObject(ulong ID, string username, int role, int status)
            {
                this.ID = ID;
                this.Username = username;
                this.Role = role;
                this.Status = status;
            }
        }
    }
}
