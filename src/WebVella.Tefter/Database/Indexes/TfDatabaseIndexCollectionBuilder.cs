namespace WebVella.Tefter.Database;

public class TfDatabaseIndexCollectionBuilder
{
    protected readonly TfDatabaseBuilder _databaseBuilder;
    private readonly List<TfDatabaseIndexBuilder> _builders;
    private readonly string _tableName; 

    internal TfDatabaseIndexCollectionBuilder(string tableName, TfDatabaseBuilder databaseBuilder)
    {
        _tableName = tableName;
        _builders = new List<TfDatabaseIndexBuilder>();
        _databaseBuilder = databaseBuilder;
    }


    #region <-- add btree --->

    public TfDatabaseIndexCollectionBuilder AddBTreeIndex(string name, Action<TfBTreeDatabaseIndexBuilder> action)
    {
		_databaseBuilder.RegisterName(name);

		TfBTreeDatabaseIndexBuilder builder = new TfBTreeDatabaseIndexBuilder(name, _tableName, _databaseBuilder);

        action(builder);

        _builders.Add(builder);

        return this;
    }
    internal TfBTreeDatabaseIndexBuilder AddBTreeIndexBuilder(string name, Action<TfBTreeDatabaseIndexBuilder> action = null)
    {
		_databaseBuilder.RegisterName(name);

		TfBTreeDatabaseIndexBuilder builder = new TfBTreeDatabaseIndexBuilder(name,  _tableName, _databaseBuilder);

        if (action != null)
            action(builder);

        _builders.Add(builder);

        return builder;
    }

    #endregion

    #region <--- add gin --->

    public TfDatabaseIndexCollectionBuilder AddGinIndex(string name, Action<TfGinDatabaseIndexBuilder> action)
    {
		_databaseBuilder.RegisterName(name);

		TfGinDatabaseIndexBuilder builder = new TfGinDatabaseIndexBuilder(name, _tableName, _databaseBuilder);

        action(builder);

        _builders.Add(builder);

        return this;
    }
    internal TfGinDatabaseIndexBuilder AddGinIndexBuilder(string name, Action<TfGinDatabaseIndexBuilder> action = null)
    {
		_databaseBuilder.RegisterName(name);

		TfGinDatabaseIndexBuilder builder = new TfGinDatabaseIndexBuilder(name, _tableName, _databaseBuilder);

        if (action != null)
            action(builder);

        _builders.Add(builder);

        return builder;
    }

    #endregion

    #region <--- gist --->

    public TfDatabaseIndexCollectionBuilder AddGistIndex(string name, Action<TfGistDatabaseIndexBuilder> action)
    {
		_databaseBuilder.RegisterName(name);

		TfGistDatabaseIndexBuilder builder = new TfGistDatabaseIndexBuilder(name, _tableName, _databaseBuilder);

        action(builder);

        _builders.Add(builder);

        return this;
    }

    internal TfGistDatabaseIndexBuilder AddGistIndexBuilder(string name, Action<TfGistDatabaseIndexBuilder> action = null)
    {
		_databaseBuilder.RegisterName(name);

		TfGistDatabaseIndexBuilder builder = new TfGistDatabaseIndexBuilder(name, _tableName, _databaseBuilder);

        if (action != null)
            action(builder);

        _builders.Add(builder);

        return builder;
    }

    #endregion

    #region <--- add hash --->

    public TfDatabaseIndexCollectionBuilder AddHashIndex(string name, Action<TfHashDatabaseIndexBuilder> action)
    {
		_databaseBuilder.RegisterName(name);

		TfHashDatabaseIndexBuilder builder = new TfHashDatabaseIndexBuilder(name, _tableName, _databaseBuilder);

        action(builder);

        _builders.Add(builder);

        return this;
    }

    internal TfHashDatabaseIndexBuilder AddHashIndexBuilder(string name, Action<TfHashDatabaseIndexBuilder> action = null)
    {
		_databaseBuilder.RegisterName(name);

		TfHashDatabaseIndexBuilder builder = new TfHashDatabaseIndexBuilder(name, _tableName, _databaseBuilder);

        if (action != null)
            action(builder);

        _builders.Add(builder);

        return builder;
    }

    #endregion

    #region <--- remove --->

    public TfDatabaseIndexCollectionBuilder Remove(string name)
    {
        var builder = _builders.SingleOrDefault(x => x.Name == name);
        if (builder is null)
            throw new TfDatabaseBuilderException($"Index with name '{name}' is not found.");

		_databaseBuilder.UnregisterName(name);

		_builders.Remove(builder);

        return this;
    }

    #endregion

    #region <--- build --->

    internal TfDatabaseIndexCollection Build()
    {
        var collection = new TfDatabaseIndexCollection();

        foreach (var builder in _builders)
            collection.Add(builder.Build());

        return collection;
    }

    #endregion
}