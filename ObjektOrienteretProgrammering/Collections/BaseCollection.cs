using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collections
{
    public abstract class BaseCollection<T>
    {
        public long TotalElapsedMiliseconds { get; private set; } = 0;
        private System.Diagnostics.Stopwatch startTime()
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            return watch;
        }

        private long stopAndAddTime(System.Diagnostics.Stopwatch watch)
        {
            watch.Stop();
            TotalElapsedMiliseconds += watch.ElapsedMilliseconds;
            return watch.ElapsedMilliseconds;
        }

        public long FillCollection(string[] input, Func<string, T> func)
        {
            System.Diagnostics.Stopwatch watch = startTime();
            fillCollection(input, func);
            return stopAndAddTime(watch);
        }

        public long SortCollection(Func<T, T> comparer)
        {
            System.Diagnostics.Stopwatch watch = startTime();
            sortCollection(comparer);
            return stopAndAddTime(watch);
        }

        public long PrintCollection(TextWriter writer)
        {
            System.Diagnostics.Stopwatch watch = startTime();
            printCollection(writer);
            return stopAndAddTime(watch);
        }

        protected abstract void fillCollection(string[] input, Func<string, T> func);
        protected abstract void sortCollection(Func<T, T> comparer);
        protected abstract void printCollection(TextWriter writer);

        public abstract T FirstObject();
        public abstract T LastObject();
        public abstract int Count();
    }
}
