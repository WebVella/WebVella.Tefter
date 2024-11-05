namespace WebVella.Tefter.Database;

public sealed record TfDifferenceCollection : IEnumerable
{
    private AsyncLock _lock;
    private readonly List<TfDifference> _differences;
    public int Count => _differences.Count;

    public TfDifference this[int i]
    {
        get { return _differences[i]; }
        private set { _differences[i] = value; }
    }

    public TfDifferenceCollection()
    {
        _lock = new AsyncLock();
        _differences = new List<TfDifference>();
    }

    internal void Add(TfDifference difference)
    {
        if (difference is null)
            throw new ArgumentNullException("difference");

        using (_lock.Lock())
        {
            _differences.Add(difference);
        }
    }

    internal void AddRange(IEnumerable<TfDifference> range)
    {
        if (range is null)
            throw new ArgumentNullException("range");

        using (_lock.Lock())
        {
            _differences.AddRange(range);
        }
    }

    internal void AddRange(TfDifferenceCollection range)
    {
        if (range is null)
            throw new ArgumentNullException("range");

        using (_lock.Lock())
        {
            _differences.AddRange(range._differences);
        }
    }

    internal void Remove(TfDifference difference)
    {
        if (difference is null)
            throw new ArgumentNullException("difference");

        using (_lock.Lock())
        {
            _differences.Remove(difference);
        }
    }

    public IEnumerator<TfDifference> GetEnumerator()
    {
        return _differences.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
