namespace WebVella.Tefter.Database;

public class DbBTreeIndexBuilder : DbIndexBuilder
{
    internal DbBTreeIndexBuilder(string name, bool isNew, DbTableBuilder tableBuilder) 
        : base(name, isNew, tableBuilder)
    {
    }

    internal DbBTreeIndexBuilder(DbBTreeIndex index, DbTableBuilder tableBuilder)
       : base(index.Name, index.IsNew, tableBuilder)
    {
        _columns.AddRange(index.Columns);
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
            IsNew = _isNew
        };

        foreach (var columnName in _columns)
            index.AddColumn(columnName);

        return index;
    }
}