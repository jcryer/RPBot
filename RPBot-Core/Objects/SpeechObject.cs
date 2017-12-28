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
                this.Id = id;
                this.Name = name;
            }

            public ulong Id { get; set; }
            public string Name { get; set; }
        }
    }
}
