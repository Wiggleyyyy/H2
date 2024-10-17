namespace Collections.Collections
{
    internal class ArrayUnknownSizeCollection<T> : BaseCollection<T>
    {
        private T[] _data;
        private int _count;

        public ArrayUnknownSizeCollection()
        {
            _data = new T[4]; // Start with a small size, e.g., 4
            _count = 0;
        }

        protected override void fillCollection(string[] input, Func<string, T> func)
        {
            foreach (var item in input)
            {
                Add(func(item));
            }
        }

        private void Add(T item)
        {
            if (_count == _data.Length)
            {
                // Resize the array when capacity is reached
                ResizeArray();
            }
            _data[_count] = item;
            _count++;
        }

        private void ResizeArray()
        {
            // Double the size of the array
            T[] newArray = new T[_data.Length * 2];
            Array.Copy(_data, newArray, _data.Length);
            _data = newArray;
        }

        protected override void printCollection(TextWriter writer)
        {
            for (int i = 0; i < _count; i++)
            {
                writer.WriteLine(_data[i]);
            }
            writer.Flush();
        }

        protected override void sortCollection(Func<T, T> comparer)
        {
            // Sorting is not implemented, so do nothing for now.
        }

        public override int Count()
        {
            return _count;
        }

        public override T FirstObject()
        {
            if (_count == 0) throw new NullReferenceException();
            return _data[0];
        }

        public override T LastObject()
        {
            if (_count == 0) throw new NullReferenceException();
            return _data[_count - 1];
        }
    }
}
