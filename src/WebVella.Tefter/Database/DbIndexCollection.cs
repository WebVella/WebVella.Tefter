using static Npgsql.Replication.PgOutput.Messages.RelationMessage;

namespace WebVella.Tefter.Database;

public class DbIndexCollection
{
    private AsyncLock _lock;
    private readonly List<DbIndex> _indexes;

    public DbIndex this[int i]
    {
        get { return _indexes[i]; }
        internal set { _indexes[i] = value; }
    }

    public DbIndexCollection()
    {
        _lock = new AsyncLock();
        _indexes = new List<DbIndex>();
    }

    public DbIndex Find(string name)
    {
        if (name is null)
            throw new ArgumentNullException("name");

        using (_lock.Lock())
        {
            return _indexes.SingleOrDefault(c => c.Name == name);
        }
    }

    internal void Add(DbIndex index)
    {
        if (index is null)
            throw new ArgumentNullException("index");

        using (_lock.Lock())
        {
            _indexes.Add(index);
        }
    }

    internal void AddRange(IEnumerable<DbIndex> range)
    {
        if (_indexes is null)
            throw new ArgumentNullException("range");

        using (_lock.Lock())
        {
            _indexes.AddRange(range);
        }
    }

    internal void Remove(string name)
    {
        if (name is null)
            throw new ArgumentNullException("name");

        using (_lock.Lock())
        {
            var index = Find(name);

            if (index is null)
                throw new DbException($"Trying to remove non existent index '{name}' from table");

            _indexes.Remove(index);
        }
    }

    internal void Remove(DbIndex index)
    {
        if (index is null)
            throw new ArgumentNullException("index");

        using (_lock.Lock())
        {
            _indexes.Remove(index);
        }
    }

    internal void Clear()
    {
        using (_lock.Lock())
        {
            _indexes.Clear();
        }
    }
}
