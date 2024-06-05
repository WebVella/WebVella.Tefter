namespace WebVella.Tefter.Database;

public class DbTableBuilder
{
    private Guid _id = Guid.Empty;
    private Guid? _applicationId = null;
    private Guid? _dataProviderId = null;
    private string _name = string.Empty;
    private readonly DbColumnCollectionBuilder _columnsBuilder = new DbColumnCollectionBuilder();
    private readonly DbConstraintCollectionBuilder _constraintsBuilder = new DbConstraintCollectionBuilder();
    private readonly DbIndexCollectionBuilder _indexesBuilder = new DbIndexCollectionBuilder();

    public static DbTableBuilder FromTable(DbTable table)
    {
        //TODO implement
        //var builder = new DbTableBuilder()
        //    .WithApplicationId(table.Meta.ApplicationId)
        return new DbTableBuilder();
    }

    public DbTableBuilder Id(Guid id) 
    { 
        _id = id; 
        return this; 
    }
    
    public DbTableBuilder ApplicationId(Guid appId) 
    {
        _applicationId = appId; 
        return this; 
    }
    
    public DbTableBuilder DataProviderId(Guid dataProviderId) 
    { 
        _dataProviderId = dataProviderId; 
        return this; 
    }

    public DbTableBuilder Name(string name) 
    { 
        _name = name; 
        return this; 
    }

    public DbTableBuilder Columns(Action<DbColumnCollectionBuilder> action)
    {
        action(_columnsBuilder);
        return this;
    }

    public DbTableBuilder Indexes(Action<DbIndexCollectionBuilder> action)
    {
        action(_indexesBuilder);
        return this;
    }

    public DbTableBuilder Constraints(Action<DbConstraintCollectionBuilder> action)
    {
        action(_constraintsBuilder);
        return this;
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