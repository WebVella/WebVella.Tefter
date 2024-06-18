namespace WebVella.Tefter.Database;

public class DatabaseForeignKeyConstraintBuilder : DatabaseConstraintBuilder
{
    protected string _foreignTableName = string.Empty;
    protected List<string> _foreignColumns = new List<string>();

    internal DatabaseForeignKeyConstraintBuilder(string name, DatabaseBuilder databaseBuilder)
        : base(name, databaseBuilder)
    {
    }

    public DatabaseForeignKeyConstraintBuilder WithColumns(params string[] columnNames)
    {
        if (columnNames == null || columnNames.Length == 0) return this;

        //TODO validate

        _columns.AddRange(columnNames);
        return this;
    }

    public DatabaseForeignKeyConstraintBuilder WithForeignTable(string foreignTableName)
    {
        //TODO validate

        _foreignTableName = foreignTableName;
        return this;
    }

    public DatabaseForeignKeyConstraintBuilder WithForeignColumns(params string[] columnNames)
    {
        if (columnNames == null || columnNames.Length == 0) return this;

        //TODO validate

        _foreignColumns.AddRange(columnNames);
        return this;
    }

    internal override DatabaseForeignKeyConstraint Build()
    {
        var constraint = new DatabaseForeignKeyConstraint
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