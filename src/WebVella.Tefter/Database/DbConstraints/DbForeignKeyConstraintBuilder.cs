namespace WebVella.Tefter.Database;

public class DbForeignKeyConstraintBuilder : DbConstraintBuilder
{
    protected string _foreignTableName = string.Empty;
    protected List<string> _foreignColumns = new List<string>();

    public DbForeignKeyConstraintBuilder Name(string name)
    {
        _name = name;
        return this;
    }

    public DbForeignKeyConstraintBuilder ForColumns(params string[] columnNames)
    {
        if (columnNames == null || columnNames.Length == 0) return this;

        _columns.AddRange(columnNames);
        return this;
    }

    public DbForeignKeyConstraintBuilder ForForeignTable(string foreignTableName)
    {
        _foreignTableName = foreignTableName;
        return this;
    }

    public DbForeignKeyConstraintBuilder ForForeignColumns(params string[] columnNames)
    {
        if (columnNames == null || columnNames.Length == 0) return this;

        _foreignColumns.AddRange(columnNames);
        return this;
    }

    internal override DbForeignKeyConstraint Build()
    {
        var constraint = new DbForeignKeyConstraint { Name = _name, ForeignTable= _foreignTableName };
        foreach (var columnName in _columns)
            constraint.AddColumn(columnName);
        foreach (var columnName in _foreignColumns)
            constraint.AddForeignColumn(columnName);

        return constraint; 
    }
}