namespace WebVella.Tefter.Database;

public class DatabaseBuilder
{
    private readonly List<DbTableBuilder> _builders;
    internal ReadOnlyCollection<DbTableBuilder> Builders => _builders.AsReadOnly();

    private DatabaseBuilder(DbTableCollection tables = null)
    {
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
                        case DbIdColumn c:
                            {
                                columnCollectionBuilder
                                    .AddTableIdColumnBuilder(c.Id)
                                    .WithLastCommited(c.LastCommited);
                            }
                            break;
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

    public DatabaseBuilder NewTable( string name, Action<DbTableBuilder> action)
    {
        return NewTable(Guid.NewGuid(),name, action);
    }

    public DatabaseBuilder NewTable(Guid id, string name, Action<DbTableBuilder> action)
    {
        if (!DbUtility.IsValidDbObjectName(name, out string error))
            throw new DbBuilderException(error);

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
        if (!DbUtility.IsValidDbObjectName(name, out string error))
            throw new DbBuilderException(error);

        var builder = (DbTableBuilder)_builders.SingleOrDefault(x => x.Name == name);

        if (builder is not null)
            throw new DbBuilderException($"Table with name '{name}' already exists. Only one instance can be created.");

        builder = new DbTableBuilder(id, name, this);

        _builders.Add(builder);

        return builder;
    }

    public DatabaseBuilder WithTable(string name, Action<DbTableBuilder> action)
    {
        if (!DbUtility.IsValidDbObjectName(name, out string error))
            throw new DbBuilderException(error);

        var builder = (DbTableBuilder)_builders.SingleOrDefault(x => x.Name == name);

        if (builder is null)
            throw new DbBuilderException($"Table with name '{name}' not found.");

        action(builder);

        return this;
    }

    public DbTableBuilder WithTableBuilder(string name, Action<DbTableBuilder> action = null)
    {
        if (!DbUtility.IsValidDbObjectName(name, out string error))
            throw new DbBuilderException(error);

        var builder = (DbTableBuilder)_builders.SingleOrDefault(x => x.Name == name);

        if (builder is null)
            throw new DbBuilderException($"Table with name '{name}' not found.");

        if(action != null )
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

    internal void ValidateColumnExists(string columnName)
    {
        //if (!DbUtility.IsValidDbObjectName(columnName, out string error))
        //{
        //    throw new DbBuilderException($"Column name error: {error}");
        //}
        //if (!_columnsBuilder.Builders.Any(c => c.Name == columnName))
        //{
        //    throw new DbBuilderException($"Column with name '{columnName}' was not found.");
        //}
    }

    internal void ValidateColumnsExists(List<string> columnNames)
    {
        //foreach (var columnName in columnNames)
        //{
        //    if (!DbUtility.IsValidDbObjectName(columnName, out string error))
        //    {
        //        throw new DbBuilderException($"Column name error: {error}");
        //    }
        //    if (!_columnsBuilder.Builders.Any(c => c.Name == columnName))
        //    {
        //        throw new DbBuilderException($"Column with name '{columnName}' was not found.");
        //    }
        //}
    }

    internal void ValidateColumnsExists(params string[] columnNames)
    {
        ValidateColumnsExists(new List<string>(columnNames));
    }

    internal void ValidateColumnName(string name, bool isNew = true)
    {
        //if (!DbUtility.IsValidDbObjectName(name, out string error))
        //    throw new DbBuilderException($"Invalid column name '{name}'. {error}");

        //if (name == Constants.DB_TABLE_ID_COLUMN_NAME && isNew)
        //    throw new DbBuilderException("Name 'id' is reserved column name");

        //if (_columnsBuilder.Builders.Any(x => x.Name == name && isNew))
        //    throw new DbBuilderException($"There is already existing column with name '{name}'");
    }
}
