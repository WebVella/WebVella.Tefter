namespace WebVella.Tefter.Database;

public class DatabaseColumnCollectionBuilder
{
	private readonly List<DatabaseColumnBuilder> _builders;
	private readonly DatabaseBuilder _databaseBuilder;
	private readonly string _tableName;

	internal ReadOnlyCollection<DatabaseColumnBuilder> Builders => _builders.AsReadOnly();

	internal DatabaseColumnCollectionBuilder(string tableName, DatabaseBuilder databaseBuilder)
	{
		_tableName = tableName;
		_builders = new List<DatabaseColumnBuilder>();
		_databaseBuilder = databaseBuilder;
	}

	#region <--- auto increment --->

	public DatabaseColumnCollectionBuilder AddAutoIncrementColumn(string name)
	{
		return AddAutoIncrementColumn(Guid.NewGuid(), name);
	}

	internal AutoIncrementDatabaseColumnBuilder AddAutoIncrementColumnBuilder(string name)
	{
		return AddAutoIncrementColumnBuilder(Guid.NewGuid(), name);
	}

	public DatabaseColumnCollectionBuilder AddAutoIncrementColumn(Guid id, string name)
	{
		_databaseBuilder.RegisterId(id);
		_databaseBuilder.RegisterColumnName(_tableName, name);

		AutoIncrementDatabaseColumnBuilder builder = new AutoIncrementDatabaseColumnBuilder(id, name, _databaseBuilder);

		_builders.Add(builder);

		return this;
	}

	internal AutoIncrementDatabaseColumnBuilder AddAutoIncrementColumnBuilder(Guid id, string name)
	{
		_databaseBuilder.RegisterId(id);

		_databaseBuilder.RegisterColumnName(_tableName, name);

		AutoIncrementDatabaseColumnBuilder builder = new AutoIncrementDatabaseColumnBuilder(id, name, _databaseBuilder);

		_builders.Add(builder);

		return builder;
	}

	#endregion

	#region <--- guid --->

	public DatabaseColumnCollectionBuilder AddGuidColumn(string name, Action<GuidDatabaseColumnBuilder> action)
	{
		return AddGuidColumn(Guid.NewGuid(), name, action);
	}

	internal GuidDatabaseColumnBuilder AddGuidColumnBuilder(string name, Action<GuidDatabaseColumnBuilder> action = null)
	{
		return AddGuidColumnBuilder(Guid.NewGuid(), name, action);
	}

	public DatabaseColumnCollectionBuilder AddGuidColumn(Guid id, string name, Action<GuidDatabaseColumnBuilder> action)
	{
		_databaseBuilder.RegisterId(id);

		_databaseBuilder.RegisterColumnName(_tableName, name);

		GuidDatabaseColumnBuilder builder = new GuidDatabaseColumnBuilder(id, name, _databaseBuilder);

		if (action != null)
			action(builder);

		_builders.Add(builder);

		return this;
	}

	internal GuidDatabaseColumnBuilder AddGuidColumnBuilder(Guid id, string name, Action<GuidDatabaseColumnBuilder> action = null)
	{
		_databaseBuilder.RegisterId(id);

		_databaseBuilder.RegisterColumnName(_tableName, name);

		GuidDatabaseColumnBuilder builder = new GuidDatabaseColumnBuilder(id, name, _databaseBuilder);

		if (action != null)
			action(builder);

		_builders.Add(builder);

		return builder;
	}

	public DatabaseColumnCollectionBuilder WithGuidColumn(string name, Action<GuidDatabaseColumnBuilder> action)
	{
		var builder = (GuidDatabaseColumnBuilder)_builders
			.SingleOrDefault(x => x.Name == name && x.GetType() == typeof(GuidDatabaseColumnBuilder));

		if (builder is null)
			throw new DatabaseBuilderException($"Column of type GUID and name '{name}' for table '{_tableName}' is not found.");

		if (action != null)
			action(builder);

		return this;
	}

