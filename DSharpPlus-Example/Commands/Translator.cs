using System;
using System.Collections.Generic;
using System.Web;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus;

namespace SLOBot
{
    public class Translator
    {
        public static async Task<ulong> GetUserID(CommandContext e)
        {
            List<DiscordMember> Members = await e.Guild.GetAllMembersAsync();
            ulong userID = 0;
            try
            {
                userID = Members.FirstOrDefault(x => x.Nickname == e.RawArguments[2]).Id;
            }
            catch
            {
                userID = Members.FirstOrDefault(x => x.Username== e.RawArguments[2]).Id;

            }
            return userID;
        }
        public static async Task Translate(CommandContext e)
        {
            EnsureInitialized();
            if (e.RawArguments != null)
            {
                if (e.RawArguments[0] == "langs")
                {
                    string ChannelMessage = "Available Languages: ";
                    foreach (string langs in Languages)
                    {
                        ChannelMessage += langs + ", ";
                    }
                    ChannelMessage = ChannelMessage.Remove(ChannelMessage.Length - 2);

                    DiscordMember user = await e.Guild.GetMemberAsync(e.Message.Author.Id);
                    DiscordDmChannel dm = await user.SendDmAsync();

                    await dm.SendMessageAsync(ChannelMessage);
                    await e.Channel.SendMessageAsync("You have been sent a PM.");
                }
                else if (e.RawArguments[0] == "auto" && e.RawArguments.Count > 2)
                {

                    if (e.RawArguments[1] == "on" && e.RawArguments.Count > 4)
                    {
                        if (_languageModeMap.ContainsKey(e.RawArguments[3]) && Translator._languageModeMap.ContainsKey(e.RawArguments[4]))
                        {
                            ulong userID = await GetUserID(e);
                            if (userID != 0)
                            {

                                AutoTranslate user = Program.UsersTranslated.FirstOrDefault(x => x.userID == userID);
                                if (user != null)
                                {
                                    Program.UsersTranslated.Remove(user);
                                }
                                Program.UsersTranslated.Add(new AutoTranslate(e.RawArguments[3], e.RawArguments[4], userID));
                            }
                        }
                        else
                        {
                            await e.Channel.SendMessageAsync("Please use a language from the \"!translate langs\" list.");

                        }
                    }
                    else if (e.RawArguments[1] == "off")
                    {
                        List<DiscordMember> Members = await e.Guild.GetAllMembersAsync();
                        ulong userID = await GetUserID(e);
                        if (userID != 0)
                        {
                            AutoTranslate user = Program.UsersTranslated.FirstOrDefault(x => x.userID == userID);
                            if (user != null)
                            {
                                Program.UsersTranslated.Remove(user);
                            }
                            else
                            {
                                await e.Channel.SendMessageAsync("There does not seem to be an active autotranslate for this user.");

                            }
                        }
                    }
                    else
                    {
                        await e.Channel.SendMessageAsync("```Available automatic translate commands: " + Environment.NewLine + "auto on NICKNAME/USERNAME FromLanguage ToLanguage" + Environment.NewLine + "auto off NICKNAME/USERNAME```");
                    }
                }
                else
                {
                    try
                    {
                        Translator t = new Translator();
                        string c = t.Translate(e.RawArguments[2], e.RawArguments[0], e.RawArguments[1]);

                        if (t.Error == null)
                        {
                            await e.Channel.SendMessageAsync(c);
                        }
                        else
                        {
                            await e.Channel.SendMessageAsync("Translation failed.");
                        }
                    }
                    catch
                    {
                        await e.Channel.SendMessageAsync("```Available translate commands: " + Environment.NewLine + "1: langs - Prints all available languages" + Environment.NewLine + "2: FromLanguage ToLanguage WhatToTranslateHere" + Environment.NewLine + "3: auto on NICKNAME/USERNAME FromLanguage ToLanguage" + Environment.NewLine + "4: auto off NICKNAME/USERNAME```");
                    }
                }
            }
        }

        public static IEnumerable<string> Languages
        {
            get
            {
                Translator.EnsureInitialized();
                return Translator._languageModeMap.Keys.OrderBy(p => p);
            }
        }

        public TimeSpan TranslationTime
        {
            get;
            private set;
        }


        public string TranslationSpeechUrl
        {
            get;
            private set;
        }

        public Exception Error
        {
            get;
            private set;
        }

