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
            public RootObject(UserData userData, InvData invData)
            {
                UserData = userData;
                Stats = new StatData();
                ModData = new ModData();
                Activity = new CountData();
            }

            public UserData UserData { get; set; }
            public int Xp { get; set; }
            public StatData Stats { get; set; }
            public ModData ModData { get; set; }
            public CountData Activity { get; set; }

            public string GetRank()
            {
                int rank = Xp;
                string UserRank = "S";
                if (rank < 7000) UserRank = "A1";
                if (rank < 5750) UserRank = "A2";
                if (rank < 4500) UserRank = "B1";
                if (rank < 3750) UserRank = "B2";
                if (rank < 2500) UserRank = "C1";
                if (rank < 1750) UserRank = "C2";
                if (rank < 1000) UserRank = "D1";
                if (rank < 500) UserRank = "D2";
                return UserRank;
            }
           
        }

        public class UserData
        {
            public UserData(ulong userID, string username)
            {
                this.UserID = userID;
                this.Username = username;
                Money = 0;
                SoulShards = 0;

                GuildID = 0;
            }

            public ulong UserID { get; set; }
            public string Username { get; set; }
            public int Money { get; set; }
            public int SoulShards { get; set; }

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
            public InvData()
            {
                Items = new List<int>();
            }

            public List<int> Items { get; set; }
        }

        public class ModData
        {
            public ModData()
            {
                this.IsStaff = 0;
            }
            public int IsStaff { get; set; }
            public TimeSpan MuteDuration { get; set; }
            public List<DiscordRole> Roles { get; set; }
        }
        public class CountData
        {
            public int MessageCount;
            public int CharacterCount;
            public int WordCount;
        }
    }
}
