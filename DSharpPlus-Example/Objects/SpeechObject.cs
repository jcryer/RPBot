using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPBot
{
    class SpeechObject
    {
        public class RootObject
        {
            public RootObject(ulong id, string name)
            {
                this.id = id;
                this.name = name;
            }

            public ulong id { get; set; }
            public string name { get; set; }
        }
    }
}
