namespace WebVella.Tefter.Database;

public class DbGistIndexBuilder : DbIndexBuilder
{
    public DbGistIndexBuilder(string name, DbTableBuilder tableBuilder)
        : base(name, tableBuilder)
    {
    }

    internal DbGistIndexBuilder(DbGistIndex index, DbTableBuilder tableBuilder)
       : base(index, tableBuilder)
    {
    }

    public DbGistIndexBuilder WithColumns(params string[] columnNames)
    {
        if (columnNames == null || columnNames.Length == 0) return this;

        _columns.AddRange(columnNames);
        return this;
    }

    internal override DbGistIndex Build()
    {
        var index = new DbGistIndex
        {
            Name = _name,
        };

        foreach (var columnName in _columns)
            index.AddColumn(columnName);

        return index;
    }
}