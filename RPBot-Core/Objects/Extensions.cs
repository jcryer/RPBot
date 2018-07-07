using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPBot
{
    public static class Extensions
    {
        public static async Task UpdateFameAndInfamy(DiscordGuild e, int type)
        {
            if (type == 0)
            {
                try
                {
                    List<DiscordMessage> msgs = new List<DiscordMessage>(await RPClass.FameChannel.GetMessagesAsync(100));
                    foreach (DiscordMessage msg in msgs)
                    {
                        await msg.DeleteAsync();
                        await Task.Delay(500);
                    }
                    await RPClass.FameChannel.SendMessageAsync("**========== Hero HQ Bounty Board ==========**");
                    await UpdateFameAndInfamy(e, 2);
                    await RPClass.FameChannel.SendMessageAsync("**========== Black Market Bounty Board ==========**");
                    await UpdateFameAndInfamy(e, 1);
                }
                catch { }
                return;
            }


            DiscordChannel FameChannel = RPClass.FameChannel;

            int longestName = 1;
            if (type == 1) longestName = RPClass.Users.Where(x => x.UserData.Fame > 0).Max(x => x.UserData.Username.Length) + 1;
            else longestName = RPClass.Users.Where(x => x.UserData.Infamy > 0).Max(x => x.UserData.Username.Length) + 1;

            int longestFame = 5;
            if (type == 2) longestFame = 7;
            int longestBounty = 7;

            int longestComment = 8;

            if (type == 1) longestComment = RPClass.Users.Where(x => x.UserData.Fame > 0).Max(x => x.UserData.FameComment.Length) + 1;
            else longestComment = RPClass.Users.Where(x => x.UserData.Infamy > 0).Max(x => x.UserData.InfamyComment.Length) + 1;

            string Count = "Pos".PadRight(longestFame) + "| ";
            string Name = "Name".PadRight(longestName) + "| ";
            string Fame = "Fame".PadRight(longestFame) + "| ";
            if (type == 2) Fame = "Infamy".PadRight(longestFame) + "| ";
            string Bounty = "Bounty".PadRight(longestBounty) + "| ";
            string Comment = "Comment";
            string value = "";
            value += $"```{Count}{Name}{Fame}{Bounty}{Comment}\n{new string('-', $"{Count}{Name}{Fame}{Bounty}".Length + longestComment)}\n";
            List<UserObject.RootObject> SortedUsers = new List<UserObject.RootObject>();
            if (type == 1) SortedUsers = RPClass.Users.Where(x => x.UserData.Fame > 0).OrderByDescending(x => (x.UserData.Fame)).ToList();
            else SortedUsers = RPClass.Users.Where(x => x.UserData.Infamy > 0).OrderByDescending(x => (x.UserData.Infamy)).ToList();

            int countNum = 1;
            foreach (UserObject.RootObject user in SortedUsers)
            {
                string FameOrInfamy = "";
                
                if (type == 1) FameOrInfamy += user.UserData.Fame;
                else FameOrInfamy += user.UserData.Infamy;

                string FinalBounty = user.GetBounty(type).ToString();

                string FinalComment = user.UserData.FameComment;
                if (type == 2) FinalComment = user.UserData.InfamyComment;
                if (value.Length > 1500)
                {
                    await FameChannel.SendMessageAsync(value + "```");
                    value = "```";
                }
                value += (countNum.ToString().PadRight(longestFame) + "| " + user.UserData.Username.PadRight(longestName) + "| " + FameOrInfamy.PadRight(longestFame) + "| " + FinalBounty.PadRight(longestBounty) + "| " + FinalComment + "\n");
                countNum += 1;
            }
            await FameChannel.SendMessageAsync(value + "```");
        }

        public class SlidingBuffer<T> : IEnumerable<T>
        {
            private readonly Queue<T> _queue;
            private readonly int _maxCount;

            public SlidingBuffer(int maxCount)
            {
                _maxCount = maxCount;
                _queue = new Queue<T>(maxCount);
            }

            public void Add(T item)
            {
                if (_queue.Count == _maxCount)
                    _queue.Dequeue();
                _queue.Enqueue(item);
            }

            public IEnumerator<T> GetEnumerator()
            {
                return _queue.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        public static List<List<T>> Split<T>(List<T> collection, int size)
        {
            var chunks = new List<List<T>>();
            var chunkCount = collection.Count() / size;

            if (collection.Count % size > 0)
                chunkCount++;

            for (var i = 0; i < chunkCount; i++)
                chunks.Add(collection.Skip(i * size).Take(size).ToList());

            return chunks;
        }

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[RPClass.Random.Next(s.Length)]).ToArray());
        }

    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class IsMuted : CheckBaseAttribute
    {
        public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
        {
            if (ctx.Member.Roles.Contains(RPClass.PunishedRole))
                return Task.FromResult(false);
                
            return Task.FromResult(true);
        }
    }
}
