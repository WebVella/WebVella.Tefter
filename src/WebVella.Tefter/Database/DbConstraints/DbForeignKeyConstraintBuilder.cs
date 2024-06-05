namespace WebVella.Tefter.Database;

public class DbForeignKeyConstraintBuilder : DbConstraintBuilder
{
    private string _originalForeignTableName = string.Empty;
    private List<string> _originalForeignColumns = new List<string>();

    protected string _foreignTableName = string.Empty;
    protected List<string> _foreignColumns = new List<string>();

    internal DbForeignKeyConstraintBuilder(string name, DbTableBuilder tableBuilder)
        : base(name, tableBuilder)
    {
    }

    internal DbForeignKeyConstraintBuilder(DbForeignKeyConstraint constraint, DbTableBuilder tableBuilder)
        : base(constraint, tableBuilder)
    {
        _foreignTableName = constraint.ForeignTable;
        _foreignColumns.AddRange(constraint.ForeignColumns);
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
        CalculateState();
        var constraint = new DbForeignKeyConstraint
        {
            State = _state,
            Name = _name,
            ForeignTable = _foreignTableName
        };

        foreach (var columnName in _columns)
            constraint.AddColumn(columnName);

        foreach (var columnName in _foreignColumns)
            constraint.AddForeignColumn(columnName);

        return constraint;
    }

    internal override void CalculateState()
    {
        base.CalculateState();

        if (_state != DbObjectState.Commited)
            return;

        if (_originalForeignTableName != _foreignTableName)
        {
            _state = DbObjectState.Changed;
            return;
        }

        if (_originalForeignColumns.Count() != _foreignColumns.Count())
        {
            _state = DbObjectState.Changed;
            return;
        }

        for (int i = 0; i < _foreignColumns.Count(); i++)
        {
            if (_foreignColumns[i] != _originalForeignColumns[i])
            {
                _state = DbObjectState.Changed;
                return;
            }
        }
    }
}