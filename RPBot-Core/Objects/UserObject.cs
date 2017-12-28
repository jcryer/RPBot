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
                this.Xp = xp;
                InvData = invData;
                this.ModData = new ModData();
            }

            public UserData UserData { get; set; }
            public int Xp { get; set; }
            public InvData InvData { get; set; }
            public ModData ModData { get; set; }
        }

        public class UserData
        {
            public UserData(ulong userID, string username, int role, int status, int money, int resolvedCases, int crimesCommitted, int guildID)
            {
                this.UserID = userID;   
                this.Username = username;
                this.Role = role;
                this.Status = status;
                this.Money = money;
                this.ResolvedCases = resolvedCases;
                this.CrimesCommitted = crimesCommitted;
                this.GuildID = guildID;
            }

            public ulong UserID { get; set; }
            public string Username { get; set; }
            public int Role { get; set; }
            public int Status { get; set; }
            public int Money { get; set; }
            public int ResolvedCases { get; set; }
            public int CrimesCommitted { get; set; }
            public int GuildID { get; set; }
        }

        public class InvData
        {
            public InvData(List<int> items)
            {
                this.Items = items;
            }

            public List<int> Items { get; set; }
        }

        public class ModData
        {
            public ModData ()
            {
                this.IsMuted = false;
            }
            public bool IsMuted { get; set; }
            public TimeSpan MuteDuration { get; set; }
        }
    }
}
