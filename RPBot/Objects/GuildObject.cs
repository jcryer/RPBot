using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPBot
{
    class GuildObject
    {
        public class RootObject
        {
            public RootObject(int id, string name, int status, List<ulong> userIDs)
            {
                this.id = id;
                this.name = name;
                this.status = status;
                this.userIDs = userIDs;
            }

            public int id { get; set; }
            public string name { get; set; }
            public int status { get; set; }
            public List<ulong> userIDs { get; set; }
        }
    }
}
