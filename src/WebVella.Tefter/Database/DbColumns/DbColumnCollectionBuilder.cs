namespace WebVella.Tefter.Database;

public class DbColumnCollectionBuilder
{
    private readonly List<DbColumnBuilder> _builders = new List<DbColumnBuilder>();
    public DbColumnCollectionBuilder AddTableIdColumn(Action<DbIdColumnBuilder> action)
    {
        DbIdColumnBuilder builder = new DbIdColumnBuilder();
        action(builder);
        _builders.Add(builder);
        return this;
    }

    public DbColumnCollectionBuilder AddGuidColumn(Action<DbGuidColumnBuilder> action)
    {
        DbGuidColumnBuilder builder = new DbGuidColumnBuilder();
        action(builder);
        _builders.Add(builder);
        return this;
    }

    public DbColumnCollectionBuilder AddAutoIncrementColumn(Action<DbAutoIncrementColumnBuilder> action)
    {
        DbAutoIncrementColumnBuilder builder = new DbAutoIncrementColumnBuilder();
        action(builder);
        _builders.Add(builder);
        return this;
    }

    public DbColumnCollectionBuilder AddNumberColumn(Action<DbNumberColumnBuilder> action)
    {
        DbNumberColumnBuilder builder = new DbNumberColumnBuilder();
        action(builder);
        _builders.Add(builder);
        return this;
    }

    public DbColumnCollectionBuilder AddBooleanColumn(Action<DbBooleanColumnBuilder> action)
    {
        DbBooleanColumnBuilder builder = new DbBooleanColumnBuilder();
        action(builder);
        _builders.Add(builder);
        return this;
    }

    public DbColumnCollectionBuilder AddDateColumn(Action<DbDateColumnBuilder> action)
    {
        DbDateColumnBuilder builder = new DbDateColumnBuilder();
        action(builder);
        _builders.Add(builder);
        return this;
    }

    public DbColumnCollectionBuilder AddDateTimeColumn(Action<DbDateTimeColumnBuilder> action)
    {
        DbDateTimeColumnBuilder builder = new DbDateTimeColumnBuilder();
        action(builder);
        _builders.Add(builder);
        return this;
    }

    public DbColumnCollectionBuilder AddTextColumn(Action<DbTextColumnBuilder> action)
    {
        DbTextColumnBuilder builder = new DbTextColumnBuilder();
        action(builder);
        _builders.Add(builder);
        return this;
    }

    internal DbColumnCollection Build()
    {
        var collection = new DbColumnCollection(); 
        
        foreach(var builder in _builders)
            collection.Add(builder.Build());
        
        return collection;
    }
}