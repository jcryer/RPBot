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
                this.Id = id;
                this.ChannelID = channelID;
                this.ChannelTemplateID = channelTemplateID;
                Active = true;
            }

            public int Id { get; set; }
            public ulong ChannelID { get; set; }
            public int ChannelTemplateID { get; set; }
            public bool Active{ get; set; }
        }

        public class ChannelTemplate
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public List<string> Content { get; set; }

            public ChannelTemplate(int id, string name, List<string> content)
            {
                this.Id = id;
                this.Name = name;
                this.Content = content;
            }
        }
    }
}