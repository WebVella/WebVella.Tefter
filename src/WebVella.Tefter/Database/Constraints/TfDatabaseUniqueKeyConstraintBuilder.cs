namespace WebVella.Tefter.Database;

public class TfDatabaseUniqueKeyConstraintBuilder : TfDatabaseConstraintBuilder
{
    internal TfDatabaseUniqueKeyConstraintBuilder(string name, TfDatabaseBuilder databaseBuilder)
        : base(name, databaseBuilder)
    {
    }

    public TfDatabaseUniqueKeyConstraintBuilder WithColumns(params string[] columnNames)
    {
        if (columnNames == null || columnNames.Length == 0) return this;

        //TODO validate

        _columns.AddRange(columnNames);
        return this;
    }


    internal override TfDatabaseUniqueKeyConstraint Build()
    {
        var constraint = new TfDatabaseUniqueKeyConstraint
        {
            Name = _name,
        };

        foreach (var columnName in _columns)
            constraint.AddColumn(columnName);

        return constraint;
    }

}