using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wiki = DotNetWikiBot;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus.Entities;
using System.Text.RegularExpressions;
using System.Linq;
using DotNetWikiBot;

namespace RPBot
{
    [Group("wiki")]
    class WikiClass : BaseCommandModule
    {
        public static Dictionary<List<string>, Action<string, Wiki.Page>> WikiFields = new Dictionary<List<string>, Action<string, Wiki.Page>>();
        public static WikiObject.InfoBox InfoBox = new WikiObject.InfoBox();
        public static Wiki.Site WikiSite;

        public static void InitWiki()
        {
             WikiSite = new Wiki.Site("http://roleplay-heroes-and-villains.wikia.com", "jcryer", "Gracie3038");
            //  WikiSite = new Wiki.Site("http://heroes-and-villains-rp.wikia.com", "jcryer", "Gracie3038");

            Action<string, Wiki.Page> Personality = (string message, Wiki.Page page) =>
            {
                if (message != "-") page.text += Environment.NewLine + "== Personality ==" + Environment.NewLine + message;
            };

            WikiFields.Add(new List<string> { "Personality", "Please enter personality text. To send over 2000 characters, add a `¬` at the end of each message that *isn't* the final message.\nSend `-` if the field should be blank." }, Personality);

            Action<string, Wiki.Page> Backstory = (string message, Wiki.Page page) =>
            {
                if (message != "-") page.text += Environment.NewLine + "== Backstory ==" + Environment.NewLine + message;
            };

            WikiFields.Add(new List<string> { "Backstory", "Please enter backstory text. To send over 2000 characters, add a `¬` at the end of each message that *isn't* the final message.\nSend `-` if the field should be blank." }, Backstory);

            Action<string, Wiki.Page> Resources = (string message, Wiki.Page page) =>
            {
                if (message != "-") page.text += Environment.NewLine + "== Resources ==" + Environment.NewLine + message;
            };

            WikiFields.Add(new List<string> { "Resources", "Please enter resources text. To send over 2000 characters, add a `¬` at the end of each message that *isn't* the final message.\nSend `-` if the field should be blank." }, Resources);

            Action<string, Wiki.Page> Equipment = (string message, Wiki.Page page) =>
            {
                if (message != "-") page.text += Environment.NewLine + "=== Equipment/Weaponry ===" + Environment.NewLine + message;
            };

            WikiFields.Add(new List<string> { "Equipment", "Please enter Equipment text. To send over 2000 characters, add a `¬` at the end of each message that *isn't* the final message.\nSend `-` if the field should be blank." }, Equipment);

            Action<string, Wiki.Page> Specialisations = (string message, Wiki.Page page) =>
            {
                if (message != "-") page.text += Environment.NewLine + "=== Specializations ===" + Environment.NewLine + message;
            };

            WikiFields.Add(new List<string> { "Specialisations", "Please enter Specialisations text. To send over 2000 characters, add a `¬` at the end of each message that *isn't* the final message.\nSend `-` if the field should be blank." }, Specialisations);

            Action<string, Wiki.Page> Quirk = (string message, Wiki.Page page) =>
            {
                if (message != "-") page.text += Environment.NewLine + "== Quirk ==" + Environment.NewLine + message;
            };
            WikiFields.Add(new List<string> { "Quirk", "Please enter Quirk text. To send over 2000 characters, add a `¬` at the end of each message that *isn't* the final message.\nSend `-` if the field should be blank." }, Quirk);

            Action<string, Wiki.Page> Versatility = (string message, Wiki.Page page) =>
            {
                if (message != "-") page.text += Environment.NewLine + "=== Versatility ===" + Environment.NewLine + message;
            };

            WikiFields.Add(new List<string> { "Versatility", "Please enter Quirk Versatility text. To send over 2000 characters, add a `¬` at the end of each message that *isn't* the final message.\nSend `-` if the field should be blank." }, Versatility);

            Action<string, Wiki.Page> Example = (string message, Wiki.Page page) =>
            {
                if (message != "-") page.text += Environment.NewLine + "=== Example ===" + Environment.NewLine + message;
            };

            WikiFields.Add(new List<string> { "Example", "Please enter Quirk Example text. To send over 2000 characters, add a `¬` at the end of each message that *isn't* the final message.\nSend `-` if the field should be blank." }, Example);
        }

