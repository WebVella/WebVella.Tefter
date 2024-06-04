namespace WebVella.Tefter.Database;

public class DbGinIndexBuilder : DbIndexBuilder
{
    public DbGinIndexBuilder Name(string name)
    {
        _name = name;
        return this;
    }

    public DbGinIndexBuilder ForColumns(params string[] columnNames)
    {
        if (columnNames == null || columnNames.Length == 0) return this;

        _columns.AddRange(columnNames);
        return this;
    }

    internal override DbGinIndex Build()
    {
        var index = new DbGinIndex { Name = _name };
        foreach (var columnName in _columns)
            index.AddColumn(columnName);

        return index;
    }
}