namespace WebVella.Tefter.Database;

public class DbConstraintCollectionBuilder
{
    internal ReadOnlyCollection<DbConstraintBuilder> Builders => _builders.AsReadOnly();
    private readonly DatabaseBuilder _databaseBuilder;
    private readonly List<DbConstraintBuilder> _builders;
    private readonly string _tableName;

    internal DbConstraintCollectionBuilder(string tableName, DatabaseBuilder databaseBuilder)
    {
        _tableName = tableName;
        _builders = new List<DbConstraintBuilder>();
        _databaseBuilder = databaseBuilder;
    }

    #region <--- add unique --->

    public DbConstraintCollectionBuilder AddUniqueKeyConstraint(string name, Action<DbUniqueKeyConstraintBuilder> action)
    {
        DbUniqueKeyConstraintBuilder builder = new DbUniqueKeyConstraintBuilder(name, _databaseBuilder);

        action(builder);

        _builders.Add(builder);

        return this;
    }

    internal DbUniqueKeyConstraintBuilder AddUniqueKeyConstraintBuilder(string name, Action<DbUniqueKeyConstraintBuilder> action = null)
    {
        DbUniqueKeyConstraintBuilder builder = new DbUniqueKeyConstraintBuilder(name, _databaseBuilder);

        if(action != null) 
            action(builder);

        _builders.Add(builder);

        return builder;
    }

    #endregion

    #region <--- add primary --->

    public DbConstraintCollectionBuilder AddPrimaryKeyConstraint(string name, Action<DbPrimaryKeyConstraintBuilder> action)
    {
        DbPrimaryKeyConstraintBuilder builder = new DbPrimaryKeyConstraintBuilder(name, _databaseBuilder);

        action(builder);

        _builders.Add(builder);

        return this;
    }

    internal DbPrimaryKeyConstraintBuilder AddPrimaryKeyConstraintBuilder(string name, Action<DbPrimaryKeyConstraintBuilder> action = null)
    {
        DbPrimaryKeyConstraintBuilder builder = new DbPrimaryKeyConstraintBuilder(name, _databaseBuilder);

        if (action != null)
            action(builder);

        _builders.Add(builder);

        return builder;
    }

    #endregion

    #region <--- add foreign --->

    public DbConstraintCollectionBuilder AddForeignKeyConstraint(string name, Action<DbForeignKeyConstraintBuilder> action)
    {
        DbForeignKeyConstraintBuilder builder = new DbForeignKeyConstraintBuilder(name, _databaseBuilder);

        action(builder);

        _builders.Add(builder);

        return this;
    }

    internal DbForeignKeyConstraintBuilder AddForeignKeyConstraintBuilder(string name, Action<DbForeignKeyConstraintBuilder> action = null)
    {
        DbForeignKeyConstraintBuilder builder = new DbForeignKeyConstraintBuilder(name, _databaseBuilder);

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