	internal GuidDatabaseColumnBuilder WithGuidColumnBuilder(string name, Action<GuidDatabaseColumnBuilder> action = null)
	{
		var builder = (GuidDatabaseColumnBuilder)_builders
			.SingleOrDefault(x => x.Name == name && x.GetType() == typeof(GuidDatabaseColumnBuilder));

		if (builder is null)
			throw new DatabaseBuilderException($"Column of type GUID and name '{name}' for table '{_tableName}' is not found.");

		if (action != null)
			action(builder);

		return builder;
	}


	#endregion

	#region <--- number --->

	public DatabaseColumnCollectionBuilder AddNumberColumn(string name, Action<NumberDatabaseColumnBuilder> action)
	{
		return AddNumberColumn(Guid.NewGuid(), name, action);
	}

	internal NumberDatabaseColumnBuilder AddNumberColumnBuilder(string name, Action<NumberDatabaseColumnBuilder> action = null)
	{
		return AddNumberColumnBuilder(Guid.NewGuid(), name, action);
	}

	public DatabaseColumnCollectionBuilder AddNumberColumn(Guid id, string name, Action<NumberDatabaseColumnBuilder> action)
	{
		_databaseBuilder.RegisterId(id);

		_databaseBuilder.RegisterColumnName(_tableName, name);

		NumberDatabaseColumnBuilder builder = new NumberDatabaseColumnBuilder(id, name, _databaseBuilder);

		if (action != null)
			action(builder);

		_builders.Add(builder);

		return this;
	}

	internal NumberDatabaseColumnBuilder AddNumberColumnBuilder(Guid id, string name, Action<NumberDatabaseColumnBuilder> action = null)
	{
		_databaseBuilder.RegisterId(id);

		_databaseBuilder.RegisterColumnName(_tableName, name);

		NumberDatabaseColumnBuilder builder = new NumberDatabaseColumnBuilder(id, name, _databaseBuilder);

		if (action != null)
			action(builder);

		_builders.Add(builder);

		return builder;
	}

	public DatabaseColumnCollectionBuilder WithNumberColumn(string name, Action<NumberDatabaseColumnBuilder> action)
	{
		var builder = (NumberDatabaseColumnBuilder)_builders
			.SingleOrDefault(x => x.Name == name && x.GetType() == typeof(NumberDatabaseColumnBuilder));

		if (builder is null)
			throw new DatabaseBuilderException($"Column of type Number and name '{name}' for table '{_tableName}' is not found.");

		if (action != null)
			action(builder);

		return this;
	}

	internal NumberDatabaseColumnBuilder WithNumberColumnBuilder(string name, Action<NumberDatabaseColumnBuilder> action = null)
	{
		var builder = (NumberDatabaseColumnBuilder)_builders
			.SingleOrDefault(x => x.Name == name && x.GetType() == typeof(NumberDatabaseColumnBuilder));

		if (builder is null)
			throw new DatabaseBuilderException($"Column of type Number and name '{name}' for table '{_tableName}' is not found.");

		if (action != null)
			action(builder);

		return builder;
	}

	#endregion

	#region <--- boolean --->

	public DatabaseColumnCollectionBuilder AddBooleanColumn(string name, Action<BooleanDatabaseColumnBuilder> action)
	{
		return AddBooleanColumn(Guid.NewGuid(), name, action);
	}

	internal BooleanDatabaseColumnBuilder AddBooleanColumnBuilder(string name, Action<BooleanDatabaseColumnBuilder> action = null)
	{
		return AddBooleanColumnBuilder(Guid.NewGuid(), name, action);
	}

	public DatabaseColumnCollectionBuilder AddBooleanColumn(Guid id, string name, Action<BooleanDatabaseColumnBuilder> action)
	{
		_databaseBuilder.RegisterId(id);

		_databaseBuilder.RegisterColumnName(_tableName, name);

		BooleanDatabaseColumnBuilder builder = new BooleanDatabaseColumnBuilder(id, name, _databaseBuilder);

		if (action != null)
			action(builder);

		_builders.Add(builder);

		return this;
	}

