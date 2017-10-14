using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPBot
{
    class UserObject
    {
        public class RootObject
        {
            public RootObject(UserData userData, int xp, InvData invData)
            {
                UserData = userData;
                this.xp = xp;
                InvData = invData;
            }

            public UserData UserData { get; set; }
            public int xp { get; set; }
            public InvData InvData { get; set; }
        }

        public class UserData
        {
            public UserData(ulong userID, string username, int role, int status, int money, int resolvedCases, int crimesCommitted, int guildID, ulong messageID)
            {
                this.userID = userID;
                this.username = username;
                this.role = role;
                this.status = status;
                this.money = money;
                this.resolvedCases = resolvedCases;
                this.crimesCommitted = crimesCommitted;
                this.guildID = guildID;
                this.messageID = messageID;
            }

            public ulong userID { get; set; }
            public string username { get; set; }
            public int role { get; set; }
            public int status { get; set; }
            public int money { get; set; }
            public int resolvedCases { get; set; }
            public int crimesCommitted { get; set; }
            public int guildID { get; set; }
            public ulong messageID { get; set; }
        }

        public class InvData
        {
            public InvData(List<int> items)
            {
                this.items = items;
            }

            public List<int> items { get; set; }
        }
    }
}
