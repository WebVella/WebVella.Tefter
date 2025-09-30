namespace WebVella.Tefter.Database;

public class TfDatabaseColumnCollectionBuilder
{
	private readonly List<TfDatabaseColumnBuilder> _builders;
	private readonly TfDatabaseBuilder _databaseBuilder;
	private readonly string _tableName;

	internal ReadOnlyCollection<TfDatabaseColumnBuilder> Builders => _builders.AsReadOnly();

	internal TfDatabaseColumnCollectionBuilder(string tableName, TfDatabaseBuilder databaseBuilder)
	{
		_tableName = tableName;
		_builders = new List<TfDatabaseColumnBuilder>();
		_databaseBuilder = databaseBuilder;
	}

	#region <--- guid --->

	public TfDatabaseColumnCollectionBuilder AddGuidColumn(string name, Action<TfGuidDatabaseColumnBuilder> action)
	{
		return AddGuidColumn(Guid.NewGuid(), name, action);
	}

	internal TfGuidDatabaseColumnBuilder AddGuidColumnBuilder(string name, Action<TfGuidDatabaseColumnBuilder> action = null)
	{
		return AddGuidColumnBuilder(Guid.NewGuid(), name, action);
	}

	public TfDatabaseColumnCollectionBuilder AddGuidColumn(Guid id, string name, Action<TfGuidDatabaseColumnBuilder> action)
	{
		_databaseBuilder.RegisterId(id);

		_databaseBuilder.RegisterColumnName(_tableName, name);

		TfGuidDatabaseColumnBuilder builder = new TfGuidDatabaseColumnBuilder(id, name, _databaseBuilder);

		if (action != null)
			action(builder);

		_builders.Add(builder);

		return this;
	}

	internal TfGuidDatabaseColumnBuilder AddGuidColumnBuilder(Guid id, string name, Action<TfGuidDatabaseColumnBuilder> action = null)
	{
		_databaseBuilder.RegisterId(id);

		_databaseBuilder.RegisterColumnName(_tableName, name);

		TfGuidDatabaseColumnBuilder builder = new TfGuidDatabaseColumnBuilder(id, name, _databaseBuilder);

		if (action != null)
			action(builder);

		_builders.Add(builder);

		return builder;
	}

	public TfDatabaseColumnCollectionBuilder WithGuidColumn(string name, Action<TfGuidDatabaseColumnBuilder> action)
	{
		var builder = (TfGuidDatabaseColumnBuilder)_builders
			.SingleOrDefault(x => x.Name == name && x.GetType() == typeof(TfGuidDatabaseColumnBuilder));

		if (builder is null)
			throw new TfDatabaseBuilderException($"Column of type GUID and name '{name}' for table '{_tableName}' is not found.");

		if (action != null)
			action(builder);

		return this;
	}

	internal TfGuidDatabaseColumnBuilder WithGuidColumnBuilder(string name, Action<TfGuidDatabaseColumnBuilder> action = null)
	{
		var builder = (TfGuidDatabaseColumnBuilder)_builders
			.SingleOrDefault(x => x.Name == name && x.GetType() == typeof(TfGuidDatabaseColumnBuilder));

		if (builder is null)
			throw new TfDatabaseBuilderException($"Column of type GUID and name '{name}' for table '{_tableName}' is not found.");

		if (action != null)
			action(builder);

		return builder;
	}


	#endregion

	#region <--- number --->

	public TfDatabaseColumnCollectionBuilder AddNumberColumn(string name, Action<TfNumberDatabaseColumnBuilder> action)
	{
		return AddNumberColumn(Guid.NewGuid(), name, action);
	}

	internal TfNumberDatabaseColumnBuilder AddNumberColumnBuilder(string name, Action<TfNumberDatabaseColumnBuilder> action = null)
	{
		return AddNumberColumnBuilder(Guid.NewGuid(), name, action);
	}

