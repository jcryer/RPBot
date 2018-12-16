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
                if (rank < 10000) UserRank = "S2";
                if (rank < 9000) UserRank = "S3";
                if (rank < 8000) UserRank = "A1";
                if (rank < 7000) UserRank = "A2";
                if (rank < 6000) UserRank = "A3";
                if (rank < 5500) UserRank = "B1";
                if (rank < 5000) UserRank = "B2";
                if (rank < 4500) UserRank = "B3";
                if (rank < 4000) UserRank = "C1";
                if (rank < 3000) UserRank = "C2";
                if (rank < 2500) UserRank = "C3";
                if (rank < 2000) UserRank = "D1";
                if (rank < 1000) UserRank = "D2";
                if (rank < 500) UserRank = "D3";
                return UserRank;
            }

            public int GetBounty(int type)
            {
                char rank = GetRank()[0];

                int points = 0;
                if (type == 1) points = UserData.Fame;
                else points = UserData.Infamy;
                int Bounty = 0;

                if (points > 250) return 600;
                if (points < 1) return 0;
                if (rank == 'S') return 600;
                if (rank == 'A') Bounty = 175;
                if (rank == 'B') Bounty = 125;
                if (rank == 'C') Bounty = 75;
                if (rank == 'D') Bounty = 25;

                if (points <= 25) return Bounty + 25;
                if (points <= 50) return Bounty + 75;
                if (points <= 100) return Bounty + 125;
                if (points <= 150) return Bounty + 175;
                if (points <= 200) return Bounty + 225;
                if (points <= 250) return Bounty + 275;

                return 0;
            }
        }

        public class UserData
        {
            public UserData(ulong userID, string username, int role)
            {
                this.UserID = userID;
                this.Username = username;
                this.Role = role;
                Money = 0;
                MeritPoints = 0;
                BloodPoints = 0;
                Fame = 0;
                Infamy = 0;
                FameComment = "";
                InfamyComment = "";
                GuildID = 0;
            }

            public ulong UserID { get; set; }
            public string Username { get; set; }
            public int Role { get; set; }
            public int Money { get; set; }
            public int MeritPoints { get; set; }
            public int BloodPoints { get; set; }
            public int Fame { get; set; }
            public int Infamy { get; set; }
            public string FameComment { get; set; }
            public string InfamyComment { get; set; }

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
                this.IsMuted = 0;
            }
            public int IsMuted { get; set; }
            public TimeSpan MuteDuration { get; set; }
            public List<DiscordRole> Roles { get; set; }
        }
    }
}
