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
                this.Id = id;
                this.Name = name;
                this.Content = content;
                this.ChannelID = channelID;
            }

            public ulong Id { get; set; }
            public string Name { get; set; }
            public string Content { get; set; }
            public ulong ChannelID { get; set; }
        }

        public class Message
        {
            public Message (string username, string fontColour, string avatar, DateTimeOffset timestamp, string content, bool isBot)
            {
                this.Username = username;
                this.FontColour = fontColour;
                this.Avatar = avatar;
                this.Timestamp = timestamp;
                this.Content = content;
                this.IsBot = isBot;
            }

            public string Username { get; set; }
            public string FontColour { get; set; }
            public string Avatar { get; set; }
            public DateTimeOffset Timestamp { get; set; }
            public string Content { get; set; }
            public bool IsBot { get; set; }
        }
    }
}
