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
                this.Id = id;
                this.Name = name;
                this.Description = description;
                this.Price = price;
                this.Availability = availability;
                this.Emoji = emoji;
                this.Location = location;
                this.MessageID = messageID;
            } 

            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public int Price { get; set; }
            public int Availability { get; set; }
            public string Emoji { get; set; }
            public int Location { get; set; }
            public ulong MessageID { get; set; }
        }
    }
}
