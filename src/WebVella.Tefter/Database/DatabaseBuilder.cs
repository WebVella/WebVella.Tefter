namespace WebVella.Tefter.Database;

public class DatabaseBuilder
{
	private HashSet<Guid> _databaseObjectIds;
	private HashSet<string> _databaseObjectNames;
	private Dictionary<string, HashSet<string>> _tableColumnNames;

	private readonly List<DatabaseTableBuilder> _builders;
	internal ReadOnlyCollection<DatabaseTableBuilder> Builders => _builders.AsReadOnly();

	private DatabaseBuilder(DatabaseTableCollection tables = null)
	{
		_databaseObjectIds = new HashSet<Guid>();
		_databaseObjectNames = new HashSet<string>();
		_tableColumnNames = new Dictionary<string, HashSet<string>>();
		_builders = new List<DatabaseTableBuilder>();
	}

	internal static DatabaseBuilder New(DatabaseTableCollection tables = null)
	{
		var builder = new DatabaseBuilder();

		if (tables != null)
		{
			foreach (var table in tables)
			{
				var tableBuilder = builder.NewTableBuilder(table.Id, table.Name);

				foreach (var column in table.Columns)
				{
					var columnCollectionBuilder = tableBuilder.WithColumnsBuilder();
					switch (column)
					{
						case AutoIncrementDatabaseColumn c:
							{
								columnCollectionBuilder
									.AddAutoIncrementColumnBuilder(c.Id, c.Name)
									.WithLastCommited(c.LastCommited);
							}
							break;
						case GuidDatabaseColumn c:
							{
								var cb = columnCollectionBuilder
									.AddGuidColumnBuilder(c.Id, c.Name)
									.WithDefaultValue((Guid?)c.DefaultValue)
									.WithLastCommited(c.LastCommited);

								if (c.IsNullable) cb.Nullable(); else cb.NotNullable();
								if (c.AutoDefaultValue) cb.WithAutoDefaultValue(); else cb.WithoutAutoDefaultValue();
							}
							break;
						case BooleanDatabaseColumn c:
							{
								var cb = columnCollectionBuilder
									.AddBooleanColumnBuilder(c.Id, c.Name)
									.WithDefaultValue((bool?)c.DefaultValue)
									.WithLastCommited(c.LastCommited);

								if (c.IsNullable) cb.Nullable(); else cb.NotNullable();
							}
							break;
						case NumberDatabaseColumn c:
							{
								var cb = columnCollectionBuilder
									.AddNumberColumnBuilder(c.Id, c.Name)
									.WithDefaultValue((decimal?)c.DefaultValue)
									.WithLastCommited(c.LastCommited);

								if (c.IsNullable) cb.Nullable(); else cb.NotNullable();
							}
							break;
						case DateDatabaseColumn c:
							{
								var cb = columnCollectionBuilder
									.AddDateColumnBuilder(c.Id, c.Name)
									.WithDefaultValue((DateOnly?)c.DefaultValue)
									.WithLastCommited(c.LastCommited);

								if (c.IsNullable) cb.Nullable(); else cb.NotNullable();
								if (c.AutoDefaultValue) cb.WithAutoDefaultValue(); else cb.WithoutAutoDefaultValue();
							}
							break;
						case DateTimeDatabaseColumn c:
							{
								var cb = columnCollectionBuilder
									.AddDateTimeColumnBuilder(c.Id, c.Name)
									.WithDefaultValue((DateTime?)c.DefaultValue)
									.WithLastCommited(c.LastCommited);

								if (c.IsNullable) cb.Nullable(); else cb.NotNullable();
								if (c.AutoDefaultValue) cb.WithAutoDefaultValue(); else cb.WithoutAutoDefaultValue();
							}
							break;
						case TextDatabaseColumn c:
							{
								var cb = columnCollectionBuilder
									.AddTextColumnBuilder(c.Id, c.Name)
									.WithDefaultValue((string)c.DefaultValue)
									.WithLastCommited(c.LastCommited);

								if (c.IsNullable) cb.Nullable(); else cb.NotNullable();
							}
							break;
						default:
							throw new DatabaseBuilderException($"{column.GetType()} is not supported");
					}
				}

				foreach (var index in table.Indexes)
				{
					var indexCollectionBuilder = tableBuilder.WithIndexesBuilder();
					switch (index)
					{
						case BTreeDatabaseIndex i:
							{
								indexCollectionBuilder
									.AddBTreeIndexBuilder(index.Name)
									.WithColumns(index.Columns.ToArray());
							}
							break;
						case GinDatabaseIndex i:
							{
								indexCollectionBuilder
									.AddGinIndexBuilder(index.Name)
									.WithColumns(index.Columns.ToArray());
							}
							break;
						case GistDatabaseIndex i:
							{
								indexCollectionBuilder
									.AddGistIndexBuilder(index.Name)
									.WithColumns(index.Columns.ToArray());
							}
							break;
						case HashDatabaseIndex i:
							{
								indexCollectionBuilder
									.AddHashIndexBuilder(index.Name)
									.WithColumn(index.Columns.First());
							}
							break;
						default:
							throw new DatabaseBuilderException($"{index.GetType()} is not supported");
					}
				}

				foreach (var constraint in table.Constraints)
				{
					var constrCollectionBuilder = tableBuilder.WithConstraintsBuilder();
					switch (constraint)
					{
						case DatabasePrimaryKeyConstraint c:
							{
								constrCollectionBuilder
									.AddPrimaryKeyConstraintBuilder(c.Name)
									.WithColumns(c.Columns.ToArray());
							}
							break;
						case DatabaseUniqueKeyConstraint c:
							{
								constrCollectionBuilder
									.AddUniqueKeyConstraintBuilder(c.Name)
									.WithColumns(c.Columns.ToArray());
							}
							break;
						case DatabaseForeignKeyConstraint c:
							{
								constrCollectionBuilder
									.AddForeignKeyConstraintBuilder(c.Name)
									.WithColumns(c.Columns.ToArray())
									.WithForeignTable(c.ForeignTable)
									.WithForeignColumns(c.ForeignColumns.ToArray());
							}
							break;
						default:
							throw new DatabaseBuilderException($"{constraint.GetType()} is not supported");
					}
				}
			}
		}

		return builder;
	}

