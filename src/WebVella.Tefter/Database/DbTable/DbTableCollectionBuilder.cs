﻿namespace WebVella.Tefter.Database;

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

    protected DbTableCollectionBuilder WithTables( DbTableCollection tables, Action<DbTableCollectionBuilder> action)
    {
        //TODO implement            
        action(this);
        return this;
    }

    public DbTableCollectionBuilder NewTable(Guid id, string name, Action<DbTableBuilder> action)
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

    public DbTableCollectionBuilder Remove(string name)
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

    internal DbTableCollectionBuilder InternalAddExistingTable(DbTable table)
    {
        if (table is null)
            throw new ArgumentNullException(nameof(table));

        _builders.Add(new DbTableBuilder(table));
        return this;
    }
}