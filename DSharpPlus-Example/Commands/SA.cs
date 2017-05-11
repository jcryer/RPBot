using DSharpPlus;
using DSharpPlus.CommandsNext;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SLOBot
{
    class SAClass : Program
    {
        public static bool active = false;
        public static bool started = false;
        public static int phase = 0;
        public static DateTime timer;

        public static List<SAObject> UsedAbility = new List<SAObject>();
        public static List<SAObject> Protected = new List<SAObject>();
        public static List<SAObject> Deaths = new List<SAObject>();
        public static List<SAObject> ActiveGame = new List<SAObject>();
        public static List<VoteObject> Votes = new List<VoteObject>(); 

		public static async Task SAGame(CommandContext e, string keyword, int vote)
        {
			DiscordMember member = await e.Guild.GetMemberAsync(e.Message.Author.Id);
            if (member.Roles.Contains(e.Guild.Roles.First(x => x.Name == "Bot Commander").Id))
            {
                if (keyword == "create")
                {
                    if (!active)
                    {
                        await e.Channel.SendMessageAsync("Welcome to Secret Assassin! New game starting...");

                        await e.Channel.SendMessageAsync("To sign up, please use the command !sa join");
                        active = true;
                    }
                    else
                    {
                        await e.Channel.SendMessageAsync("Secret Assassin game already active.");
                    }
                }
                else if (keyword == "stop")
                {
                    if (active || started)
                    {

                        await EndGame(e);
                        await e.Channel.SendMessageAsync("Secret Assassin game ended prematurely.");
                    }
                    else
                    {
                        await e.Channel.SendMessageAsync("There is no active Secret Assassin game.");
                    }
                }
                else if (keyword == "start")
                {
                    if (active && !started && ActiveGame.Count >= 4)
                    {
                        await StartGame(e);
                        timer = DateTime.Now;
                        started = true;
                        Thread.Sleep(2000);
                        Thread game = new Thread(() => ProcessTurn(e, 2));
                        game.Start();
                    }

                    else if (active && !started && ActiveGame.Count < 4)
                    {
                        await e.Channel.SendMessageAsync("Not enough players to begin the game.");
                    }

                    else if (active && started)
                    {
                        await e.Channel.SendMessageAsync("Game already started.");
                    }
                    else
                    {
                        await e.Channel.SendMessageAsync("There is no active Secret Assassin game.");
                    }
                }
            }


            else if (keyword == "join")
            {
                SAObject a = ActiveGame.FirstOrDefault(x => x.userID == e.Message.Author.Id);
                if (active && !started)
                {
                    if (a == null)
                    {
                        await e.Channel.SendMessageAsync("You have been added to the Secret Assassin game, " + e.Message.Author.Username);
                        ActiveGame.Add(new SAObject(e.Message.Author.Id, e.Message.Author.Username, -1, 0, -1));

                    }
                    else
                    {
                        await e.Channel.SendMessageAsync(e.Message.Author.Username + " is already in the Secret Assassin game.");
                    }
                }
                else if (active && started)
                {
                    await e.Channel.SendMessageAsync("The Secret Assassin game has already begun. Sorry!");
                }
                else
                {
                    await e.Channel.SendMessageAsync("There is no active Secret Assassin game.");

                }
            }

            else if (keyword == "info")
            {
                if (active || started)
                {
                    await DisplayData(e, 0);
                }
                else
                {
                    await e.Channel.SendMessageAsync("No active game to display.");
                }
            }
            else if (keyword == "vote")
            {
                if (vote != -1)
                { 
                    if (phase % 2 == 0)
                    {
                        SAObject PlayerVoting = ActiveGame.FirstOrDefault(x => x.userID == e.Message.Author.Id);

                        if (PlayerVoting != null && ActiveGame.Exists(x => x.userID == e.Message.Author.Id && x.status == 0) && !UsedAbility.Contains(PlayerVoting))
                        {
                            SAObject PlayerVoted = ActiveGame.First(x => x.playerNum == vote);
                            if (PlayerVoted != PlayerVoting && PlayerVoted.status == 0)
                            {
                                await e.Channel.SendMessageAsync("You have voted for " + PlayerVoted.username + ".");
                                Votes.Add(new VoteObject(PlayerVoted, 1));
                                UsedAbility.Add(PlayerVoting);
                            }
                            else if (PlayerVoted == PlayerVoting)
                            {
                                await e.Channel.SendMessageAsync("You cannot vote for yourself.");
                            }
                            else if (PlayerVoted.status == 1)
                            {
                                await e.Channel.SendMessageAsync(PlayerVoted.username + " has already been killed. You cannot vote for a dead player.");
                            }

                        }
                    }
                    else
                    {
                        await e.Channel.SendMessageAsync("It is currently the night phase. You can only vote for who should die in the day phase.");
                    }
                }
                else
                {
                    await e.Channel.SendMessageAsync("Please include the playernum");
                }
            }

			SaveData(1);

        }

		public static async Task DisplayData(CommandContext e, int DataToDisplay)
        {

            List<DiscordEmbedField> Fields = new List<DiscordEmbedField>();
            DiscordChannel Channel;
            if (started)
            {
                Channel = e.Guild.Channels.First(x => x.Name == "sa");
            }
            else
            {
               Channel = e.Guild.Channels.First(x => x.Name == "messing-with-the-bots");

            }

            foreach (SAObject user in ActiveGame)
            {
                string statusString = user.status == 0 ? "Alive" : user.status == 1 ? "Dead" : "ERROR, Please report this to Jcryer.";
                string playerNumString = user.playerNum == -1 ? "N/A" : user.status >= 0 ? user.playerNum.ToString() : "ERROR, Please report this to Jcryer.";
                string roleType = user.role == 1 ? "Assassin" : user.role == 2 ? "Medic" : user.role == 3 ? "ANBU" : user.role == 4 ? "Joker" : user.role == 5 ? "Shinobi" : "ERROR, Please report this to Jcryer.";

                if (DataToDisplay == 1)
                {
                    Fields.Add(new DiscordEmbedField() { Name = user.username, Value = "Role: " + roleType });
                }
                else
                {
                    Fields.Add(new DiscordEmbedField() { Name = user.username, Value = "Player number: " + playerNumString + Environment.NewLine + "Status: " + statusString });
                }
            }

            DiscordEmbed embed = new DiscordEmbed()
            {
                Color = 4589319,

                Fields = Fields,

                Footer = new DiscordEmbedFooter()
                {
                    Text = "Secret Assassin"
                },

                Title = "Current Secret Assassin game info!",
            };
            await Channel.SendMessageAsync("", embed: embed);
        }

        public static async Task DisplayData(DiscordDmChannel d, int DataToDisplay)
        {
            List<DiscordEmbedField> Fields = new List<DiscordEmbedField>();

            foreach (SAObject user in ActiveGame)
            {
                string statusString = user.status == 0 ? "Alive" : user.status == 1 ? "Dead" : "ERROR, Please report this to Jcryer.";
                string playerNumString = user.playerNum == -1 ? "N/A" : user.status >= 0 ? user.playerNum.ToString() : "ERROR, Please report this to Jcryer.";
                string roleType = user.role == 1 ? "Assassin" : user.role == 2 ? "Medic" : user.role == 3 ? "ANBU" : user.role == 4 ? "Joker" : user.role == 5 ? "Shinobi" : "ERROR, Please report this to Jcryer.";

                if (DataToDisplay == 1)
                {
                    Fields.Add(new DiscordEmbedField() { Name = user.username, Value = "Role: " + roleType });
                }
                else
                {
                    Fields.Add(new DiscordEmbedField() { Name = user.username, Value = "Player number: " + playerNumString + Environment.NewLine + "Status: " + statusString });
                }
            }

            DiscordEmbed embed = new DiscordEmbed()
            {
                Color = 4589319,

                Fields = Fields,

                Footer = new DiscordEmbedFooter()
                {
                    Text = "Secret Assassin"
                },

                Title = "Current Secret Assassin game info!",
            };
            await d.SendMessageAsync("", embed: embed);
        }
		public static void SaveData(int saveType)
        {
            if (ActiveGame.Any() && saveType == 1)
            {
                List<string> gameData = new List<string>
                {
                    "*a" + active,
                    "*b" + started,
                    "*c" + phase,
                    "*d" + timer
                };
                gameData.AddRange(UsedAbility.Select(x => "*e" + x.userID + ":" + x.username + ":" + x.role + ":" + x.status + ":" + x.playerNum).ToArray());
				gameData.AddRange(Protected.Select(x => "*f" + x.userID + ":" + x.username + ":" + x.role + ":" + x.status + ":" + x.playerNum).ToArray());
				gameData.AddRange(Deaths.Select(x => "*g" + x.userID + ":" + x.username + ":" + x.role + ":" + x.status + ":" + x.playerNum).ToArray());
				gameData.AddRange(ActiveGame.Select(x => "*h" + x.userID + ":" + x.username + ":" + x.role + ":" + x.status + ":" + x.playerNum).ToArray());
				gameData.AddRange(Votes.Select(x => "*i" + x.SAObject + ":" + x.voteNum).ToArray());

				Console.WriteLine("");

				File.WriteAllLines("SA.txt", gameData);
            }
            else if (saveType == 2)
            {
                File.WriteAllText("SA.txt", "");
            }
        }

		public static void LoadData()
        {
            string[] data = File.ReadAllLines("SA.txt");
            foreach (string user in data)
            {
                if (user[0] == '*')
				{
					if (user[1] == 'a') active = bool.Parse(user.Substring(2));
					else if (user[1] == 'b') started = bool.Parse(user.Substring(2));
					else if (user[1] == 'c') phase = int.Parse(user.Substring(2));
					else if (user[1] == 'd') timer = DateTime.Parse(user.Substring(2));
					else if (user[1] == 'e')
					{
						string[] userData = user.Substring(2).Split(':');
						UsedAbility.Add(new SAObject(ulong.Parse(userData[0]), userData[1], int.Parse(userData[2]), int.Parse(userData[3]), int.Parse(userData[4])));
					}
					else if (user[1] == 'f')
					{
						string[] userData = user.Substring(2).Split(':');
						Protected.Add(new SAObject(ulong.Parse(userData[0]), userData[1], int.Parse(userData[2]), int.Parse(userData[3]), int.Parse(userData[4])));
					}
					else if (user[1] == 'g')
					{
						string[] userData = user.Substring(2).Split(':');
						Deaths.Add(new SAObject(ulong.Parse(userData[0]), userData[1], int.Parse(userData[2]), int.Parse(userData[3]), int.Parse(userData[4])));
					}
					else if (user[1] == 'h')
					{
						string[] userData = user.Substring(2).Split(':');
						ActiveGame.Add(new SAObject(ulong.Parse(userData[0]), userData[1], int.Parse(userData[2]), int.Parse(userData[3]), int.Parse(userData[4])));
					}
					else if (user[1] == 'i')
					{
						string[] userData = user.Substring(2).Split(':');
						Votes.Add(new VoteObject(new SAObject(ulong.Parse(userData[0]), userData[1], int.Parse(userData[2]), int.Parse(userData[3]), int.Parse(userData[4])), int.Parse(userData[5])));
					}
                }
            }
        }
		public static async Task StartGame(CommandContext e)
        {
            foreach (SAObject user in ActiveGame)
            { 
                await e.Guild.AddRoleAsync(user.userID, 302577136148021248);

            }
            DiscordChannel Channel = e.Guild.Channels.First(x => x.Name == "sa");
            await Channel.SendMessageAsync("Game started.");
            started = true;
            int Total = ActiveGame.Count();
            int playerNumIncrement = Total;

            int Assassins = Convert.ToInt32(Math.Floor((float)(Total / 4)));
            int Medics = Convert.ToInt32(Math.Floor((float)(Assassins / 2)));
            int ANBU = Convert.ToInt32(Math.Floor((float)(Assassins / 2)));
            int Joker = Convert.ToInt32(Math.Floor((float)(Total / 6)));
            int Shinobi = Total - Assassins - Medics - ANBU - Joker;
            await Channel.SendMessageAsync("```There are " + Total + " players in this game, meaning there will be:" + Environment.NewLine + Assassins + " Assassins," + Environment.NewLine + Medics + " Medic(s)," + Environment.NewLine + ANBU + " ANBU," + Environment.NewLine + Joker + " Joker(s)," + Environment.NewLine + Shinobi + " Shinobi.```");

            for (int i = 0; i < Assassins; i++)
            {
                List<SAObject> usersLeft = ActiveGame.Where(x => x.role < 0).ToList();
                int r = random.Next(usersLeft.Count(x => x.role < 0));
                usersLeft[r].role = 1;
               

            }
            for (int i = 0; i < Medics; i++)
            {
                List<SAObject> usersLeft = ActiveGame.Where(x => x.role < 0).ToList();
                int r = random.Next(usersLeft.Count(x => x.role < 0));
                usersLeft[r].role = 2;
                
            }
            for (int i = 0; i < ANBU; i++)
            {
                List<SAObject> usersLeft = ActiveGame.Where(x => x.role < 0).ToList();
                int r = random.Next(usersLeft.Count(x => x.role < 0));
                usersLeft[r].role = 3;
                
            }
            for (int i = 0; i < Joker; i++)
            {
                List<SAObject> usersLeft = ActiveGame.Where(x => x.role < 0).ToList();
                int r = random.Next(usersLeft.Count(x => x.role < 0));
                usersLeft[r].role = 4;
                
            }
            for (int i = 0; i < Shinobi; i++)
            {
                List<SAObject> usersLeft = ActiveGame.Where(x => x.role < 0).ToList();
                int r = random.Next(usersLeft.Count(x => x.role < 0));
                usersLeft[r].role = 5;
                
            }
            List<int> numberList = Enumerable.Range(0, Total).ToList();
            for (int i = 0; i < Total; i++)
            {
                int rnd = numberList.PickRandom();
                ActiveGame[i].playerNum = rnd;
                numberList.Remove(rnd);
            }
            ActiveGame = ActiveGame.OrderBy(x => x.playerNum).ToList();

            try
            {
                foreach (SAObject userData in ActiveGame)
                {

                    DiscordMember user = await e.Guild.GetMemberAsync(userData.userID);
                    DiscordDmChannel dm = await user.SendDmAsync();
                    string roleType = userData.role == 1 ? "Assassin" : userData.role == 2 ? "Medic" : userData.role == 3 ? "ANBU" : userData.role == 4 ? "Joker" : userData.role == 5 ? "Shinobi" : "ERROR, Please report this to Jcryer.";
                    if (userData.role == 1)
                    {
                        if (ActiveGame.Count(x => x.role == 1) > 1)
                        {
                            await dm.SendMessageAsync("Hi! your role is: Assassin, and the other Assassins are: ");
                            foreach (SAObject a in ActiveGame.FindAll(x => x.role == 1 && x.userID != user.Id))
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
                    await DisplayData(dm, 0);
                }
            }
            catch 
			{
				Console.WriteLine("Catch!");
			}
            SaveData(1);
        }

		public static async void ProcessTurn(CommandContext e, int dayModifier)
        {
            DiscordChannel Channel = e.Guild.Channels.First(x => x.Name == "sa");
            processing = true;
            while (started)
            {
                if (timer.AddMinutes(dayModifier) < DateTime.Now || phase == 0)
                {
                    phase += 1;
                    if (phase % 2 == 0)
                    {
                        foreach (SAObject user in ActiveGame)
                        {
                            await e.Guild.AddRoleAsync(user.userID, 302577136148021248);
                        }
                        if (Deaths.Any())
                        {
                            SAObject Death = Deaths.FirstOrDefault(x => !Protected.Contains(x));
                            string roleType = Death.role == 1 ? "Assassin" : Death.role == 2 ? "Medic" : Death.role == 3 ? "ANBU" : Death.role == 4 ? "Joker" : Death.role == 5 ? "Shinobi" : "ERROR, Please report this to Jcryer.";

                            if (Death != null)
                                await Channel.SendMessageAsync(string.Format("__**Secret Assassin**__\n\n*It is now the day phase!* {0} *was killed by the assassin(s).* {1} *was a(n):* {2}", "**" + Death.username + "**", "**" + Death.username + "**", "**" + roleType + "**"));
                            else
                                await Channel.SendMessageAsync(string.Format("__**Secret Assassin**__\n\n*It is now the day phase! The assassins attempted to kill* {0},  *however was protected by a* **medic**.", "**" + Death.username + "**"));
                        }
                        else
                        {
                            await Channel.SendMessageAsync("__**Secret Assassin**__\n\n*It is now the day phase! Nobody was killed by the assassins this round.*");
                        }
                        foreach (SAObject PlayerData in ActiveGame)
                        {
                            if (Deaths.Contains(PlayerData) && !Protected.Contains(PlayerData))
                            {
                                PlayerData.status = 1;
                            }
                        }
                        if (ActiveGame.Count(x => x.role == 1 && x.status == 0) >= ActiveGame.Count(x => (x.role == 2 || x.role == 3 || x.role == 5) && x.status == 0))
                        {
                            await Channel.SendMessageAsync("__**Secret Assassin**__\n\n*The assassins turn on the helpless shinobi, and assassinate them all.* \n __**The assassins have won!**__");
                            await DisplayData(e, 1);

                            await EndGame(e);

                        }
                        else if (ActiveGame.Count(x => x.role == 1 && x.status == 0) == 0)
                        {
                            await Channel.SendMessageAsync("__**Secret Assassin**__\n\n*The shinobi turn on the final remaining assassin, killing them with anger and hatred.* __**The shinobi have won!**__");
                            await DisplayData(e, 1);

                            await EndGame(e);
                        }
                        await Channel.SendMessageAsync("\n*Please, vote on who you think the assassins are using the command*  **!sa vote PLAYERNUM.** \n*To get the player numbers, please use the command* **!sa info.**");

                        Deaths.Clear();
                        UsedAbility.Clear();
                        Protected.Clear();
                        Votes.Clear();
                    }
                    else
                    {
                        foreach (SAObject user in ActiveGame)
                        {
                            await e.Guild.RemoveRoleAsync(user.userID, 302577136148021248);
                        }
                        string MedicText = "";
                        string ANBUText = "";
                        if (ActiveGame.Any(x => x.role == 2 && x.status == 0))
                        {
                            MedicText = "\n# Medic, PM me who you wish to protect using the command !protect PLAYERNUM.";

                        }
                        if (ActiveGame.Any(x => x.role == 3 && x.status == 0))
                        {
                            ANBUText = "\n+ ANBU, PM me who you wish to find out about using the command !find PLAYERNUM.";

                        }
                        if (Votes.Any())
                        {
                            List<VoteObject> TalliedVotes = new List<VoteObject>();
                            foreach (SAObject UniqueVotes in Votes.Select(x => x.SAObject).Distinct())
                            {
                                TalliedVotes.Add(new VoteObject(UniqueVotes, Votes.Count(x => x.SAObject == UniqueVotes)));
                            }
                            int VoteNum = TalliedVotes.Where(x => x.voteNum != 0).Max(x => x.voteNum);
                            if (TalliedVotes.FindAll(x => x.voteNum == VoteNum).Count == 1)
                            {
                                SAObject HighestVote = TalliedVotes.First(x => x.voteNum == VoteNum).SAObject;
                                string roleType = HighestVote.role == 1 ? "Assassin" : HighestVote.role == 2 ? "Medic" : HighestVote.role == 3 ? "ANBU" : HighestVote.role == 4 ? "Joker" : HighestVote.role == 5 ? "Shinobi" : "ERROR, Please report this to Jcryer.";

                                await Channel.SendMessageAsync(string.Format("__**Secret Assassin**__\n\n*It is now the night phase!* {0} *was the majority vote with* {1} *vote(s), and was killed.*\n{2} *was a(n)* {3   }.", "**" + HighestVote.username + "**", "**" + VoteNum + "**", "**" + HighestVote.username + "**", "**" + roleType + "**"));
                                Deaths.Add(TalliedVotes.First(x => x.voteNum == VoteNum).SAObject);
                            }
                            else
                            {
                                await Channel.SendMessageAsync("__**Secret Assassin**__\n\n*It is now the night phase! There was a tie with the votes, and as such nobody will die this phase.*");

                            }
                        }
                        else 
                        {
                            if (phase == 1)
                            {
                                await Channel.SendMessageAsync("__**Secret Assassin**__\n\n*Welcome to Round One of this Secret Assassin game! It is now the night phase.*");
                            }
                            else
                            {
                                await Channel.SendMessageAsync("__**Secret Assassin**__\n\n*It is now the night phase! There was no majority vote, and therefore nobody was killed.*");
                            }
                        }
                        await Channel.SendMessageAsync(string.Format("```diff\n- Assassins, PM me your next kill using the command !kill PLAYERNUM. {0} {1}``` \n *To get the player numbers, please use the command* **!sa info.**", MedicText, ANBUText));
                        foreach (SAObject PlayerData in ActiveGame)
                        {
                            if (Deaths.Contains(PlayerData))
                            {
                                PlayerData.status = 1;
                            }
                        }
                        if (ActiveGame.Count(x => x.role == 1 && x.status == 0) >= ActiveGame.Count(x => (x.role == 2 || x.role == 3 || x.role == 5) && x.status == 0))
                        {
                            await Channel.SendMessageAsync("__**Secret Assassin**__\n\n*The assassins turn on the helpless shinobi, and assassinate them all.* \n__**The assassins have won!**__");
                            await DisplayData(e, 1);
                            await EndGame(e);

                        }
                        else if (ActiveGame.Count(x => x.role == 1 && x.status == 0) == 0)
                        {
                            await Channel.SendMessageAsync("__**Secret Assassin**__\n\n*The shinobi turn on the final remaining assassin, killing them with anger and hatred.* \n__**The shinobi have won!**__");
                            await DisplayData(e, 1);

                            await EndGame(e);
                        }
                        else if (Deaths.Contains(ActiveGame.Find(x => x.role == 4 && x.status == 1)))
                        {
                            await Channel.SendMessageAsync("__**Secret Assassin**__\n\n__**The Joker has won**__, *as he was killed by the shinobi.*");
                        }
                        Deaths.Clear();
                        UsedAbility.Clear();
                        Protected.Clear();
                        Votes.Clear();

                    }
                    timer = DateTime.Now;
                }
                SaveData(1);
                Thread.Sleep(30000);
            }
        }

		public static async Task EndGame (CommandContext e)
        {
            foreach (SAObject user in ActiveGame)
            {
                await e.Guild.RemoveRoleAsync(user.userID, 302577136148021248);
            }
            SaveData(2);
            Deaths.Clear();
            UsedAbility.Clear();
            Protected.Clear();
            ActiveGame.Clear();
            Votes.Clear();
            started = false;
            active = false;
            processing = false;

            DiscordChannel Channel = e.Guild.Channels.First(x => x.Name == "sa");
            for (int i = 0; i < 10; i++)
            {
                List<DiscordMessage> Messages = await Channel.GetMessagesAsync(99);
                if (Messages.Count == 0) break;
                await Channel.BulkDeleteMessagesAsync(Messages.Select(x => x.Id).ToList());
            }
        }
        public static async Task PMTasks (CommandContext e, int role, int vote)
        {
            if (phase % 2 != 0)
            {
                if (role == 1)
                {
                    SAObject PlayerKilling = ActiveGame.First(x => x.userID == e.Message.Author.Id);
                    if (vote != -1)
                    {
                        if (ActiveGame.Exists(x => x.userID == e.Message.Author.Id && x.role == 1) && !UsedAbility.Contains(PlayerKilling))
                        {
                            SAObject PlayerKilled = ActiveGame.First(x => x.playerNum == vote);
                            if (PlayerKilled != PlayerKilling)
                            {
                                await e.Channel.SendMessageAsync("Player " + PlayerKilled.username + " will be assassinated.");


                                foreach (SAObject a in ActiveGame.FindAll(x => x.role == 1 && x.userID != e.Message.Author.Id))
                                {
                                    DiscordMember user = await e.Guild.GetMemberAsync(a.userID);
                                    DiscordDmChannel dm = await user.SendDmAsync();
                                    await dm.SendMessageAsync(a.username + " just voted to kill " + PlayerKilled.username + ".");
                                }
                                if (!Protected.Contains(PlayerKilled)) Deaths.Add(PlayerKilled);
                                SAObject KillsListAlts = Deaths.FirstOrDefault(x => x.userID != PlayerKilled.userID);
                                if (KillsListAlts != null)
                                {
                                    await e.Channel.SendMessageAsync("Removing " + KillsListAlts.username + " from assassination list, assassinating " + PlayerKilled.username + " instead.");
                                    Deaths.Clear();
                                    if (!Protected.Contains(PlayerKilled)) Deaths.Add(PlayerKilled);
                                }
                            }
                            else
                            {
                                await e.Channel.SendMessageAsync("You cannot kill yourself.");
                            }
                        }
                        else
                        {
                            await e.Channel.SendMessageAsync("You have either already used your ability for this night cycle, or you do not have permission to.");
                        }
                    }
                    else
                    {
                        await e.Channel.SendMessageAsync("Please include the playernum");
                    }

                }
                else if (role == 2)
                {
                    SAObject PlayerSearching = ActiveGame.First(x => x.userID == e.Message.Author.Id);
                    if (vote != -1)
                    {
                        if (ActiveGame.Exists(x => x.userID == e.Message.Author.Id && x.role == 2) && !UsedAbility.Contains(PlayerSearching))
                        {
                            SAObject PlayerSearched = ActiveGame.First(x => x.playerNum == vote);
                            if (PlayerSearched != PlayerSearching)
                            {
                                await e.Channel.SendMessageAsync("Player " + PlayerSearched.username + " protected.");
                                if (!Protected.Contains(PlayerSearched)) Protected.Add(PlayerSearched);
                                UsedAbility.Add(PlayerSearching);
                            }
                            else
                            {
                                await e.Channel.SendMessageAsync("You cannot protect yourself.");
                            }
                        }
                        else
                        {
                            await e.Channel.SendMessageAsync("You have either already used your ability for this night cycle, or you do not have permission to.");
                        }
                    }
                    else
                    {
                        await e.Channel.SendMessageAsync("Please include the playernum");

                    }

                }
                else if (role == 3)
                {
                    if (ActiveGame.Exists(x => x.userID == e.Message.Author.Id && x.role == 3))
                    {
                        if (vote != -1)
                        {
                            SAObject PlayerSearched = ActiveGame.First(x => x.playerNum == vote);
                            string roleType = PlayerSearched.role == 1 ? "Assassin" : PlayerSearched.role == 2 ? "Medic" : PlayerSearched.role == 3 ? "ANBU" : PlayerSearched.role == 4 ? "Joker" : PlayerSearched.role == 5 ? "Shinobi" : "ERROR, Please report this to Jcryer.";
                            await e.Channel.SendMessageAsync("Player " + PlayerSearched.username + "'s role is: " + roleType + ".");
                        }
                        else
                        {
                            await e.Channel.SendMessageAsync("Please include the playernum");

                        }
                    }
                }
            }
            else
            {
                await e.Channel.SendMessageAsync("It is currently the day phase, you may only use abilities in the night phase.");
            }
        }
    }

    class SAObject
    {
        public ulong userID;
        public string username;
        public int role;
        public int status;
        public int playerNum;

        public SAObject(ulong userID, string username, int role, int status, int playerNum)
        {
            this.username = username;
            this.userID = userID;
            this.role = role;
            this.status = status;
            this.playerNum = playerNum;
        }
    }
    class VoteObject
    {
        public SAObject SAObject;
        public int voteNum;

        public VoteObject(SAObject SAObject, int voteNum)
        {
            this.SAObject = SAObject;
            this.voteNum = voteNum;
        }
    }
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
}


