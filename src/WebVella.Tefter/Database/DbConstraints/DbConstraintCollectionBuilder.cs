namespace WebVella.Tefter.Database;

public class DbConstraintCollectionBuilder
{
    internal ReadOnlyCollection<DbConstraintBuilder> Builders => _builders.AsReadOnly();
    private readonly DbTableBuilder _tableBuilder;
    private readonly List<DbConstraintBuilder> _builders;
    
    internal DbConstraintCollectionBuilder(DbTableBuilder tableBuilder)
    {
        _builders = new List<DbConstraintBuilder>();
        _tableBuilder = tableBuilder; 
    }

    public DbConstraintCollectionBuilder AddNewUniqueConstraint(string name, Action<DbUniqueKeyConstraintBuilder> action)
    {
        DbUniqueKeyConstraintBuilder builder = new DbUniqueKeyConstraintBuilder(name, true, _tableBuilder);
        action(builder);
        _builders.Add(builder);
        return this;
    }

    public DbConstraintCollectionBuilder AddNewPrimaryKeyConstraint(string name, Action<DbPrimaryKeyConstraintBuilder> action)
    {
        DbPrimaryKeyConstraintBuilder builder = new DbPrimaryKeyConstraintBuilder(name, true, _tableBuilder);
        action(builder);
        _builders.Add(builder);
        return this;
    }

    public DbConstraintCollectionBuilder AddNewForeignKeyConstraint(string name, Action<DbForeignKeyConstraintBuilder> action)
    {
        DbForeignKeyConstraintBuilder builder = new DbForeignKeyConstraintBuilder(name, true, _tableBuilder);
        action(builder);
        _builders.Add(builder);
        return this;
    }

    internal DbConstraintCollection Build()
    {
        var collection = new DbConstraintCollection();

        foreach (var builder in _builders)
            collection.Add(builder.Build());

        return collection;
    }
}
