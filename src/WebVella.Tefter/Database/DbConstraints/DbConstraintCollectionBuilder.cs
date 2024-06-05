namespace WebVella.Tefter.Database;

public class DbConstraintCollectionBuilder
{
    private readonly List<DbConstraintBuilder> _builders = new List<DbConstraintBuilder>();

    public DbConstraintCollectionBuilder AddUniqueConstraint(Action<DbUniqueKeyConstraintBuilder> action)
    {
        DbUniqueKeyConstraintBuilder builder = new DbUniqueKeyConstraintBuilder();
        action(builder);
        _builders.Add(builder);
        return this;
    }

    public DbConstraintCollectionBuilder AddPrimaryKeyConstraint(Action<DbPrimaryKeyConstraintBuilder> action)
    {
        DbPrimaryKeyConstraintBuilder builder = new DbPrimaryKeyConstraintBuilder();
        action(builder);
        _builders.Add(builder);
        return this;
    }

    public DbConstraintCollectionBuilder AddForeignKeyConstraint(Action<DbForeignKeyConstraintBuilder> action)
    {
        DbForeignKeyConstraintBuilder builder = new DbForeignKeyConstraintBuilder();
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
