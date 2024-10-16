using Collections;

public class ConcreteCollection<T> : BaseCollection<T> where T : IComparable<T>
{
    private List<T> _collection = new List<T>();

    protected override void fillCollection(string[] input, Func<string, T> func)
    {
        _collection = input.Select(func).ToList();
    }

    protected override void sortCollection(Func<T, T> comparer)
    {
        _collection = _collection.OrderBy(comparer).ToList();
    }

    protected override void printCollection(TextWriter writer)
    {
        foreach (var item in _collection)
        {
            writer.WriteLine(item);
        }
    }

    public override T FirstObject()
    {
        return _collection.FirstOrDefault();
    }

    public override T LastObject()
    {
        return _collection.LastOrDefault();
    }

    public override int Count()
    {
        return _collection.Count;
    }
}
