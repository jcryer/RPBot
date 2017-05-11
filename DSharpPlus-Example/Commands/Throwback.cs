using DSharpPlus.CommandsNext;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLOBot
{
    class ThrowbackClass
    {
        public static async Task Throwback(CommandContext e , string search)
        {
            bool filecontains = false;
            string[] files = Directory.GetFiles("SLO Shit/Funny/");

            if (search != "")
            {
                List<string> FoundFiles = new List<string>();

                foreach (string file in files)
                {
                    if (file.ToLower().Contains(search.ToLower()))
                    {
                        string FileEnd = file.Substring(file.LastIndexOf('\\') + 1);
                        FoundFiles.Add(FileEnd);
                        filecontains = true;
                    }
                }
                if (filecontains == true)
                {
                    Console.WriteLine("1");
                    int index = new Random().Next(0, FoundFiles.Count);
                    string FileEnd = FoundFiles[index].Substring(FoundFiles[index].LastIndexOf('\\') + 1);
                    await e.Channel.SendFileAsync(new FileStream(FoundFiles[index], FileMode.Open), FileEnd);
                }
                else
                {
                    Console.WriteLine("2");

                    int index = new Random().Next(0, files.Length);
                    string FilePath = files[index].Substring(files[index].LastIndexOf('\\') + 1);

                    string FileEnd = files[index].Split('.')[1];
                    Console.WriteLine(FilePath + ":" + FileEnd);

                    await e.Channel.SendFileAsync(new FileStream(FilePath, FileMode.Open), "a." + FileEnd, "No image found  in search. Displaying random image.");
                }

            }
            else
            {
                Console.WriteLine("3");

                int index = new Random().Next(0, files.Length);
                string FilePath = files[index].Substring(files[index].LastIndexOf('\\') + 1);

                string FileEnd = files[index].Split('.')[1];
                Console.WriteLine(FilePath + ":" + FileEnd);
                await e.Channel.SendFileAsync(new FileStream(FilePath, FileMode.Open),"a." + FileEnd, "No image specified. Displaying random image.");
            }
        }
    }
}