	public TfDatabaseColumnCollectionBuilder AddNumberColumn(Guid id, string name, Action<TfNumberDatabaseColumnBuilder> action)
	{
		_databaseBuilder.RegisterId(id);

		_databaseBuilder.RegisterColumnName(_tableName, name);

		TfNumberDatabaseColumnBuilder builder = new TfNumberDatabaseColumnBuilder(id, name, _databaseBuilder);

		if (action != null)
			action(builder);

		_builders.Add(builder);

		return this;
	}

	internal TfNumberDatabaseColumnBuilder AddNumberColumnBuilder(Guid id, string name, Action<TfNumberDatabaseColumnBuilder> action = null)
	{
		_databaseBuilder.RegisterId(id);

		_databaseBuilder.RegisterColumnName(_tableName, name);

		TfNumberDatabaseColumnBuilder builder = new TfNumberDatabaseColumnBuilder(id, name, _databaseBuilder);

		if (action != null)
			action(builder);

		_builders.Add(builder);

		return builder;
	}

	public TfDatabaseColumnCollectionBuilder WithNumberColumn(string name, Action<TfNumberDatabaseColumnBuilder> action)
	{
		var builder = (TfNumberDatabaseColumnBuilder)_builders
			.SingleOrDefault(x => x.Name == name && x.GetType() == typeof(TfNumberDatabaseColumnBuilder));

		if (builder is null)
			throw new TfDatabaseBuilderException($"Column of type Number and name '{name}' for table '{_tableName}' is not found.");

		if (action != null)
			action(builder);

		return this;
	}

	internal TfNumberDatabaseColumnBuilder WithNumberColumnBuilder(string name, Action<TfNumberDatabaseColumnBuilder> action = null)
	{
		var builder = (TfNumberDatabaseColumnBuilder)_builders
			.SingleOrDefault(x => x.Name == name && x.GetType() == typeof(TfNumberDatabaseColumnBuilder));

		if (builder is null)
			throw new TfDatabaseBuilderException($"Column of type Number and name '{name}' for table '{_tableName}' is not found.");

		if (action != null)
			action(builder);

		return builder;
	}

	#endregion

	#region <--- boolean --->

	public TfDatabaseColumnCollectionBuilder AddBooleanColumn(string name, Action<TfBooleanDatabaseColumnBuilder> action)
	{
		return AddBooleanColumn(Guid.NewGuid(), name, action);
	}

	internal TfBooleanDatabaseColumnBuilder AddBooleanColumnBuilder(string name, Action<TfBooleanDatabaseColumnBuilder> action = null)
	{
		return AddBooleanColumnBuilder(Guid.NewGuid(), name, action);
	}

	public TfDatabaseColumnCollectionBuilder AddBooleanColumn(Guid id, string name, Action<TfBooleanDatabaseColumnBuilder> action)
	{
		_databaseBuilder.RegisterId(id);

		_databaseBuilder.RegisterColumnName(_tableName, name);

		TfBooleanDatabaseColumnBuilder builder = new TfBooleanDatabaseColumnBuilder(id, name, _databaseBuilder);

		if (action != null)
			action(builder);

		_builders.Add(builder);

		return this;
	}

	internal TfBooleanDatabaseColumnBuilder AddBooleanColumnBuilder(Guid id, string name, Action<TfBooleanDatabaseColumnBuilder> action = null)
	{
		_databaseBuilder.RegisterId(id);

		_databaseBuilder.RegisterColumnName(_tableName, name);

		TfBooleanDatabaseColumnBuilder builder = new TfBooleanDatabaseColumnBuilder(id, name, _databaseBuilder);

		if (action != null)
			action(builder);

		_builders.Add(builder);

		return builder;
	}

	public TfDatabaseColumnCollectionBuilder WithBooleanColumn(string name, Action<TfBooleanDatabaseColumnBuilder> action)
	{
		var builder = (TfBooleanDatabaseColumnBuilder)_builders
			.SingleOrDefault(x => x.Name == name && x.GetType() == typeof(TfBooleanDatabaseColumnBuilder));

		if (builder is null)
			throw new TfDatabaseBuilderException($"Column of type Boolean and name '{name}' for table '{_tableName}' is not found.");

		if (action != null)
			action(builder);

		return this;
	}

