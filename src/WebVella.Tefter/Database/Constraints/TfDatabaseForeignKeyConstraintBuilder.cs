namespace WebVella.Tefter.Database;

public class TfDatabaseForeignKeyConstraintBuilder : TfDatabaseConstraintBuilder
{
    protected string _foreignTableName = string.Empty;
    protected List<string> _foreignColumns = new List<string>();

    internal TfDatabaseForeignKeyConstraintBuilder(string name, TfDatabaseBuilder databaseBuilder)
        : base(name, databaseBuilder)
    {
    }

    public TfDatabaseForeignKeyConstraintBuilder WithColumns(params string[] columnNames)
    {
        if (columnNames == null || columnNames.Length == 0) return this;

        //TODO validate

        _columns.AddRange(columnNames);
        return this;
    }

    public TfDatabaseForeignKeyConstraintBuilder WithForeignTable(string foreignTableName)
    {
        //TODO validate

        _foreignTableName = foreignTableName;
        return this;
    }

    public TfDatabaseForeignKeyConstraintBuilder WithForeignColumns(params string[] columnNames)
    {
        if (columnNames == null || columnNames.Length == 0) return this;

        //TODO validate

        _foreignColumns.AddRange(columnNames);
        return this;
    }

    internal override TfDatabaseForeignKeyConstraint Build()
    {
        var constraint = new TfDatabaseForeignKeyConstraint
        {
            Name = _name,
            ForeignTable = _foreignTableName
        };

        foreach (var columnName in _columns)
            constraint.AddColumn(columnName);

        foreach (var columnName in _foreignColumns)
            constraint.AddForeignColumn(columnName);

        return constraint;
    }
}