namespace WebVella.Tefter.Database;

public class DbColumnCollectionBuilder
{
	private readonly List<DbColumnBuilder> _builders;
	private readonly DatabaseBuilder _databaseBuilder;
	private readonly string _tableName;

	internal ReadOnlyCollection<DbColumnBuilder> Builders => _builders.AsReadOnly();

	internal DbColumnCollectionBuilder(string tableName, DatabaseBuilder databaseBuilder)
	{
		_tableName = tableName;
		_builders = new List<DbColumnBuilder>();
		_databaseBuilder = databaseBuilder;
	}

	#region <--- auto increment --->

	public DbColumnCollectionBuilder AddAutoIncrementColumn(string name)
	{
		return AddAutoIncrementColumn(Guid.NewGuid(), name);
	}

	internal DbAutoIncrementColumnBuilder AddAutoIncrementColumnBuilder(string name)
	{
		return AddAutoIncrementColumnBuilder(Guid.NewGuid(), name);
	}

	public DbColumnCollectionBuilder AddAutoIncrementColumn(Guid id, string name)
	{
		_databaseBuilder.RegisterId(id);
		_databaseBuilder.RegisterColumnName(_tableName, name);

		DbAutoIncrementColumnBuilder builder = new DbAutoIncrementColumnBuilder(id, name, _databaseBuilder);

		_builders.Add(builder);

		return this;
	}

	internal DbAutoIncrementColumnBuilder AddAutoIncrementColumnBuilder(Guid id, string name)
	{
		_databaseBuilder.RegisterId(id);

		_databaseBuilder.RegisterColumnName(_tableName, name);

		DbAutoIncrementColumnBuilder builder = new DbAutoIncrementColumnBuilder(id, name, _databaseBuilder);

		_builders.Add(builder);

		return builder;
	}

	#endregion

	#region <--- guid --->

	public DbColumnCollectionBuilder AddGuidColumn(string name, Action<DbGuidColumnBuilder> action)
	{
		return AddGuidColumn(Guid.NewGuid(), name, action);
	}

	internal DbGuidColumnBuilder AddGuidColumnBuilder(string name, Action<DbGuidColumnBuilder> action = null)
	{
		return AddGuidColumnBuilder(Guid.NewGuid(), name, action);
	}

	public DbColumnCollectionBuilder AddGuidColumn(Guid id, string name, Action<DbGuidColumnBuilder> action)
	{
		_databaseBuilder.RegisterId(id);

		_databaseBuilder.RegisterColumnName(_tableName, name);

		DbGuidColumnBuilder builder = new DbGuidColumnBuilder(id, name, _databaseBuilder);

		if (action != null)
			action(builder);

		_builders.Add(builder);

		return this;
	}

	internal DbGuidColumnBuilder AddGuidColumnBuilder(Guid id, string name, Action<DbGuidColumnBuilder> action = null)
	{
		_databaseBuilder.RegisterId(id);

		_databaseBuilder.RegisterColumnName(_tableName, name);

		DbGuidColumnBuilder builder = new DbGuidColumnBuilder(id, name, _databaseBuilder);

		if (action != null)
			action(builder);

		_builders.Add(builder);

		return builder;
	}

	public DbColumnCollectionBuilder WithGuidColumn(string name, Action<DbGuidColumnBuilder> action)
	{
		var builder = (DbGuidColumnBuilder)_builders
			.SingleOrDefault(x => x.Name == name && x.GetType() == typeof(DbGuidColumnBuilder));

		if (builder is null)
			throw new DbBuilderException($"Column of type GUID and name '{name}' for table '{_tableName}' is not found.");

		if (action != null)
			action(builder);

		return this;
	}

	internal DbGuidColumnBuilder WithGuidColumnBuilder(string name, Action<DbGuidColumnBuilder> action = null)
	{
		var builder = (DbGuidColumnBuilder)_builders
			.SingleOrDefault(x => x.Name == name && x.GetType() == typeof(DbGuidColumnBuilder));

		if (builder is null)
			throw new DbBuilderException($"Column of type GUID and name '{name}' for table '{_tableName}' is not found.");

		if (action != null)
			action(builder);

		return builder;
	}


	#endregion

	#region <--- number --->

