namespace WebVella.Tefter.Database;

public class DbPrimaryKeyConstraintBuilder : DbConstraintBuilder
{
    internal DbPrimaryKeyConstraintBuilder(string name, DbTableBuilder tableBuilder)
        : base(name, tableBuilder)
    {
    }

    internal DbPrimaryKeyConstraintBuilder(DbPrimaryKeyConstraint constraint, DbTableBuilder tableBuilder)
        : base(constraint, tableBuilder)
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