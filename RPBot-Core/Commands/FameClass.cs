using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RPBot
{
    [Group("fame", CanInvokeWithoutSubcommand = true), Description("Staff command to give fame points"), RequireRoles(RoleCheckMode.Any, "Staff"), IsMuted]
    class FameClass : BaseCommandModule
    {
        public async Task ExecuteGroupAsync(CommandContext e, [Description("User to change fame of")] DiscordMember user, [Description("How much you wish to change it by")] int fameNum)
        {
            if (fameNum != 0)
            {
                UserObject.RootObject userData = RPClass.Users.Find(x => x.UserData.UserID == user.Id);
                userData.UserData.Fame += fameNum;
                if (userData.UserData.Fame < 0) userData.UserData.Fame = 0;

                await Extensions.UpdateFameAndInfamy(e.Guild, 0);
                RPClass.SaveData(1);

                await e.RespondAsync("Stat changed.");
            }
        }

        [Command("comment"), Description("Adds or replaces bounty comment")]
        public async Task Comment(CommandContext e, [Description("User to change comment of")] DiscordMember user, [RemainingText] string comment)
        {
            RPClass.Users.Find(x => x.UserData.UserID == user.Id).UserData.FameComment = comment;
            await Extensions.UpdateFameAndInfamy(e.Guild, 0);
            RPClass.SaveData(1);
            await e.RespondAsync("Done!");
        }
    }

    [Group("infamy", CanInvokeWithoutSubcommand = true), Description("Staff command to give infamy points"), RequireRoles(RoleCheckMode.Any, "Staff"), IsMuted]
    class InfamyClass : BaseCommandModule
    {
        public async Task ExecuteGroupAsync(CommandContext e, [Description("User to change fame of")] DiscordMember user, [Description("How much you wish to change it by")] int infamyNum)
        {
            if (infamyNum != 0)
            {
                UserObject.RootObject userData = RPClass.Users.Find(x => x.UserData.UserID == user.Id);
                userData.UserData.Infamy += infamyNum;
                if (userData.UserData.Infamy < 0) userData.UserData.Infamy = 0;

                await Extensions.UpdateFameAndInfamy(e.Guild, 0);
                RPClass.SaveData(1);

                await e.RespondAsync("Stat changed.");
            }
        }

        [Command("comment"), Description("Adds or replaces bounty comment")]
        public async Task Comment(CommandContext e, [Description("User to change comment of")] DiscordMember user, [RemainingText] string comment)
        {
            RPClass.Users.Find(x => x.UserData.UserID == user.Id).UserData.InfamyComment = comment;
            await Extensions.UpdateFameAndInfamy(e.Guild, 0);
            RPClass.SaveData(1);
            await e.RespondAsync("Done!");
        }
    }
}