	public DbColumnCollectionBuilder AddNumberColumn(string name, Action<DbNumberColumnBuilder> action)
	{
		return AddNumberColumn(Guid.NewGuid(), name, action);
	}

	internal DbNumberColumnBuilder AddNumberColumnBuilder(string name, Action<DbNumberColumnBuilder> action = null)
	{
		return AddNumberColumnBuilder(Guid.NewGuid(), name, action);
	}

	public DbColumnCollectionBuilder AddNumberColumn(Guid id, string name, Action<DbNumberColumnBuilder> action)
	{
		_databaseBuilder.RegisterId(id);

		_databaseBuilder.RegisterColumnName(_tableName, name);

		DbNumberColumnBuilder builder = new DbNumberColumnBuilder(id, name, _databaseBuilder);

		if (action != null)
			action(builder);

		_builders.Add(builder);

		return this;
	}

	internal DbNumberColumnBuilder AddNumberColumnBuilder(Guid id, string name, Action<DbNumberColumnBuilder> action = null)
	{
		_databaseBuilder.RegisterId(id);

		_databaseBuilder.RegisterColumnName(_tableName, name);

		DbNumberColumnBuilder builder = new DbNumberColumnBuilder(id, name, _databaseBuilder);

		if (action != null)
			action(builder);

		_builders.Add(builder);

		return builder;
	}

	public DbColumnCollectionBuilder WithNumberColumn(string name, Action<DbNumberColumnBuilder> action)
	{
		var builder = (DbNumberColumnBuilder)_builders
			.SingleOrDefault(x => x.Name == name && x.GetType() == typeof(DbNumberColumnBuilder));

		if (builder is null)
			throw new DbBuilderException($"Column of type Number and name '{name}' for table '{_tableName}' is not found.");

		if (action != null)
			action(builder);

		return this;
	}

	internal DbNumberColumnBuilder WithNumberColumnBuilder(string name, Action<DbNumberColumnBuilder> action = null)
	{
		var builder = (DbNumberColumnBuilder)_builders
			.SingleOrDefault(x => x.Name == name && x.GetType() == typeof(DbNumberColumnBuilder));

		if (builder is null)
			throw new DbBuilderException($"Column of type Number and name '{name}' for table '{_tableName}' is not found.");

		if (action != null)
			action(builder);

		return builder;
	}

	#endregion

	#region <--- boolean --->

	public DbColumnCollectionBuilder AddBooleanColumn(string name, Action<DbBooleanColumnBuilder> action)
	{
		return AddBooleanColumn(Guid.NewGuid(), name, action);
	}

	internal DbBooleanColumnBuilder AddBooleanColumnBuilder(string name, Action<DbBooleanColumnBuilder> action = null)
	{
		return AddBooleanColumnBuilder(Guid.NewGuid(), name, action);
	}

	public DbColumnCollectionBuilder AddBooleanColumn(Guid id, string name, Action<DbBooleanColumnBuilder> action)
	{
		_databaseBuilder.RegisterId(id);

		_databaseBuilder.RegisterColumnName(_tableName, name);

		DbBooleanColumnBuilder builder = new DbBooleanColumnBuilder(id, name, _databaseBuilder);

		if (action != null)
			action(builder);

		_builders.Add(builder);

		return this;
	}

	internal DbBooleanColumnBuilder AddBooleanColumnBuilder(Guid id, string name, Action<DbBooleanColumnBuilder> action = null)
	{
		_databaseBuilder.RegisterId(id);

		_databaseBuilder.RegisterColumnName(_tableName, name);

		DbBooleanColumnBuilder builder = new DbBooleanColumnBuilder(id, name, _databaseBuilder);

		if (action != null)
			action(builder);

		_builders.Add(builder);

		return builder;
	}

	public DbColumnCollectionBuilder WithBooleanColumn(string name, Action<DbBooleanColumnBuilder> action)
	{
		var builder = (DbBooleanColumnBuilder)_builders
			.SingleOrDefault(x => x.Name == name && x.GetType() == typeof(DbBooleanColumnBuilder));

		if (builder is null)
			throw new DbBuilderException($"Column of type Boolean and name '{name}' for table '{_tableName}' is not found.");

		if (action != null)
			action(builder);

		return this;
	}

