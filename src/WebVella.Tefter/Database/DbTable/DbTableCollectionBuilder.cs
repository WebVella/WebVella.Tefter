namespace WebVella.Tefter.Database;

public class DbTableCollectionBuilder
{
    private readonly List<DbTableBuilder> _builders;
    private IDbManager _dbManager;

    internal ReadOnlyCollection<DbTableBuilder> Builders => _builders.AsReadOnly();

    public DbTableCollectionBuilder(IDbManager dbManager)
    {
        _builders = new List<DbTableBuilder>();
        _dbManager = dbManager;
    }

    public DbTableCollectionBuilder NewTable(Guid id, string name, Action<DbTableBuilder> action)
    {
        if (!DbUtility.IsValidDbObjectName(name, out string error))
            throw new DbBuilderException(error);

        var builder = (DbTableBuilder)_builders.SingleOrDefault(x => x.Name == name);

        if (builder is not null)
            throw new DbBuilderException($"Table with name '{name}' already exists in columns. Only one instance can be created.");

        builder = new DbTableBuilder(id, name);
        action(builder);
        _builders.Add(builder);
        return this;
    }

    public DbTableCollectionBuilder Remove(string name)
    {
        var builder = _builders.SingleOrDefault(x => x.Name == name && x.State != DbObjectState.Removed);
        if (builder is null)
            throw new DbBuilderException($"Table with name '{name}' is not found.");

        if (builder.State != DbObjectState.New)
            builder.State = DbObjectState.Removed;
        else
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

    internal DbTableCollectionBuilder InternalAddExistingTable(DbTable table)
    {
        if (table is null)
            throw new ArgumentNullException(nameof(table));

        _builders.Add(new DbTableBuilder(table));
        return this;
    }
}