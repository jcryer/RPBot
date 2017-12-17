using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPBot
{
    [Group("guild"), Description("Guild commands")]
    class GuildClass : RPClass
    {

        [Command("create"), Description("Command for admins to create a guild."), RequireRolesAttribute("Staff", "Bot-Test")]
        public async Task Create(CommandContext e, [RemainingText, Description("Name of new Guild")] string guildName)
        {
            Guilds.Add(new GuildObject.RootObject(1 + Guilds.Count, guildName, 1, new List<ulong>()));
            await XPClass.UpdateGuildRanking(e.Guild);
            SaveData(3);
            await e.RespondAsync("Guild created.");
        }

        [Command("destroy"), Description("Command for admins to destroy a guild."), RequireRolesAttribute("Staff", "Bot-Test")]
        public async Task Destroy(CommandContext e, [RemainingText, Description("Name of guild to be destroyed.")] string guildName)
        {
            try
            {
                foreach (UserObject.RootObject user in Users.FindAll(x => x.UserData.guildID == Guilds.First(y => y.name == guildName).id))
                {
                    user.UserData.guildID = 0;
                }
                Guilds.Remove(Guilds.First(x => x.name == guildName));
                await XPClass.UpdatePlayerRanking(e.Guild, 1);
                await XPClass.UpdatePlayerRanking(e.Guild, 2);
                await XPClass.UpdateGuildRanking(e.Guild);

                SaveData(3);
                SaveData(1);

                await e.RespondAsync("Guild deleted.");
            }
            catch
            {
                await e.RespondAsync("No guild found with that name. Are you sure you typed it in correctly?");
            }
        }

        [Command("changestatus"), Description("Command for admins to change a guild's status."), RequireRolesAttribute("Staff", "Bot-Test")]
        public async Task ChangeStatus(CommandContext e, [Description("Name of guild whose status will be changed.")] string guildName, [Description("Status to set the guild to (1 for Active, 2 for Inactive")] string status = null)
        {
            if (status != null)
            {
                int statusInt = 0;
                if (int.TryParse(status, out statusInt) && (statusInt == 1 || statusInt == 2))
                {
                    try
                    {
                        Guilds.First(x => x.name == guildName).status = statusInt;
                        await XPClass.UpdateGuildRanking(e.Guild);
                        SaveData(3);
                        await e.RespondAsync("Guild status changed.");
                    }
                    catch
                    {
                        await e.RespondAsync("No guild found with that name. Are you sure you typed it in correctly?");
                    }
                }
                else
                {
                    await e.RespondAsync("Use 1 for Active or 2 for Inactive.");
                }
            }

            else
            {
                await e.RespondAsync("Specify the new status - 1 for Active or 2 for Inactive.");
            }
        }

        [Command("addmember"), Description("Command for admins to add a member to a guild."), RequireRolesAttribute("Staff", "Bot-Test")]
        public async Task AddMember(CommandContext e, [Description("Member to be added")] DiscordMember user, [RemainingText, Description("Name of guild which the user will be added to.")] string guildName)
        {
            try
            {
                Guilds.First(x => x.name == guildName).userIDs.Add(Users.First(x => x.UserData.userID == user.Id).UserData.userID);
                Users.First(x => x.UserData.userID == user.Id).UserData.guildID = Guilds.First(x => x.name == guildName).id;
                if (Users.First(x => x.UserData.userID == user.Id).UserData.role == 1) await XPClass.UpdatePlayerRanking(e.Guild, 1);
                else if (Users.First(x => x.UserData.userID == user.Id).UserData.role == 2) await XPClass.UpdatePlayerRanking(e.Guild, 2);
                await XPClass.UpdateGuildRanking(e.Guild);
                SaveData(3);
                SaveData(1);

                await e.RespondAsync("User added to guild.");
            }
            catch
            {
                await e.RespondAsync("No guild found with that name. Are you sure you typed it in correctly?");
            }
        }

        [Command("removemember"), Description("Command for admins to remove a member from a guild."), RequireRolesAttribute("Staff", "Bot-Test")]
        public async Task RemoveMember(CommandContext e, [Description("Member to be removed ")] DiscordMember user, [RemainingText, Description("Name of guild which the user will be removed from.")] string guildName)
        {
            try
            {
                Guilds.First(x => x.name == guildName).userIDs.Remove(Users.First(x => x.UserData.userID == user.Id).UserData.userID);
                Users.First(x => x.UserData.userID == user.Id).UserData.guildID = 0;
                if (Users.First(x => x.UserData.userID == user.Id).UserData.role == 1) await XPClass.UpdatePlayerRanking(e.Guild, 1);
                else if (Users.First(x => x.UserData.userID == user.Id).UserData.role == 2) await XPClass.UpdatePlayerRanking(e.Guild, 2);
                await XPClass.UpdateGuildRanking(e.Guild);

                SaveData(3);
                SaveData(1);
                await e.RespondAsync("User removed from guild.");
            }
            catch
            {
                await e.RespondAsync("No guild found with that name. Are you sure you typed it in correctly?");
            }
        }
    }
}
