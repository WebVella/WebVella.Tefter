namespace WebVella.Tefter.Database;

public class DbBTreeIndexBuilder : DbIndexBuilder
{
    public DbBTreeIndexBuilder Name(string name)
    {
        _name = name;
        return this;
    }

    public DbBTreeIndexBuilder ForColumns( params string[] columnNames)
    {
        if(columnNames == null || columnNames.Length == 0) return this;

        _columns.AddRange(columnNames);
        return this;
    }

    internal override DbBTreeIndex Build()
    {
        var index = new DbBTreeIndex { Name = _name };
        foreach (var columnName in _columns)
            index.AddColumn(columnName);

        return index;
    }
}