        [Command("addoc")]
        public async Task AddOCAsync(CommandContext e, [RemainingText]string charName)
        {
            string role = "";
            Wiki.Page page = new Wiki.Page(WikiSite, charName);
            page.text = "";

            var interactivity = e.Client.GetInteractivity();

            var embed = new DiscordEmbedBuilder()
            {
                Title = "Character Data",
                Color = DiscordColor.Red
            };


            var infoBoxEmbed = new DiscordEmbedBuilder()
            {
                Title = "Infobox",
                Color = DiscordColor.SpringGreen
            };

            DiscordMessage embedMessage = await e.RespondAsync(embed: embed);
            DiscordMessage infoBoxEmbedMessage = await e.RespondAsync(embed: infoBoxEmbed);
            DiscordMessage mainMessage = await e.RespondAsync("Okay, setting up!");

            
            foreach (var pair in WikiFields)
            {
                bool errored = false;
                await mainMessage.ModifyAsync(pair.Key[1]);
                string content = "";
                AnotherMessage:
                var msg = await interactivity.WaitForMessageAsync(x => x.Author == e.Member, TimeSpan.FromSeconds(120));
                if (msg != null)
                {
                    content += msg.Message.Content.Replace("¬", "") + Environment.NewLine;

                    if (msg.Message.Content.Contains("¬"))
                    {
                        await msg.Message.DeleteAsync();
                        goto AnotherMessage;
                    }
                    else if (msg.Message.Content.ToLower() == "stop")
                    {
                        goto End;
                    }
                    Errored:
                    if (content.Length < 1024)
                    {
                        embed.AddField(pair.Key[0], content);
                    }
                    else
                    {
                        var strings = content.Split(1024);
                        foreach (var s in strings)
                        {
                            embed.AddField(pair.Key[0], new string(s.ToArray()));
                        }
                    }
                    pair.Value(content, page);

                    if (errored)
                    {
                        embedMessage = await e.RespondAsync(embed: embed);
                        await infoBoxEmbedMessage.DeleteAsync();
                        await mainMessage.DeleteAsync();
                        infoBoxEmbedMessage = await e.RespondAsync(embed: infoBoxEmbed);
                        mainMessage = await e.RespondAsync("Okay, setting up!");
                    }
                    else
                    {
                        try
                        {
                            await embedMessage.ModifyAsync(embed: embed.Build());
                        }
                        catch
                        {
                            embed.ClearFields();
                            errored = true;
                            goto Errored;
                        }
                    }
                    await mainMessage.ModifyAsync("Added field!");
                    await msg.Message.DeleteAsync();
                    await Task.Delay(1000);
                }
            }
            
            foreach (var infoBoxField in InfoBox.Fields)
            {
                await mainMessage.ModifyAsync(infoBoxField.Question + "\nSend `-` if the field should be blank.");
                Failed:
                var msg = await interactivity.WaitForMessageAsync(x => x.Author == e.Member, TimeSpan.FromSeconds(120));
                if (msg != null)
                {
                    if (infoBoxField.FieldId == "affiliation")
                    {
                        string message = msg.Message.Content.ToLower();
                        if (message.Contains("hero"))
                        {
                            role = "Heroes";
                            infoBoxField.FieldValue = "Pro Hero";
                        }
                        else if (message.Contains("rogue") || message.Contains("rouge"))
                        {
                            role = "Rogues";
                            infoBoxField.FieldValue = "Rogue";
                        }
                        else if (message.Contains("villain"))
                        {
                            role = "Villains";
                            infoBoxField.FieldValue = "Villain";
                        }
                        else if (message.Contains("academy student"))
                        {
                            role = "Academy Students";
                            infoBoxField.FieldValue = "Academy Student";
                        }
                        else
                        {
                            await msg.Message.DeleteAsync();
                            goto Failed;
                        }
                        infoBoxEmbed.AddField(infoBoxField.FieldId, infoBoxField.FieldValue);
                    }
                    else
                    {
                        if (msg.Message.Content.ToLower() == "stop")
                        {
                            goto End;
                        }
                        infoBoxField.FieldValue = msg.Message.Content;
                        try
                        {
                            if (infoBoxField.FieldValue.Length < 1024)
                            {
                                infoBoxEmbed.AddField(infoBoxField.FieldId, infoBoxField.FieldValue);
                            }
                            else
                            {
                                var strings = infoBoxField.FieldValue.Split(1024);
                                foreach (var s in strings)
                                {
                                    infoBoxEmbed.AddField(infoBoxField.FieldId, infoBoxField.FieldValue);
                                }
                            }
                        }
                        catch
                        {
                            await msg.Message.DeleteAsync();
                            goto Failed;
                        }
                    }
                    await infoBoxEmbedMessage.ModifyAsync(embed: infoBoxEmbed.Build());
                    await msg.Message.DeleteAsync();
                    await mainMessage.ModifyAsync("Added field!");
                    await Task.Delay(1000);
                }

            }
            
            imageMessage:
            await mainMessage.ModifyAsync("Please link an image for the infobox.");

            var imageMessage = await interactivity.WaitForMessageAsync(x => x.Author == e.Member, TimeSpan.FromSeconds(120));
            if (imageMessage != null)
            {

                string imageName;
                if (imageMessage.Message.Content.ToLower() == "stop")
                {
                    goto End;
                }
                Regex ItemRegex = new Regex(@"\.(png|gif|jpg|jpeg|tiff|webp)");
                if (ItemRegex.IsMatch(imageMessage.Message.Content))
                {
                    imageName = imageMessage.Message.Content.Split('/').Last();

                    Wiki.Page p = new Wiki.Page(WikiSite, "File:" + imageName);
                    p.UploadImageFromWeb(imageMessage.Message.Content, "N/A", "N/A", "N/A");
                    infoBoxEmbed.ImageUrl = imageMessage.Message.Content;
                    await imageMessage.Message.DeleteAsync();
                    await infoBoxEmbedMessage.ModifyAsync(embed: infoBoxEmbed.Build());
                    await mainMessage.ModifyAsync("Added field!");
                    await Task.Delay(1000);
                }
                else
                {
                    await imageMessage.Message.DeleteAsync();
                    await mainMessage.ModifyAsync("That is invalid.");
                    goto imageMessage;
                }
                string infoboxStuff = InfoBox.BuildInfoBox("image = " + imageName + "|");
                page.text = page.text.Insert(0, infoboxStuff);
                page.text += $"{Environment.NewLine}[[Category:OC]] [[Category:All Characters]] [[Category:OC {role}]] [[Category:{role}]]";

                if (role == "Academy Students")
                {
                    academyMessage:
                    await mainMessage.ModifyAsync("Please state which year the academy student is in: 1, 2, 3 or 4.");

                    var academyMessage = await interactivity.WaitForMessageAsync(x => x.Author == e.Member, TimeSpan.FromSeconds(120));
                    if (academyMessage != null)
                    {
                        if (academyMessage.Message.Content.ToLower() == "stop")
                        {
                            goto End;
                        }
                        switch (academyMessage.Message.Content)
                        {
                            case "1":
                                page.text += $"[[Category:1st Year Student]]";
                                break;
                            case "2":
                                page.text += $"[[Category:2nd Year Student]]";
                                break;
                            case "3":
                                page.text += $"[[Category:3rd Year Student]]";
                                break;
                            case "4":
                                page.text += $"[[Category:4th Year Student]]";
                                break;
                            default:
                                await mainMessage.ModifyAsync("That is invalid.");
                                goto academyMessage;
                        }
                        await academyMessage.Message.DeleteAsync();
                    }
                }
                page.Save();
                await mainMessage.ModifyAsync("Complete!");
                await e.RespondAsync($"http://roleplay-heroes-and-villains.wikia.com/wiki/{page.title}");
                return;
            }

            End:
            await mainMessage.ModifyAsync("Action cancelled.");
            await Task.Delay(2000);
            await mainMessage.DeleteAsync();
            await infoBoxEmbedMessage.DeleteAsync();
            await embedMessage.DeleteAsync();
        }

        [Command("uploadtest"), RequirePermissions(DSharpPlus.Permissions.Administrator)]
        public async Task aaa(CommandContext e)
        {


            PageList p = new PageList(WikiSite);
            p.FillAndLoadFromFiles("Pages");
            // p.FillFromAllPages("", 0, true, 1000);
            //p.LoadWithMetadata();
            //p.SaveToFiles("Pages");
            p.Save();
            //limit=500&user=&title=Special%3AListFiles&
            //        var x = WikiSite.GetApiQueryResult("user=", "title=Special:ListFiles", 1000);

            // p.FillFromCustomSpecialPage("ListFiles", 10000);
            //p.FillFromCategoryTree("Category:All_Characters");
            //  }
            //WikiSite.fil

           // string[] images = Directory.GetFiles("Images");
           /* int count = 0;
            foreach (string imageName in images)
            {
                
                count++;
                Wiki.Page l = new Wiki.Page(WikiSite, imageName);
                l.UploadImageFromWeb("http://51.15.222.156/wiki/" + imageName.Replace("File:", ""), "N/A", "N/A", "N/A");
                Console.WriteLine(count);
            }*/
                await e.RespondAsync("Done!");
        }
    }
}

