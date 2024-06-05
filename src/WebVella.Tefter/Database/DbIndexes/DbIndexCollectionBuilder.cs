namespace WebVella.Tefter.Database;

public class DbIndexCollectionBuilder
{
    internal ReadOnlyCollection<DbIndexBuilder> Builders => _builders.AsReadOnly();
    protected readonly DbTableBuilder _tableBuilder;
    private readonly List<DbIndexBuilder> _builders;

    internal DbIndexCollectionBuilder(DbTableBuilder tableBuilder)
    {
        _builders = new List<DbIndexBuilder>();
        _tableBuilder = tableBuilder;
    }


    public DbIndexCollectionBuilder AddNewBTreeIndex(string name, Action<DbBTreeIndexBuilder> action)
    {
        DbBTreeIndexBuilder builder = new DbBTreeIndexBuilder(name, true, _tableBuilder);
        action(builder);
        _builders.Add(builder);
        return this;
    }

    public DbIndexCollectionBuilder AddGinIndex(string name, Action<DbGinIndexBuilder> action)
    {
        DbGinIndexBuilder builder = new DbGinIndexBuilder(name, true, _tableBuilder);
        action(builder);
        _builders.Add(builder);
        return this;
    }

    public DbIndexCollectionBuilder AddGistIndex(string name, Action<DbGistIndexBuilder> action)
    {
        DbGistIndexBuilder builder = new DbGistIndexBuilder(name, true, _tableBuilder);
        action(builder);
        _builders.Add(builder);
        return this;
    }

    public DbIndexCollectionBuilder AddHashIndex(string name, Action<DbHashIndexBuilder> action)
    {
        DbHashIndexBuilder builder = new DbHashIndexBuilder(name, true, _tableBuilder);
        action(builder);
        _builders.Add(builder);
        return this;
    }


    internal DbIndexCollection Build()
    {
        var collection = new DbIndexCollection();

        foreach (var builder in _builders)
            collection.Add(builder.Build());

        return collection;
    }
}