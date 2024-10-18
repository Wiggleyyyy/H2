using System;
using System.Collections.Generic;
using System.Threading;

namespace Collections.Collections
{
    internal class ConcurrentListCollection<T> : BaseCollection<T>
    {
        private List<T>? _data;
        private readonly object _lock = new object();

        public ConcurrentListCollection()
        {
            _data = new List<T>();
        }

        protected override void fillCollection(string[] input, Func<string, T> func)
        {
            lock (_lock)
            {
                foreach (var item in input)
                {
                    _data.Add(func(item));
                }
            }
        }

        protected override void printCollection(TextWriter writer)
        {
            lock (_lock)
            {
                foreach (var item in _data)
                {
                    writer.WriteLine(item);
                }
                writer.Flush();
            }
        }

        protected override void sortCollection(Func<T, T> comparer)
        {
            lock (_lock)
            {
                // Sorting the list in a thread-safe way
                /*_data.Sort();*/  // Assuming default sorting or provide comparison logic
            }
        }

        public override int Count()
        {
            lock (_lock)
            {
                return _data?.Count ?? 0;
            }
        }

        public override T FirstObject()
        {
            lock (_lock)
            {
                if (_data == null || _data.Count == 0) throw new NullReferenceException();
                return _data[0];
            }
        }

        public override T LastObject()
        {
            lock (_lock)
            {
                if (_data == null || _data.Count == 0) throw new NullReferenceException();
                return _data[_data.Count - 1];
            }
        }
    }
}
