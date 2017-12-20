using DSharpPlus;
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
   
    [Group("instance"), Description("Instance commands")]
    class InstanceClass : RPClass
    {
        [Command("add"), Description("Command to create a new location instance."), RequireRolesAttribute("Staff", "Bot-Test")]
        public async Task Add(CommandContext e, [Description("Give the ID of the channel you wish to create from the !instance channels command.")]int channelID)
        {
            InstanceObject.ChannelTemplate template = ChannelTemplates.FirstOrDefault(x => x.id == channelID);
            if (template != null)
            {
                int instanceID = InstanceList.Last().id + 1;

                DiscordChannel c = await e.Guild.CreateChannelAsync(instanceID + ":" + template.name, ChannelType.Text, parent: InstanceCategory);
                InstanceList.Add(new InstanceObject.RootObject(instanceID, c.Id, template.id));
                if (template.content.Count > 0)
                {
                    foreach (string content in template.content)
                    {
                        await c.SendMessageAsync(content);
                    }
                }
                SaveData(7);
                await e.RespondAsync("Channel: " + template.name + " created with ID: " + instanceID + ".");
            }
            else
            {
                await e.RespondAsync("Enter a valid channel ID.");
            }
        }

        [Command("end"), Description("Command to end a roleplay."), RequireRolesAttribute("Staff")]
        public async Task End(CommandContext e, [Description("Quote the ID at the beginning of the channel name.")]int rpID)
        {
            InstanceObject.RootObject instance = InstanceList.FirstOrDefault(x => x.id == rpID);
            if (instance != null)
            {

                DiscordChannel c = e.Guild.GetChannel(instance.channelID);
                await c.AddOverwriteAsync(e.Guild.EveryoneRole, Permissions.ReadMessageHistory, Permissions.SendMessages);
                instance.active = false;

                SaveData(7);
                await e.RespondAsync("Instance closed.");
            }
            else
            {
                await e.RespondAsync("Use the ID at the beginning of the channel name.");
            }
        }

        [Command("destroy"), Description("Command to delete an instance."), RequireRolesAttribute("Administrator")]
        public async Task Destroy(CommandContext e, [Description("Quote the ID at the beginning of the channel name.")]int rpID)
        {
            InstanceObject.RootObject instance = InstanceList.FirstOrDefault(x => x.id == rpID);
            if (instance != null)
            {
                DiscordChannel c = e.Guild.GetChannel(instance.channelID);
                await c.DeleteAsync();
                InstanceList.Remove(instance);
                SaveData(7);
                await e.RespondAsync("Instance destroyed.");
            }
            else
            {
                await e.RespondAsync("Use the ID at the beginning of the channel name.");
            }
        }


        [Command("addtemplate"), Description("Admin command to add a channel template to the list."), RequireRolesAttribute("Staff")]
        public async Task AddTemplate(CommandContext e, [Description("Name of channel for template.")]string name, [Description("All text to be displayed at the start of the instance. Send in multiple messages, as the character limit is 2000. If there is more than one message, end the message with '¬' and start the next message with '¬'."), RemainingText] string content)
        {
            List<string> ContentList = new List<string>();
            ContentList.Add(content.Replace("¬", ""));
            if (content.Contains("¬"))
            {
                var interactivity = e.Client.GetInteractivityModule();

                AnotherMessage:

                var msg = await interactivity.WaitForMessageAsync(x => x.Content.StartsWith("¬"), TimeSpan.FromSeconds(120));
                if (msg != null)
                {
                    await e.RespondAsync("Message added to end of template.");
                    string strippedContent = msg.Message.Content.Replace("¬", "");
                    ContentList.Add(strippedContent);
                    if (msg.Message.Content.EndsWith("¬"))
                    {
                        goto AnotherMessage;
                    }
                    else
                    {
                        ChannelTemplates.Add(new InstanceObject.ChannelTemplate(ChannelTemplates.Count + 1, name, ContentList));
                        SaveData(6);
                        await e.RespondAsync("Template added and saved.");
                    }
                }
                else
                {
                    ChannelTemplates.Add(new InstanceObject.ChannelTemplate(ChannelTemplates.Count + 1, name, ContentList));
                    SaveData(6);
                    await e.RespondAsync("Template added and saved.");
                }
            }
            else
            {
                ChannelTemplates.Add(new InstanceObject.ChannelTemplate(ChannelTemplates.Count + 1, name, ContentList));
                SaveData(6);
                await e.RespondAsync("Template added and saved.");
            }
        }
        [Command("channels"), Description("Command to list all channel templates and their IDs."), RequireRolesAttribute("Staff")]
        public async Task ListChannels(CommandContext e)
        {
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder();
            embed.Color = new DiscordColor("4169E1");
            embed.WithFooter("Heroes & Villains");
            embed.WithTimestamp(DateTime.UtcNow);
            foreach (InstanceObject.ChannelTemplate c in ChannelTemplates)
            {
                if (embed.Fields.Count > 24)
                {
                    await e.RespondAsync("", embed: embed);
                    embed.ClearFields();
                }
                embed.AddField(c.id.ToString(), c.name);
            }
            await e.RespondAsync("", embed: embed);
        }
    }
}
