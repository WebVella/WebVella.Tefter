namespace WebVella.Tefter.Database;

public class DbIndexCollectionBuilder
{
    private readonly List<DbIndexBuilder> _builders = new List<DbIndexBuilder>();

    public DbIndexCollectionBuilder AddBTreeIndex(Action<DbBTreeIndexBuilder> action)
    {
        DbBTreeIndexBuilder builder = new DbBTreeIndexBuilder();
        action(builder);
        _builders.Add(builder);
        return this;
    }

    public DbIndexCollectionBuilder AddGinIndex(Action<DbGinIndexBuilder> action)
    {
        DbGinIndexBuilder builder = new DbGinIndexBuilder();
        action(builder);
        _builders.Add(builder);
        return this;
    }

    public DbIndexCollectionBuilder AddGistIndex(Action<DbGistIndexBuilder> action)
    {
        DbGistIndexBuilder builder = new DbGistIndexBuilder();
        action(builder);
        _builders.Add(builder);
        return this;
    }

    public DbIndexCollectionBuilder AddHashIndex(Action<DbHashIndexBuilder> action)
    {
        DbHashIndexBuilder builder = new DbHashIndexBuilder();
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