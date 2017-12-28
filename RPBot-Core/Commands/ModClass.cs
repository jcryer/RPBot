using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RPBot
{
    class ModClass : RPClass
    {
        [Command("punish"), Description("Command for staff to give out the Punished role."), RequireRoles(RoleCheckMode.Any, "Staff", "Helpful people")]
        public async Task Punish(CommandContext e, [Description("Member to be muted")] DiscordMember user)
        {
            try
            {
                DiscordMember member = await e.Guild.GetMemberAsync(user.Id);
                UserObject.RootObject userObject = Users.First(x => x.UserData.UserID == user.Id);

                if (userObject.ModData.IsMuted)
                {
                    userObject.ModData.IsMuted = false;
                    await member.RevokeRoleAsync(PunishedRole);
                    await e.RespondAsync("User unmuted.");

                }
                else
                {
                    //if (string.IsNullOrWhiteSpace(duration))
                    //{
                    userObject.ModData.IsMuted = true;
                    await member.GrantRoleAsync(PunishedRole);
                    await e.RespondAsync("User muted.");

                    userObject.ModData.MuteDuration = new TimeSpan();
                    //}
                    /*
                    else
                    {
                        userObject.ModData.isMuted = true;
                        try
                        {
                            userObject.ModData.muteDuration = TimeSpan.Parse(duration);
                            await e.RespondAsync("User muted for: " + duration);
                        }
                        catch
                        {
                            userObject.ModData.muteDuration = new TimeSpan();
                            await e.RespondAsync("User muted.");

                        }
                    }*/
                }
                SaveData(1);

            }
            catch
            {
                await e.RespondAsync("No guild found with that name. Are you sure you typed it in correctly?");
            }
        }
    }
}
