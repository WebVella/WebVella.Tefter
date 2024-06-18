namespace WebVella.Tefter.Database;

public class BTreeDatabaseIndexBuilder : DatabaseIndexBuilder
{
    internal BTreeDatabaseIndexBuilder(string name, string tableName, DatabaseBuilder databaseBuilder) 
        : base(name, tableName, databaseBuilder)
    {
    }

    public BTreeDatabaseIndexBuilder WithColumns( params string[] columnNames)
    {
        if(columnNames == null || columnNames.Length == 0) return this;

        //TODO validate columns

        _columns.AddRange(columnNames);
        return this;
    }

    internal override BTreeDatabaseIndex Build()
    {
        var index = new BTreeDatabaseIndex 
        {
            Name = _name,
        };

        foreach (var columnName in _columns)
            index.AddColumn(columnName);

        return index;
    }
}