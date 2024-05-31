using System.Data;
using System.Xml.Linq;

namespace WebVella.Tefter.Database;

public class DbColumnCollection
{
    private AsyncLock _lock;
    private readonly List<DbColumn> _columns;

    public DbColumn this[int i]
    {
        get { return _columns[i]; }
        internal set { _columns[i] = value; }
    }

    public DbColumnCollection()
    {
        _lock = new AsyncLock();
        _columns = new List<DbColumn>();
    }

    public DbColumn Find(string name)
    {
        if (name is null)
            throw new ArgumentNullException("name");

        using (_lock.Lock())
        {
            return _columns.SingleOrDefault(c => c.Name == name);
        }
    }

    internal void Add(DbColumn column)
    {
        if (column is null)
            throw new ArgumentNullException("column");

        using (_lock.Lock())
        {
            _columns.Add(column);
        }
    }

    internal void AddRange(IEnumerable<DbColumn> range)
    {
        if (_columns is null)
            throw new ArgumentNullException("columns");

        using (_lock.Lock())
        {
            _columns.AddRange(range);
        }
    }

    internal void Remove(DbColumn column)
    {
        if (column is null)
            throw new ArgumentNullException("column");

        using (_lock.Lock())
        {
            _columns.Remove(column);
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
                throw new DbException($"Trying to remove non existent column '{name}' from table");
            
            _columns.Remove(column);
        }
    }

    internal void Clear()
    {
        using (_lock.Lock())
        {
            _columns.Clear();
        }
    }
}
