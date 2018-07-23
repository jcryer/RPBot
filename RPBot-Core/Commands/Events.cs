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
    [Group("event"), Aliases("e", "ev"), Description("Event commands"), IsMuted]
    class Events
    {
        public static List<EventObject> Participants = new List<EventObject>();

        [Command("adduser"), Aliases("a", "au"), Description("Adds a participant to the event."), RequireRoles(RoleCheckMode.Any, "Staff")]
        public async Task AddUser(CommandContext e, [Description("User to add to the event")] DiscordMember user)
        {
            if (!Participants.Any(x => x.UserId == user.Id))
            {
                Participants.Add(new EventObject(user.Id));
                // Update scoreboard
                // Save file
                await e.RespondAsync("Done!");
                return;
            }
            await e.RespondAsync("That user is already in the event!");
        }

        [Command("removeuser"), Aliases("r", "ru"), Description("Removes a participant from the event."), RequireRoles(RoleCheckMode.Any, "Staff")]
        public async Task RemoveUser(CommandContext e, [Description("User to remove from the event")] DiscordMember user)
        {
            if (Participants.Any(x => x.UserId == user.Id))
            {
                Participants.Remove(new EventObject(user.Id));
                // Update scoreboard
                // Save file
                await e.RespondAsync("Done!");
                return;
            }
            await e.RespondAsync("That user is already in the event!");
        }

        [Command("health"), Aliases("h"), Description("Edits participant health"), RequireRoles(RoleCheckMode.Any, "Staff")]
        public async Task Health(CommandContext e, [Description("User to change health of")] DiscordMember user, int health)
        {
            if (Participants.Any(x => x.UserId == user.Id))
            {
                var participant = Participants.First(x => x.UserId == user.Id);
                participant.Health -= health;
                if (participant.Health < 0) participant.Health = 0;
                aa
                // Update scoreboard
                // Check if user is dead
                // Save file
                await e.RespondAsync("Done!");
                return;
            }
            await e.RespondAsync("That user is not in the event!");
        }

        [Command("reset"), Aliases("r"), Description("Resets event"), RequireOwner]
        public async Task Reset(CommandContext e)
        {
            Participants = new List<EventObject>();
            // Update scoreboard
            // Save file
            await e.RespondAsync("Done!");
        }

    }

    public class EventObject
    {
        public EventObject(ulong userId)
        {
            UserId = userId;
            Points = 0;
            Health = 0;
            Bounty = 0;
            Items = new List<int>();
            InArena = false;
        }

        public ulong UserId;
        public int Points;
        public int Health;
        public int Bounty;
        public List<int> Items;
        public bool InArena;
    }
}
