namespace WebVella.Tefter.Database;

public class DbGinIndexBuilder : DbIndexBuilder
{
    public DbGinIndexBuilder(string name, bool isNew, DbTableBuilder tableBuilder) 
        : base(name, isNew, tableBuilder)
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
        var index = new DbGinIndex 
        { 
            Name = _name,
            IsNew = _isNew
        };

        foreach (var columnName in _columns)
            index.AddColumn(columnName);

        return index;
    }
}