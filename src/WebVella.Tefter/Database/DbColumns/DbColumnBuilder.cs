namespace WebVella.Tefter.Database;

public abstract class DbColumnBuilder
{
    protected Guid _id;
    protected string _name;
    protected object _defaultValue = null;
    protected bool _isNullable = true;
    protected bool _autoDefaultValue = false;
    private readonly DatabaseBuilder _databaseBuilder;

    internal string Name { get { return _name; } }

    internal DbColumnBuilder(string name, DatabaseBuilder databaseBuilder) 
        : this(Guid.NewGuid(), name, databaseBuilder )
    {
    }

    internal DbColumnBuilder(Guid id, string name, DatabaseBuilder databaseBuilder)
    {
        _id = id;
        _name = name;
        _databaseBuilder = databaseBuilder;
    }

    internal abstract DbColumn Build();
}