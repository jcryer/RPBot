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
    class GuildClass : BaseCommandModule
    {

        [Command("create"), Description("Command for admins to create a guild."), RequireRoles(RoleCheckMode.Any, "Staff")]
        public async Task Create(CommandContext e, [RemainingText, Description("Name of new Guild")] string guildName)
        {
            RPClass.Guilds.Add(new GuildObject.RootObject(1 + RPClass.Guilds.Count, guildName, 1, new List<ulong>()));
            await XPClass.UpdateGuildRanking(e.Guild);
            RPClass.SaveData(3);
            await e.RespondAsync("Guild created.");
        }

        [Command("destroy"), Description("Command for admins to destroy a guild."), RequireRoles(RoleCheckMode.Any, "Staff")]
        public async Task Destroy(CommandContext e, [RemainingText, Description("Name of guild to be destroyed.")] string guildName)
        {
            try
            {
                foreach (UserObject.RootObject user in RPClass.Users.FindAll(x => x.UserData.GuildID == RPClass.Guilds.First(y => y.Name == guildName).Id))
                {
                    user.UserData.GuildID = 0;
                }
                RPClass.Guilds.Remove(RPClass.Guilds.First(x => x.Name == guildName));
                await XPClass.UpdatePlayerRanking(e.Guild, 1);
                await XPClass.UpdatePlayerRanking(e.Guild, 2);
                await XPClass.UpdatePlayerRanking(e.Guild, 3);
                await XPClass.UpdatePlayerRanking(e.Guild, 4);

                await XPClass.UpdateGuildRanking(e.Guild);

                RPClass.SaveData(3);
                RPClass.SaveData(1);

                await e.RespondAsync("Guild deleted.");
            }
            catch
            {
                await e.RespondAsync("No guild found with that name. Are you sure you typed it in correctly?");
            }
        }

        [Command("changestatus"), Description("Command for admins to change a guild's status."), RequireRoles(RoleCheckMode.Any, "Staff")]
        public async Task ChangeStatus(CommandContext e, [Description("Name of guild whose status will be changed.")] string guildName, [Description("Status to set the guild to (1 for Active, 2 for Inactive")] string status = null)
        {
            if (status != null)
            {
                if (int.TryParse(status, out int statusInt) && (statusInt == 1 || statusInt == 2))
                {
                    try
                    {
                        RPClass.Guilds.First(x => x.Name == guildName).Status = statusInt;
                        await XPClass.UpdateGuildRanking(e.Guild);
                        RPClass.SaveData(3);
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

        [Command("addmember"), Description("Command for admins to add a member to a guild."), RequireRoles(RoleCheckMode.Any, "Staff")]
        public async Task AddMember(CommandContext e, [Description("Member to be added")] DiscordMember user, [RemainingText, Description("Name of guild which the user will be added to.")] string guildName)
        {
            try
            {
                RPClass.Guilds.First(x => x.Name == guildName).UserIDs.Add(RPClass.Users.First(x => x.UserData.UserID == user.Id).UserData.UserID);
                RPClass.Users.First(x => x.UserData.UserID == user.Id).UserData.GuildID = RPClass.Guilds.First(x => x.Name == guildName).Id;
                if (RPClass.Users.First(x => x.UserData.UserID == user.Id).UserData.Role == 1) await XPClass.UpdatePlayerRanking(e.Guild, 1);
                else if (RPClass.Users.First(x => x.UserData.UserID == user.Id).UserData.Role == 2) await XPClass.UpdatePlayerRanking(e.Guild, 2);
                else if (RPClass.Users.First(x => x.UserData.UserID == user.Id).UserData.Role == 3) await XPClass.UpdatePlayerRanking(e.Guild, 3);
                else if (RPClass.Users.First(x => x.UserData.UserID == user.Id).UserData.Role == 4) await XPClass.UpdatePlayerRanking(e.Guild, 4);

                await XPClass.UpdateGuildRanking(e.Guild);
                RPClass.SaveData(3);
                RPClass.SaveData(1);

                await e.RespondAsync("User added to guild.");
            }
            catch
            {
                await e.RespondAsync("No guild found with that name. Are you sure you typed it in correctly?");
            }
        }

        [Command("removemember"), Description("Command for admins to remove a member from a guild."), RequireRoles(RoleCheckMode.Any, "Staff")]
        public async Task RemoveMember(CommandContext e, [Description("Member to be removed ")] DiscordMember user, [RemainingText, Description("Name of guild which the user will be removed from.")] string guildName)
        {
            try
            {
                RPClass.Guilds.First(x => x.Name == guildName).UserIDs.Remove(RPClass.Users.First(x => x.UserData.UserID == user.Id).UserData.UserID);
                RPClass.Users.First(x => x.UserData.UserID == user.Id).UserData.GuildID = 0;
                if (RPClass.Users.First(x => x.UserData.UserID == user.Id).UserData.Role == 1) await XPClass.UpdatePlayerRanking(e.Guild, 1);
                else if (RPClass.Users.First(x => x.UserData.UserID == user.Id).UserData.Role == 2) await XPClass.UpdatePlayerRanking(e.Guild, 2);
                else if (RPClass.Users.First(x => x.UserData.UserID == user.Id).UserData.Role == 3) await XPClass.UpdatePlayerRanking(e.Guild, 3);
                else if (RPClass.Users.First(x => x.UserData.UserID == user.Id).UserData.Role == 4) await XPClass.UpdatePlayerRanking(e.Guild, 4);

                await XPClass.UpdateGuildRanking(e.Guild);

                RPClass.SaveData(3);
                RPClass.SaveData(1);
                await e.RespondAsync("User removed from guild.");
            }
            catch
            {
                await e.RespondAsync("No guild found with that name. Are you sure you typed it in correctly?");
            }
        }
    }
}