	internal DbBooleanColumnBuilder WithBooleanColumnBuilder(string name, Action<DbBooleanColumnBuilder> action = null)
	{
		var builder = (DbBooleanColumnBuilder)_builders
			.SingleOrDefault(x => x.Name == name && x.GetType() == typeof(DbBooleanColumnBuilder));

		if (builder is null)
			throw new DbBuilderException($"Column of type Boolean and name '{name}' for table '{_tableName}' is not found.");

		if (action != null)
			action(builder);

		return builder;
	}

	#endregion

	#region <--- date --->

	public DbColumnCollectionBuilder AddDateColumn(string name, Action<DbDateColumnBuilder> action)
	{
		return AddDateColumn(Guid.NewGuid(), name, action);
	}
	internal DbDateColumnBuilder AddDateColumnBuilder(string name, Action<DbDateColumnBuilder> action = null)
	{
		return AddDateColumnBuilder(Guid.NewGuid(), name, action);
	}

	public DbColumnCollectionBuilder AddDateColumn(Guid id, string name, Action<DbDateColumnBuilder> action)
	{
		_databaseBuilder.RegisterId(id);

		_databaseBuilder.RegisterColumnName(_tableName, name);

		DbDateColumnBuilder builder = new DbDateColumnBuilder(id, name, _databaseBuilder);

		if (action != null)
			action(builder);

		_builders.Add(builder);

		return this;
	}

	internal DbDateColumnBuilder AddDateColumnBuilder(Guid id, string name, Action<DbDateColumnBuilder> action = null)
	{
		_databaseBuilder.RegisterId(id);

		_databaseBuilder.RegisterColumnName(_tableName, name);

		DbDateColumnBuilder builder = new DbDateColumnBuilder(id, name, _databaseBuilder);

		if (action != null)
			action(builder);

		_builders.Add(builder);

		return builder;
	}

	public DbColumnCollectionBuilder WithDateColumn(string name, Action<DbDateColumnBuilder> action)
	{
		var builder = (DbDateColumnBuilder)_builders
			.SingleOrDefault(x => x.Name == name && x.GetType() == typeof(DbDateColumnBuilder));

		if (builder is null)
			throw new DbBuilderException($"Column of type Date and name '{name}' for table '{_tableName}' is not found.");

		if (action != null)
			action(builder);

		return this;
	}

	internal DbDateColumnBuilder WithDateColumnBuilder(string name, Action<DbDateColumnBuilder> action = null)
	{
		var builder = (DbDateColumnBuilder)_builders
			.SingleOrDefault(x => x.Name == name && x.GetType() == typeof(DbDateColumnBuilder));

		if (builder is null)
			throw new DbBuilderException($"Column of type Date and name '{name}' for table '{_tableName}' is not found.");

		if (action != null)
			action(builder);

		return builder;
	}

	#endregion

	#region <--- datetime --->

	public DbColumnCollectionBuilder AddDateTimeColumn(string name, Action<DbDateTimeColumnBuilder> action)
	{
		return AddDateTimeColumn(Guid.NewGuid(), name, action);
	}

	internal DbDateTimeColumnBuilder AddDateTimeColumnBuilder(string name, Action<DbDateTimeColumnBuilder> action = null)
	{
		return AddDateTimeColumnBuilder(Guid.NewGuid(), name, action);
	}

	public DbColumnCollectionBuilder AddDateTimeColumn(Guid id, string name, Action<DbDateTimeColumnBuilder> action)
	{
		_databaseBuilder.RegisterId(id);

		_databaseBuilder.RegisterColumnName(_tableName, name);

		DbDateTimeColumnBuilder builder = new DbDateTimeColumnBuilder(id, name, _databaseBuilder);

		if (action != null)
			action(builder);

		_builders.Add(builder);

		return this;
	}

	internal DbDateTimeColumnBuilder AddDateTimeColumnBuilder(Guid id, string name, Action<DbDateTimeColumnBuilder> action = null)
	{
		_databaseBuilder.RegisterId(id);

		_databaseBuilder.RegisterColumnName(_tableName, name);

		DbDateTimeColumnBuilder builder = new DbDateTimeColumnBuilder(id, name, _databaseBuilder);

		if (action != null)
			action(builder);

		_builders.Add(builder);

		return builder;
	}

