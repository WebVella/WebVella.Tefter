namespace WebVella.Tefter.Database;

public class DbPrimaryKeyConstraintBuilder : DbConstraintBuilder
{
    public DbPrimaryKeyConstraintBuilder Name(string name)
    {
        _name = name;
        return this;
    }

    public DbPrimaryKeyConstraintBuilder ForColumns(params string[] columnNames)
    {
        if (columnNames == null || columnNames.Length == 0) return this;

        _columns.AddRange(columnNames);
        return this;
    }

    internal override DbPrimaryKeyConstraint Build()
    {
        var constraint = new DbPrimaryKeyConstraint { Name = _name };
        foreach (var columnName in _columns)
            constraint.AddColumn(columnName);

        return constraint; 
    }
}