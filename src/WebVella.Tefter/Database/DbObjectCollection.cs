namespace WebVella.Tefter.Database;

public abstract record DbObjectCollection<T> : IEnumerable<T> where T : DbObject
{
    protected AsyncLock _lock;
    protected readonly List<T> _dbObjects;

    public T this[int i]
    {
        get { return _dbObjects[i]; }
        private set { _dbObjects[i] = value; }
    }

    public DbObjectCollection()
    {
        _lock = new AsyncLock();
        _dbObjects = new List<T>();
    }

    public T Find(string name)
    {
        if (name is null)
            throw new ArgumentNullException("name");

        using (_lock.Lock())
        {
            return _dbObjects.SingleOrDefault(c => c.Name == name);
        }
    }

    internal void Add(T dbObject)
    {
        if (dbObject is null)
            throw new ArgumentNullException("dbObject");

        using (_lock.Lock())
        {
            _dbObjects.Add(dbObject);
        }
    }

    internal void AddRange(IEnumerable<T> range)
    {
        if (range is null)
            throw new ArgumentNullException("range");

        using (_lock.Lock())
        {
            _dbObjects.AddRange(range);
        }
    }

    internal void Remove(T dbObject)
    {
        if (dbObject is null)
            throw new ArgumentNullException("dbObject");

        using (_lock.Lock())
        {
            _dbObjects.Remove(dbObject);
        }
    }

    internal void Remove(string name)
    {
        if (name is null)
            throw new ArgumentNullException("name");

        using (_lock.Lock())
        {
            var column = Find(name);
            if (column is null)
                throw new DbException($"Trying to remove non existent {typeof(T)} with '{name}' from table");

            _dbObjects.Remove(column);
        }
    }

    internal void Clear()
    {
        using (_lock.Lock())
        {
            _dbObjects.Clear();
        }
    }

    public IEnumerator<T> GetEnumerator()
    {
        return _dbObjects.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public override string ToString()
    {
        using (_lock.Lock())
        {
            return string.Join(",", _dbObjects.Select(x => x.Name));
        }
    }
}
