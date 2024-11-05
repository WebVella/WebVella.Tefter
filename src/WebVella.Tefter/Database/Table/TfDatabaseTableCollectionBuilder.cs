namespace WebVella.Tefter.Database;

public class TfDatabaseTableCollectionBuilder
{
    private readonly List<TfDatabaseTableBuilder> _builders;
    private readonly TfDatabaseBuilder _databaseBuilder;

    internal ReadOnlyCollection<TfDatabaseTableBuilder> Builders => _builders.AsReadOnly();

    internal TfDatabaseTableCollectionBuilder(TfDatabaseBuilder databaseBuilder)
    {
        _builders = new List<TfDatabaseTableBuilder>();
        _databaseBuilder = databaseBuilder;
    }

    public TfDatabaseTableCollectionBuilder NewTable(Guid id, string name, Action<TfDatabaseTableBuilder> action)
    {
		_databaseBuilder.RegisterId(id);
		_databaseBuilder.RegisterName(name);
		
		var builder = (TfDatabaseTableBuilder)_builders.SingleOrDefault(x => x.Name == name);

        if (builder is not null)
            throw new TfDatabaseBuilderException($"Table with name '{name}' already exists. Only one instance can be created.");

        builder = new TfDatabaseTableBuilder(id, name, _databaseBuilder);

        action(builder);

        _builders.Add(builder);

        return this;
    }

    internal TfDatabaseTableBuilder NewTableBuilder(Guid id, string name, Action<TfDatabaseTableBuilder> action = null)
    {
		_databaseBuilder.RegisterId(id);
		_databaseBuilder.RegisterName(name);

		var builder = (TfDatabaseTableBuilder)_builders.SingleOrDefault(x => x.Name == name);

        if (builder is not null)
            throw new TfDatabaseBuilderException($"Table with name '{name}' already exists. Only one instance can be created.");

        builder = new TfDatabaseTableBuilder(id, name, _databaseBuilder);

        if (action != null)
            action(builder);

        _builders.Add(builder);

        return builder;
    }

    public TfDatabaseTableCollectionBuilder Remove(string name)
    {
        var builder = _builders.SingleOrDefault(x => x.Name == name);
        if (builder is null)
            throw new TfDatabaseBuilderException($"Table with name '{name}' is not found.");

		_databaseBuilder.UnregisterId(builder.Id);

		_databaseBuilder.UnregisterName(builder.Name);

		_builders.Remove(builder);

        return this;
    }

    internal TfDatabaseTableCollection Build()
    {
        var collection = new TfDatabaseTableCollection();

        foreach (var builder in _builders)
            collection.Add(builder.Build());

        return collection;
    }
}