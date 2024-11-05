namespace WebVella.Tefter.Database;

public class TfBTreeDatabaseIndexBuilder : TfDatabaseIndexBuilder
{
    internal TfBTreeDatabaseIndexBuilder(string name, string tableName, TfDatabaseBuilder databaseBuilder) 
        : base(name, tableName, databaseBuilder)
    {
    }

    public TfBTreeDatabaseIndexBuilder WithColumns( params string[] columnNames)
    {
        if(columnNames == null || columnNames.Length == 0) return this;

        //TODO validate columns

        _columns.AddRange(columnNames);
        return this;
    }

    internal override TfBTreeDatabaseIndex Build()
    {
        var index = new TfBTreeDatabaseIndex 
        {
            Name = _name,
        };

        foreach (var columnName in _columns)
            index.AddColumn(columnName);

        return index;
    }
}