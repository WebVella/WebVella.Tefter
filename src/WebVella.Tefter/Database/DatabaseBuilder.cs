namespace WebVella.Tefter.Database;

public class DatabaseBuilder 
{
    private readonly List<DbTableBuilder> _builders;
    internal ReadOnlyCollection<DbTableBuilder> Builders => _builders.AsReadOnly();

    internal DatabaseBuilder()
    {
        _builders = new List<DbTableBuilder>();
    }

    public DatabaseBuilder WithTables(DbTableCollection tables)
    {
        foreach(var table in tables)
            InternalAddExistingTable(table);

        return this;
    }

    public DatabaseBuilder NewTable(Guid id, string name, Action<DbTableBuilder> action)
    {
        if (!DbUtility.IsValidDbObjectName(name, out string error))
            throw new DbBuilderException(error);

        var builder = (DbTableBuilder)_builders.SingleOrDefault(x => x.Name == name);

        if (builder is not null)
            throw new DbBuilderException($"Table with name '{name}' already exists. Only one instance can be created.");

        builder = new DbTableBuilder(id, name);
        action(builder);
        _builders.Add(builder);
        return this;
    }

    public DatabaseBuilder Remove(string name)
    {
        var builder = _builders.SingleOrDefault(x => x.Name == name);
        if (builder is null)
            throw new DbBuilderException($"Table with name '{name}' is not found.");

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

    internal DatabaseBuilder InternalAddExistingTable(DbTable table)
    {
        if (table is null)
            throw new ArgumentNullException(nameof(table));

        _builders.Add(new DbTableBuilder(table));
        return this;
    }
}
