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
    [Group("instance"), Description("Instancing commands")]
    class InstanceClass : RPClass
    {
        [Command("create"), Description("Creates a roleplay instance."), RequireRolesAttribute("Staff", "Bot-Test", "Bot-Test")]
        public async Task CreateRolePlay(CommandContext e, [Description("Title for the event.")]string name, [Description("All people part of the roleplay. Mention them, separated by a space."), RemainingText] string includedPeople)
        {
            DiscordChannel newCategory = await e.Guild.CreateChannelAsync(InstanceList.Count + 1 + ": " + name, ChannelType.Category);
            //await newCategory.AddOverwriteAsync(e.Guild.EveryoneRole, Permissions.SendMessages, Permissions.None);
            await newCategory.AddOverwriteAsync(e.Guild.EveryoneRole, Permissions.ReadMessageHistory, Permissions.SendMessages);
            await newCategory.AddOverwriteAsync(StaffRole, Permissions.SendMessages, Permissions.None);
            List<ulong> mentionedMembers = new List<ulong>();
            mentionedMembers.Add(e.Member.Id);
            await newCategory.AddOverwriteAsync(e.Member, Permissions.SendMessages, Permissions.None);

            if (!string.IsNullOrWhiteSpace(includedPeople))
            {
                IReadOnlyList<DiscordMember> allMembers = await e.Guild.GetAllMembersAsync();
                includedPeople = includedPeople.Replace("<", "").Replace(">", "").Replace("@", "").Replace("!", "");
                List<string> mentions = includedPeople.Split(' ').ToList();

                foreach (string mention in mentions)
                {
                    ulong memberID = 0;
                    if (ulong.TryParse(mention, out memberID))
                    {
                        if (allMembers.Any(x => x.Id == memberID))
                        {
                            mentionedMembers.Add(memberID);
                            await newCategory.AddOverwriteAsync(allMembers.First(x => x.Id == memberID), Permissions.SendMessages, Permissions.None);
                        }
                    }
                }
                int id = InstanceList.Count + 1;
                InstanceList.Add(new InstanceObject.RootObject(id, name, newCategory.Id, new List<ulong>(), new List<int>(), mentionedMembers));
                await e.RespondAsync("New instance made with ID: " + id + " -  Quote this when adding channels to your instance.");
                SaveData(7);
            }
            else
            {
                int id = InstanceList.Count + 1;
                InstanceList.Add(new InstanceObject.RootObject(id, name, newCategory.Id, new List<ulong>(), new List<int>(), mentionedMembers));
                await e.RespondAsync("New instance made with ID: " + id + " -  Quote this when adding channels to your instance.");
                SaveData(7);
            }
        }

        [Command("addchannel"), Description("Command to add a channel to a roleplaying instance."), RequireRolesAttribute("Staff", "Bot-Test", "Bot-Test")]
        public async Task CreateInstance(CommandContext e, [Description("Quote your Roleplay ID given on creation of your instance (also on the name of the category).")]int rpID, [Description("Give the ID of the channel you wish to create from the !instance channels command.")]int channelID)
        {
            InstanceObject.RootObject instance = InstanceList.FirstOrDefault(x => x.id == rpID);
            if (instance != null)
            {
                if (instance.active)
                {
                    DiscordChannel category = e.Guild.GetChannel(instance.categoryID);

                    InstanceObject.ChannelTemplate template = ChannelTemplates.FirstOrDefault(x => x.id == channelID);
                    if (template != null)
                    {
                        if (!instance.channelTemplateIDs.Contains(template.id))
                        {
                            DiscordChannel newChannel = await e.Guild.CreateChannelAsync(template.name, ChannelType.Text, parent: category);
                            instance.channelIDs.Add(newChannel.Id);
                            instance.channelTemplateIDs.Add(template.id);
                            foreach (string content in template.content)
                            {
                                await newChannel.SendMessageAsync(content);
                            }
                            SaveData(7);
                            await e.RespondAsync("Channel: " + template.name + " created on instance with ID: " + instance.id + ".");
                        }
                        else
                        {
                            await e.RespondAsync("This channel already exists!");

                        }

                    }
                    else
                    {
                        await e.RespondAsync("Use a channel template ID.");

                    }
                }
                else
                {
                    await e.RespondAsync("This RP has completed.");
                }
            }
            else
            {
                await e.RespondAsync("Use your RP ID.");
            }
        }

        [Command("adduser"), Description("Adds a user to a roleplay instance."), RequireRolesAttribute("Staff", "Bot-Test", "Bot-Test")]
        public async Task AddUser(CommandContext e, [Description("Quote your Roleplay ID given on creation of your instance (also on the name of the category).")]int rpID, [Description("People to add to the roleplay. them, separated by a space."), RemainingText] string includedPeople)
        {
            InstanceObject.RootObject instance = InstanceList.FirstOrDefault(x => x.id == rpID);
            if (instance != null)
            {
                if (instance.active)
                {
                    DiscordChannel category = e.Guild.GetChannel(instance.categoryID);

                    IReadOnlyList<DiscordMember> allMembers = await e.Guild.GetAllMembersAsync();
                    includedPeople = includedPeople.Replace("<", "").Replace(">", "").Replace("@", "").Replace("!", "");
                    List<string> mentions = includedPeople.Split(' ').ToList();

                    foreach (string mention in mentions)
                    {
                        ulong memberID = 0;
                        if (ulong.TryParse(mention, out memberID))
                        {
                            if (allMembers.Any(x => x.Id == memberID))
                            {
                                await category.AddOverwriteAsync(allMembers.First(x => x.Id == memberID), Permissions.SendMessages, Permissions.None);
                                instance.userIDs.Add(memberID);
                            }
                        }
                    }
                    await e.RespondAsync("Users added to instance with ID: " + instance.id + "!");
                    SaveData(7);
                }
                else
                {
                    await e.RespondAsync("This RP has completed.");
                }
            }
            else
            {
                await e.RespondAsync("Use your RP ID.");
            }
        }

        [Command("end"), Description("Command to end a roleplay."), RequireRolesAttribute("Staff", "Bot-Test", "Bot-Test")]
        public async Task EndRolePlay(CommandContext e, [Description("Quote your Roleplay ID given on creation of your instance (also on the name of the category).")]int rpID)
        {
            InstanceObject.RootObject instance = InstanceList.FirstOrDefault(x => x.id == rpID);
            if (instance != null)
            {
                DiscordChannel category = e.Guild.GetChannel(instance.categoryID);
                IReadOnlyList<DiscordOverwrite> Overwrites = category.PermissionOverwrites;
                List<DiscordOverwrite> OverwriteList = new List<DiscordOverwrite>(Overwrites);
                foreach (DiscordOverwrite overwrite in OverwriteList)
                {
                    try
                    {
                        await category.UpdateOverwriteAsync(overwrite, Permissions.ReadMessageHistory, Permissions.SendMessages);
                    }
                    catch
                    {

                    }
                }
                instance.active = false;
                    
                SaveData(7);
                await e.RespondAsync("Instance closed.");
            }
            else
            {
                await e.RespondAsync("Use your RP ID.");
            }
        }

        [Command("destroy"), Description("Command to delete a roleplay category."), RequireRolesAttribute("Administrator", "Bot-Test")]
        public async Task Destroy(CommandContext e, [Description("Quote your Roleplay ID given on creation of your instance (also on the name of the category).")]int rpID)
        {
            InstanceObject.RootObject instance = InstanceList.FirstOrDefault(x => x.id == rpID);
            if (instance != null)
            {
                foreach (ulong test in instance.channelIDs)
                {
                    await e.Guild.GetChannel(test).DeleteAsync();
                }
                await e.Guild.GetChannel(instance.categoryID).DeleteAsync();
                Console.WriteLine(instance.categoryID);
                InstanceList.Remove(instance);

                SaveData(7);
                await e.RespondAsync("Instance destroyed.");
            }
            else
            {
                await e.RespondAsync("Use your RP ID.");
            }
        }


        [Command("addtemplate"), Description("Admin command to add a channel template to the list."), RequireRolesAttribute("Staff", "Bot-Test", "Bot-Test")]
        public async Task AddTemplate(CommandContext e, [Description("Name of channel for template.")]string name, [Description("All text to be displayed at the start of the instance. Send in multiple messages, as the character limit is 2000. If there is more than one message, end the message with '¬' and start the next message with '¬'."), RemainingText] string content)
        {
            List<string> ContentList = new List<string>();
            ContentList.Add(content.Replace("¬",""));
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
        [Command("channels"), Description("Command to list all channel templates and their IDs."), RequireRolesAttribute("Staff", "Bot-Test", "Bot-Test")]
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
