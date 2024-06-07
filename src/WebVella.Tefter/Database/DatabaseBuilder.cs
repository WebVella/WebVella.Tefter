namespace WebVella.Tefter.Database;

public class DatabaseBuilder 
{
    private readonly List<DbTableBuilder> _builders;
    internal ReadOnlyCollection<DbTableBuilder> Builders => _builders.AsReadOnly();

    private DatabaseBuilder(DbTableCollection tables = null)
    {
        _builders = new List<DbTableBuilder>();
    }

    public static DatabaseBuilder New(DbTableCollection tables = null)
    {
        var builder = new DatabaseBuilder();
        
        if (tables != null)
        {
            foreach (var table in tables)
                builder.InternalAddExistingTable(table);

        }

        return builder;
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

    public DbTableBuilder NewTableBuilder(Guid id, string name)
    {
        if (!DbUtility.IsValidDbObjectName(name, out string error))
            throw new DbBuilderException(error);

        var builder = (DbTableBuilder)_builders.SingleOrDefault(x => x.Name == name);

        if (builder is not null)
            throw new DbBuilderException($"Table with name '{name}' already exists. Only one instance can be created.");

        builder = new DbTableBuilder(id, name);
        _builders.Add(builder);
        return builder;
    }

    public DatabaseBuilder Remove(string name)
    {
        var builder = _builders.SingleOrDefault(x => x.Name == name);
        if (builder is null)
            throw new DbBuilderException($"Table with name '{name}' is not found.");

        _builders.Remove(builder);

        return this;
    }

    public DbTableCollection Build()
    {
        var collection = new DbTableCollection();

        foreach (var builder in _builders)
            collection.Add(builder.Build());

        return collection;
    }

    private DatabaseBuilder InternalAddExistingTable(DbTable table)
    {
        if (table is null)
            throw new ArgumentNullException(nameof(table));

        _builders.Add(new DbTableBuilder(table));
        return this;
    }
}