	internal TfBooleanDatabaseColumnBuilder WithBooleanColumnBuilder(string name, Action<TfBooleanDatabaseColumnBuilder> action = null)
	{
		var builder = (TfBooleanDatabaseColumnBuilder)_builders
			.SingleOrDefault(x => x.Name == name && x.GetType() == typeof(TfBooleanDatabaseColumnBuilder));

		if (builder is null)
			throw new TfDatabaseBuilderException($"Column of type Boolean and name '{name}' for table '{_tableName}' is not found.");

		if (action != null)
			action(builder);

		return builder;
	}

	#endregion

	#region <--- date --->

	public TfDatabaseColumnCollectionBuilder AddDateColumn(string name, Action<TfDateDatabaseColumnBuilder> action)
	{
		return AddDateColumn(Guid.NewGuid(), name, action);
	}
	internal TfDateDatabaseColumnBuilder AddDateColumnBuilder(string name, Action<TfDateDatabaseColumnBuilder> action = null)
	{
		return AddDateColumnBuilder(Guid.NewGuid(), name, action);
	}

	public TfDatabaseColumnCollectionBuilder AddDateColumn(Guid id, string name, Action<TfDateDatabaseColumnBuilder> action)
	{
		_databaseBuilder.RegisterId(id);

		_databaseBuilder.RegisterColumnName(_tableName, name);

		TfDateDatabaseColumnBuilder builder = new TfDateDatabaseColumnBuilder(id, name, _databaseBuilder);

		if (action != null)
			action(builder);

		_builders.Add(builder);

		return this;
	}

	internal TfDateDatabaseColumnBuilder AddDateColumnBuilder(Guid id, string name, Action<TfDateDatabaseColumnBuilder> action = null)
	{
		_databaseBuilder.RegisterId(id);

		_databaseBuilder.RegisterColumnName(_tableName, name);

		TfDateDatabaseColumnBuilder builder = new TfDateDatabaseColumnBuilder(id, name, _databaseBuilder);

		if (action != null)
			action(builder);

		_builders.Add(builder);

		return builder;
	}

	public TfDatabaseColumnCollectionBuilder WithDateColumn(string name, Action<TfDateDatabaseColumnBuilder> action)
	{
		var builder = (TfDateDatabaseColumnBuilder)_builders
			.SingleOrDefault(x => x.Name == name && x.GetType() == typeof(TfDateDatabaseColumnBuilder));

		if (builder is null)
			throw new TfDatabaseBuilderException($"Column of type Date and name '{name}' for table '{_tableName}' is not found.");

		if (action != null)
			action(builder);

		return this;
	}

	internal TfDateDatabaseColumnBuilder WithDateColumnBuilder(string name, Action<TfDateDatabaseColumnBuilder> action = null)
	{
		var builder = (TfDateDatabaseColumnBuilder)_builders
			.SingleOrDefault(x => x.Name == name && x.GetType() == typeof(TfDateDatabaseColumnBuilder));

		if (builder is null)
			throw new TfDatabaseBuilderException($"Column of type Date and name '{name}' for table '{_tableName}' is not found.");

		if (action != null)
			action(builder);

		return builder;
	}

	#endregion

	#region <--- datetime --->

	public TfDatabaseColumnCollectionBuilder AddDateTimeColumn(string name, Action<TfDateTimeDatabaseColumnBuilder> action)
	{
		return AddDateTimeColumn(Guid.NewGuid(), name, action);
	}

	internal TfDateTimeDatabaseColumnBuilder AddDateTimeColumnBuilder(string name, Action<TfDateTimeDatabaseColumnBuilder> action = null)
	{
		return AddDateTimeColumnBuilder(Guid.NewGuid(), name, action);
	}

	public TfDatabaseColumnCollectionBuilder AddDateTimeColumn(Guid id, string name, Action<TfDateTimeDatabaseColumnBuilder> action)
	{
		_databaseBuilder.RegisterId(id);

		_databaseBuilder.RegisterColumnName(_tableName, name);

		TfDateTimeDatabaseColumnBuilder builder = new TfDateTimeDatabaseColumnBuilder(id, name, _databaseBuilder);

		if (action != null)
			action(builder);

		_builders.Add(builder);

		return this;
	}

