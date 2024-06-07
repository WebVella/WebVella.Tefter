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

    #region <--- add unique --->

    public DbConstraintCollectionBuilder AddNewUniqueConstraint(string name, Action<DbUniqueKeyConstraintBuilder> action)
    {
        DbUniqueKeyConstraintBuilder builder = new DbUniqueKeyConstraintBuilder(name, _tableBuilder);

        action(builder);

        _builders.Add(builder);

        return this;
    }

    internal DbUniqueKeyConstraintBuilder AddNewUniqueConstraintBuilder(string name, Action<DbUniqueKeyConstraintBuilder> action = null)
    {
        DbUniqueKeyConstraintBuilder builder = new DbUniqueKeyConstraintBuilder(name, _tableBuilder);

        if(action != null) 
            action(builder);

        _builders.Add(builder);

        return builder;
    }

    #endregion

    #region <--- add primary --->

    public DbConstraintCollectionBuilder AddNewPrimaryKeyConstraint(string name, Action<DbPrimaryKeyConstraintBuilder> action)
    {
        DbPrimaryKeyConstraintBuilder builder = new DbPrimaryKeyConstraintBuilder(name, _tableBuilder);

        action(builder);

        _builders.Add(builder);

        return this;
    }

    internal DbPrimaryKeyConstraintBuilder AddNewPrimaryKeyConstraintBuilder(string name, Action<DbPrimaryKeyConstraintBuilder> action = null)
    {
        DbPrimaryKeyConstraintBuilder builder = new DbPrimaryKeyConstraintBuilder(name, _tableBuilder);

        if (action != null)
            action(builder);

        _builders.Add(builder);

        return builder;
    }

    #endregion

    #region <--- add foreign --->

    public DbConstraintCollectionBuilder AddNewForeignKeyConstraint(string name, Action<DbForeignKeyConstraintBuilder> action)
    {
        DbForeignKeyConstraintBuilder builder = new DbForeignKeyConstraintBuilder(name, _tableBuilder);

        action(builder);

        _builders.Add(builder);

        return this;
    }

    internal DbForeignKeyConstraintBuilder AddNewForeignKeyConstraintBuilder(string name, Action<DbForeignKeyConstraintBuilder> action = null)
    {
        DbForeignKeyConstraintBuilder builder = new DbForeignKeyConstraintBuilder(name, _tableBuilder);

        if(action != null)
            action(builder);

        _builders.Add(builder);

        return builder;
    }

    #endregion

    #region <--- remove --->

    public DbConstraintCollectionBuilder Remove(string name)
    {
        var builder = _builders.SingleOrDefault(x => x.Name == name);
        if (builder is null)
            throw new DbBuilderException($"Constraint with name '{name}' is not found.");

        _builders.Remove(builder);

        return this;
    }

    #endregion

    #region <--- build --->

    internal DbConstraintCollection Build()
    {
        var collection = new DbConstraintCollection();

        foreach (var builder in _builders)
            collection.Add(builder.Build());

        return collection;
    } 

    #endregion
}
