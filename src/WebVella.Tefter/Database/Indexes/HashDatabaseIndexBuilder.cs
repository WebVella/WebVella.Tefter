namespace WebVella.Tefter.Database;

public class HashDatabaseIndexBuilder : DatabaseIndexBuilder
{
    public HashDatabaseIndexBuilder(string name, string tableName, DatabaseBuilder databaseBuilder) 
        : base(name, tableName, databaseBuilder)
    {
    }

    public HashDatabaseIndexBuilder WithColumn(string name)
    {
        //TODO validate name

        _columns.Add(name);
        return this;
    }

    internal override HashDatabaseIndex Build()
    {
        var index = new HashDatabaseIndex
        {
            Name = _name,
        };

        foreach (var columnName in _columns)
            index.AddColumn(columnName);

        return index;
    }
}