	internal TfDateTimeDatabaseColumnBuilder AddDateTimeColumnBuilder(Guid id, string name, Action<TfDateTimeDatabaseColumnBuilder> action = null)
	{
		_databaseBuilder.RegisterId(id);

		_databaseBuilder.RegisterColumnName(_tableName, name);

		TfDateTimeDatabaseColumnBuilder builder = new TfDateTimeDatabaseColumnBuilder(id, name, _databaseBuilder);

		if (action != null)
			action(builder);

		_builders.Add(builder);

		return builder;
	}

	public TfDatabaseColumnCollectionBuilder WithDateTimeColumn(string name, Action<TfDateTimeDatabaseColumnBuilder> action)
	{
		var builder = (TfDateTimeDatabaseColumnBuilder)_builders
			.SingleOrDefault(x => x.Name == name && x.GetType() == typeof(TfDateTimeDatabaseColumnBuilder));

		if (builder is null)
			throw new TfDatabaseBuilderException($"Column of type DateTime and name '{name}' for table '{_tableName}' is not found.");

		if (action != null)
			action(builder);

		return this;
	}

	internal TfDateTimeDatabaseColumnBuilder WithDateTimeColumnBuilder(string name, Action<TfDateTimeDatabaseColumnBuilder> action = null)
	{
		var builder = (TfDateTimeDatabaseColumnBuilder)_builders
			.SingleOrDefault(x => x.Name == name && x.GetType() == typeof(TfDateTimeDatabaseColumnBuilder));

		if (builder is null)
			throw new TfDatabaseBuilderException($"Column of type DateTime and name '{name}' for table '{_tableName}' is not found.");

		if (action != null)
			action(builder);

		return builder;
	}

	#endregion

	#region <--- text --->

	public TfDatabaseColumnCollectionBuilder AddTextColumn(string name, Action<TfTextDatabaseColumnBuilder> action)
	{
		return AddTextColumn(Guid.NewGuid(), name, action);
	}

	internal TfTextDatabaseColumnBuilder AddTextColumnBuilder(string name, Action<TfTextDatabaseColumnBuilder> action = null)
	{
		return AddTextColumnBuilder(Guid.NewGuid(), name, action);
	}

	public TfDatabaseColumnCollectionBuilder AddTextColumn(Guid id, string name, Action<TfTextDatabaseColumnBuilder> action)
	{
		_databaseBuilder.RegisterId(id);

		_databaseBuilder.RegisterColumnName(_tableName, name);

		TfTextDatabaseColumnBuilder builder = new TfTextDatabaseColumnBuilder(id, name, _databaseBuilder);

		if (action != null)
			action(builder);

		_builders.Add(builder);

		return this;
	}

	internal TfTextDatabaseColumnBuilder AddTextColumnBuilder(Guid id, string name, Action<TfTextDatabaseColumnBuilder> action = null)
	{
		_databaseBuilder.RegisterId(id);

		_databaseBuilder.RegisterColumnName(_tableName, name);

		TfTextDatabaseColumnBuilder builder = new TfTextDatabaseColumnBuilder(id, name, _databaseBuilder);

		if (action != null)
			action(builder);

		_builders.Add(builder);

		return builder;
	}

	public TfDatabaseColumnCollectionBuilder WithTextColumn(string name, Action<TfTextDatabaseColumnBuilder> action)
	{
		var builder = (TfTextDatabaseColumnBuilder)_builders
			.SingleOrDefault(x => x.Name == name && x.GetType() == typeof(TfTextDatabaseColumnBuilder));

		if (builder is null)
			throw new TfDatabaseBuilderException($"Column of type Text and name '{name}' for table '{_tableName}' is not found.");

		if (action != null)
			action(builder);

		return this;
	}

