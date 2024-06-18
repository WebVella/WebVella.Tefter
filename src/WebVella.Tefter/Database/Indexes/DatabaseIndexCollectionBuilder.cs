namespace WebVella.Tefter.Database;

public class DatabaseIndexCollectionBuilder
{
    protected readonly DatabaseBuilder _databaseBuilder;
    private readonly List<DatabaseIndexBuilder> _builders;
    private readonly string _tableName; 

    internal DatabaseIndexCollectionBuilder(string tableName, DatabaseBuilder databaseBuilder)
    {
        _tableName = tableName;
        _builders = new List<DatabaseIndexBuilder>();
        _databaseBuilder = databaseBuilder;
    }


    #region <-- add btree --->

    public DatabaseIndexCollectionBuilder AddBTreeIndex(string name, Action<BTreeDatabaseIndexBuilder> action)
    {
		_databaseBuilder.RegisterName(name);

		BTreeDatabaseIndexBuilder builder = new BTreeDatabaseIndexBuilder(name, _tableName, _databaseBuilder);

        action(builder);

        _builders.Add(builder);

        return this;
    }
    internal BTreeDatabaseIndexBuilder AddBTreeIndexBuilder(string name, Action<BTreeDatabaseIndexBuilder> action = null)
    {
		_databaseBuilder.RegisterName(name);

		BTreeDatabaseIndexBuilder builder = new BTreeDatabaseIndexBuilder(name,  _tableName, _databaseBuilder);

        if (action != null)
            action(builder);

        _builders.Add(builder);

        return builder;
    }

    #endregion

    #region <--- add gin --->

    public DatabaseIndexCollectionBuilder AddGinIndex(string name, Action<GinDatabaseIndexBuilder> action)
    {
		_databaseBuilder.RegisterName(name);

		GinDatabaseIndexBuilder builder = new GinDatabaseIndexBuilder(name, _tableName, _databaseBuilder);

        action(builder);

        _builders.Add(builder);

        return this;
    }
    internal GinDatabaseIndexBuilder AddGinIndexBuilder(string name, Action<GinDatabaseIndexBuilder> action = null)
    {
		_databaseBuilder.RegisterName(name);

		GinDatabaseIndexBuilder builder = new GinDatabaseIndexBuilder(name, _tableName, _databaseBuilder);

        if (action != null)
            action(builder);

        _builders.Add(builder);

        return builder;
    }

    #endregion

    #region <--- gist --->

    public DatabaseIndexCollectionBuilder AddGistIndex(string name, Action<GistDatabaseIndexBuilder> action)
    {
		_databaseBuilder.RegisterName(name);

		GistDatabaseIndexBuilder builder = new GistDatabaseIndexBuilder(name, _tableName, _databaseBuilder);

        action(builder);

        _builders.Add(builder);

        return this;
    }

    internal GistDatabaseIndexBuilder AddGistIndexBuilder(string name, Action<GistDatabaseIndexBuilder> action = null)
    {
		_databaseBuilder.RegisterName(name);

		GistDatabaseIndexBuilder builder = new GistDatabaseIndexBuilder(name, _tableName, _databaseBuilder);

        if (action != null)
            action(builder);

        _builders.Add(builder);

        return builder;
    }

    #endregion

    #region <--- add hash --->

    public DatabaseIndexCollectionBuilder AddHashIndex(string name, Action<HashDatabaseIndexBuilder> action)
    {
		_databaseBuilder.RegisterName(name);

		HashDatabaseIndexBuilder builder = new HashDatabaseIndexBuilder(name, _tableName, _databaseBuilder);

        action(builder);

        _builders.Add(builder);

        return this;
    }

    internal HashDatabaseIndexBuilder AddHashIndexBuilder(string name, Action<HashDatabaseIndexBuilder> action = null)
    {
		_databaseBuilder.RegisterName(name);

		HashDatabaseIndexBuilder builder = new HashDatabaseIndexBuilder(name, _tableName, _databaseBuilder);

        if (action != null)
            action(builder);

        _builders.Add(builder);

        return builder;
    }

    #endregion

    #region <--- remove --->

    public DatabaseIndexCollectionBuilder Remove(string name)
    {
        var builder = _builders.SingleOrDefault(x => x.Name == name);
        if (builder is null)
            throw new DatabaseBuilderException($"Index with name '{name}' is not found.");

		_databaseBuilder.UnregisterName(name);

		_builders.Remove(builder);

        return this;
    }

    #endregion

    #region <--- build --->

    internal DatabaseIndexCollection Build()
    {
        var collection = new DatabaseIndexCollection();

        foreach (var builder in _builders)
            collection.Add(builder.Build());

        return collection;
    }

    #endregion
}