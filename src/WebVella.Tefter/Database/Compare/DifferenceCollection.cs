namespace WebVella.Tefter.Database;

public sealed record DifferenceCollection : IEnumerable
{
    private AsyncLock _lock;
    private readonly List<Difference> _differences;

    public Difference this[int i]
    {
        get { return _differences[i]; }
        private set { _differences[i] = value; }
    }

    public DifferenceCollection()
    {
        _lock = new AsyncLock();
        _differences = new List<Difference>();
    }

    internal void Add(Difference difference)
    {
        if (difference is null)
            throw new ArgumentNullException("difference");

        using (_lock.Lock())
        {
            _differences.Add(difference);
        }
    }

    internal void AddRange(IEnumerable<Difference> range)
    {
        if (range is null)
            throw new ArgumentNullException("range");

        using (_lock.Lock())
        {
            _differences.AddRange(range);
        }
    }

    internal void AddRange(DifferenceCollection range)
    {
        if (range is null)
            throw new ArgumentNullException("range");

        using (_lock.Lock())
        {
            _differences.AddRange(range._differences);
        }
    }

    internal void Remove(Difference difference)
    {
        if (difference is null)
            throw new ArgumentNullException("difference");

        using (_lock.Lock())
        {
            _differences.Remove(difference);
        }
    }

    public IEnumerator<Difference> GetEnumerator()
    {
        return _differences.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    public void OrderForExecution()
    {
        //TODO implement
    }
}
