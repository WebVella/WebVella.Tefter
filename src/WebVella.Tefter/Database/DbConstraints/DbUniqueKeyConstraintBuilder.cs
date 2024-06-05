namespace WebVella.Tefter.Database;

public class DbUniqueKeyConstraintBuilder : DbConstraintBuilder
{
    internal DbUniqueKeyConstraintBuilder(string name, bool isNew, DbTableBuilder tableBuilder)
        : base(name, isNew, tableBuilder)
    {
    }
    internal DbUniqueKeyConstraintBuilder(DbUniqueKeyConstraint constraint, DbTableBuilder tableBuilder)
    : base(constraint.Name, constraint.IsNew, tableBuilder)
    {
        _columns.AddRange(constraint.Columns);
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
            IsNew = _isNew,
        };

        foreach (var columnName in _columns)
            constraint.AddColumn(columnName);

        return constraint;
    }

}