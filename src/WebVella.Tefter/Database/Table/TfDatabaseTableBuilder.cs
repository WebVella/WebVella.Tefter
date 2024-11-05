namespace WebVella.Tefter.Database;

public class TfDatabaseTableBuilder
{
    private Guid _id;
    private Guid? _applicationId;
    private Guid? _dataProviderId;
    private DateTime _lastCommited;
    private string _name;
    private readonly TfDatabaseColumnCollectionBuilder _columnsBuilder;
    private readonly TfDatabaseConstraintCollectionBuilder _constraintsBuilder;
    private readonly TfDatabaseIndexCollectionBuilder _indexesBuilder;
    private readonly TfDatabaseBuilder _databaseBuilder;

    internal Guid Id => _id;
    internal string Name => _name;
    internal DateTime LastCommited { get { return _lastCommited; } set { _lastCommited = value; } }
    internal TfDatabaseColumnCollectionBuilder ColumnsCollectionBuilder { get { return _columnsBuilder; } }
    internal TfDatabaseConstraintCollectionBuilder ConstraintsCollectionBuilder { get { return _constraintsBuilder; } }
    internal TfDatabaseIndexCollectionBuilder IndexesCollectionBuilder { get { return _indexesBuilder; } }

    internal TfDatabaseTableBuilder(Guid id, string name, TfDatabaseBuilder databaseBuilder)
    {
        _id = id;
        _name = name;
        _applicationId = null;
        _dataProviderId = null;
        _databaseBuilder = databaseBuilder;
        _columnsBuilder = new TfDatabaseColumnCollectionBuilder(name,_databaseBuilder);
        _constraintsBuilder = new TfDatabaseConstraintCollectionBuilder(name,_databaseBuilder);
        _indexesBuilder = new TfDatabaseIndexCollectionBuilder(name,_databaseBuilder);
    }

    public TfDatabaseTableBuilder WithApplicationId(Guid? appId)
    {
        _applicationId = appId;
        return this;
    }

    public TfDatabaseTableBuilder WithDataProviderId(Guid? dataProviderId)
    {
        _dataProviderId = dataProviderId;
        return this;
    }

    public TfDatabaseTableBuilder WithColumns(Action<TfDatabaseColumnCollectionBuilder> action)
    {
        action(_columnsBuilder);
        return this;
    }

    internal TfDatabaseColumnCollectionBuilder WithColumnsBuilder(Action<TfDatabaseColumnCollectionBuilder> action = null)
    {
        if (action != null)
            action(_columnsBuilder);

        return _columnsBuilder;
    }

    public TfDatabaseTableBuilder WithIndexes(Action<TfDatabaseIndexCollectionBuilder> action)
    {
        action(_indexesBuilder);
        return this;
    }

    internal TfDatabaseIndexCollectionBuilder WithIndexesBuilder(Action<TfDatabaseIndexCollectionBuilder> action = null)
    {
        if(action != null)
            action(_indexesBuilder);

        return _indexesBuilder;
    }

    public TfDatabaseTableBuilder WithConstraints(Action<TfDatabaseConstraintCollectionBuilder> action)
    {
        action(_constraintsBuilder);
        return this;
    }

    internal TfDatabaseConstraintCollectionBuilder WithConstraintsBuilder(Action<TfDatabaseConstraintCollectionBuilder> action= null)
    {
        if( action != null)
            action(_constraintsBuilder);

        return _constraintsBuilder;
    }
    internal TfDatabaseTableBuilder WithLastCommited(DateTime lastCommited)
    {
        _lastCommited = lastCommited;

        return this;
    }

    public TfDatabaseTable Build()
    {
        return new TfDatabaseTable
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