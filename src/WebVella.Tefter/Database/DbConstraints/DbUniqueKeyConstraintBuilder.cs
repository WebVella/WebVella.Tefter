namespace WebVella.Tefter.Database;

public class DbUniqueKeyConstraintBuilder : DbConstraintBuilder
{
    internal DbUniqueKeyConstraintBuilder(string name, DbTableBuilder tableBuilder)
        : base(name, tableBuilder)
    {
    }
    internal DbUniqueKeyConstraintBuilder(DbUniqueKeyConstraint constraint, DbTableBuilder tableBuilder)
        : base(constraint, tableBuilder)
    {
    }

    public DbUniqueKeyConstraintBuilder WithColumns(params string[] columnNames)
    {
        if (columnNames == null || columnNames.Length == 0) return this;

        //TODO validate

        _columns.AddRange(columnNames);
        return this;
    }


    internal override DbUniqueKeyConstraint Build()
    {
        var constraint = new DbUniqueKeyConstraint
        {
            Name = _name,
        };

        foreach (var columnName in _columns)
            constraint.AddColumn(columnName);

        return constraint;
    }

}