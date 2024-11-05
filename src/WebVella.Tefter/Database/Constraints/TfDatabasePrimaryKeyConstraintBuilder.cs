namespace WebVella.Tefter.Database;

public class TfDatabasePrimaryKeyConstraintBuilder : TfDatabaseConstraintBuilder
{
    internal TfDatabasePrimaryKeyConstraintBuilder(string name, TfDatabaseBuilder databaseBuilder)
        : base(name, databaseBuilder)
    {
    }

    public TfDatabasePrimaryKeyConstraintBuilder WithColumns(params string[] columnNames)
    {
        if (columnNames == null || columnNames.Length == 0) return this;

        //TODO validate

        _columns.AddRange(columnNames);
        return this;
    }

    internal override TfDatabasePrimaryKeyConstraint Build()
    {
        var constraint = new TfDatabasePrimaryKeyConstraint
        {
            Name = _name,
        };

        foreach (var columnName in _columns)
            constraint.AddColumn(columnName);

        return constraint;
    }
}