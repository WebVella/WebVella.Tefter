namespace WebVella.Tefter.Database;

public class DbGinIndexBuilder : DbIndexBuilder
{
    public DbGinIndexBuilder(string name, DbTableBuilder tableBuilder)
        : base(name, tableBuilder)
    {
    }

    internal DbGinIndexBuilder(DbGinIndex index, DbTableBuilder tableBuilder)
        : base(index, tableBuilder)
    {
    }

    public DbGinIndexBuilder WithColumns(params string[] columnNames)
    {
        if (columnNames == null || columnNames.Length == 0) return this;

        //TODO validate

        _columns.AddRange(columnNames);
        return this;
    }

    internal override DbGinIndex Build()
    {
        CalculateState();
        var index = new DbGinIndex
        {
            Name = _name,
            State = _state
        };

        foreach (var columnName in _columns)
            index.AddColumn(columnName);

        return index;
    }
}