	internal TfTextDatabaseColumnBuilder WithTextColumnBuilder(string name, Action<TfTextDatabaseColumnBuilder> action = null)
	{
		var builder = (TfTextDatabaseColumnBuilder)_builders
			.SingleOrDefault(x => x.Name == name && x.GetType() == typeof(TfTextDatabaseColumnBuilder));

		if (builder is null)
			throw new TfDatabaseBuilderException($"Column of type Text and name '{name}' for table '{_tableName}' is not found.");

		if (action != null)
			action(builder);

		return builder;
	}

	#endregion

	#region <--- short text --->

	public TfDatabaseColumnCollectionBuilder AddShortTextColumn(string name, Action<TfShortTextDatabaseColumnBuilder> action)
	{
		return AddShortTextColumn(Guid.NewGuid(), name, action);
	}

	internal TfShortTextDatabaseColumnBuilder AddShortTextColumnBuilder(string name, Action<TfShortTextDatabaseColumnBuilder> action = null)
	{
		return AddShortTextColumnBuilder(Guid.NewGuid(), name, action);
	}

	public TfDatabaseColumnCollectionBuilder AddShortTextColumn(Guid id, string name, Action<TfShortTextDatabaseColumnBuilder> action)
	{
		_databaseBuilder.RegisterId(id);

		_databaseBuilder.RegisterColumnName(_tableName, name);

		TfShortTextDatabaseColumnBuilder builder = new TfShortTextDatabaseColumnBuilder(id, name, _databaseBuilder);

		if (action != null)
			action(builder);

		_builders.Add(builder);

		return this;
	}

	internal TfShortTextDatabaseColumnBuilder AddShortTextColumnBuilder(Guid id, string name, Action<TfShortTextDatabaseColumnBuilder> action = null)
	{
		_databaseBuilder.RegisterId(id);

		_databaseBuilder.RegisterColumnName(_tableName, name);

		TfShortTextDatabaseColumnBuilder builder = new TfShortTextDatabaseColumnBuilder(id, name, _databaseBuilder);

		if (action != null)
			action(builder);

		_builders.Add(builder);

		return builder;
	}

	public TfDatabaseColumnCollectionBuilder WithShortTextColumn(string name, Action<TfShortTextDatabaseColumnBuilder> action)
	{
		var builder = (TfShortTextDatabaseColumnBuilder)_builders
			.SingleOrDefault(x => x.Name == name && x.GetType() == typeof(TfShortTextDatabaseColumnBuilder));

		if (builder is null)
			throw new TfDatabaseBuilderException($"Column of type ShortText and name '{name}' for table '{_tableName}' is not found.");

		if (action != null)
			action(builder);

		return this;
	}

	internal TfShortTextDatabaseColumnBuilder WithShortTextColumnBuilder(string name, Action<TfShortTextDatabaseColumnBuilder> action = null)
	{
		var builder = (TfShortTextDatabaseColumnBuilder)_builders
			.SingleOrDefault(x => x.Name == name && x.GetType() == typeof(TfShortTextDatabaseColumnBuilder));

		if (builder is null)
			throw new TfDatabaseBuilderException($"Column of type ShortText and name '{name}' for table '{_tableName}' is not found.");

		if (action != null)
			action(builder);

		return builder;
	}

	#endregion

	#region <--- short integer --->

	public TfDatabaseColumnCollectionBuilder AddShortIntegerColumn(string name, Action<TfShortIntegerDatabaseColumnBuilder> action)
	{
		return AddShortIntegerColumn(Guid.NewGuid(), name, action);
	}

	internal TfShortIntegerDatabaseColumnBuilder AddShortIntegerColumnBuilder(string name, Action<TfShortIntegerDatabaseColumnBuilder> action = null)
	{
		return AddShortIntegerColumnBuilder(Guid.NewGuid(), name, action);
	}

	public TfDatabaseColumnCollectionBuilder AddShortIntegerColumn(Guid id, string name, Action<TfShortIntegerDatabaseColumnBuilder> action)
	{
		_databaseBuilder.RegisterId(id);

		_databaseBuilder.RegisterColumnName(_tableName, name);

		TfShortIntegerDatabaseColumnBuilder builder = new TfShortIntegerDatabaseColumnBuilder(id, name, _databaseBuilder);

		if (action != null)
			action(builder);

		_builders.Add(builder);

		return this;
	}

