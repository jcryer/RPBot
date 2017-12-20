using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPBot
{
    class InstanceObject
    {
        public class RootObject
        {
            public RootObject(int id, ulong channelID, int channelTemplateID)
            {
                this.id = id;
                this.channelID = channelID;
                this.channelTemplateID = channelTemplateID;
                active = true;
            }

            public int id { get; set; }
            public ulong channelID { get; set; }
            public int channelTemplateID { get; set; }
            public bool active{ get; set; }
        }

        public class ChannelTemplate
        {
            public int id { get; set; }
            public string name { get; set; }
            public List<string> content { get; set; }

            public ChannelTemplate(int id, string name, List<string> content)
            {
                this.id = id;
                this.name = name;
                this.content = content;
            }
        }
    }
}