	public DatabaseBuilder NewTable(string name, Action<DatabaseTableBuilder> action)
	{
		return NewTable(Guid.NewGuid(), name, action);
	}

	public DatabaseBuilder NewTable(Guid id, string name, Action<DatabaseTableBuilder> action)
	{
		RegisterId(id);
		RegisterName(name);

		var builder = (DatabaseTableBuilder)_builders.SingleOrDefault(x => x.Name == name);

		if (builder is not null)
			throw new DatabaseBuilderException($"Table with name '{name}' already exists. Only one instance can be created.");

		builder = new DatabaseTableBuilder(id, name, this);

		action(builder);

		_builders.Add(builder);

		return this;
	}

	public DatabaseTableBuilder NewTableBuilder(Guid id, string name)
	{
		RegisterId(id);
		RegisterName(name);

		var builder = (DatabaseTableBuilder)_builders.SingleOrDefault(x => x.Name == name);

		if (builder is not null)
			throw new DatabaseBuilderException($"Table with name '{name}' already exists. Only one instance can be created.");

		builder = new DatabaseTableBuilder(id, name, this);

		_builders.Add(builder);

		return builder;
	}

	public DatabaseBuilder WithTable(string name, Action<DatabaseTableBuilder> action)
	{
		var builder = (DatabaseTableBuilder)_builders.SingleOrDefault(x => x.Name == name);

		if (builder is null)
			throw new DatabaseBuilderException($"Table with name '{name}' not found.");

		action(builder);

		return this;
	}

	public DatabaseTableBuilder WithTableBuilder(string name, Action<DatabaseTableBuilder> action = null)
	{
		var builder = (DatabaseTableBuilder)_builders.SingleOrDefault(x => x.Name == name);

		if (builder is null)
			throw new DatabaseBuilderException($"Table with name '{name}' not found.");

		if (action != null)
			action(builder);

		return builder;
	}

	public DatabaseBuilder WithTable(Guid id, Action<DatabaseTableBuilder> action)
	{
		var builder = (DatabaseTableBuilder)_builders.SingleOrDefault(x => x.Id == id);

		if (builder is null)
			throw new DatabaseBuilderException($"Table with id '{id}' not found.");

		action(builder);

		return this;
	}

	public DatabaseTableBuilder WithTableBuilder(Guid id, Action<DatabaseTableBuilder> action = null)
	{
		var builder = (DatabaseTableBuilder)_builders.SingleOrDefault(x => x.Id == id);

		if (builder is null)
			throw new DatabaseBuilderException($"Table with id '{id}' not found.");

		if (action != null)
			action(builder);

		return builder;
	}

