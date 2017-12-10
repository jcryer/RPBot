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
            public RootObject(int id, string name, ulong categoryID, List<ulong> channelIDs, List<int> channelTemplateIDs, List<ulong> userIDs)
            {
                this.id = id;
                this.name = name;
                this.categoryID = categoryID;
                this.channelIDs = channelIDs;
                this.channelTemplateIDs = channelTemplateIDs;
                this.userIDs = userIDs;
                active = true;
            }

            public int id { get; set; }
            public string name { get; set; }
            public ulong categoryID { get; set; }
            public List<ulong> channelIDs { get; set; }
            public List<int> channelTemplateIDs { get; set; }
            public List<ulong> userIDs { get; set; }
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
