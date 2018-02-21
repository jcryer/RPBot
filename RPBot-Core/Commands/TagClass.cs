using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPBot
{
    [Group("tag", CanInvokeWithoutSubcommand = true), Aliases("t"), Description("Commands to manage tags"), IsMuted]
    class TagClass : BaseCommandModule
    {
        public async Task ExecuteGroupAsync(CommandContext e, [RemainingText] string tagName)
        {
            if (RPClass.TagsList.Exists(x => x.Name == tagName.ToLower()))
            {
                await e.RespondAsync(RPClass.TagsList.Find(x => x.Name == tagName.ToLower()).Content);
            }
            else
            {
                await e.RespondAsync("No tag by that name found.");
            }
        }
        [Command("add"), Description("Adds a tag.")]
        public async Task Add(CommandContext e, [Description("Name of the tag (in speech marks)")] string tagName, [RemainingText, Description("Tag info")] string text)
        {
            if (RPClass.TagsList.Exists(x => x.Name == tagName.ToLower()))
            {
                await e.RespondAsync("A tag already exists with that name.");
                return;
            }
            if (string.IsNullOrEmpty(text))
            {
                await e.RespondAsync("You have nothing in this tag.");
                return;
            }

            RPClass.TagsList.Add(new TagObject.RootObject(tagName.ToLower(), text));
            await e.RespondAsync("Tag: " + tagName + " added.");
            RPClass.SaveData(9);
        }

        [Command("list"), Description("Lists all tags.")]
        public async Task List(CommandContext e)
        {
            var interactivity = e.Client.GetInteractivity();
            List<Page> interactivityPages = new List<Page>();

            Page p = new Page();

            DiscordEmbedBuilder b = new DiscordEmbedBuilder()
            {
                Color = new DiscordColor("4169E1"),
                Timestamp = DateTime.UtcNow
            }
            .WithFooter("Heroes & Villains");
            bool even = false;
            foreach (TagObject.RootObject t in RPClass.TagsList)
            {                
                if (!even)
                {
                    b.AddField(t.Name, "-");
                }
                else
                {
                    b.Fields.Last().Value = t.Name;
                }
                even = !even;
                if (b.Fields.Count >= 10 && !even)
                {
                    p.Embed = b;
                    interactivityPages.Add(p);
                    p = new Page();
                    b.ClearFields();
                    even = false;
                }
            }
            p.Embed = b;
            interactivityPages.Add(p);
            p = new Page();
            b.ClearFields();
            await interactivity.SendPaginatedMessage(e.Channel, e.Member, interactivityPages, timeoutoverride: TimeSpan.FromSeconds(60));



        }

        [Command("remove"), Description("Adds a tag."), RequireRoles(RoleCheckMode.Any, "Staff")]
        public async Task Remove(CommandContext e, [Description("Name of the tag "), RemainingText] string tagName)
        {
            if (RPClass.TagsList.Exists(x => x.Name == tagName.ToLower()))
            {
                RPClass.TagsList.Remove(RPClass.TagsList.Find(x => x.Name == tagName.ToLower()));
                await e.RespondAsync("Tag: " + tagName + " removed.");
                RPClass.SaveData(9);
            }
            else
            {
                await e.RespondAsync("No tag by that name found.");
            }
        }
    }
}
