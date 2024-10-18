using System.Text;

namespace Collections;
internal class CollectionTester<T>
{
    private string[] _inputStrings;
    Func<string, T> _stringToT;
    Func<T, T> _comparer;
    private List<BaseCollection<T>> _collection = new List<BaseCollection<T>>();

    public CollectionTester(string[] input, Func<string, T> stringToT, Func<T, T> comparer)
    {
        _inputStrings = input;
        _stringToT = stringToT;
        _comparer = comparer;
    }

    public void Add(BaseCollection<T> collection)
    {
        _collection.Add(collection);
    }

    public int Count()
    {
        return _collection.Count;
    }

    private long RunTest(BaseCollection<T> collection)
    {
        Console.WriteLine($"Test {collection.GetType()}.");
        Console.WriteLine($"Fill collection in {collection.FillCollection(_inputStrings, _stringToT)} ms");
        Console.WriteLine($"Sort collection in {collection.SortCollection(_comparer)} ms.");
        using (var writer = new StreamWriter("dump.txt", false, Encoding.UTF8))
        {
            Console.WriteLine($"Print collection in {collection.PrintCollection(writer)} ms.");
        }
        Console.WriteLine($"Total time {collection.TotalElapsedMiliseconds} ms.");
        Console.WriteLine($"For {collection.Count()} objects, first object: {collection.FirstObject().ToString()}, last object: {collection.LastObject().ToString()}");

        Console.WriteLine();
        return 0;
    }

    public void RunAllTest()
    {
        foreach (BaseCollection<T> collection in _collection)
        {
            RunTest(collection);
        }
    }
}
