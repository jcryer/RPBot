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
   // DEPRECATED - COMMENTED OUT IN RPBOT.CS
    [Group("instance"), Description("Instance commands"), IsMuted]
    class InstanceClass : BaseCommandModule
    {
        [Command("add"), Description("Command to create a new location instance."), RequireRoles(RoleCheckMode.Any, "Staff")]
        public async Task Add(CommandContext e, [Description("Give the ID of the channel you wish to create from the !instance channels command.")]int channelID)
        {
            InstanceObject.ChannelTemplate template = RPClass.ChannelTemplates.FirstOrDefault(x => x.Id == channelID);
            if (template != null)
            {
                int instanceID = 1;
                if (RPClass.InstanceList.Count > 0)
                {
                    instanceID = RPClass.InstanceList.Last().Id + 1;
                }

                DiscordChannel c = await e.Guild.CreateChannelAsync(instanceID + "-" + template.Name, ChannelType.Text, parent: RPClass.InstanceCategory);
                RPClass.InstanceList.Add(new InstanceObject.RootObject(instanceID, c.Id, template.Id));
                if (template.Content.Count > 0)
                {
                    foreach (string content in template.Content)
                    {
                        await c.SendMessageAsync(content);
                    }
                }
                RPClass.SaveData(7);
                await e.RespondAsync("Channel: " + template.Name + " created with ID: " + instanceID + ".");
            }
            else
            {
                await e.RespondAsync("Enter a valid channel ID.");
            }
        }

        [Command("end"), Description("Command to end a roleplay."), RequireRoles(RoleCheckMode.Any, "Staff")]
        public async Task End(CommandContext e, [Description("Quote the ID at the beginning of the channel name.")]int rpID)
        {
            InstanceObject.RootObject instance = RPClass.InstanceList.FirstOrDefault(x => x.Id == rpID);
            if (instance != null)
            {

                DiscordChannel c = e.Guild.GetChannel(instance.ChannelID);
                await c.AddOverwriteAsync(e.Guild.EveryoneRole, Permissions.ReadMessageHistory, Permissions.SendMessages);
                instance.Active = false;

                RPClass.SaveData(7);
                await e.RespondAsync("Instance closed.");
            }
            else
            {
                await e.RespondAsync("Use the ID at the beginning of the channel name.");
            }
        }

        [Command("destroy"), Description("Command to delete an instance."), RequireRoles(RoleCheckMode.Any, "Administrator")]
        public async Task Destroy(CommandContext e, [Description("Quote the ID at the beginning of the channel name.")]int rpID)
        {
            InstanceObject.RootObject instance = RPClass.InstanceList.FirstOrDefault(x => x.Id == rpID);
            if (instance != null)
            {
                DiscordChannel c = e.Guild.GetChannel(instance.ChannelID);
                await c.DeleteAsync();
                RPClass.InstanceList.Remove(instance);
                RPClass.SaveData(7);
                await e.RespondAsync("Instance destroyed.");
            }
            else
            {
                await e.RespondAsync("Use the ID at the beginning of the channel name.");
            }
        }


        [Command("addtemplate"), Description("Admin command to add a channel template to the list."), RequireRoles(RoleCheckMode.Any, "Staff")]
        public async Task AddTemplate(CommandContext e, [Description("Name of channel for template.")]string name, [Description("All text to be displayed at the start of the instance. Send in multiple messages, as the character limit is 2000. If there is more than one message, end the message with '¬' and start the next message with '¬'."), RemainingText] string content)
        {
            List<string> ContentList = new List<string>
            {
                content.Replace("¬", "")
            };
            if (content.Contains("¬"))
            {
                var interactivity = e.Client.GetInteractivity();

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
                        RPClass.ChannelTemplates.Add(new InstanceObject.ChannelTemplate(RPClass.ChannelTemplates.Count + 1, name, ContentList));
                        RPClass.SaveData(6);
                        await e.RespondAsync("Template added and saved.");
                    }
                }
                else
                {
                    RPClass.ChannelTemplates.Add(new InstanceObject.ChannelTemplate(RPClass.ChannelTemplates.Count + 1, name, ContentList));
                    RPClass.SaveData(6);
                    await e.RespondAsync("Template added and saved.");
                }
            }
            else
            {
                RPClass.ChannelTemplates.Add(new InstanceObject.ChannelTemplate(RPClass.ChannelTemplates.Count + 1, name, ContentList));
                RPClass.SaveData(6);
                await e.RespondAsync("Template added and saved.");
            }
        }
        [Command("channels"), Description("Command to list all channel templates and their IDs."), RequireRoles(RoleCheckMode.Any, "Staff")]
        public async Task ListChannels(CommandContext e)
        {
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder
            {
                Color = new DiscordColor("4169E1"),
                Timestamp = DateTime.UtcNow
            }
            .WithFooter("Heroes & Villains");
            foreach (InstanceObject.ChannelTemplate c in RPClass.ChannelTemplates)
            {
                if (embed.Fields.Count > 24)
                {
                    await e.RespondAsync("", embed: embed);
                    embed.ClearFields();
                }
                embed.AddField(c.Id.ToString(), c.Name);
            }
            await e.RespondAsync("", embed: embed);
        }
    }
}
