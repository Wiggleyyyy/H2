using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collections.Collections
{
    public abstract class BaseLineCollection<T> : BaseCollection<T>
    {
        private T[]? _data = new T[0];
        protected override void fillCollection(string[] input, Func<string, T> func)
        {
            //Add data to simulate data size
            _data = new T[input.Length];
            //Not Implemented, so just simulate call to RNG
            for (int i = 0; i < input.Length; i++)
            {
                _data[i] = func(input[i]);
            }
        }

        protected override void printCollection(TextWriter writer)
        {
            if (_data is null) return;
            //print all data
            foreach (var item in _data)
            {
                writer.WriteLine(item);
            }
            writer.Flush();
        }

        protected override void sortCollection(Func<T, T> comparer)
        {
            //No sort so do nothing
        }

        public override int Count()
        {
            if (_data is null) return 0;
            return _data.Length;
        }
        public override T FirstObject()
        {
            if (_data is null) throw new NullReferenceException();
            return _data[0];
        }

        public override T LastObject()
        {
            if (_data is null) throw new NullReferenceException();
            return _data[_data.Length - 1];
        }
    }
}
