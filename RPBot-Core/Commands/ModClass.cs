using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPBot
{
    class ModClass : BaseCommandModule
    {
        [Command("punish"), Description("Command for staff to give out the Punished role."), RequireRoles(RoleCheckMode.Any, "Staff", "Helpful people")]
        public async Task Punish(CommandContext e, [Description("Member to be muted")] DiscordMember user)
        {
            try
            {
                UserObject.RootObject userObject = RPClass.Users.First(x => x.UserData.UserID == user.Id);

                if (userObject.ModData.IsMuted == 2)
                {
                    await e.RespondAsync("Fail: user is ultimuted.");
                    return;
                }
                else if (userObject.ModData.IsMuted == 1)
                {
                    userObject.ModData.IsMuted = 0;
                    await user.RevokeRoleAsync(RPClass.PunishedRole);
                    await e.RespondAsync("User unmuted.");

                }
                else
                {
                    userObject.ModData.IsMuted = 1;
                    await user.GrantRoleAsync(RPClass.PunishedRole);
                    await e.RespondAsync("User muted.");
                }
                RPClass.SaveData(1);

            }
            catch
            {
                await e.RespondAsync("NO");
            }
        }

        [Command("ultimatemute"), Aliases("ultmute", "ultimatepunish", "upunish", "umute", "um", "up", "ultimute"), Description("Command for admins to temporarily strip away a user's ranks when muted."), RequireRoles(RoleCheckMode.Any, "Administrator"), IsMuted]
        public async Task UltimateMute(CommandContext e, [Description("Member to be muted")] DiscordMember user)
        {
            try
            {
                
                UserObject.RootObject userObject = RPClass.Users.First(x => x.UserData.UserID == user.Id);

                if (userObject.ModData.IsMuted == 2)
                {
                    userObject.ModData.IsMuted = 0;
                    await user.ReplaceRolesAsync(userObject.ModData.Roles);
                    await e.RespondAsync("User un-ultimuted.");

                }
                else if (userObject.ModData.IsMuted == 1)
                {
                    userObject.ModData.IsMuted = 0;
                    await user.RevokeRoleAsync(RPClass.PunishedRole);
                    await e.RespondAsync("User unmuted.");
                }
                else
                {
                    userObject.ModData.IsMuted = 2;
                    userObject.ModData.Roles = user.Roles.ToList();
                    await user.ReplaceRolesAsync(new List<DiscordRole>() { RPClass.PunishedRole });
                    await e.RespondAsync("User ultimuted.");
                }
                RPClass.SaveData(1);

            }
            catch
            {
                await e.RespondAsync("NO");
            }
        }
    }
}
