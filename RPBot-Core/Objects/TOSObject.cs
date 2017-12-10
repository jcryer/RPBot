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
    class TOSObject
    {
        public class RootObject
        {
            public List<UserObject> players { get; set; }
            public bool active { get; set; }
            public bool started { get; set; }
            public DateTime timer { get; set; }
            
            public RootObject()
            {
                players = new List<UserObject>();
            }
            public async Task DisplayData(CommandContext e, bool Roles)
            {
                DiscordEmbedBuilder embed = new DiscordEmbedBuilder();

                foreach (UserObject user in players)
                {
                    string statusString = user.status == 0 ? "Alive" : user.status == 1 ? "Dead" : "ERROR, Please report this to jcryer.";
                    string playerNumString = user.playerNum == -1 ? "N/A" : user.status >= 0 ? user.playerNum.ToString() : "ERROR, Please report this to jcryer.";
                    string roleType = user.role == 1 ? "Villain" : user.role == 2 ? "Medic" : user.role == 3 ? "Spy" : user.role == 4 ? "Joker" : user.role == 5 ? "Hero" : "ERROR, Please report this to jcryer.";

                    if (Roles)
                    {
                        embed.AddField(user.username, "Role: " + roleType);
                    }
                    else
                    {
                        embed.AddField(user.username, "Player number: " + playerNumString + Environment.NewLine + "Status: " + statusString);
                    }
                }
                embed.Color = DiscordColor.Blue;
                embed.Title = "Town of Salem";
                embed.WithFooter("Heroes & Villains");

                await e.RespondAsync("", embed: embed);
            }

            public async Task DisplayData(DiscordDmChannel d, bool Roles)
            {
                DiscordEmbedBuilder embed = new DiscordEmbedBuilder();


                foreach (UserObject user in players)
                {
                    string statusString = user.status == 0 ? "Alive" : user.status == 1 ? "Dead" : "ERROR, Please report this to Jcryer.";
                    string playerNumString = user.playerNum == -1 ? "N/A" : user.status >= 0 ? user.playerNum.ToString() : "ERROR, Please report this to Jcryer.";
                    string roleType = user.role == 1 ? "Villain" : user.role == 2 ? "Medic" : user.role == 3 ? "Spy" : user.role == 4 ? "Joker" : user.role == 5 ? "Hero" : "ERROR, Please report this to Jcryer.";

                    if (Roles)
                    {
                        embed.AddField(user.username,"Role: " + roleType);
                    }
                    else
                    {
                        embed.AddField(user.username, "Player number: " + playerNumString + Environment.NewLine + "Status: " + statusString);
                    }
                }
                embed.Color = DiscordColor.Blue;
                embed.Title = "Town of Salem";
                embed.WithFooter("Heroes & Villains");

                await d.SendMessageAsync("", embed: embed);
            }

            public async Task StartGame(CommandContext e)
            {

                await e.RespondAsync("Game started.");
                started = true;
                int Total = players.Count;
                int playerNumIncrement = Total;

                int Villains = Convert.ToInt32(Math.Floor((float)(Total / 4)));
                int Medics = Convert.ToInt32(Math.Floor((float)(Villains / 2)));
                int Spies = Convert.ToInt32(Math.Floor((float)(Villains / 2)));
                int Joker = Convert.ToInt32(Math.Floor((float)(Total / 6)));
                int Heroes = Total - Villains - Medics - Spies - Joker;
                await e.RespondAsync("```There are " + Total + " players in this game, meaning there will be:" + Environment.NewLine + Villains + " Villain(s)," + Environment.NewLine + Medics + " Medic(s)," + Environment.NewLine + Spies + " Spies," + Environment.NewLine + Joker + " Joker(s)," + Environment.NewLine + Heroes + " Heroes.```");

                for (int i = 0; i < Villains; i++)
                {
                    List<UserObject> usersLeft = players.Where(x => x.role < 0).ToList();
                    int r = Program.random.Next(usersLeft.Count(x => x.role < 0));
                    usersLeft[r].role = 1;
                }
                for (int i = 0; i < Medics; i++)
                {
                    List<UserObject> usersLeft = players.Where(x => x.role < 0).ToList();
                    int r = Program.random.Next(usersLeft.Count(x => x.role < 0));
                    usersLeft[r].role = 2;
                }
                for (int i = 0; i < Spies; i++)
                {
                    List<UserObject> usersLeft = players.Where(x => x.role < 0).ToList();
                    int r = Program.random.Next(usersLeft.Count(x => x.role < 0));
                    usersLeft[r].role = 3;
                }
                for (int i = 0; i < Joker; i++)
                {
                    List<UserObject> usersLeft = players.Where(x => x.role < 0).ToList();
                    int r = Program.random.Next(usersLeft.Count(x => x.role < 0));
                    usersLeft[r].role = 4;
                }
                for (int i = 0; i < Heroes; i++)
                {
                    List<UserObject> usersLeft = players.Where(x => x.role < 0).ToList();
                    int r = Program.random.Next(usersLeft.Count(x => x.role < 0));
                    usersLeft[r].role = 5;
                }
                List<int> numberList = Enumerable.Range(0, Total).ToList();
                for (int i = 0; i < Total; i++)
                {
                    int rnd = numberList.PickRandom();
                    players[i].playerNum = rnd;
                    numberList.Remove(rnd);
                }
                players = players.OrderBy(x => x.playerNum).ToList();

                try
                {
                    foreach (UserObject userData in players)
                    {
                        DiscordMember user = await e.Guild.GetMemberAsync(userData.ID);
                        DiscordDmChannel dm = await user.CreateDmChannelAsync();
                        string roleType = userData.role == 1 ? "Villain" : userData.role == 2 ? "Medic" : userData.role == 3 ? "Spy" : userData.role == 4 ? "Joker" : userData.role == 5 ? "Hero" : "ERROR, Please report this to Jcryer.";
                        if (userData.role == 1)
                        {
                            if (players.Count(x => x.role == 1) > 1)
                            {
                                await dm.SendMessageAsync("Hi! your role is: Villain, and the other Villains are: ");
                                foreach (UserObject a in players.FindAll(x => x.role == 1 && x.ID != user.Id))
                                {
                                    await dm.SendMessageAsync(a.username);
                                }
                            }
                            else
                            {
                                await dm.SendMessageAsync("Hi! your role is: Assassin.");
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
            public string username { get; set; }
            public int role { get; set; }
            public int status { get; set; }
            public int playerNum { get; set; }

            public UserObject(ulong ID, string username, int role, int status)
            {
                this.ID = ID;
                this.username = username;
                this.role = role;
                this.status = status;
            }
        }
    }
}
