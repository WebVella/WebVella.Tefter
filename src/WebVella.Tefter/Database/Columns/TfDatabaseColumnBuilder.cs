namespace WebVella.Tefter.Database;

public abstract class TfDatabaseColumnBuilder
{
    protected Guid _id;
    protected DateTime _lastCommited;
    protected string _name;
    protected object _defaultValue = null;
    protected bool _isNullable = true;
    protected bool _autoDefaultValue = false;
	protected string _expression = null;
	private readonly TfDatabaseBuilder _databaseBuilder;

    internal Guid Id => _id;
	internal string Name => _name;
	internal DateTime LastCommited => _lastCommited;

    internal TfDatabaseColumnBuilder(string name, TfDatabaseBuilder databaseBuilder) 
        : this(Guid.NewGuid(), name, databaseBuilder )
    {
    }

    internal TfDatabaseColumnBuilder(Guid id, string name,  TfDatabaseBuilder databaseBuilder)
    {
        _id = id;
        _name = name;
        _databaseBuilder = databaseBuilder;
    }

    internal virtual TfDatabaseColumnBuilder WithLastCommited(DateTime lastCommited)
    {
        _lastCommited = lastCommited;

        return this;
    }

	internal abstract TfDatabaseColumn Build();
}