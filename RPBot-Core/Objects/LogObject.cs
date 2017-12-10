using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPBot
{
    class LogObject
    {
        public class RootObject
        {
            public RootObject(ulong id, string name, string content, ulong channelID)
            {
                this.id = id;
                this.name = name;
                this.content = content;
                this.channelID = channelID;
            }

            public ulong id { get; set; }
            public string name { get; set; }
            public string content { get; set; }
            public ulong channelID { get; set; }
        }

        public class Message
        {
            public Message (string username, string fontColour, string avatar, DateTimeOffset timestamp, string content, bool isBot)
            {
                this.username = username;
                this.fontColour = fontColour;
                this.avatar = avatar;
                this.timestamp = timestamp;
                this.content = content;
                this.isBot = isBot;
            }

            public string username { get; set; }
            public string fontColour { get; set; }
            public string avatar { get; set; }
            public DateTimeOffset timestamp { get; set; }
            public string content { get; set; }
            public bool isBot { get; set; }
        }
    }
}
