using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RPBot
{
    [Group("tag", CanInvokeWithoutSubcommand = true), Aliases("t"), Description("Commands to manage tags")]
    class TagClass : RPClass
    {
        public async Task ExecuteGroupAsync(CommandContext e, [RemainingText] string tagName)
        {
            if (TagsList.Exists(x => x.name == tagName.ToLower()))
            {
                await e.RespondAsync(TagsList.Find(x => x.name == tagName.ToLower()).content);
            }
            else
            {
                await e.RespondAsync("No tag by that name found.");
            }
        }
        [Command("add"), Description("Adds a tag.")]
        public async Task Add(CommandContext e, [Description("Name of the tag (in speech marks)")] string tagName, [RemainingText, Description("Tag info")] string text)
        {
            if (TagsList.Exists(x => x.name == tagName.ToLower()))
            {
                await e.RespondAsync("A tag already exists with that name.");
                return;
            }
            if (string.IsNullOrEmpty(text))
            {
                await e.RespondAsync("You have nothing in this tag.");
                return;
            }

            TagsList.Add(new TagObject.RootObject(tagName.ToLower(), text));
            await e.RespondAsync("Tag:" + tagName + " added.");
            SaveData(9);
        }

        [Command("list"), Description("Lists all tags.")]
        public async Task List(CommandContext e)
        {
            string retVal = "```\n";
            foreach (TagObject.RootObject t in TagsList)
            {
                retVal += t.name + "\n";
                if (retVal.Length > 1500)
                {
                    await e.RespondAsync(retVal + "```");
                    retVal = "```\n";
                }
            }
            await e.RespondAsync(retVal + "```");

        }

        [Command("remove"), Description("Adds a tag."), RequireRolesAttribute("Staff")]
        public async Task Remove(CommandContext e, [Description("Name of the tag "), RemainingText] string tagName)
        {
            if (TagsList.Exists(x => x.name == tagName.ToLower()))
            {
                TagsList.Remove(TagsList.Find(x => x.name == tagName.ToLower()));
                await e.RespondAsync("Tag: " + tagName + " removed.");
                SaveData(9);
            }
            else
            {
                await e.RespondAsync("No tag by that name found.");
            }
        }
    }
}
