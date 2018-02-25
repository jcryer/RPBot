using DSharpPlus.Entities;
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
                Stats = new StatData();
                this.ModData = new ModData();
            }

            public UserData UserData { get; set; }
            public int Xp { get; set; }
            public StatData Stats { get; set; }
            public ModData ModData { get; set; }

            public string GetRank()
            {
                int rank = Xp;
                string UserRank = "S1";
                if (rank <= 16000) UserRank = "S2";
                if (rank <= 14000) UserRank = "S3";
                if (rank <= 12000) UserRank = "A1";
                if (rank <= 10500) UserRank = "A2";
                if (rank <= 9250) UserRank = "A3";
                if (rank <= 8000) UserRank = "B1";
                if (rank <= 7000) UserRank = "B2";
                if (rank <= 6000) UserRank = "B3";
                if (rank <= 5000) UserRank = "C1";
                if (rank <= 4000) UserRank = "C2";
                if (rank <= 3250) UserRank = "C3";
                if (rank <= 2500) UserRank = "D1";
                if (rank <= 2000) UserRank = "D2";
                if (rank <= 1250) UserRank = "D3";
                return UserRank;
            }
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
            public int MeritPoints { get; set; }
            public int BloodPoints { get; set; }
            public int ResolvedCases { get; set; }
            public int CrimesCommitted { get; set; }
            public int GuildID { get; set; }
        }

        public class StatData
        {
            public int Melee;
            public int Ranged;
            public int Mobility;
            public int Dodge;
            public int Durability;
            public int Utility;
            public int Healing;
            public int Influence;
            public int Potential;

            public StatData()
            {
                Melee = 0;
                Ranged = 0;
                Mobility = 0;
                Dodge = 0;
                Durability = 0;
                Utility = 0;
                Healing = 0;
                Influence = 0;
                Potential = 0;
            }

            public int[] GetList()
            {
                return new int[] { Melee, Ranged, Mobility, Dodge, Durability, Utility, Healing, Influence, Potential };
            }

            public void SetList(int[] stats)
            {
                Melee = stats[0];
                Ranged = stats[1];
                Mobility = stats[2];
                Dodge = stats[3];
                Durability = stats[4];
                Utility = stats[5];
                Healing = stats[6];
                Influence = stats[7];
                Potential = stats[8];
            }
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
            public ModData()
            {
                this.IsMuted = false;
            }
            public bool IsMuted { get; set; }
            public TimeSpan MuteDuration { get; set; }
            public List<DiscordRole> Roles { get; set; }
        }
    }
}
