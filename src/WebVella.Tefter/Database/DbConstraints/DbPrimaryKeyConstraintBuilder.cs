namespace WebVella.Tefter.Database;

public class DbPrimaryKeyConstraintBuilder : DbConstraintBuilder
{
    internal DbPrimaryKeyConstraintBuilder(string name, bool isNew, DbTableBuilder tableBuilder)
        : base(name, isNew, tableBuilder)
    {
    }

    internal DbPrimaryKeyConstraintBuilder(DbPrimaryKeyConstraint constraint, DbTableBuilder tableBuilder)
      : base(constraint.Name, constraint.IsNew, tableBuilder)
    {
        _columns.AddRange(constraint.Columns);
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
            IsNew = _isNew
        };

        foreach (var columnName in _columns)
            constraint.AddColumn(columnName);

        return constraint; 
    }
}