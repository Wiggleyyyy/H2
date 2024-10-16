namespace Collections.Collections
{
    internal class ListCollection<T> : BaseCollection<T>
    {
        private List<T>? _data;

        protected override void fillCollection(string[] input, Func<string, T> func)
        {
            // Initialize the list
            _data = new List<T>(input.Length);
            // Convert input using func and add to list
            foreach (var item in input)
            {
                _data.Add(func(item));
            }
        }

        protected override void printCollection(TextWriter writer)
        {
            if (_data == null) return;
            // Print all data
            foreach (var item in _data)
            {
                writer.WriteLine(item);
            }
            writer.Flush();
        }

        protected override void sortCollection(Func<T, T> comparer)
        {
            // No sorting needed, do nothing
        }

        public override int Count()
        {
            return _data?.Count ?? 0;
        }

        public override T FirstObject()
        {
            if (_data == null || _data.Count == 0) throw new NullReferenceException();
            return _data[0];
        }

        public override T LastObject()
        {
            if (_data == null || _data.Count == 0) throw new NullReferenceException();
            return _data[_data.Count - 1];
        }
    }
}
