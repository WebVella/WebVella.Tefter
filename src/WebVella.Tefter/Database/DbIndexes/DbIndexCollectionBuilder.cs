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


    #region <-- add btree --->

    public DbIndexCollectionBuilder AddNewBTreeIndex(string name, Action<DbBTreeIndexBuilder> action)
    {
        DbBTreeIndexBuilder builder = new DbBTreeIndexBuilder(name, _tableBuilder);

        action(builder);

        _builders.Add(builder);

        return this;
    }
    internal DbBTreeIndexBuilder AddNewBTreeIndexBuilder(string name, Action<DbBTreeIndexBuilder> action = null)
    {
        DbBTreeIndexBuilder builder = new DbBTreeIndexBuilder(name, _tableBuilder);

        if (action != null)
            action(builder);

        _builders.Add(builder);

        return builder;
    }

    #endregion

    #region <--- add gin --->

    public DbIndexCollectionBuilder AddGinIndex(string name, Action<DbGinIndexBuilder> action)
    {
        DbGinIndexBuilder builder = new DbGinIndexBuilder(name, _tableBuilder);

        action(builder);

        _builders.Add(builder);

        return this;
    }
    internal DbGinIndexBuilder AddGinIndexBuilder(string name, Action<DbGinIndexBuilder> action = null)
    {
        DbGinIndexBuilder builder = new DbGinIndexBuilder(name, _tableBuilder);

        if (action != null)
            action(builder);

        _builders.Add(builder);

        return builder;
    }

    #endregion

    #region <--- gist --->

    public DbIndexCollectionBuilder AddGistIndex(string name, Action<DbGistIndexBuilder> action)
    {
        DbGistIndexBuilder builder = new DbGistIndexBuilder(name, _tableBuilder);

        action(builder);

        _builders.Add(builder);

        return this;
    }

    internal DbGistIndexBuilder AddGistIndexBuilder(string name, Action<DbGistIndexBuilder> action = null)
    {
        DbGistIndexBuilder builder = new DbGistIndexBuilder(name, _tableBuilder);

        if (action != null)
            action(builder);

        _builders.Add(builder);

        return builder;
    }

    #endregion

    #region <--- add hash --->

    public DbIndexCollectionBuilder AddHashIndex(string name, Action<DbHashIndexBuilder> action)
    {
        DbHashIndexBuilder builder = new DbHashIndexBuilder(name, _tableBuilder);

        action(builder);

        _builders.Add(builder);

        return this;
    }

    internal DbHashIndexBuilder AddHashIndexBuilder(string name, Action<DbHashIndexBuilder> action = null)
    {
        DbHashIndexBuilder builder = new DbHashIndexBuilder(name, _tableBuilder);

        if (action != null)
            action(builder);

        _builders.Add(builder);

        return builder;
    }

    #endregion

    #region <--- remove --->

    public DbIndexCollectionBuilder Remove(string name)
    {
        var builder = _builders.SingleOrDefault(x => x.Name == name);
        if (builder is null)
            throw new DbBuilderException($"Index with name '{name}' is not found.");

        _builders.Remove(builder);

        return this;
    }

    #endregion

    #region <--- build --->

    internal DbIndexCollection Build()
    {
        var collection = new DbIndexCollection();

        foreach (var builder in _builders)
            collection.Add(builder.Build());

        return collection;
    }

    #endregion
}