	internal BooleanDatabaseColumnBuilder AddBooleanColumnBuilder(Guid id, string name, Action<BooleanDatabaseColumnBuilder> action = null)
	{
		_databaseBuilder.RegisterId(id);

		_databaseBuilder.RegisterColumnName(_tableName, name);

		BooleanDatabaseColumnBuilder builder = new BooleanDatabaseColumnBuilder(id, name, _databaseBuilder);

		if (action != null)
			action(builder);

		_builders.Add(builder);

		return builder;
	}

	public DatabaseColumnCollectionBuilder WithBooleanColumn(string name, Action<BooleanDatabaseColumnBuilder> action)
	{
		var builder = (BooleanDatabaseColumnBuilder)_builders
			.SingleOrDefault(x => x.Name == name && x.GetType() == typeof(BooleanDatabaseColumnBuilder));

		if (builder is null)
			throw new DatabaseBuilderException($"Column of type Boolean and name '{name}' for table '{_tableName}' is not found.");

		if (action != null)
			action(builder);

		return this;
	}

	internal BooleanDatabaseColumnBuilder WithBooleanColumnBuilder(string name, Action<BooleanDatabaseColumnBuilder> action = null)
	{
		var builder = (BooleanDatabaseColumnBuilder)_builders
			.SingleOrDefault(x => x.Name == name && x.GetType() == typeof(BooleanDatabaseColumnBuilder));

		if (builder is null)
			throw new DatabaseBuilderException($"Column of type Boolean and name '{name}' for table '{_tableName}' is not found.");

		if (action != null)
			action(builder);

		return builder;
	}

	#endregion

	#region <--- date --->

	public DatabaseColumnCollectionBuilder AddDateColumn(string name, Action<DateDatabaseColumnBuilder> action)
	{
		return AddDateColumn(Guid.NewGuid(), name, action);
	}
	internal DateDatabaseColumnBuilder AddDateColumnBuilder(string name, Action<DateDatabaseColumnBuilder> action = null)
	{
		return AddDateColumnBuilder(Guid.NewGuid(), name, action);
	}

	public DatabaseColumnCollectionBuilder AddDateColumn(Guid id, string name, Action<DateDatabaseColumnBuilder> action)
	{
		_databaseBuilder.RegisterId(id);

		_databaseBuilder.RegisterColumnName(_tableName, name);

		DateDatabaseColumnBuilder builder = new DateDatabaseColumnBuilder(id, name, _databaseBuilder);

		if (action != null)
			action(builder);

		_builders.Add(builder);

		return this;
	}

	internal DateDatabaseColumnBuilder AddDateColumnBuilder(Guid id, string name, Action<DateDatabaseColumnBuilder> action = null)
	{
		_databaseBuilder.RegisterId(id);

		_databaseBuilder.RegisterColumnName(_tableName, name);

		DateDatabaseColumnBuilder builder = new DateDatabaseColumnBuilder(id, name, _databaseBuilder);

		if (action != null)
			action(builder);

		_builders.Add(builder);

		return builder;
	}

	public DatabaseColumnCollectionBuilder WithDateColumn(string name, Action<DateDatabaseColumnBuilder> action)
	{
		var builder = (DateDatabaseColumnBuilder)_builders
			.SingleOrDefault(x => x.Name == name && x.GetType() == typeof(DateDatabaseColumnBuilder));

		if (builder is null)
			throw new DatabaseBuilderException($"Column of type Date and name '{name}' for table '{_tableName}' is not found.");

		if (action != null)
			action(builder);

		return this;
	}

	internal DateDatabaseColumnBuilder WithDateColumnBuilder(string name, Action<DateDatabaseColumnBuilder> action = null)
	{
		var builder = (DateDatabaseColumnBuilder)_builders
			.SingleOrDefault(x => x.Name == name && x.GetType() == typeof(DateDatabaseColumnBuilder));

		if (builder is null)
			throw new DatabaseBuilderException($"Column of type Date and name '{name}' for table '{_tableName}' is not found.");

		if (action != null)
			action(builder);

		return builder;
	}

	#endregion

	#region <--- datetime --->

	public DatabaseColumnCollectionBuilder AddDateTimeColumn(string name, Action<DateTimeDatabaseColumnBuilder> action)
	{
		return AddDateTimeColumn(Guid.NewGuid(), name, action);
	}