	public DatabaseBuilder Remove(string name)
	{
		var builder = _builders.SingleOrDefault(x => x.Name == name);

		if (builder is null)
			throw new DatabaseBuilderException($"Table with name '{name}' is not found.");

		_builders.Remove(builder);

		return this;
	}

	internal DatabaseTableCollection Build()
	{
		var collection = new DatabaseTableCollection();

		foreach (var builder in _builders)
			collection.Add(builder.Build());

		return collection;
	}

	#region <--- Validation --->
	internal void RegisterId(Guid id)
	{
		if (_databaseObjectIds.Contains(id))
			throw new DatabaseBuilderException($"There is already existing database object with id '{id}'");

		_databaseObjectIds.Add(id);
	}

	internal void UnregisterId(Guid id)
	{
		if (!_databaseObjectIds.Contains(id))
			throw new DatabaseBuilderException($"There is no existing object with specified id '{id}'");

		_databaseObjectIds.Remove(id);
	}

	internal void RegisterName(string name)
	{
		if (name == null)
			throw new ArgumentNullException("name");

		if (!IsValidDbObjectName(name, out string error))
			throw new DatabaseBuilderException($"Invalid name '{name}'. {error}");

		if (_databaseObjectNames.Contains(name))
			throw new DatabaseBuilderException($"There is already existing database object with name '{name}'");

		_databaseObjectNames.Add(name);
	}

	internal void UnregisterName(string name)
	{
		if (name == null)
			throw new ArgumentNullException("name");

		if (!_databaseObjectNames.Contains(name))
			throw new DatabaseBuilderException($"No object with name '{name}' found in registered database objects.");

		_databaseObjectNames.Remove(name);
	}

	internal void RegisterColumnName(string tableName, string columnName)
	{
		if (string.IsNullOrWhiteSpace(tableName))
			throw new ArgumentNullException(nameof(tableName));

		if (string.IsNullOrWhiteSpace(columnName))
			throw new ArgumentNullException(nameof(columnName));

		if (!IsValidDbObjectName(columnName, out string error))
			throw new DatabaseBuilderException($"Invalid name '{columnName}'. {error}");

		if (!_tableColumnNames.ContainsKey(tableName))
			_tableColumnNames.Add(tableName, new HashSet<string>());

		if (_tableColumnNames[tableName].Contains(columnName))
			throw new DatabaseBuilderException($"There is already existing column with name '{columnName}' for table '{tableName}'");

		_tableColumnNames[tableName].Add(columnName);
	}

	internal void UnregisterColumnName(string tableName, string columnName)
	{
		if (string.IsNullOrWhiteSpace(tableName))
			throw new ArgumentNullException(nameof(tableName));

		if (string.IsNullOrWhiteSpace(columnName))
			throw new ArgumentNullException(nameof(columnName));

		if (!_tableColumnNames.ContainsKey(tableName))
			throw new DatabaseBuilderException($"Table with name '{tableName}' not found while trying to remove column '{columnName}'");

		if (!_tableColumnNames[tableName].Contains(columnName))
			throw new DatabaseBuilderException($"Not found a column with name '{columnName}' for table '{tableName}' while trying to remove it.");

		_tableColumnNames[tableName].Remove(columnName);
	}

	private bool IsValidDbObjectName(string name, out string error)
	{
		error = null;

		if (string.IsNullOrEmpty(name))
		{
			error = "Name is required and cannot be empty";
			return false;
		}

		if (name.Length < Constants.DB_MIN_OBJECT_NAME_LENGTH)
			error = $"The name must be at least {Constants.DB_MIN_OBJECT_NAME_LENGTH} characters long";

		if (name.Length > Constants.DB_MAX_OBJECT_NAME_LENGTH)
			error = $"The length of name must be less or equal than {Constants.DB_MAX_OBJECT_NAME_LENGTH} characters";

		Match match = Regex.Match(name, Constants.DB_OBJECT_NAME_VALIDATION_PATTERN);
		if (!match.Success || match.Value != name.Trim())
			error = $"Name can only contains underscores and lowercase alphanumeric characters. It must begin with a letter, " +
				$"not include spaces, not end with an underscore, and not contain two consecutive underscores";

		return string.IsNullOrWhiteSpace(error);
	}

	#endregion
}
