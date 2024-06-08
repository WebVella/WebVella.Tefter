namespace WebVella.Tefter.Database;

public abstract class DbColumnBuilder
{
    protected Guid _id;
    protected DateTime _lastCommited;
    protected string _name;
    protected object _defaultValue = null;
    protected bool _isNullable = true;
    protected bool _autoDefaultValue = false;
    private readonly DatabaseBuilder _databaseBuilder;

    internal string Name => _name;
    internal DateTime LastCommited => _lastCommited;

    internal DbColumnBuilder(string name, DatabaseBuilder databaseBuilder) 
        : this(Guid.NewGuid(), name, databaseBuilder )
    {
    }

    internal DbColumnBuilder(Guid id, string name,  DatabaseBuilder databaseBuilder)
    {
        _id = id;
        _name = name;
        _databaseBuilder = databaseBuilder;
    }

    internal virtual DbColumnBuilder WithLastCommited(DateTime lastCommited)
    {
        _lastCommited = lastCommited;

        return this;
    }

    internal abstract DbColumn Build();
}