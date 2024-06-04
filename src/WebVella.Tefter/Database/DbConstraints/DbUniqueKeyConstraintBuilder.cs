namespace WebVella.Tefter.Database;

public class DbUniqueKeyConstraintBuilder : DbConstraintBuilder
{
    public DbUniqueKeyConstraintBuilder Name(string name)
    {
        _name = name;
        return this;
    }

    public DbUniqueKeyConstraintBuilder ForColumns(params string[] columnNames)
    {
        if (columnNames == null || columnNames.Length == 0) return this;

        _columns.AddRange(columnNames);
        return this;
    }


    internal override DbUniqueKeyConstraint Build()
    {
        var constraint = new DbUniqueKeyConstraint { Name = _name };
        foreach (var columnName in _columns)
            constraint.AddColumn(columnName);

        return constraint;
    }

}