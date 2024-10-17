namespace Collections.Collections
{
    internal class LinkedListCollection<T> : BaseCollection<T>
    {
        private LinkedList<T>? _data;

        protected override void fillCollection(string[] input, Func<string, T> func)
        {
            // Initialize the LinkedList
            _data = new LinkedList<T>();
            // Convert input using func and add to linked list
            foreach (var item in input)
            {
                _data.AddLast(func(item));
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
            // No sorting needed for now, do nothing
        }

        public override int Count()
        {
            return _data?.Count ?? 0;
        }

        public override T FirstObject()
        {
            if (_data == null || _data.Count == 0) throw new NullReferenceException();
            return _data.First.Value;
        }

        public override T LastObject()
        {
            if (_data == null || _data.Count == 0) throw new NullReferenceException();
            return _data.Last.Value;
        }
    }
}
