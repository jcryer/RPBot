using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPBot
{
    class JokeObject
    {
        public class Attachment
        {
            public string Fallback { get; set; }
            public string Footer { get; set; }
            public string Text { get; set; }
        }

        public class RootObject
        {
            public List<Attachment> Attachments { get; set; }
            public string Response_type { get; set; }
            public string Username { get; set; }
        }
    }
}
