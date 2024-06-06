using static Npgsql.Replication.PgOutput.Messages.RelationMessage;

namespace WebVella.Tefter.Database;

public class DbTableBuilder
{
    private Guid _id;
    private Guid? _applicationId;
    private Guid? _dataProviderId;
    private string _name;
    private readonly DbColumnCollectionBuilder _columnsBuilder;
    private readonly DbConstraintCollectionBuilder _constraintsBuilder;
    private readonly DbIndexCollectionBuilder _indexesBuilder;

    internal string Name { get { return _name; } }
    internal DbColumnCollectionBuilder ColumnsCollectionBuilder { get { return _columnsBuilder; } }
    internal DbConstraintCollectionBuilder ConstraintsCollectionBuilder { get { return _constraintsBuilder; } }
    internal DbIndexCollectionBuilder IndexesCollectionBuilder { get { return _indexesBuilder; } }

    internal DbTableBuilder(Guid id, string name)
    {
        _id = id;
        _name = name;
        _applicationId = null;
        _dataProviderId = null;

        _columnsBuilder = new DbColumnCollectionBuilder(this);
        _constraintsBuilder = new DbConstraintCollectionBuilder(this);
        _indexesBuilder = new DbIndexCollectionBuilder(this);
    }

    internal DbTableBuilder(DbTable table)
    {
        _id = table.Id;
        _name = table.Name;
        _applicationId = table.ApplicationId;
        _dataProviderId = table.DataProviderId;

        //TODO implementation pending

        _columnsBuilder = new DbColumnCollectionBuilder(this);
        _constraintsBuilder = new DbConstraintCollectionBuilder(this);
        _indexesBuilder = new DbIndexCollectionBuilder(this);
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
            throw new DbBuilderException($"Column name error: {error}");
        }
        if (!_columnsBuilder.Builders.Any(c => c.Name == columnName))
        {
            throw new DbBuilderException($"Column with name '{columnName}' was not found.");
        }
    }

    internal void ValidateColumnsExists(List<string> columnNames)
    {
        foreach (var columnName in columnNames)
        {
            if (!DbUtility.IsValidDbObjectName(columnName, out string error))
            {
                throw new DbBuilderException($"Column name error: {error}");
            }
            if (!_columnsBuilder.Builders.Any(c => c.Name == columnName))
            {
                throw new DbBuilderException($"Column with name '{columnName}' was not found.");
            }
        }
    }

    internal void ValidateColumnsExists(params string[] columnNames)
    {
        ValidateColumnsExists(new List<string>(columnNames));
    }

    internal void ValidateColumnName(string name, bool isNew = true)
    {
        if (!DbUtility.IsValidDbObjectName(name, out string error))
            throw new DbBuilderException($"Invalid column name '{name}'. {error}");

        if (name == Constants.DB_TABLE_ID_COLUMN_NAME && isNew)
            throw new DbBuilderException("Name 'id' is reserved column name");

        if (_columnsBuilder.Builders.Any(x => x.Name == name && isNew))
            throw new DbBuilderException($"There is already existing column with name '{name}'");
    }
}