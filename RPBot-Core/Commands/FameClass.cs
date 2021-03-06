﻿using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Linq;
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
                await Extensions.UpdateFameAndInfamyRoles(userData.UserData.Fame, userData.UserData.Infamy, user, userData.UserData.Role == 1 ? true : false);
                await Extensions.UpdateFameAndInfamy(0);
                RPClass.SaveData(1);

                await e.RespondAsync("Stat changed.");
            }
        }

        [Command("comment"), Description("Adds or replaces bounty comment")]
        public async Task Comment(CommandContext e, [Description("User to change comment of")] DiscordMember user, [RemainingText, Description("Comment for bounty, put '-' for no comment.")] string comment)
        {
            if (comment == "-") comment = " ";
            RPClass.Users.Find(x => x.UserData.UserID == user.Id).UserData.FameComment = comment;
            await Extensions.UpdateFameAndInfamy(0);
            RPClass.SaveData(1);
            await e.RespondAsync("Done!");
        }

        [Command("update"), Description("Updates Fame/Infamy Board")]
        public async Task Update(CommandContext e)
        {
            var members = await e.Guild.GetAllMembersAsync();
            foreach (var user in RPClass.Users.Where(x => x.UserData.Fame > 0 || x.UserData.Infamy > 0)) 
            {
                await Extensions.UpdateFameAndInfamyRoles(user.UserData.Fame, user.UserData.Infamy, members.First(x => x.Id == user.UserData.UserID), user.UserData.Role == 1 ? true : false);
            }
            await Extensions.UpdateFameAndInfamy(0);
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
                await Extensions.UpdateFameAndInfamyRoles(userData.UserData.Fame, userData.UserData.Infamy, user, userData.UserData.Role == 1 ? true : false);
                await Extensions.UpdateFameAndInfamy(0);
                RPClass.SaveData(1);

                await e.RespondAsync("Stat changed.");
            }
        }

        [Command("comment"), Description("Adds or replaces bounty comment")]
        public async Task Comment(CommandContext e, [Description("User to change comment of")] DiscordMember user, [RemainingText, Description("Comment for bounty, put '-' for no comment.")] string comment)
        {
            if (comment == "-") comment = " ";
            RPClass.Users.Find(x => x.UserData.UserID == user.Id).UserData.InfamyComment = comment;
            await Extensions.UpdateFameAndInfamy(0);
            RPClass.SaveData(1);
            await e.RespondAsync("Done!");
        }

        [Command("update"), Description("Updates Fame/Infamy Board")]
        public async Task Update(CommandContext e)
        {
            var members = await e.Guild.GetAllMembersAsync();
            foreach (var user in RPClass.Users.Where(x => x.UserData.Fame > 0 || x.UserData.Infamy > 0))
            {
                await Extensions.UpdateFameAndInfamyRoles(user.UserData.Fame, user.UserData.Infamy, members.First(x => x.Id == user.UserData.UserID), user.UserData.Role == 1 ? true : false);
            }
            await Extensions.UpdateFameAndInfamy(0);
            RPClass.SaveData(1);
            await e.RespondAsync("Done!");
        }
    }
}