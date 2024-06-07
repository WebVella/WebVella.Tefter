namespace WebVella.Tefter.Database;

public class DbBTreeIndexBuilder : DbIndexBuilder
{
    internal DbBTreeIndexBuilder(string name, string tableName, DatabaseBuilder databaseBuilder) 
        : base(name, tableName, databaseBuilder)
    {
    }

    public DbBTreeIndexBuilder WithColumns( params string[] columnNames)
    {
        if(columnNames == null || columnNames.Length == 0) return this;

        //TODO validate columns

        _columns.AddRange(columnNames);
        return this;
    }

    internal override DbBTreeIndex Build()
    {
        var index = new DbBTreeIndex 
        {
            Name = _name,
        };

        foreach (var columnName in _columns)
            index.AddColumn(columnName);

        return index;
    }
}