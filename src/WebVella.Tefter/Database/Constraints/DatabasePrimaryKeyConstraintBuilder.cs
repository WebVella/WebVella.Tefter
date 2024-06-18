namespace WebVella.Tefter.Database;

public class DatabasePrimaryKeyConstraintBuilder : DatabaaseConstraintBuilder
{
    internal DatabasePrimaryKeyConstraintBuilder(string name, DatabaseBuilder databaseBuilder)
        : base(name, databaseBuilder)
    {
    }

    public DatabasePrimaryKeyConstraintBuilder WithColumns(params string[] columnNames)
    {
        if (columnNames == null || columnNames.Length == 0) return this;

        //TODO validate

        _columns.AddRange(columnNames);
        return this;
    }

    internal override DatabasePrimaryKeyConstraint Build()
    {
        var constraint = new DatabasePrimaryKeyConstraint
        {
            Name = _name,
        };

        foreach (var columnName in _columns)
            constraint.AddColumn(columnName);

        return constraint;
    }
}