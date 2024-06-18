namespace WebVella.Tefter.Database;

public class DatabaseTableBuilder
{
    private Guid _id;
    private Guid? _applicationId;
    private Guid? _dataProviderId;
    private DateTime _lastCommited;
    private string _name;
    private readonly DatabaseColumnCollectionBuilder _columnsBuilder;
    private readonly DatabaseConstraintCollectionBuilder _constraintsBuilder;
    private readonly DatabaseIndexCollectionBuilder _indexesBuilder;
    private readonly DatabaseBuilder _databaseBuilder;

    internal Guid Id => _id;
    internal string Name => _name;
    internal DateTime LastCommited { get { return _lastCommited; } set { _lastCommited = value; } }
    internal DatabaseColumnCollectionBuilder ColumnsCollectionBuilder { get { return _columnsBuilder; } }
    internal DatabaseConstraintCollectionBuilder ConstraintsCollectionBuilder { get { return _constraintsBuilder; } }
    internal DatabaseIndexCollectionBuilder IndexesCollectionBuilder { get { return _indexesBuilder; } }

    internal DatabaseTableBuilder(Guid id, string name, DatabaseBuilder databaseBuilder)
    {
        _id = id;
        _name = name;
        _applicationId = null;
        _dataProviderId = null;
        _databaseBuilder = databaseBuilder;
        _columnsBuilder = new DatabaseColumnCollectionBuilder(name,_databaseBuilder);
        _constraintsBuilder = new DatabaseConstraintCollectionBuilder(name,_databaseBuilder);
        _indexesBuilder = new DatabaseIndexCollectionBuilder(name,_databaseBuilder);
    }

    public DatabaseTableBuilder WithApplicationId(Guid? appId)
    {
        _applicationId = appId;
        return this;
    }

    public DatabaseTableBuilder WithDataProviderId(Guid? dataProviderId)
    {
        _dataProviderId = dataProviderId;
        return this;
    }

    public DatabaseTableBuilder WithColumns(Action<DatabaseColumnCollectionBuilder> action)
    {
        action(_columnsBuilder);
        return this;
    }

    internal DatabaseColumnCollectionBuilder WithColumnsBuilder(Action<DatabaseColumnCollectionBuilder> action = null)
    {
        if (action != null)
            action(_columnsBuilder);

        return _columnsBuilder;
    }

    public DatabaseTableBuilder WithIndexes(Action<DatabaseIndexCollectionBuilder> action)
    {
        action(_indexesBuilder);
        return this;
    }

    internal DatabaseIndexCollectionBuilder WithIndexesBuilder(Action<DatabaseIndexCollectionBuilder> action = null)
    {
        if(action != null)
            action(_indexesBuilder);

        return _indexesBuilder;
    }

    public DatabaseTableBuilder WithConstraints(Action<DatabaseConstraintCollectionBuilder> action)
    {
        action(_constraintsBuilder);
        return this;
    }

    internal DatabaseConstraintCollectionBuilder WithConstraintsBuilder(Action<DatabaseConstraintCollectionBuilder> action= null)
    {
        if( action != null)
            action(_constraintsBuilder);

        return _constraintsBuilder;
    }
    internal DatabaseTableBuilder WithLastCommited(DateTime lastCommited)
    {
        _lastCommited = lastCommited;

        return this;
    }

    public DatabaseTable Build()
    {
        return new DatabaseTable
        {
            Id = _id,
            ApplicationId = _applicationId,
            DataProviderId = _dataProviderId,
            Name = _name,
            Columns = _columnsBuilder.Build(),
            Indexes = _indexesBuilder.Build(),
            Constraints = _constraintsBuilder.Build()
        };
    }

   
}