	internal DateTimeDatabaseColumnBuilder AddDateTimeColumnBuilder(string name, Action<DateTimeDatabaseColumnBuilder> action = null)
	{
		return AddDateTimeColumnBuilder(Guid.NewGuid(), name, action);
	}

	public DatabaseColumnCollectionBuilder AddDateTimeColumn(Guid id, string name, Action<DateTimeDatabaseColumnBuilder> action)
	{
		_databaseBuilder.RegisterId(id);

		_databaseBuilder.RegisterColumnName(_tableName, name);

		DateTimeDatabaseColumnBuilder builder = new DateTimeDatabaseColumnBuilder(id, name, _databaseBuilder);

		if (action != null)
			action(builder);

		_builders.Add(builder);

		return this;
	}

	internal DateTimeDatabaseColumnBuilder AddDateTimeColumnBuilder(Guid id, string name, Action<DateTimeDatabaseColumnBuilder> action = null)
	{
		_databaseBuilder.RegisterId(id);

		_databaseBuilder.RegisterColumnName(_tableName, name);

		DateTimeDatabaseColumnBuilder builder = new DateTimeDatabaseColumnBuilder(id, name, _databaseBuilder);

		if (action != null)
			action(builder);

		_builders.Add(builder);

		return builder;
	}

	public DatabaseColumnCollectionBuilder WithDateTimeColumn(string name, Action<DateTimeDatabaseColumnBuilder> action)
	{
		var builder = (DateTimeDatabaseColumnBuilder)_builders
			.SingleOrDefault(x => x.Name == name && x.GetType() == typeof(DateTimeDatabaseColumnBuilder));

		if (builder is null)
			throw new DatabaseBuilderException($"Column of type DateTime and name '{name}' for table '{_tableName}' is not found.");

		if (action != null)
			action(builder);

		return this;
	}

	internal DateTimeDatabaseColumnBuilder WithDateTimeColumnBuilder(string name, Action<DateTimeDatabaseColumnBuilder> action = null)
	{
		var builder = (DateTimeDatabaseColumnBuilder)_builders
			.SingleOrDefault(x => x.Name == name && x.GetType() == typeof(DateTimeDatabaseColumnBuilder));

		if (builder is null)
			throw new DatabaseBuilderException($"Column of type DateTime and name '{name}' for table '{_tableName}' is not found.");

		if (action != null)
			action(builder);

		return builder;
	}

	#endregion

	#region <--- text --->

	public DatabaseColumnCollectionBuilder AddTextColumn(string name, Action<TextDatabaseColumnBuilder> action)
	{
		return AddTextColumn(Guid.NewGuid(), name, action);
	}

	internal TextDatabaseColumnBuilder AddTextColumnBuilder(string name, Action<TextDatabaseColumnBuilder> action = null)
	{
		return AddTextColumnBuilder(Guid.NewGuid(), name, action);
	}

	public DatabaseColumnCollectionBuilder AddTextColumn(Guid id, string name, Action<TextDatabaseColumnBuilder> action)
	{
		_databaseBuilder.RegisterId(id);

		_databaseBuilder.RegisterColumnName(_tableName, name);

		TextDatabaseColumnBuilder builder = new TextDatabaseColumnBuilder(id, name, _databaseBuilder);

		if (action != null)
			action(builder);

		_builders.Add(builder);

		return this;
	}

	internal TextDatabaseColumnBuilder AddTextColumnBuilder(Guid id, string name, Action<TextDatabaseColumnBuilder> action = null)
	{
		_databaseBuilder.RegisterId(id);

		_databaseBuilder.RegisterColumnName(_tableName, name);

		TextDatabaseColumnBuilder builder = new TextDatabaseColumnBuilder(id, name, _databaseBuilder);

		if (action != null)
			action(builder);

		_builders.Add(builder);

		return builder;
	}

