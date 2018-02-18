using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RPBot
{
    public static class Extensions
    {
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

    public class Element
    {
        public string name { get; set; }
        public string appearance { get; set; }
        public double atomic_mass { get; set; }
        public double? boil { get; set; }
        public string category { get; set; }
        public string color { get; set; }
        public double? density { get; set; }
        public string discovered_by { get; set; }
        public double? melt { get; set; }
        public double? molar_heat { get; set; }
        public string named_by { get; set; }
        public int number { get; set; }
        public int period { get; set; }
        public string phase { get; set; }
        public string source { get; set; }
        public string spectral_img { get; set; }
        public string summary { get; set; }
        public string symbol { get; set; }
        public int xpos { get; set; }
        public int ypos { get; set; }
        public List<int> shells { get; set; }
    }
    public class Elements
    {
        public List<Element> elements { get; set; }
    }
}
