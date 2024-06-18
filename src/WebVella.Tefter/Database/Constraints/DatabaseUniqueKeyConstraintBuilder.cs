namespace WebVella.Tefter.Database;

public class DatabaseUniqueKeyConstraintBuilder : DatabaseConstraintBuilder
{
    internal DatabaseUniqueKeyConstraintBuilder(string name, DatabaseBuilder databaseBuilder)
        : base(name, databaseBuilder)
    {
    }

    public DatabaseUniqueKeyConstraintBuilder WithColumns(params string[] columnNames)
    {
        if (columnNames == null || columnNames.Length == 0) return this;

        //TODO validate

        _columns.AddRange(columnNames);
        return this;
    }


    internal override DatabaseUniqueKeyConstraint Build()
    {
        var constraint = new DatabaseUniqueKeyConstraint
        {
            Name = _name,
        };

        foreach (var columnName in _columns)
            constraint.AddColumn(columnName);

        return constraint;
    }

}