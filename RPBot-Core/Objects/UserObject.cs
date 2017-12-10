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
                this.ModData = new ModData();
            }

            public UserData UserData { get; set; }
            public int xp { get; set; }
            public InvData InvData { get; set; }
            public ModData ModData { get; set; }
        }

        public class UserData
        {
            public UserData(ulong userID, string username, int role, int status, int money, int resolvedCases, int crimesCommitted, int guildID)
            {
                this.userID = userID;   
                this.username = username;
                this.role = role;
                this.status = status;
                this.money = money;
                this.resolvedCases = resolvedCases;
                this.crimesCommitted = crimesCommitted;
                this.guildID = guildID;
            }

            public ulong userID { get; set; }
            public string username { get; set; }
            public int role { get; set; }
            public int status { get; set; }
            public int money { get; set; }
            public int resolvedCases { get; set; }
            public int crimesCommitted { get; set; }
            public int guildID { get; set; }
        }

        public class InvData
        {
            public InvData(List<int> items)
            {
                this.items = items;
            }

            public List<int> items { get; set; }
        }

        public class ModData
        {
            public ModData ()
            {
                this.isMuted = false;
            }
            public bool isMuted { get; set; }
            public TimeSpan muteDuration { get; set; }
        }
    }
}
