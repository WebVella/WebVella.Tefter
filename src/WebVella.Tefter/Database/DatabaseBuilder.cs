namespace WebVella.Tefter.Database;

public class DatabaseBuilder
{
	private HashSet<Guid> _databaseObjectIds;
	private HashSet<string> _databaseObjectNames;
	private Dictionary<string, HashSet<string>> _tableColumnNames;

	private readonly List<DbTableBuilder> _builders;
	internal ReadOnlyCollection<DbTableBuilder> Builders => _builders.AsReadOnly();

	private DatabaseBuilder(DbTableCollection tables = null)
	{
		_databaseObjectIds = new HashSet<Guid>();
		_databaseObjectNames = new HashSet<string>();
		_tableColumnNames = new Dictionary<string, HashSet<string>>();
		_builders = new List<DbTableBuilder>();
	}

	internal static DatabaseBuilder New(DbTableCollection tables = null)
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
						case DbAutoIncrementColumn c:
							{
								columnCollectionBuilder
									.AddAutoIncrementColumnBuilder(c.Id, c.Name)
									.WithLastCommited(c.LastCommited);
							}
							break;
						case DbGuidColumn c:
							{
								var cb = columnCollectionBuilder
									.AddGuidColumnBuilder(c.Id, c.Name)
									.WithDefaultValue((Guid?)c.DefaultValue)
									.WithLastCommited(c.LastCommited);

								if (c.IsNullable) cb.Nullable(); else cb.NotNullable();
								if (c.AutoDefaultValue) cb.WithAutoDefaultValue(); else cb.WithoutAutoDefaultValue();
							}
							break;
						case DbBooleanColumn c:
							{
								var cb = columnCollectionBuilder
									.AddBooleanColumnBuilder(c.Id, c.Name)
									.WithDefaultValue((bool?)c.DefaultValue)
									.WithLastCommited(c.LastCommited);

								if (c.IsNullable) cb.Nullable(); else cb.NotNullable();
							}
							break;
						case DbNumberColumn c:
							{
								var cb = columnCollectionBuilder
									.AddNumberColumnBuilder(c.Id, c.Name)
									.WithDefaultValue((decimal?)c.DefaultValue)
									.WithLastCommited(c.LastCommited);

								if (c.IsNullable) cb.Nullable(); else cb.NotNullable();
							}
							break;
						case DbDateColumn c:
							{
								var cb = columnCollectionBuilder
									.AddDateColumnBuilder(c.Id, c.Name)
									.WithDefaultValue((DateOnly?)c.DefaultValue)
									.WithLastCommited(c.LastCommited);

								if (c.IsNullable) cb.Nullable(); else cb.NotNullable();
								if (c.AutoDefaultValue) cb.WithAutoDefaultValue(); else cb.WithoutAutoDefaultValue();
							}
							break;
						case DbDateTimeColumn c:
							{
								var cb = columnCollectionBuilder
									.AddDateTimeColumnBuilder(c.Id, c.Name)
									.WithDefaultValue((DateTime?)c.DefaultValue)
									.WithLastCommited(c.LastCommited);

								if (c.IsNullable) cb.Nullable(); else cb.NotNullable();
								if (c.AutoDefaultValue) cb.WithAutoDefaultValue(); else cb.WithoutAutoDefaultValue();
							}
							break;
						case DbTextColumn c:
							{
								var cb = columnCollectionBuilder
									.AddTextColumnBuilder(c.Id, c.Name)
									.WithDefaultValue((string)c.DefaultValue)
									.WithLastCommited(c.LastCommited);

								if (c.IsNullable) cb.Nullable(); else cb.NotNullable();
							}
							break;
						default:
							throw new DbBuilderException($"{column.GetType()} is not supported");
					}
				}

				foreach (var index in table.Indexes)
				{
					var indexCollectionBuilder = tableBuilder.WithIndexesBuilder();
					switch (index)
					{
						case DbBTreeIndex i:
							{
								indexCollectionBuilder
									.AddBTreeIndexBuilder(index.Name)
									.WithColumns(index.Columns.ToArray());
							}
							break;
						case DbGinIndex i:
							{
								indexCollectionBuilder
									.AddGinIndexBuilder(index.Name)
									.WithColumns(index.Columns.ToArray());
							}
							break;
						case DbGistIndex i:
							{
								indexCollectionBuilder
									.AddGistIndexBuilder(index.Name)
									.WithColumns(index.Columns.ToArray());
							}
							break;
						case DbHashIndex i:
							{
								indexCollectionBuilder
									.AddHashIndexBuilder(index.Name)
									.WithColumn(index.Columns.First());
							}
							break;
						default:
							throw new DbBuilderException($"{index.GetType()} is not supported");
					}
				}

				foreach (var constraint in table.Constraints)
				{
					var constrCollectionBuilder = tableBuilder.WithConstraintsBuilder();
					switch (constraint)
					{
						case DbPrimaryKeyConstraint c:
							{
								constrCollectionBuilder
									.AddPrimaryKeyConstraintBuilder(c.Name)
									.WithColumns(c.Columns.ToArray());
							}
							break;
						case DbUniqueKeyConstraint c:
							{
								constrCollectionBuilder
									.AddUniqueKeyConstraintBuilder(c.Name)
									.WithColumns(c.Columns.ToArray());
							}
							break;
						case DbForeignKeyConstraint c:
							{
								constrCollectionBuilder
									.AddForeignKeyConstraintBuilder(c.Name)
									.WithColumns(c.Columns.ToArray())
									.WithForeignTable(c.ForeignTable)
									.WithForeignColumns(c.ForeignColumns.ToArray());
							}
							break;
						default:
							throw new DbBuilderException($"{constraint.GetType()} is not supported");
					}
				}
			}
		}

		return builder;
	}

	public DatabaseBuilder NewTable(string name, Action<DbTableBuilder> action)
	{
		return NewTable(Guid.NewGuid(), name, action);
	}

	public DatabaseBuilder NewTable(Guid id, string name, Action<DbTableBuilder> action)
	{
		RegisterId(id);
		RegisterName(name);

		var builder = (DbTableBuilder)_builders.SingleOrDefault(x => x.Name == name);

		if (builder is not null)
			throw new DbBuilderException($"Table with name '{name}' already exists. Only one instance can be created.");

		builder = new DbTableBuilder(id, name, this);

		action(builder);

		_builders.Add(builder);

		return this;
	}

	public DbTableBuilder NewTableBuilder(Guid id, string name)
	{
		RegisterId(id);
		RegisterName(name);

		var builder = (DbTableBuilder)_builders.SingleOrDefault(x => x.Name == name);

		if (builder is not null)
			throw new DbBuilderException($"Table with name '{name}' already exists. Only one instance can be created.");

		builder = new DbTableBuilder(id, name, this);

		_builders.Add(builder);

		return builder;
	}

	public DatabaseBuilder WithTable(string name, Action<DbTableBuilder> action)
	{
		var builder = (DbTableBuilder)_builders.SingleOrDefault(x => x.Name == name);

		if (builder is null)
			throw new DbBuilderException($"Table with name '{name}' not found.");

		action(builder);

		return this;
	}

	public DbTableBuilder WithTableBuilder(string name, Action<DbTableBuilder> action = null)
	{
		var builder = (DbTableBuilder)_builders.SingleOrDefault(x => x.Name == name);

		if (builder is null)
			throw new DbBuilderException($"Table with name '{name}' not found.");

		if (action != null)
			action(builder);

		return builder;
	}

	public DatabaseBuilder WithTable(Guid id, Action<DbTableBuilder> action)
	{
		var builder = (DbTableBuilder)_builders.SingleOrDefault(x => x.Id == id);

		if (builder is null)
			throw new DbBuilderException($"Table with id '{id}' not found.");

		action(builder);

		return this;
	}

	public DbTableBuilder WithTableBuilder(Guid id, Action<DbTableBuilder> action = null)
	{
		var builder = (DbTableBuilder)_builders.SingleOrDefault(x => x.Id == id);

		if (builder is null)
			throw new DbBuilderException($"Table with id '{id}' not found.");

		if (action != null)
			action(builder);

		return builder;
	}

	public DatabaseBuilder Remove(string name)
	{
		var builder = _builders.SingleOrDefault(x => x.Name == name);

		if (builder is null)
			throw new DbBuilderException($"Table with name '{name}' is not found.");

		_builders.Remove(builder);

		return this;
	}

	public DbTableCollection Build()
	{
		var collection = new DbTableCollection();

		foreach (var builder in _builders)
			collection.Add(builder.Build());

		return collection;
	}

	#region <--- Validation --->
	internal void RegisterId(Guid id)
	{
		if (_databaseObjectIds.Contains(id))
			throw new DbBuilderException($"There is already existing database object with id '{id}'");

		_databaseObjectIds.Add(id);
	}

	internal void UnregisterId(Guid id)
	{
		if (!_databaseObjectIds.Contains(id))
			throw new DbBuilderException($"There is no existing object with specified id '{id}'");

		_databaseObjectIds.Remove(id);
	}

	internal void RegisterName(string name)
	{
		if (name == null)
			throw new ArgumentNullException("name");

		if (!IsValidDbObjectName(name, out string error))
			throw new DbBuilderException($"Invalid name '{name}'. {error}");

		if (_databaseObjectNames.Contains(name))
			throw new DbBuilderException($"There is already existing database object with name '{name}'");

		_databaseObjectNames.Add(name);
	}

	internal void UnregisterName(string name)
	{
		if (name == null)
			throw new ArgumentNullException("name");

		if (!_databaseObjectNames.Contains(name))
			throw new DbBuilderException($"No object with name '{name}' found in registered database objects.");

		_databaseObjectNames.Remove(name);
	}

	internal void RegisterColumnName(string tableName, string columnName)
	{
		if (string.IsNullOrWhiteSpace(tableName))
			throw new ArgumentNullException(nameof(tableName));

		if (string.IsNullOrWhiteSpace(columnName))
			throw new ArgumentNullException(nameof(columnName));

		if (!IsValidDbObjectName(columnName, out string error))
			throw new DbBuilderException($"Invalid name '{columnName}'. {error}");

		if (!_tableColumnNames.ContainsKey(tableName))
			_tableColumnNames.Add(tableName, new HashSet<string>());

		if (_tableColumnNames[tableName].Contains(columnName))
			throw new DbBuilderException($"There is already existing column with name '{columnName}' for table '{tableName}'");

		_tableColumnNames[tableName].Add(columnName);
	}

	internal void UnregisterColumnName(string tableName, string columnName)
	{
		if (string.IsNullOrWhiteSpace(tableName))
			throw new ArgumentNullException(nameof(tableName));

		if (string.IsNullOrWhiteSpace(columnName))
			throw new ArgumentNullException(nameof(columnName));

		if (!_tableColumnNames.ContainsKey(tableName))
			throw new DbBuilderException($"Table with name '{tableName}' not found while trying to remove column '{columnName}'");

		if (!_tableColumnNames[tableName].Contains(columnName))
			throw new DbBuilderException($"Not found a column with name '{columnName}' for table '{tableName}' while trying to remove it.");

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
