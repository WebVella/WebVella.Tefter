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

    #region <=== Public Methods ===>

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
    #endregion

    #region <=== Build and Remove Methods ===>

    public DbIndexCollectionBuilder Remove(string name)
    {
        var builder = _builders.SingleOrDefault(x => x.Name == name);
        if (builder is null)
            throw new DbBuilderException($"Index with name '{name}' is not found.");

        _builders.Remove(builder);
        return this;
    }

    internal DbIndexCollection Build()
    {
        var collection = new DbIndexCollection();

        foreach (var builder in _builders)
            collection.Add(builder.Build());

        return collection;
    }

    #endregion

    #region <=== Internal Methods ==>

    //used for building new DbTable from existing instance

    internal DbIndexCollectionBuilder InternalAddExistingIndex(DbIndex index)
    {
        if (index is null)
            throw new ArgumentNullException(nameof(index));

        if (index is DbBTreeIndex)
            _builders.Add(new DbBTreeIndexBuilder((DbBTreeIndex)index, _tableBuilder));
        else if (index is DbGinIndex)
            _builders.Add(new DbGinIndexBuilder((DbGinIndex)index, _tableBuilder));
        else if (index is DbGistIndex)
            _builders.Add(new DbGistIndexBuilder((DbGistIndex)index, _tableBuilder));
        else if (index is DbHashIndex)
            _builders.Add(new DbHashIndexBuilder((DbHashIndex)index, _tableBuilder));
        else
            throw new DbBuilderException($"Not supported db constraint type {index.GetType()}");

        return this;
    }

    #endregion
}