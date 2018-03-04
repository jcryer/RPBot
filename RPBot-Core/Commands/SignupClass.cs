using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RPBot
{
    [Group("event"), Description("Event commands")]
    class SignupClass : BaseCommandModule
    {
        [Command("create"), Description("Creates an event signup"), RequireRoles(RoleCheckMode.Any, "Staff")]
        public async Task Create(CommandContext e, [RemainingText]string id)
        {
            if (RPClass.SignupList.Any(x => x.Id == id))
            {
                await e.RespondAsync("Fail: Event already exists with that ID.");
                return;
            };
            RPClass.SignupList.Add(new SignupObject.RootObject(id));
            await e.RespondAsync($"Event signup setup! Please, join using the command: `!event join {id}`.");
        }

        [Command("destroy"), Description("Destroys an event signup"), RequireRoles(RoleCheckMode.Any, "Staff")]
        public async Task Destroy(CommandContext e, [RemainingText]string id)
        {
            if (!RPClass.SignupList.Any(x => x.Id == id))
            {
                await e.RespondAsync("Fail: No event exists with that ID.");
                return;
            };
            RPClass.SignupList.Remove(new SignupObject.RootObject(id));
            await e.RespondAsync($"Event signup `{id}` destroyed.");
        }

        [Command("join"), Description("Command to join an event")]
        public async Task Join(CommandContext e, [RemainingText]string id)
        {
            if (!RPClass.SignupList.Any(x => x.Id == id))
            {
                await e.RespondAsync("No event exists with that ID.");
                return;
            };
            if (RPClass.SignupList.First(x => x.Id == id).UserIDs.Any(x => x == e.Member.Id))
            {
                await e.RespondAsync($"You are already signed up to event: {id}");
                return;
            }
            RPClass.SignupList.First(x => x.Id == id).UserIDs.Add(e.Message.Author.Id);
            await e.RespondAsync($"You are now signed up to event: {id}");
        }

        [Command("leave"), Description("Command to leave an event")]
        public async Task Leave(CommandContext e, [RemainingText]string id)
        {
            if (!RPClass.SignupList.Any(x => x.Id == id))
            {
                await e.RespondAsync("No event exists with that ID.");
                return;
            };
            if (!RPClass.SignupList.First(x => x.Id == id).UserIDs.Any(x => x == e.Member.Id))
            {
                await e.RespondAsync($"You are not signed up to event: {id}");
                return;
            }
            RPClass.SignupList.First(x => x.Id == id).UserIDs.Remove(e.Message.Author.Id);
            await e.RespondAsync($"You have now been removed from the event: {id}");
        }

        [Command("list"), Description("List of people in a specific event."), RequireRoles(RoleCheckMode.Any, "Staff")]
        public async Task List(CommandContext e, [RemainingText]string id)
        {
            if (!RPClass.SignupList.Any(x => x.Id == id))
            {
                await e.RespondAsync("No event exists with that ID.");
                return;
            };

            var interactivity = e.Client.GetInteractivity();
            List<Page> interactivityPages = new List<Page>();

            Page p = new Page();

            DiscordEmbedBuilder b = new DiscordEmbedBuilder()
            {
                Color = new DiscordColor("4169E1"),
                Timestamp = DateTime.UtcNow,
                Title = "Event: " + id
            }
            .WithFooter("Heroes & Villains");

            bool even = false;
            var users = await e.Guild.GetAllMembersAsync();

            foreach (ulong userID in RPClass.SignupList.First(x => x.Id == id).UserIDs)
            {
                var user = users.FirstOrDefault(x => x.Id == userID);
                if (user != null)
                {
                    if (!even)
                    {
                        b.AddField(user.DisplayName, "-");
                    }
                    else
                    {
                        b.Fields.Last().Value = user.DisplayName;
                    }
                    even = !even;
                    if (b.Fields.Count >= 20)
                    {
                        p.Embed = b;
                        interactivityPages.Add(p);
                        p = new Page();
                        b.ClearFields();
                        even = false;
                    }
                }
            }

            p.Embed = b;
            interactivityPages.Add(p);
            p = new Page();
            b.ClearFields();
            await interactivity.SendPaginatedMessage(e.Channel, e.Member, interactivityPages, timeoutoverride: TimeSpan.FromSeconds(60));
        }
    }
}
