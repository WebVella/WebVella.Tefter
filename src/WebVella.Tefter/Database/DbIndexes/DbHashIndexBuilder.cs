namespace WebVella.Tefter.Database;

public class DbHashIndexBuilder : DbIndexBuilder
{
    public DbHashIndexBuilder Name(string name)
    {
        _name = name;
        return this;
    }

    public DbHashIndexBuilder ForColumn(string name)
    {
        _columns.Clear();
        _columns.Add(name);
        return this;
    }

    internal override DbHashIndex Build()
    {
        var index = new DbHashIndex { Name = _name };
        foreach (var columnName in _columns)
            index.AddColumn(columnName);

        return index;
    }
}