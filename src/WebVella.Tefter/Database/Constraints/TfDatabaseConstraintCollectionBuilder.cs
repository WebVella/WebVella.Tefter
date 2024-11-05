namespace WebVella.Tefter.Database;

public class TfDatabaseConstraintCollectionBuilder
{
    internal ReadOnlyCollection<TfDatabaseConstraintBuilder> Builders => _builders.AsReadOnly();
    private readonly TfDatabaseBuilder _databaseBuilder;
    private readonly List<TfDatabaseConstraintBuilder> _builders;
    private readonly string _tableName;

    internal TfDatabaseConstraintCollectionBuilder(string tableName, TfDatabaseBuilder databaseBuilder)
    {
        _tableName = tableName;
        _builders = new List<TfDatabaseConstraintBuilder>();
        _databaseBuilder = databaseBuilder;
    }

    #region <--- add unique --->

    public TfDatabaseConstraintCollectionBuilder AddUniqueKeyConstraint(string name, Action<TfDatabaseUniqueKeyConstraintBuilder> action)
    {
		_databaseBuilder.RegisterName(name);

        TfDatabaseUniqueKeyConstraintBuilder builder = new TfDatabaseUniqueKeyConstraintBuilder(name, _databaseBuilder);

        action(builder);

        _builders.Add(builder);

        return this;
    }

    internal TfDatabaseUniqueKeyConstraintBuilder AddUniqueKeyConstraintBuilder(string name, Action<TfDatabaseUniqueKeyConstraintBuilder> action = null)
    {
		_databaseBuilder.RegisterName(name);

		TfDatabaseUniqueKeyConstraintBuilder builder = new TfDatabaseUniqueKeyConstraintBuilder(name, _databaseBuilder);

        if(action != null) 
            action(builder);

        _builders.Add(builder);

        return builder;
    }

    #endregion

    #region <--- add primary --->

    public TfDatabaseConstraintCollectionBuilder AddPrimaryKeyConstraint(string name, Action<TfDatabasePrimaryKeyConstraintBuilder> action)
    {
		_databaseBuilder.RegisterName(name);

		TfDatabasePrimaryKeyConstraintBuilder builder = new TfDatabasePrimaryKeyConstraintBuilder(name, _databaseBuilder);

        action(builder);

        _builders.Add(builder);

        return this;
    }

    internal TfDatabasePrimaryKeyConstraintBuilder AddPrimaryKeyConstraintBuilder(string name, Action<TfDatabasePrimaryKeyConstraintBuilder> action = null)
    {
		_databaseBuilder.RegisterName(name);

		TfDatabasePrimaryKeyConstraintBuilder builder = new TfDatabasePrimaryKeyConstraintBuilder(name, _databaseBuilder);

        if (action != null)
            action(builder);

        _builders.Add(builder);

        return builder;
    }

    #endregion

    #region <--- add foreign --->

    public TfDatabaseConstraintCollectionBuilder AddForeignKeyConstraint(string name, Action<TfDatabaseForeignKeyConstraintBuilder> action)
    {
		_databaseBuilder.RegisterName(name);

		TfDatabaseForeignKeyConstraintBuilder builder = new TfDatabaseForeignKeyConstraintBuilder(name, _databaseBuilder);

        action(builder);

        _builders.Add(builder);

        return this;
    }

    internal TfDatabaseForeignKeyConstraintBuilder AddForeignKeyConstraintBuilder(string name, Action<TfDatabaseForeignKeyConstraintBuilder> action = null)
    {
		_databaseBuilder.RegisterName(name);

		TfDatabaseForeignKeyConstraintBuilder builder = new TfDatabaseForeignKeyConstraintBuilder(name, _databaseBuilder);

        if(action != null)
            action(builder);

        _builders.Add(builder);

        return builder;
    }

    #endregion

    #region <--- remove --->

    public TfDatabaseConstraintCollectionBuilder Remove(string name)
    {
        var builder = _builders.SingleOrDefault(x => x.Name == name);
        if (builder is null)
            throw new TfDatabaseBuilderException($"Constraint with name '{name}' is not found.");

		_databaseBuilder.UnregisterName(builder.Name);

		_builders.Remove(builder);

        return this;
    }

    #endregion

    #region <--- build --->

    internal TfDatabaseConstraintCollection Build()
    {
        var collection = new TfDatabaseConstraintCollection();

        foreach (var builder in _builders)
            collection.Add(builder.Build());

        return collection;
    } 

    #endregion
}
