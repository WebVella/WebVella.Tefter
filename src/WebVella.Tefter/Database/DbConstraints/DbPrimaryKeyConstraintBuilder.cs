namespace WebVella.Tefter.Database;

public class DbPrimaryKeyConstraintBuilder : DbConstraintBuilder
{
    internal DbPrimaryKeyConstraintBuilder(string name, DatabaseBuilder databaseBuilder)
        : base(name, databaseBuilder)
    {
    }

    public DbPrimaryKeyConstraintBuilder WithColumns(params string[] columnNames)
    {
        if (columnNames == null || columnNames.Length == 0) return this;

        //TODO validate

        _columns.AddRange(columnNames);
        return this;
    }

    internal override DbPrimaryKeyConstraint Build()
    {
        var constraint = new DbPrimaryKeyConstraint
        {
            Name = _name,
        };

        foreach (var columnName in _columns)
            constraint.AddColumn(columnName);

        return constraint;
    }
}