	public DatabaseColumnCollectionBuilder WithTextColumn(string name, Action<TextDatabaseColumnBuilder> action)
	{
		var builder = (TextDatabaseColumnBuilder)_builders
			.SingleOrDefault(x => x.Name == name && x.GetType() == typeof(TextDatabaseColumnBuilder));

		if (builder is null)
			throw new DatabaseBuilderException($"Column of type Text and name '{name}' for table '{_tableName}' is not found.");

		if (action != null)
			action(builder);

		return this;
	}

	internal TextDatabaseColumnBuilder WithTextColumnBuilder(string name, Action<TextDatabaseColumnBuilder> action = null)
	{
		var builder = (TextDatabaseColumnBuilder)_builders
			.SingleOrDefault(x => x.Name == name && x.GetType() == typeof(TextDatabaseColumnBuilder));

		if (builder is null)
			throw new DatabaseBuilderException($"Column of type Text and name '{name}' for table '{_tableName}' is not found.");

		if (action != null)
			action(builder);

		return builder;
	}

	#endregion

	#region <--- short text --->

	public DatabaseColumnCollectionBuilder AddShortTextColumn(string name, Action<ShortTextDatabaseColumnBuilder> action)
	{
		return AddShortTextColumn(Guid.NewGuid(), name, action);
	}

	internal ShortTextDatabaseColumnBuilder AddShortTextColumnBuilder(string name, Action<ShortTextDatabaseColumnBuilder> action = null)
	{
		return AddShortTextColumnBuilder(Guid.NewGuid(), name, action);
	}

	public DatabaseColumnCollectionBuilder AddShortTextColumn(Guid id, string name, Action<ShortTextDatabaseColumnBuilder> action)
	{
		_databaseBuilder.RegisterId(id);

		_databaseBuilder.RegisterColumnName(_tableName, name);

		ShortTextDatabaseColumnBuilder builder = new ShortTextDatabaseColumnBuilder(id, name, _databaseBuilder);

		if (action != null)
			action(builder);

		_builders.Add(builder);

		return this;
	}

	internal ShortTextDatabaseColumnBuilder AddShortTextColumnBuilder(Guid id, string name, Action<ShortTextDatabaseColumnBuilder> action = null)
	{
		_databaseBuilder.RegisterId(id);

		_databaseBuilder.RegisterColumnName(_tableName, name);

		ShortTextDatabaseColumnBuilder builder = new ShortTextDatabaseColumnBuilder(id, name, _databaseBuilder);

		if (action != null)
			action(builder);

		_builders.Add(builder);

		return builder;
	}

	public DatabaseColumnCollectionBuilder WithShortTextColumn(string name, Action<ShortTextDatabaseColumnBuilder> action)
	{
		var builder = (ShortTextDatabaseColumnBuilder)_builders
			.SingleOrDefault(x => x.Name == name && x.GetType() == typeof(ShortTextDatabaseColumnBuilder));

		if (builder is null)
			throw new DatabaseBuilderException($"Column of type ShortText and name '{name}' for table '{_tableName}' is not found.");

		if (action != null)
			action(builder);

		return this;
	}

	internal ShortTextDatabaseColumnBuilder WithShortTextColumnBuilder(string name, Action<ShortTextDatabaseColumnBuilder> action = null)
	{
		var builder = (ShortTextDatabaseColumnBuilder)_builders
			.SingleOrDefault(x => x.Name == name && x.GetType() == typeof(ShortTextDatabaseColumnBuilder));

		if (builder is null)
			throw new DatabaseBuilderException($"Column of type ShortText and name '{name}' for table '{_tableName}' is not found.");

		if (action != null)
			action(builder);

		return builder;
	}

	#endregion

	#region <--- short integer --->

	public DatabaseColumnCollectionBuilder AddShortIntegerColumn(string name, Action<ShortIntegerDatabaseColumnBuilder> action)
	{
		return AddShortIntegerColumn(Guid.NewGuid(), name, action);
	}

	internal ShortIntegerDatabaseColumnBuilder AddShortIntegerColumnBuilder(string name, Action<ShortIntegerDatabaseColumnBuilder> action = null)
	{
		return AddShortIntegerColumnBuilder(Guid.NewGuid(), name, action);
	}

