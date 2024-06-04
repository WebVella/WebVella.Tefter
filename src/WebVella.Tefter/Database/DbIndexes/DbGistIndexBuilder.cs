namespace WebVella.Tefter.Database;

public class DbGistIndexBuilder : DbIndexBuilder
{
    public DbGistIndexBuilder Name(string name)
    {
        _name = name;
        return this;
    }

    public DbGistIndexBuilder ForColumns(params string[] columnNames)
    {
        if (columnNames == null || columnNames.Length == 0) return this;

        _columns.AddRange(columnNames);
        return this;
    }

    internal override DbGistIndex Build()
    {
        var index = new DbGistIndex { Name = _name };
        foreach (var columnName in _columns)
            index.AddColumn(columnName);

        return index;
    }
}