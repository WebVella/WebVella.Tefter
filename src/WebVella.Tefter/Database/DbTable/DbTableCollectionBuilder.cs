namespace WebVella.Tefter.Database;

public class DbTableCollectionBuilder
{
    private readonly List<DbTableBuilder> _builders;
    private readonly DatabaseBuilder _databaseBuilder;

    internal ReadOnlyCollection<DbTableBuilder> Builders => _builders.AsReadOnly();

    internal DbTableCollectionBuilder(DatabaseBuilder databaseBuilder)
    {
        _builders = new List<DbTableBuilder>();
        _databaseBuilder = databaseBuilder;
    }

    public DbTableCollectionBuilder NewTable(Guid id, string name, Action<DbTableBuilder> action)
    {
		_databaseBuilder.RegisterId(id);
		_databaseBuilder.RegisterName(name);
		
		var builder = (DbTableBuilder)_builders.SingleOrDefault(x => x.Name == name);

        if (builder is not null)
            throw new DbBuilderException($"Table with name '{name}' already exists. Only one instance can be created.");

        builder = new DbTableBuilder(id, name, _databaseBuilder);

        action(builder);

        _builders.Add(builder);

        return this;
    }

    internal DbTableBuilder NewTableBuilder(Guid id, string name, Action<DbTableBuilder> action = null)
    {
		_databaseBuilder.RegisterId(id);
		_databaseBuilder.RegisterName(name);

		var builder = (DbTableBuilder)_builders.SingleOrDefault(x => x.Name == name);

        if (builder is not null)
            throw new DbBuilderException($"Table with name '{name}' already exists. Only one instance can be created.");

        builder = new DbTableBuilder(id, name, _databaseBuilder);

        if (action != null)
            action(builder);

        _builders.Add(builder);

        return builder;
    }

    public DbTableCollectionBuilder Remove(string name)
    {
        var builder = _builders.SingleOrDefault(x => x.Name == name);
        if (builder is null)
            throw new DbBuilderException($"Table with name '{name}' is not found.");

		_databaseBuilder.UnregisterId(builder.Id);

		_databaseBuilder.UnregisterName(builder.Name);

		_builders.Remove(builder);

        return this;
    }

    internal DbTableCollection Build()
    {
        var collection = new DbTableCollection();

        foreach (var builder in _builders)
            collection.Add(builder.Build());

        return collection;
    }
}