	public DatabaseColumnCollectionBuilder AddShortIntegerColumn(Guid id, string name, Action<ShortIntegerDatabaseColumnBuilder> action)
	{
		_databaseBuilder.RegisterId(id);

		_databaseBuilder.RegisterColumnName(_tableName, name);

		ShortIntegerDatabaseColumnBuilder builder = new ShortIntegerDatabaseColumnBuilder(id, name, _databaseBuilder);

		if (action != null)
			action(builder);

		_builders.Add(builder);

		return this;
	}

	internal ShortIntegerDatabaseColumnBuilder AddShortIntegerColumnBuilder(Guid id, string name, Action<ShortIntegerDatabaseColumnBuilder> action = null)
	{
		_databaseBuilder.RegisterId(id);

		_databaseBuilder.RegisterColumnName(_tableName, name);

		ShortIntegerDatabaseColumnBuilder builder = new ShortIntegerDatabaseColumnBuilder(id, name, _databaseBuilder);

		if (action != null)
			action(builder);

		_builders.Add(builder);

		return builder;
	}

	public DatabaseColumnCollectionBuilder WithShortIntegerColumn(string name, Action<ShortIntegerDatabaseColumnBuilder> action)
	{
		var builder = (ShortIntegerDatabaseColumnBuilder)_builders
			.SingleOrDefault(x => x.Name == name && x.GetType() == typeof(ShortIntegerDatabaseColumnBuilder));

		if (builder is null)
			throw new DatabaseBuilderException($"Column of type ShortInteger and name '{name}' for table '{_tableName}' is not found.");

		if (action != null)
			action(builder);

		return this;
	}

	internal ShortIntegerDatabaseColumnBuilder WithShortIntegerColumnBuilder(string name, Action<ShortIntegerDatabaseColumnBuilder> action = null)
	{
		var builder = (ShortIntegerDatabaseColumnBuilder)_builders
			.SingleOrDefault(x => x.Name == name && x.GetType() == typeof(ShortIntegerDatabaseColumnBuilder));

		if (builder is null)
			throw new DatabaseBuilderException($"Column of type ShortInteger and name '{name}' for table '{_tableName}' is not found.");

		if (action != null)
			action(builder);

		return builder;
	}

	#endregion

	#region <--- integer --->

	public DatabaseColumnCollectionBuilder AddIntegerColumn(string name, Action<IntegerDatabaseColumnBuilder> action)
	{
		return AddIntegerColumn(Guid.NewGuid(), name, action);
	}

	internal IntegerDatabaseColumnBuilder AddIntegerColumnBuilder(string name, Action<IntegerDatabaseColumnBuilder> action = null)
	{
		return AddIntegerColumnBuilder(Guid.NewGuid(), name, action);
	}

	public DatabaseColumnCollectionBuilder AddIntegerColumn(Guid id, string name, Action<IntegerDatabaseColumnBuilder> action)
	{
		_databaseBuilder.RegisterId(id);

		_databaseBuilder.RegisterColumnName(_tableName, name);

		IntegerDatabaseColumnBuilder builder = new IntegerDatabaseColumnBuilder(id, name, _databaseBuilder);

		if (action != null)
			action(builder);

		_builders.Add(builder);

		return this;
	}

	internal IntegerDatabaseColumnBuilder AddIntegerColumnBuilder(Guid id, string name, Action<IntegerDatabaseColumnBuilder> action = null)
	{
		_databaseBuilder.RegisterId(id);

		_databaseBuilder.RegisterColumnName(_tableName, name);

		IntegerDatabaseColumnBuilder builder = new IntegerDatabaseColumnBuilder(id, name, _databaseBuilder);

		if (action != null)
			action(builder);

		_builders.Add(builder);

		return builder;
	}

	public DatabaseColumnCollectionBuilder WithIntegerColumn(string name, Action<IntegerDatabaseColumnBuilder> action)
	{
		var builder = (IntegerDatabaseColumnBuilder)_builders
			.SingleOrDefault(x => x.Name == name && x.GetType() == typeof(IntegerDatabaseColumnBuilder));

		if (builder is null)
			throw new DatabaseBuilderException($"Column of type Integer and name '{name}' for table '{_tableName}' is not found.");

		if (action != null)
			action(builder);

		return this;
	}

