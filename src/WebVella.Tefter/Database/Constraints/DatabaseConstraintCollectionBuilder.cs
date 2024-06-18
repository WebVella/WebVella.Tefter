namespace WebVella.Tefter.Database;

public class DatabaseConstraintCollectionBuilder
{
    internal ReadOnlyCollection<DatabaseConstraintBuilder> Builders => _builders.AsReadOnly();
    private readonly DatabaseBuilder _databaseBuilder;
    private readonly List<DatabaseConstraintBuilder> _builders;
    private readonly string _tableName;

    internal DatabaseConstraintCollectionBuilder(string tableName, DatabaseBuilder databaseBuilder)
    {
        _tableName = tableName;
        _builders = new List<DatabaseConstraintBuilder>();
        _databaseBuilder = databaseBuilder;
    }

    #region <--- add unique --->

    public DatabaseConstraintCollectionBuilder AddUniqueKeyConstraint(string name, Action<DatabaseUniqueKeyConstraintBuilder> action)
    {
		_databaseBuilder.RegisterName(name);

        DatabaseUniqueKeyConstraintBuilder builder = new DatabaseUniqueKeyConstraintBuilder(name, _databaseBuilder);

        action(builder);

        _builders.Add(builder);

        return this;
    }

    internal DatabaseUniqueKeyConstraintBuilder AddUniqueKeyConstraintBuilder(string name, Action<DatabaseUniqueKeyConstraintBuilder> action = null)
    {
		_databaseBuilder.RegisterName(name);

		DatabaseUniqueKeyConstraintBuilder builder = new DatabaseUniqueKeyConstraintBuilder(name, _databaseBuilder);

        if(action != null) 
            action(builder);

        _builders.Add(builder);

        return builder;
    }

    #endregion

    #region <--- add primary --->

    public DatabaseConstraintCollectionBuilder AddPrimaryKeyConstraint(string name, Action<DatabasePrimaryKeyConstraintBuilder> action)
    {
		_databaseBuilder.RegisterName(name);

		DatabasePrimaryKeyConstraintBuilder builder = new DatabasePrimaryKeyConstraintBuilder(name, _databaseBuilder);

        action(builder);

        _builders.Add(builder);

        return this;
    }

    internal DatabasePrimaryKeyConstraintBuilder AddPrimaryKeyConstraintBuilder(string name, Action<DatabasePrimaryKeyConstraintBuilder> action = null)
    {
		_databaseBuilder.RegisterName(name);

		DatabasePrimaryKeyConstraintBuilder builder = new DatabasePrimaryKeyConstraintBuilder(name, _databaseBuilder);

        if (action != null)
            action(builder);

        _builders.Add(builder);

        return builder;
    }

    #endregion

    #region <--- add foreign --->

    public DatabaseConstraintCollectionBuilder AddForeignKeyConstraint(string name, Action<DatabaseForeignKeyConstraintBuilder> action)
    {
		_databaseBuilder.RegisterName(name);

		DatabaseForeignKeyConstraintBuilder builder = new DatabaseForeignKeyConstraintBuilder(name, _databaseBuilder);

        action(builder);

        _builders.Add(builder);

        return this;
    }

    internal DatabaseForeignKeyConstraintBuilder AddForeignKeyConstraintBuilder(string name, Action<DatabaseForeignKeyConstraintBuilder> action = null)
    {
		_databaseBuilder.RegisterName(name);

		DatabaseForeignKeyConstraintBuilder builder = new DatabaseForeignKeyConstraintBuilder(name, _databaseBuilder);

        if(action != null)
            action(builder);

        _builders.Add(builder);

        return builder;
    }

    #endregion

    #region <--- remove --->

    public DatabaseConstraintCollectionBuilder Remove(string name)
    {
        var builder = _builders.SingleOrDefault(x => x.Name == name);
        if (builder is null)
            throw new DatabaseBuilderException($"Constraint with name '{name}' is not found.");

		_databaseBuilder.UnregisterName(builder.Name);

		_builders.Remove(builder);

        return this;
    }

    #endregion

    #region <--- build --->

    internal DatabaseConstraintCollection Build()
    {
        var collection = new DatabaseConstraintCollection();

        foreach (var builder in _builders)
            collection.Add(builder.Build());

        return collection;
    } 

    #endregion
}
