using System.Collections.Generic;

namespace RPBot
{
    class TagObject {
        public class RootObject
        {
            public string name { get; set; }
            public string content { get; set; }

            public RootObject(string name, string content)
            {
                this.name = name;
                this.content = content;
            }
        }
    }
}
