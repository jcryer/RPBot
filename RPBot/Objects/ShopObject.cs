using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPBot
{
    class ShopObject
    {
        public class RootObject
        {
            public RootObject(int id, string name, string description, int price, int availability, string emoji, int location, ulong messageID)
            {
                this.id = id;
                this.name = name;
                this.description = description;
                this.price = price;
                this.availability = availability;
                this.emoji = emoji;
                this.location = location;
                this.messageID = messageID;
            } 

            public int id { get; set; }
            public string name { get; set; }
            public string description { get; set; }
            public int price { get; set; }
            public int availability { get; set; }
            public string emoji { get; set; }
            public int location { get; set; }
            public ulong messageID { get; set; }
        }
    }
}