	internal TfShortIntegerDatabaseColumnBuilder AddShortIntegerColumnBuilder(Guid id, string name, Action<TfShortIntegerDatabaseColumnBuilder> action = null)
	{
		_databaseBuilder.RegisterId(id);

		_databaseBuilder.RegisterColumnName(_tableName, name);

		TfShortIntegerDatabaseColumnBuilder builder = new TfShortIntegerDatabaseColumnBuilder(id, name, _databaseBuilder);

		if (action != null)
			action(builder);

		_builders.Add(builder);

		return builder;
	}

	public TfDatabaseColumnCollectionBuilder WithShortIntegerColumn(string name, Action<TfShortIntegerDatabaseColumnBuilder> action)
	{
		var builder = (TfShortIntegerDatabaseColumnBuilder)_builders
			.SingleOrDefault(x => x.Name == name && x.GetType() == typeof(TfShortIntegerDatabaseColumnBuilder));

		if (builder is null)
			throw new TfDatabaseBuilderException($"Column of type ShortInteger and name '{name}' for table '{_tableName}' is not found.");

		if (action != null)
			action(builder);

		return this;
	}

	internal TfShortIntegerDatabaseColumnBuilder WithShortIntegerColumnBuilder(string name, Action<TfShortIntegerDatabaseColumnBuilder> action = null)
	{
		var builder = (TfShortIntegerDatabaseColumnBuilder)_builders
			.SingleOrDefault(x => x.Name == name && x.GetType() == typeof(TfShortIntegerDatabaseColumnBuilder));

		if (builder is null)
			throw new TfDatabaseBuilderException($"Column of type ShortInteger and name '{name}' for table '{_tableName}' is not found.");

		if (action != null)
			action(builder);

		return builder;
	}

	#endregion

	#region <--- integer --->

	public TfDatabaseColumnCollectionBuilder AddIntegerColumn(string name, Action<TfIntegerDatabaseColumnBuilder> action)
	{
		return AddIntegerColumn(Guid.NewGuid(), name, action);
	}

	internal TfIntegerDatabaseColumnBuilder AddIntegerColumnBuilder(string name, Action<TfIntegerDatabaseColumnBuilder> action = null)
	{
		return AddIntegerColumnBuilder(Guid.NewGuid(), name, action);
	}

	public TfDatabaseColumnCollectionBuilder AddIntegerColumn(Guid id, string name, Action<TfIntegerDatabaseColumnBuilder> action)
	{
		_databaseBuilder.RegisterId(id);

		_databaseBuilder.RegisterColumnName(_tableName, name);

		TfIntegerDatabaseColumnBuilder builder = new TfIntegerDatabaseColumnBuilder(id, name, _databaseBuilder);

		if (action != null)
			action(builder);

		_builders.Add(builder);

		return this;
	}

	internal TfIntegerDatabaseColumnBuilder AddIntegerColumnBuilder(Guid id, string name, Action<TfIntegerDatabaseColumnBuilder> action = null)
	{
		_databaseBuilder.RegisterId(id);

		_databaseBuilder.RegisterColumnName(_tableName, name);

		TfIntegerDatabaseColumnBuilder builder = new TfIntegerDatabaseColumnBuilder(id, name, _databaseBuilder);

		if (action != null)
			action(builder);

		_builders.Add(builder);

		return builder;
	}

	public TfDatabaseColumnCollectionBuilder WithIntegerColumn(string name, Action<TfIntegerDatabaseColumnBuilder> action)
	{
		var builder = (TfIntegerDatabaseColumnBuilder)_builders
			.SingleOrDefault(x => x.Name == name && x.GetType() == typeof(TfIntegerDatabaseColumnBuilder));

		if (builder is null)
			throw new TfDatabaseBuilderException($"Column of type Integer and name '{name}' for table '{_tableName}' is not found.");

		if (action != null)
			action(builder);

		return this;
	}

