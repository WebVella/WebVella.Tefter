namespace WebVella.Tefter.Database;

public class DbForeignKeyConstraintBuilder : DbConstraintBuilder
{
    protected string _foreignTableName = string.Empty;
    protected List<string> _foreignColumns = new List<string>();

    internal DbForeignKeyConstraintBuilder(string name, bool isNew, DbTableBuilder tableBuilder)
        : base(name, isNew, tableBuilder)
    {
    }

    public DbForeignKeyConstraintBuilder WithColumns(params string[] columnNames)
    {
        if (columnNames == null || columnNames.Length == 0) return this;

        //TODO validate

        _columns.AddRange(columnNames);
        return this;
    }

    public DbForeignKeyConstraintBuilder WithForeignTable(string foreignTableName)
    {
        //TODO validate

        _foreignTableName = foreignTableName;
        return this;
    }

    public DbForeignKeyConstraintBuilder WithForeignColumns(params string[] columnNames)
    {
        if (columnNames == null || columnNames.Length == 0) return this;

        //TODO validate

        _foreignColumns.AddRange(columnNames);
        return this;
    }

    internal override DbForeignKeyConstraint Build()
    {
        var constraint = new DbForeignKeyConstraint 
        {
            IsNew = _isNew,
            Name = _name, 
            ForeignTable= _foreignTableName 
        };
      
        foreach (var columnName in _columns)
            constraint.AddColumn(columnName);
       
        foreach (var columnName in _foreignColumns)
            constraint.AddForeignColumn(columnName);

        return constraint; 
    }
}