using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RPBot
{
    [Group("sv")]
    class SVClass : RPClass
    {
        public static bool active = false;
        public static bool processing = false;
        public static int phase = 0;

        public static List<SVObject.UserObject> UsedAbility = new List<SVObject.UserObject>();
        public static List<SVObject.UserObject> Protected = new List<SVObject.UserObject>();
        public static List<SVObject.UserObject> Deaths = new List<SVObject.UserObject>();
        public static List<SVObject.VoteObject> Votes = new List<SVObject.VoteObject>();

        [Command("kill")]
        public async Task Kill(CommandContext e, int playerNum)
        {
            if (e.Channel.IsPrivate)
                await PMTasks(e, 1, playerNum);
        }

        [Command("protect")]
        public async Task Protect(CommandContext e, int playerNum)
        {
            if (e.Channel.IsPrivate)
                await PMTasks(e, 2, playerNum);
        }

        [Command("find")]
        public async Task Find(CommandContext e, int playerNum)
        {
            if (e.Channel.IsPrivate)
                await PMTasks(e, 3, playerNum);
        }

        [Command("vote")]
        public async Task Vote(CommandContext e, int playerNum)
        {
            if (phase % 2 == 0)
            {
                SVObject.UserObject PlayerVoting = SVData.Players.FirstOrDefault(x => x.ID == e.Message.Author.Id);

                if (PlayerVoting != null && SVData.Players.Exists(x => x.ID == e.Message.Author.Id && x.Status == 0) && !UsedAbility.Contains(PlayerVoting))
                {
                    SVObject.UserObject PlayerVoted = SVData.Players.First(x => x.PlayerNum == playerNum);
                    if (PlayerVoted != PlayerVoting && PlayerVoted.Status == 0)
                    {
                        await e.RespondAsync("You have voted for " + PlayerVoted.Username + ".");
                        Votes.Add(new SVObject.VoteObject(PlayerVoted, 1));
                        UsedAbility.Add(PlayerVoting);
                    }
                    else if (PlayerVoted == PlayerVoting)
                    {
                        await e.RespondAsync("You cannot vote for yourself.");
                    }
                    else if (PlayerVoted.Status == 1)
                    {
                        await e.RespondAsync(PlayerVoted.Username + " has already been killed. You cannot vote for a dead player.");
                    }

                }
            }
            else
            {
                await e.RespondAsync("You can only vote in the day.");  
            }
        }
        [Command("create"), Description("Creates a new Secret Villain™ game."), RequireRoles(RoleCheckMode.Any, "Staff")]
        public async Task Create(CommandContext e)
        {
            if (!active)
            {
                SVData = new SVObject.RootObject();
                active = true;
                await e.RespondAsync("Welcome to Secret Villain™! New game starting...\n\nTo sign up, please use the command !sv join");
            }
            else
            {
                await e.RespondAsync("Secret Villain™ game already active.");
            }
        }

        [Command("start"), Description("Starts the Secret Villain™ game."), RequireRoles(RoleCheckMode.Any, "Staff")]
        public async Task Start(CommandContext e)
        {
            if (active && !SVData.Started && SVData.Players.Count >= 4)
            {
                await SVData.StartGame(e);
                SVData.Timer = DateTime.UtcNow;
                SVData.Started = true;
                Thread t = new Thread(()=> ProcessTurn(e, 1));
                t.Start();
            }
            else if (active && !SVData.Started && SVData.Players.Count < 4)
            {
                await e.RespondAsync("Not enough players to begin the game.");
            }

            else if (active && SVData.Started)
            {
                await e.RespondAsync("Game already started.");
            }
            else
            {
                await e.RespondAsync("There is no active Secret Villain™ game. Use the command !tos create to make a new game.");
            }
        }

        [Command("stop"), Description("Stops the active Secret Villain™ game."), RequireRoles(RoleCheckMode.Any, "Staff")]
        public async Task Stop(CommandContext e)
        {
            if (active && !SVData.Started)
            {
                SVData = new SVObject.RootObject();
                active = false;
                await e.RespondAsync("Secret Villain™ game prematurely ended.");
                
            }
            else
            {
                await e.RespondAsync("There is no active Secret Villain™ game. Use the command !sv create to make a new game.");
            }
        }

        [Command("join"), Description("Adds you to the Secret Villain™ game.")]
        public async Task Join(CommandContext e)
        {
            if (active && !SVData.Started)
            {
                if (SVData.Players.Any(x => x.ID == e.Member.Id))
                {
                    await e.RespondAsync("You are already in the Secret Villain™ game, " + e.Member.DisplayName);
                }
                else
                {
                    SVData.Players.Add(new SVObject.UserObject(e.Member.Id, e.Member.DisplayName, -1, 0));
                    await e.RespondAsync("Added " + e.Member.DisplayName + " to the Secret Villain™ game.");
                }
            }
            else if (SVData.Started)
            {
                await e.RespondAsync("The game has already begun. Sorry!");
            }
            else
            {
                await Create(e);
                SVData.Players.Add(new SVObject.UserObject(e.Member.Id, e.Member.DisplayName, -1, 0));
                await e.RespondAsync("Added " + e.Member.DisplayName + " to the Secret Villain™ game.");
            }
        }

        [Command("info"), Description("Prints info on the currently active Secret Villain™ game.")]
        public async Task Info(CommandContext e)
        {
            if (active || !SVData.Started)
            {
                await SVData.DisplayData(e, false);
            }
            else
            {
                await e.RespondAsync("No active game.");
            }
        }

        public static async void ProcessTurn(CommandContext e, int dayModifier)
        {
            DiscordChannel Channel = RPClass.GameChannel;
            processing = true;
            while (SVData != null)
            {
                if (SVData.Timer.AddMinutes(dayModifier) < DateTime.Now || phase == 0)
                {
                    phase += 1;
                    if (phase % 2 == 0)
                    {
                        if (Deaths.Any())
                        {
                            SVObject.UserObject Death = Deaths.FirstOrDefault(x => !Protected.Contains(x));
                            string roleType = Death.Role == 1 ? "Villain" : Death.Role == 2 ? "Medic" : Death.Role == 3 ? "Spy" : Death.Role == 4 ? "Joker" : Death.Role == 5 ? "Hero" : "ERROR, Please report this to Jcryer.";

                            if (Death != null)
                                await Channel.SendMessageAsync(string.Format("__**Secret Villain™**__\n\n*It is now the day phase!* {0} *was killed by the Villain(s).* {1} *was a(n):* {2}", "**" + Death.Username + "**", "**" + Death.Username + "**", "**" + roleType + "**"));
                            else
                                await Channel.SendMessageAsync(string.Format("__**Secret Villain™**__\n\n*It is now the day phase! The Villains attempted to kill* {0},  *however was protected by a* **Medic**.", "**" + Death.Username + "**"));
                        }
                        else
                        {
                            await Channel.SendMessageAsync("__**Secret Villain™**__\n\n*It is now the day phase! Nobody was killed by the Villains this round.*");
                        }
                        foreach (SVObject.UserObject PlayerData in SVData.Players)
                        {
                            if (Deaths.Contains(PlayerData) && !Protected.Contains(PlayerData))
                            {
                                PlayerData.Status = 1;
                            }
                        }
                        if (SVData.Players.Count(x => x.Role == 1 && x.Status == 0) >= SVData.Players.Count(x => (x.Role == 2 || x.Role == 3 || x.Role == 5) && x.Status == 0))
                        {
                            await Channel.SendMessageAsync("__**Secret Villain™**__\n\n*The Villains turn on the helpless Heroes, and assassinate them all.* \n __**The Villains have won!**__");
                            await SVData.DisplayData(e, true);

                            await EndGame(e);

                        }
                        else if (SVData.Players.Count(x => x.Role == 1 && x.Status == 0) == 0)
                        {
                            await Channel.SendMessageAsync("__**Secret Villain™**__\n\n*The Heroes turn on the final remaining Villain, killing them with anger and hatred.* __**The Heroes have won!**__");
                            await SVData.DisplayData(e, true);

                            await EndGame(e);
                        }
                        await Channel.SendMessageAsync("\n*Please, vote on who you think the Villains are using the command*  **!sv vote PLAYERNUM.** \n*To get the player numbers, please use the command* **!sv info.**");

                        Deaths.Clear();
                        UsedAbility.Clear();
                        Protected.Clear();
                        Votes.Clear();
                    }
                    else
                    {
                        string MedicText = "";
                        string SpyText = "";
                        if (SVData.Players.Any(x => x.Role == 2 && x.Status == 0))
                        {
                            MedicText = "\n# Medic, PM me who you wish to protect using the command !sv protect PLAYERNUM.";

                        }
                        if (SVData.Players.Any(x => x.Role == 3 && x.Status == 0))
                        {
                            SpyText = "\n+ Spy, PM me who you wish to find out about using the command !sv find PLAYERNUM.";

                        }
                        if (Votes.Any())
                        {
                            List<SVObject.VoteObject> TalliedVotes = new List<SVObject.VoteObject>();
                            foreach (SVObject.UserObject UniqueVotes in Votes.Select(x => x.user).Distinct())
                            {
                                TalliedVotes.Add(new SVObject.VoteObject(UniqueVotes, Votes.Count(x => x.user == UniqueVotes)));
                            }
                            int VoteNum = TalliedVotes.Where(x => x.voteNum != 0).Max(x => x.voteNum);
                            if (TalliedVotes.FindAll(x => x.voteNum == VoteNum).Count == 1)
                            {
                                SVObject.UserObject HighestVote = TalliedVotes.First(x => x.voteNum == VoteNum).user;
                                string roleType = HighestVote.Role == 1 ? "Villain" : HighestVote.Role == 2 ? "Medic" : HighestVote.Role == 3 ? "Spy" : HighestVote.Role == 4 ? "Joker" : HighestVote.Role == 5 ? "Hero" : "ERROR, Please report this to Jcryer.";

                                await Channel.SendMessageAsync(string.Format("__**Secret Villain™**__\n\n*It is now the night phase!* {0} *was the majority vote with* {1} *vote(s), and was killed.*\n{2} *was a(n)* {3   }.", "**" + HighestVote.Username + "**", "**" + VoteNum + "**", "**" + HighestVote.Username + "**", "**" + roleType + "**"));
                                Deaths.Add(TalliedVotes.First(x => x.voteNum == VoteNum).user);
                            }
                            else
                            {
                                await Channel.SendMessageAsync("__**Secret Villain™**__\n\n*It is now the night phase! There was a tie with the votes, and as such nobody will die this phase.*");

                            }
                        }
                        else
                        {
                            if (phase == 1)
                            {
                                await Channel.SendMessageAsync("__**Secret Villain™**__\n\n*Welcome to Round One of this Secret Villain™ game! It is now the night phase.*");
                            }
                            else
                            {
                                await Channel.SendMessageAsync("__**Secret Villain™**__\n\n*It is now the night phase! There was no majority vote, and therefore nobody was killed.*");
                            }
                        }
                        await Channel.SendMessageAsync(string.Format("```diff\n- Villains, PM me your next kill using the command !sv kill PLAYERNUM. {0} {1}``` \n *To get the player numbers, please use the command* **!sv info.**", MedicText, SpyText));
                        foreach (SVObject.UserObject PlayerData in SVData.Players)
                        {
                            if (Deaths.Contains(PlayerData))
                            {
                                PlayerData.Status = 1;
                            }
                        }
                        if (SVData.Players.Count(x => x.Role == 1 && x.Status == 0) >= SVData.Players.Count(x => (x.Role == 2 || x.Role == 3 || x.Role == 5) && x.Status == 0))
                        {
                            await Channel.SendMessageAsync("__**Secret Villain™**__\n\n*The Villains turn on the helpless Heroes, and assassinate them all.* \n__**The Villains have won!**__");
                            await SVData.DisplayData(e, true);
                            await EndGame(e);

                        }
                        else if (SVData.Players.Count(x => x.Role == 1 && x.Status == 0) == 0)
                        {
                            await Channel.SendMessageAsync("__**Secret Villain™**__\n\n*The Heroes turn on the final remaining Villain, killing them with anger and hatred.* \n__**The shinobi have won!**__");
                            await SVData.DisplayData(e, true);

                            await EndGame(e);
                        }
                        else if (Deaths.Contains(SVData.Players.Find(x => x.Role == 4 && x.Status == 1)))
                        {
                            await Channel.SendMessageAsync("__**Secret Villain™**__\n\n__**The Joker has won**__, *as he was killed by the shinobi.*");
                        }
                        Deaths.Clear();
                        UsedAbility.Clear();
                        Protected.Clear();
                        Votes.Clear();

                    }
                    SVData.Timer = DateTime.Now;
                }
                await Task.Delay(60000);
            }
        }

        public static async Task EndGame(CommandContext e)
        {
            Deaths.Clear();
            UsedAbility.Clear();
            Protected.Clear();
            Votes.Clear();
            SVData = new SVObject.RootObject();
            active = false;
            processing = false;
            await Task.Delay(0);
        }
        public static async Task PMTasks(CommandContext e, int role, int playerNum)
        {
            if (phase % 2 != 0)
            {
                if (role == 1)
                {

                    if (SVData.Players.Exists(x => x.ID == e.Message.Author.Id && x.Role == 1))
                    {
                        SVObject.UserObject PlayerKilling = SVData.Players.First(x => x.ID == e.Message.Author.Id);
                        if (!UsedAbility.Contains(PlayerKilling))
                        {
                            SVObject.UserObject PlayerKilled = SVData.Players.FirstOrDefault(x => x.PlayerNum == playerNum);
                            if (PlayerKilled != null)
                            {
                                if (PlayerKilled != PlayerKilling)
                                {
                                    UsedAbility.Add(PlayerKilling);
                                    await e.RespondAsync("Player " + PlayerKilled.Username + " will be assassinated.");


                                    foreach (SVObject.UserObject a in SVData.Players.FindAll(x => x.Role == 1 && x.ID != e.Message.Author.Id))
                                    {
                                        DiscordMember user = await e.Guild.GetMemberAsync(a.ID);
                                        await user.SendMessageAsync(a.Username + " just voted to kill " + PlayerKilled.Username + ".");
                                    }
                                    if (!Protected.Contains(PlayerKilled)) Deaths.Add(PlayerKilled);
                                }
                                else
                                {
                                    await e.RespondAsync("You cannot kill yourself.");
                                }
                            }
                            else
                            {
                                await e.RespondAsync("Invalid player number.");
                            }
                        }
                    }
                }
                else if (role == 2)
                {

                    if (SVData.Players.Exists(x => x.ID == e.Message.Author.Id && x.Role == 2))
                    {
                        SVObject.UserObject PlayerSearching = SVData.Players.First(x => x.ID == e.Message.Author.Id);
                        if (!UsedAbility.Contains(PlayerSearching))
                        {
                            SVObject.UserObject PlayerSearched = SVData.Players.FirstOrDefault(x => x.PlayerNum == playerNum);
                            if (PlayerSearched != null)
                            {
                                if (PlayerSearched != PlayerSearching)
                                {
                                    await e.RespondAsync("Player " + PlayerSearched.Username + " protected.");
                                    if (!Protected.Contains(PlayerSearched)) Protected.Add(PlayerSearched);
                                    UsedAbility.Add(PlayerSearching);
                                }
                                else
                                {
                                    await e.RespondAsync("You cannot protect yourself.");
                                }
                            }
                            else
                            {
                                await e.RespondAsync("Invalid player number.");
                            }
                        }
                    }
                }
                else if (role == 3)
                {
                    if (SVData.Players.Exists(x => x.ID == e.Message.Author.Id && x.Role == 3))
                    {
                        SVObject.UserObject PlayerSearching = SVData.Players.First(x => x.ID == e.Message.Author.Id);
                        if (!UsedAbility.Contains(PlayerSearching))
                        {
                            SVObject.UserObject PlayerSearched = SVData.Players.FirstOrDefault(x => x.PlayerNum == playerNum);
                            if (PlayerSearched != null)
                            {
                                if (PlayerSearched != PlayerSearching)
                                {
                                    string roleType = PlayerSearched.Role == 1 ? "Villain" : PlayerSearched.Role == 2 ? "Medic" : PlayerSearched.Role == 3 ? "Spy" : PlayerSearched.Role == 4 ? "Joker" : PlayerSearched.Role == 5 ? "Hero" : "ERROR, Please report this to Jcryer.";
                                    await e.RespondAsync("Player " + PlayerSearched.Username + "'s role is: " + roleType + ".");
                                    UsedAbility.Add(PlayerSearching);
                                }
                                else
                                {
                                    await e.RespondAsync("You can not find information about yourself!");
                                }
                            }
                            else {
                                await e.RespondAsync("Invalid player number.");
                            }
                        }
                    }
                }
            }
            else
            {
                await e.RespondAsync("It is currently the day phase, you may only use abilities in the night phase.");
            }
        }
    }
}

