using static Npgsql.Replication.PgOutput.Messages.RelationMessage;

namespace WebVella.Tefter.Database;

public class DbTableBuilder
{
    private Guid _id = Guid.Empty;
    private Guid? _applicationId = null;
    private Guid? _dataProviderId = null;
    private string _name = string.Empty;
    private readonly DbColumnCollectionBuilder _columnsBuilder;
    private readonly DbConstraintCollectionBuilder _constraintsBuilder;
    private readonly DbIndexCollectionBuilder _indexesBuilder;

    internal DbColumnCollectionBuilder ColumnsCollectionBuilder { get { return _columnsBuilder; } }
    internal DbConstraintCollectionBuilder ConstraintsCollectionBuilder { get { return _constraintsBuilder; } }
    internal DbIndexCollectionBuilder IndexesCollectionBuilder { get { return _indexesBuilder; } }

    private DbTableBuilder()
    {
        _columnsBuilder = new DbColumnCollectionBuilder(this);
        _constraintsBuilder = new DbConstraintCollectionBuilder(this);
        _indexesBuilder = new DbIndexCollectionBuilder(this);
    }

    public static DbTableBuilder New(string name, Guid? id = null)
    {
        return new DbTableBuilder { _id = id is null ? Guid.NewGuid() : id.Value, _name = name };
    }

    public static DbTableBuilder FromTable(DbTable table)
    {
        //TODO implement
        //var builder = new DbTableBuilder()
        //    .WithApplicationId(table.Meta.ApplicationId)
        return new DbTableBuilder();
    }

    public DbTableBuilder WithApplicationId(Guid appId)
    {
        _applicationId = appId;
        return this;
    }

    public DbTableBuilder WithDataProviderId(Guid dataProviderId)
    {
        _dataProviderId = dataProviderId;
        return this;
    }

    public DbTableBuilder WithColumns(Action<DbColumnCollectionBuilder> action)
    {
        action(_columnsBuilder);
        return this;
    }

    public DbTableBuilder WithIndexes(Action<DbIndexCollectionBuilder> action)
    {
        action(_indexesBuilder);
        return this;
    }

    public DbTableBuilder WithConstraints(Action<DbConstraintCollectionBuilder> action)
    {
        action(_constraintsBuilder);
        return this;
    }

    public DbTable Build()
    {
        return new DbTable
        {
            Id = _id,
            ApplicationId = _applicationId,
            DataProviderId = _dataProviderId,
            Name = _name,
            Columns = _columnsBuilder.Build(),
            Indexes = _indexesBuilder.Build(),
            Constraints = _constraintsBuilder.Build()
        };
    }

    internal void ValidateColumnExists(string columnName)
    {
        if (!DbUtility.IsValidDbObjectName(columnName, out string error))
        {
            throw new DbBuilderException($"Constraint column name error: {error}");
        }
        if (! _columnsBuilder.Builders.Any(c => c.Name == columnName))
        {
            throw new DbBuilderException($"Constraint column with name '{columnName}' was not found.");
        }
    }

    internal void ValidateColumnsExists(List<string> columnNames )
    {
        foreach (var columnName in columnNames)
        {
            if (!DbUtility.IsValidDbObjectName(columnName, out string error))
            {
                throw new DbBuilderException($"Constraint column name error: {error}");
            }
            if (!_columnsBuilder.Builders.Any(c => c.Name == columnName))
            {
                throw new DbBuilderException($"Constraint column with name '{columnName}' was not found.");
            }
        }
    }

    internal void ValidateColumnsExists(string[] columnNames)
    {
        ValidateColumnsExists(new List<string>(columnNames));
    }

    internal void ValidateColumnName(string name, bool isNew = true)
    {
        if (!DbUtility.IsValidDbObjectName(name, out string error))
            throw new DbBuilderException($"Invalid column name '{name}'. {error}");

        if (name == Constants.DB_TABLE_ID_COLUMN_NAME && isNew)
            throw new DbBuilderException("Name 'id' is reserved column name");

        if (_columnsBuilder.Builders.Any(x => x.Name == name) && isNew)
            throw new DbBuilderException($"There is already existing column with name '{name}'");
    }
}