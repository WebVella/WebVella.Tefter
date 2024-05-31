using static Npgsql.Replication.PgOutput.Messages.RelationMessage;

namespace WebVella.Tefter.Database;

public class DbConstraintCollection
{
    private AsyncLock _lock;
    private readonly List<DbConstraint> _constraints;

    public DbConstraint this[int i]
    {
        get { return _constraints[i]; }
        internal set { _constraints[i] = value; }
    }

    public DbConstraintCollection()
    {
        _lock = new AsyncLock();
        _constraints = new List<DbConstraint>();
    }

    public DbConstraint Find(string name)
    {
        if (name is null)
            throw new ArgumentNullException("name");

        using (_lock.Lock())
        {
            return _constraints.SingleOrDefault(c => c.Name == name);
        }
    }

    internal void Add(DbConstraint constraint)
    {
        if (constraint is null)
            throw new ArgumentNullException("constraint");

        using (_lock.Lock())
        {
            _constraints.Add(constraint);
        }
    }

    internal void AddRange(IEnumerable<DbConstraint> range)
    {
        if (_constraints is null)
            throw new ArgumentNullException("range");

        using (_lock.Lock())
        {
            _constraints.AddRange(range);
        }
    }

    internal void Remove(string name)
    {
        if (name is null)
            throw new ArgumentNullException("name");

        using (_lock.Lock())
        {
            var constraint = Find(name);

            if (constraint is null)
                throw new DbException($"Trying to remove non existent constraint '{name}' from table");

            _constraints.Remove(constraint);
        }
    }

    internal void Remove(DbConstraint constraint)
    {
        if (constraint is null)
            throw new ArgumentNullException("constraint");

        using (_lock.Lock())
        {
            _constraints.Remove(constraint);
        }
    }

    internal void Clear()
    {
        using (_lock.Lock())
        {
            _constraints.Clear();
        }
    }
}