        public string Translate
            (string sourceText,
             string sourceLanguage,
             string targetLanguage)
        {
            this.Error = null;
            this.TranslationSpeechUrl = null;
            this.TranslationTime = TimeSpan.Zero;
            DateTime tmStart = DateTime.Now;
            string translation = string.Empty;

            try
            {
                string url = string.Format("https://translate.googleapis.com/translate_a/single?client=gtx&sl={0}&tl={1}&dt=t&q={2}",
                                            Translator.LanguageEnumToIdentifier(sourceLanguage),
                                            Translator.LanguageEnumToIdentifier(targetLanguage),
                                            HttpUtility.UrlEncode(sourceText));
                string outputFile = Path.GetTempFileName();
                using (WebClient wc = new WebClient())
                {
                    wc.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36");
                    wc.DownloadFile(url, outputFile);
                }

                // Get translated text
                if (File.Exists(outputFile))
                {

                    // Get phrase collection
                    string text = File.ReadAllText(outputFile);
                    int index = text.IndexOf(string.Format(",,\"{0}\"", Translator.LanguageEnumToIdentifier(sourceLanguage)));
                    if (index == -1)
                    {
                        // Translation of single word
                        int startQuote = text.IndexOf('\"');
                        if (startQuote != -1)
                        {
                            int endQuote = text.IndexOf('\"', startQuote + 1);
                            if (endQuote != -1)
                            {
                                translation = text.Substring(startQuote + 1, endQuote - startQuote - 1);
                            }
                        }
                    }
                    else
                    {
                        // Translation of phrase
                        text = text.Substring(0, index);
                        text = text.Replace("],[", ",");
                        text = text.Replace("]", string.Empty);
                        text = text.Replace("[", string.Empty);
                        text = text.Replace("\",\"", "\"");

                        // Get translated phrases
                        string[] phrases = text.Split(new[] { '\"' }, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; (i < phrases.Count()); i += 2)
                        {
                            string translatedPhrase = phrases[i];
                            if (translatedPhrase.StartsWith(",,"))
                            {
                                i--;
                                continue;
                            }
                            translation += translatedPhrase + "  ";
                        }
                    }

                    // Fix up translation
                    translation = translation.Trim();
                    translation = translation.Replace(" ?", "?");
                    translation = translation.Replace(" !", "!");
                    translation = translation.Replace(" ,", ",");
                    translation = translation.Replace(" .", ".");
                    translation = translation.Replace(" ;", ";");

                    // And translation speech URL
                    this.TranslationSpeechUrl = string.Format("https://translate.googleapis.com/translate_tts?ie=UTF-8&q={0}&tl={1}&total=1&idx=0&textlen={2}&client=gtx",
                                                               HttpUtility.UrlEncode(translation), Translator.LanguageEnumToIdentifier(targetLanguage), translation.Length);
                }
            }
            catch (Exception ex)
            {
                this.Error = ex;
            }

            // Return result
            this.TranslationTime = DateTime.Now - tmStart;
            return translation;
        }

        private static string LanguageEnumToIdentifier
            (string language)
        {
            string mode = string.Empty;
            Translator.EnsureInitialized();
            Translator._languageModeMap.TryGetValue(language, out mode);
            return mode;
        }

        public static void EnsureInitialized()
        {
            if (Translator._languageModeMap == null)
            {
                Translator._languageModeMap = new Dictionary<string, string>
                {
                    { "Afrikaans", "af" },
                    { "Albanian", "sq" },
                    { "Arabic", "ar" },
                    { "Armenian", "hy" },
                    { "Azerbaijani", "az" },
                    { "Basque", "eu" },
                    { "Belarusian", "be" },
                    { "Bengali", "bn" },
                    { "Bulgarian", "bg" },
                    { "Catalan", "ca" },
                    { "Chinese", "zh-CN" },
                    { "Croatian", "hr" },
                    { "Czech", "cs" },
                    { "Danish", "da" },
                    { "Dutch", "nl" },
                    { "English", "en" },
                    { "Esperanto", "eo" },
                    { "Estonian", "et" },
                    { "Filipino", "tl" },
                    { "Finnish", "fi" },
                    { "French", "fr" },
                    { "Galician", "gl" },
                    { "German", "de" },
                    { "Georgian", "ka" },
                    { "Greek", "el" },
                    { "Haitian Creole", "ht" },
                    { "Hebrew", "iw" },
                    { "Hindi", "hi" },
                    { "Hungarian", "hu" },
                    { "Icelandic", "is" },
                    { "Indonesian", "id" },
                    { "Irish", "ga" },
                    { "Italian", "it" },
                    { "Japanese", "ja" },
                    { "Korean", "ko" },
                    { "Lao", "lo" },
                    { "Latin", "la" },
                    { "Latvian", "lv" },
                    { "Lithuanian", "lt" },
                    { "Macedonian", "mk" },
                    { "Malay", "ms" },
                    { "Maltese", "mt" },
                    { "Norwegian", "no" },
                    { "Persian", "fa" },
                    { "Polish", "pl" },
                    { "Portuguese", "pt" },
                    { "Romanian", "ro" },
                    { "Russian", "ru" },
                    { "Serbian", "sr" },
                    { "Slovak", "sk" },
                    { "Slovenian", "sl" },
                    { "Spanish", "es" },
                    { "Swahili", "sw" },
                    { "Swedish", "sv" },
                    { "Tamil", "ta" },
                    { "Telugu", "te" },
                    { "Thai", "th" },
                    { "Turkish", "tr" },
                    { "Ukrainian", "uk" },
                    { "Urdu", "ur" },
                    { "Vietnamese", "vi" },
                    { "Welsh", "cy" },
                    { "Yiddish", "yi" }
                };
            }
        }

        public static Dictionary<string, string> _languageModeMap;
    }
    public class AutoTranslate 
    {
        public string from;
        public string to;
        public ulong userID;
        public AutoTranslate(string from, string to, ulong userID)
        {
            this.from = from;
            this.to = to;
            this.userID = userID;
        }
    }
}
