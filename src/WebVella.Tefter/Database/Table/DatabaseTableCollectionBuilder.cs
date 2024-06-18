namespace WebVella.Tefter.Database;

public class DatabaseTableCollectionBuilder
{
    private readonly List<DatabaseTableBuilder> _builders;
    private readonly DatabaseBuilder _databaseBuilder;

    internal ReadOnlyCollection<DatabaseTableBuilder> Builders => _builders.AsReadOnly();

    internal DatabaseTableCollectionBuilder(DatabaseBuilder databaseBuilder)
    {
        _builders = new List<DatabaseTableBuilder>();
        _databaseBuilder = databaseBuilder;
    }

    public DatabaseTableCollectionBuilder NewTable(Guid id, string name, Action<DatabaseTableBuilder> action)
    {
		_databaseBuilder.RegisterId(id);
		_databaseBuilder.RegisterName(name);
		
		var builder = (DatabaseTableBuilder)_builders.SingleOrDefault(x => x.Name == name);

        if (builder is not null)
            throw new DatabaseBuilderException($"Table with name '{name}' already exists. Only one instance can be created.");

        builder = new DatabaseTableBuilder(id, name, _databaseBuilder);

        action(builder);

        _builders.Add(builder);

        return this;
    }

    internal DatabaseTableBuilder NewTableBuilder(Guid id, string name, Action<DatabaseTableBuilder> action = null)
    {
		_databaseBuilder.RegisterId(id);
		_databaseBuilder.RegisterName(name);

		var builder = (DatabaseTableBuilder)_builders.SingleOrDefault(x => x.Name == name);

        if (builder is not null)
            throw new DatabaseBuilderException($"Table with name '{name}' already exists. Only one instance can be created.");

        builder = new DatabaseTableBuilder(id, name, _databaseBuilder);

        if (action != null)
            action(builder);

        _builders.Add(builder);

        return builder;
    }

    public DatabaseTableCollectionBuilder Remove(string name)
    {
        var builder = _builders.SingleOrDefault(x => x.Name == name);
        if (builder is null)
            throw new DatabaseBuilderException($"Table with name '{name}' is not found.");

		_databaseBuilder.UnregisterId(builder.Id);

		_databaseBuilder.UnregisterName(builder.Name);

		_builders.Remove(builder);

        return this;
    }

    internal DatabaseTableCollection Build()
    {
        var collection = new DatabaseTableCollection();

        foreach (var builder in _builders)
            collection.Add(builder.Build());

        return collection;
    }
}