	public DbColumnCollectionBuilder WithDateTimeColumn(string name, Action<DbDateTimeColumnBuilder> action)
	{
		var builder = (DbDateTimeColumnBuilder)_builders
			.SingleOrDefault(x => x.Name == name && x.GetType() == typeof(DbDateTimeColumnBuilder));

		if (builder is null)
			throw new DbBuilderException($"Column of type DateTime and name '{name}' for table '{_tableName}' is not found.");

		if (action != null)
			action(builder);

		return this;
	}

	internal DbDateTimeColumnBuilder WithDateTimeColumnBuilder(string name, Action<DbDateTimeColumnBuilder> action = null)
	{
		var builder = (DbDateTimeColumnBuilder)_builders
			.SingleOrDefault(x => x.Name == name && x.GetType() == typeof(DbDateTimeColumnBuilder));

		if (builder is null)
			throw new DbBuilderException($"Column of type DateTime and name '{name}' for table '{_tableName}' is not found.");

		if (action != null)
			action(builder);

		return builder;
	}

	#endregion

	#region <--- text --->

	public DbColumnCollectionBuilder AddTextColumn(string name, Action<DbTextColumnBuilder> action)
	{
		return AddTextColumn(Guid.NewGuid(), name, action);
	}

	internal DbTextColumnBuilder AddTextColumnBuilder(string name, Action<DbTextColumnBuilder> action = null)
	{
		return AddTextColumnBuilder(Guid.NewGuid(), name, action);
	}

	public DbColumnCollectionBuilder AddTextColumn(Guid id, string name, Action<DbTextColumnBuilder> action)
	{
		_databaseBuilder.RegisterId(id);

		_databaseBuilder.RegisterColumnName(_tableName, name);

		DbTextColumnBuilder builder = new DbTextColumnBuilder(id, name, _databaseBuilder);

		if (action != null)
			action(builder);

		_builders.Add(builder);

		return this;
	}

	internal DbTextColumnBuilder AddTextColumnBuilder(Guid id, string name, Action<DbTextColumnBuilder> action = null)
	{
		_databaseBuilder.RegisterId(id);

		_databaseBuilder.RegisterColumnName(_tableName, name);

		DbTextColumnBuilder builder = new DbTextColumnBuilder(id, name, _databaseBuilder);

		if (action != null)
			action(builder);

		_builders.Add(builder);

		return builder;
	}

	public DbColumnCollectionBuilder WithTextColumn(string name, Action<DbTextColumnBuilder> action)
	{
		var builder = (DbTextColumnBuilder)_builders
			.SingleOrDefault(x => x.Name == name && x.GetType() == typeof(DbTextColumnBuilder));

		if (builder is null)
			throw new DbBuilderException($"Column of type Text and name '{name}' for table '{_tableName}' is not found.");

		if (action != null)
			action(builder);

		return this;
	}

	internal DbTextColumnBuilder WithTextColumnBuilder(string name, Action<DbTextColumnBuilder> action = null)
	{
		var builder = (DbTextColumnBuilder)_builders
			.SingleOrDefault(x => x.Name == name && x.GetType() == typeof(DbTextColumnBuilder));

		if (builder is null)
			throw new DbBuilderException($"Column of type Text and name '{name}' for table '{_tableName}' is not found.");

		if (action != null)
			action(builder);

		return builder;
	}

	#endregion

	#region <=== remove ===>

	public DbColumnCollectionBuilder Remove(string name)
	{

		var builder = _builders.SingleOrDefault(x => x.Name == name);

		if (builder is null)
			throw new DbBuilderException($"Column with name '{name}' for table '{_tableName}' is not found.");

		_databaseBuilder.UnregisterId(builder.Id);

		_databaseBuilder.UnregisterColumnName(_tableName, builder.Name);

		_builders.Remove(builder);

		return this;
	}

	#endregion

	#region <--- build --->

	internal DbColumnCollection Build()
	{
		var collection = new DbColumnCollection();

		foreach (var builder in _builders)
			collection.Add(builder.Build());

		return collection;
	}

	#endregion
}