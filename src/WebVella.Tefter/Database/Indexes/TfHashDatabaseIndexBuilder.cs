namespace WebVella.Tefter.Database;

public class TfHashDatabaseIndexBuilder : TfDatabaseIndexBuilder
{
    public TfHashDatabaseIndexBuilder(string name, string tableName, TfDatabaseBuilder databaseBuilder) 
        : base(name, tableName, databaseBuilder)
    {
    }

    public TfHashDatabaseIndexBuilder WithColumn(string name)
    {
        //TODO validate name

        _columns.Add(name);
        return this;
    }

    internal override TfHashDatabaseIndex Build()
    {
        var index = new TfHashDatabaseIndex
        {
            Name = _name,
        };

        foreach (var columnName in _columns)
            index.AddColumn(columnName);

        return index;
    }
}