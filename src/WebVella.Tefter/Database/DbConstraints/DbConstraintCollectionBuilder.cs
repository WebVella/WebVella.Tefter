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

    #region <=== Public Methods ===>

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

    #endregion

    #region <=== Build and Remove methods ===>

    public DbConstraintCollectionBuilder Remove(string name)
    {
        var builder = _builders.SingleOrDefault(x => x.Name == name);
        if (builder is null)
            throw new DbBuilderException($"Constraint with name '{name}' is not found.");

        _builders.Remove(builder);
        return this;
    }

    internal DbConstraintCollection Build()
    {
        var collection = new DbConstraintCollection();

        foreach (var builder in _builders)
            collection.Add(builder.Build());

        return collection;
    }

    #endregion

    #region <=== Internal Methods ==>

    //used for building new DbTable from existing instance

    internal DbConstraintCollectionBuilder InternalAddExistingConstraint(DbConstraint constraint)
    {
        if (constraint is null)
            throw new ArgumentNullException(nameof(constraint));

        if (constraint is DbPrimaryKeyConstraint)
            _builders.Add(new DbPrimaryKeyConstraintBuilder((DbPrimaryKeyConstraint)constraint, _tableBuilder));
        else if (constraint is DbForeignKeyConstraint)
            _builders.Add(new DbForeignKeyConstraintBuilder((DbForeignKeyConstraint)constraint, _tableBuilder));
        else if (constraint is DbUniqueKeyConstraint)
            _builders.Add(new DbUniqueKeyConstraintBuilder((DbUniqueKeyConstraint)constraint, _tableBuilder));
        else
            throw new DbBuilderException($"Not supported db constraint type {constraint.GetType()}");

        return this;
    }

    #endregion
}
