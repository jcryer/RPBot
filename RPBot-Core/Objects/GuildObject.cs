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
                this.Id = id;
                this.Name = name;
                this.Status = status;
                this.UserIDs = userIDs;
            }

            public int Id { get; set; }
            public string Name { get; set; }
            public int Status { get; set; }
            public List<ulong> UserIDs { get; set; }
        }
    }
}