	internal IntegerDatabaseColumnBuilder WithIntegerColumnBuilder(string name, Action<IntegerDatabaseColumnBuilder> action = null)
	{
		var builder = (IntegerDatabaseColumnBuilder)_builders
			.SingleOrDefault(x => x.Name == name && x.GetType() == typeof(IntegerDatabaseColumnBuilder));

		if (builder is null)
			throw new DatabaseBuilderException($"Column of type Integer and name '{name}' for table '{_tableName}' is not found.");

		if (action != null)
			action(builder);

		return builder;
	}

	#endregion

	#region <--- long integer --->

	public DatabaseColumnCollectionBuilder AddLongIntegerColumn(string name, Action<LongIntegerDatabaseColumnBuilder> action)
	{
		return AddLongIntegerColumn(Guid.NewGuid(), name, action);
	}

	internal LongIntegerDatabaseColumnBuilder AddLongIntegerColumnBuilder(string name, Action<LongIntegerDatabaseColumnBuilder> action = null)
	{
		return AddLongIntegerColumnBuilder(Guid.NewGuid(), name, action);
	}

	public DatabaseColumnCollectionBuilder AddLongIntegerColumn(Guid id, string name, Action<LongIntegerDatabaseColumnBuilder> action)
	{
		_databaseBuilder.RegisterId(id);

		_databaseBuilder.RegisterColumnName(_tableName, name);

		LongIntegerDatabaseColumnBuilder builder = new LongIntegerDatabaseColumnBuilder(id, name, _databaseBuilder);

		if (action != null)
			action(builder);

		_builders.Add(builder);

		return this;
	}

	internal LongIntegerDatabaseColumnBuilder AddLongIntegerColumnBuilder(Guid id, string name, Action<LongIntegerDatabaseColumnBuilder> action = null)
	{
		_databaseBuilder.RegisterId(id);

		_databaseBuilder.RegisterColumnName(_tableName, name);

		LongIntegerDatabaseColumnBuilder builder = new LongIntegerDatabaseColumnBuilder(id, name, _databaseBuilder);

		if (action != null)
			action(builder);

		_builders.Add(builder);

		return builder;
	}

	public DatabaseColumnCollectionBuilder WithLongIntegerColumn(string name, Action<LongIntegerDatabaseColumnBuilder> action)
	{
		var builder = (LongIntegerDatabaseColumnBuilder)_builders
			.SingleOrDefault(x => x.Name == name && x.GetType() == typeof(LongIntegerDatabaseColumnBuilder));

		if (builder is null)
			throw new DatabaseBuilderException($"Column of type LongInteger and name '{name}' for table '{_tableName}' is not found.");

		if (action != null)
			action(builder);

		return this;
	}

	internal LongIntegerDatabaseColumnBuilder WithLongIntegerColumnBuilder(string name, Action<LongIntegerDatabaseColumnBuilder> action = null)
	{
		var builder = (LongIntegerDatabaseColumnBuilder)_builders
			.SingleOrDefault(x => x.Name == name && x.GetType() == typeof(LongIntegerDatabaseColumnBuilder));

		if (builder is null)
			throw new DatabaseBuilderException($"Column of type LongInteger and name '{name}' for table '{_tableName}' is not found.");

		if (action != null)
			action(builder);

		return builder;
	}

	#endregion

	#region <=== remove ===>

	public DatabaseColumnCollectionBuilder Remove(string name)
	{

		var builder = _builders.SingleOrDefault(x => x.Name == name);

		if (builder is null)
			throw new DatabaseBuilderException($"Column with name '{name}' for table '{_tableName}' is not found.");

		_databaseBuilder.UnregisterId(builder.Id);

		_databaseBuilder.UnregisterColumnName(_tableName, builder.Name);

		_builders.Remove(builder);

		return this;
	}

	#endregion

	#region <--- build --->

	internal DatabaseColumnCollection Build()
	{
		var collection = new DatabaseColumnCollection();

		foreach (var builder in _builders)
			collection.Add(builder.Build());

		return collection;
	}

	#endregion
}