	internal TfIntegerDatabaseColumnBuilder WithIntegerColumnBuilder(string name, Action<TfIntegerDatabaseColumnBuilder> action = null)
	{
		var builder = (TfIntegerDatabaseColumnBuilder)_builders
			.SingleOrDefault(x => x.Name == name && x.GetType() == typeof(TfIntegerDatabaseColumnBuilder));

		if (builder is null)
			throw new TfDatabaseBuilderException($"Column of type Integer and name '{name}' for table '{_tableName}' is not found.");

		if (action != null)
			action(builder);

		return builder;
	}

	#endregion

	#region <--- long integer --->

	public TfDatabaseColumnCollectionBuilder AddLongIntegerColumn(string name, Action<TfLongIntegerDatabaseColumnBuilder> action)
	{
		return AddLongIntegerColumn(Guid.NewGuid(), name, action);
	}

	internal TfLongIntegerDatabaseColumnBuilder AddLongIntegerColumnBuilder(string name, Action<TfLongIntegerDatabaseColumnBuilder> action = null)
	{
		return AddLongIntegerColumnBuilder(Guid.NewGuid(), name, action);
	}

	public TfDatabaseColumnCollectionBuilder AddLongIntegerColumn(Guid id, string name, Action<TfLongIntegerDatabaseColumnBuilder> action)
	{
		_databaseBuilder.RegisterId(id);

		_databaseBuilder.RegisterColumnName(_tableName, name);

		TfLongIntegerDatabaseColumnBuilder builder = new TfLongIntegerDatabaseColumnBuilder(id, name, _databaseBuilder);

		if (action != null)
			action(builder);

		_builders.Add(builder);

		return this;
	}

	internal TfLongIntegerDatabaseColumnBuilder AddLongIntegerColumnBuilder(Guid id, string name, Action<TfLongIntegerDatabaseColumnBuilder> action = null)
	{
		_databaseBuilder.RegisterId(id);

		_databaseBuilder.RegisterColumnName(_tableName, name);

		TfLongIntegerDatabaseColumnBuilder builder = new TfLongIntegerDatabaseColumnBuilder(id, name, _databaseBuilder);

		if (action != null)
			action(builder);

		_builders.Add(builder);

		return builder;
	}

	public TfDatabaseColumnCollectionBuilder WithLongIntegerColumn(string name, Action<TfLongIntegerDatabaseColumnBuilder> action)
	{
		var builder = (TfLongIntegerDatabaseColumnBuilder)_builders
			.SingleOrDefault(x => x.Name == name && x.GetType() == typeof(TfLongIntegerDatabaseColumnBuilder));

		if (builder is null)
			throw new TfDatabaseBuilderException($"Column of type LongInteger and name '{name}' for table '{_tableName}' is not found.");

		if (action != null)
			action(builder);

		return this;
	}

	internal TfLongIntegerDatabaseColumnBuilder WithLongIntegerColumnBuilder(string name, Action<TfLongIntegerDatabaseColumnBuilder> action = null)
	{
		var builder = (TfLongIntegerDatabaseColumnBuilder)_builders
			.SingleOrDefault(x => x.Name == name && x.GetType() == typeof(TfLongIntegerDatabaseColumnBuilder));

		if (builder is null)
			throw new TfDatabaseBuilderException($"Column of type LongInteger and name '{name}' for table '{_tableName}' is not found.");

		if (action != null)
			action(builder);

		return builder;
	}

	#endregion

	#region <=== remove ===>

	public TfDatabaseColumnCollectionBuilder Remove(string name)
	{

		var builder = _builders.SingleOrDefault(x => x.Name == name);

		if (builder is null)
			throw new TfDatabaseBuilderException($"Column with name '{name}' for table '{_tableName}' is not found.");

		_databaseBuilder.UnregisterId(builder.Id);

		_databaseBuilder.UnregisterColumnName(_tableName, builder.Name);

		_builders.Remove(builder);

		return this;
	}

	#endregion

	#region <--- build --->

	internal TfDatabaseColumnCollection Build()
	{
		var collection = new TfDatabaseColumnCollection();

		foreach (var builder in _builders)
			collection.Add(builder.Build());

		return collection;
	}

	#endregion
}