namespace WebVella.Tefter.Database;

public class DbTableBuilder
{
    private Guid _id;
    private Guid? _applicationId;
    private Guid? _dataProviderId;
    private string _name;
    private readonly DbColumnCollectionBuilder _columnsBuilder;
    private readonly DbConstraintCollectionBuilder _constraintsBuilder;
    private readonly DbIndexCollectionBuilder _indexesBuilder;
    private readonly DatabaseBuilder _databaseBuilder;

    internal string Name { get { return _name; } }
    internal DbColumnCollectionBuilder ColumnsCollectionBuilder { get { return _columnsBuilder; } }
    internal DbConstraintCollectionBuilder ConstraintsCollectionBuilder { get { return _constraintsBuilder; } }
    internal DbIndexCollectionBuilder IndexesCollectionBuilder { get { return _indexesBuilder; } }

    internal DbTableBuilder(Guid id, string name, DatabaseBuilder databaseBuilder)
    {
        _id = id;
        _name = name;
        _applicationId = null;
        _dataProviderId = null;
        _databaseBuilder = databaseBuilder;
        _columnsBuilder = new DbColumnCollectionBuilder(name,_databaseBuilder);
        _constraintsBuilder = new DbConstraintCollectionBuilder(name,_databaseBuilder);
        _indexesBuilder = new DbIndexCollectionBuilder(name,_databaseBuilder);
    }

    public DbTableBuilder WithApplicationId(Guid appId)
    {
        _applicationId = appId;
        return this;
    }

    public DbTableBuilder WithDataProviderId(Guid dataProviderId)
    {
        _dataProviderId = dataProviderId;
        return this;
    }

    public DbTableBuilder WithColumns(Action<DbColumnCollectionBuilder> action)
    {
        action(_columnsBuilder);
        return this;
    }

    internal DbColumnCollectionBuilder WithColumnsBuilder(Action<DbColumnCollectionBuilder> action = null)
    {
        if (action != null)
            action(_columnsBuilder);

        return _columnsBuilder;
    }

    public DbTableBuilder WithIndexes(Action<DbIndexCollectionBuilder> action)
    {
        action(_indexesBuilder);
        return this;
    }

    internal DbIndexCollectionBuilder WithIndexesBuilder(Action<DbIndexCollectionBuilder> action = null)
    {
        if(action != null)
            action(_indexesBuilder);

        return _indexesBuilder;
    }

    public DbTableBuilder WithConstraints(Action<DbConstraintCollectionBuilder> action)
    {
        action(_constraintsBuilder);
        return this;
    }

    internal DbConstraintCollectionBuilder WithConstraintsBuilder(Action<DbConstraintCollectionBuilder> action= null)
    {
        if( action != null)
            action(_constraintsBuilder);

        return _constraintsBuilder;
    }

    public DbTable Build()
    {
        return new DbTable
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