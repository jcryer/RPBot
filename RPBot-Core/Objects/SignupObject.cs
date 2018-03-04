using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPBot
{
    class SignupObject
    {
        public class RootObject
        {
            public RootObject(string id)
            {
                this.Id = id;
                UserIDs = new List<ulong>();
            }
            public string Id { get; private set; }
            public List<ulong> UserIDs { get; set; }
        }
    }
}
