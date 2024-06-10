namespace WebVella.Tefter.Database;

public class DbIndexCollectionBuilder
{
    protected readonly DatabaseBuilder _databaseBuilder;
    private readonly List<DbIndexBuilder> _builders;
    private readonly string _tableName; 

    internal DbIndexCollectionBuilder(string tableName, DatabaseBuilder databaseBuilder)
    {
        _tableName = tableName;
        _builders = new List<DbIndexBuilder>();
        _databaseBuilder = databaseBuilder;
    }


    #region <-- add btree --->

    public DbIndexCollectionBuilder AddBTreeIndex(string name, Action<DbBTreeIndexBuilder> action)
    {
		_databaseBuilder.RegisterName(name);

		DbBTreeIndexBuilder builder = new DbBTreeIndexBuilder(name, _tableName, _databaseBuilder);

        action(builder);

        _builders.Add(builder);

        return this;
    }
    internal DbBTreeIndexBuilder AddBTreeIndexBuilder(string name, Action<DbBTreeIndexBuilder> action = null)
    {
		_databaseBuilder.RegisterName(name);

		DbBTreeIndexBuilder builder = new DbBTreeIndexBuilder(name,  _tableName, _databaseBuilder);

        if (action != null)
            action(builder);

        _builders.Add(builder);

        return builder;
    }

    #endregion

    #region <--- add gin --->

    public DbIndexCollectionBuilder AddGinIndex(string name, Action<DbGinIndexBuilder> action)
    {
		_databaseBuilder.RegisterName(name);

		DbGinIndexBuilder builder = new DbGinIndexBuilder(name, _tableName, _databaseBuilder);

        action(builder);

        _builders.Add(builder);

        return this;
    }
    internal DbGinIndexBuilder AddGinIndexBuilder(string name, Action<DbGinIndexBuilder> action = null)
    {
		_databaseBuilder.RegisterName(name);

		DbGinIndexBuilder builder = new DbGinIndexBuilder(name, _tableName, _databaseBuilder);

        if (action != null)
            action(builder);

        _builders.Add(builder);

        return builder;
    }

    #endregion

    #region <--- gist --->

    public DbIndexCollectionBuilder AddGistIndex(string name, Action<DbGistIndexBuilder> action)
    {
		_databaseBuilder.RegisterName(name);

		DbGistIndexBuilder builder = new DbGistIndexBuilder(name, _tableName, _databaseBuilder);

        action(builder);

        _builders.Add(builder);

        return this;
    }

    internal DbGistIndexBuilder AddGistIndexBuilder(string name, Action<DbGistIndexBuilder> action = null)
    {
		_databaseBuilder.RegisterName(name);

		DbGistIndexBuilder builder = new DbGistIndexBuilder(name, _tableName, _databaseBuilder);

        if (action != null)
            action(builder);

        _builders.Add(builder);

        return builder;
    }

    #endregion

    #region <--- add hash --->

    public DbIndexCollectionBuilder AddHashIndex(string name, Action<DbHashIndexBuilder> action)
    {
		_databaseBuilder.RegisterName(name);

		DbHashIndexBuilder builder = new DbHashIndexBuilder(name, _tableName, _databaseBuilder);

        action(builder);

        _builders.Add(builder);

        return this;
    }

    internal DbHashIndexBuilder AddHashIndexBuilder(string name, Action<DbHashIndexBuilder> action = null)
    {
		_databaseBuilder.RegisterName(name);

		DbHashIndexBuilder builder = new DbHashIndexBuilder(name, _tableName, _databaseBuilder);

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

		_databaseBuilder.UnregisterName(name);

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