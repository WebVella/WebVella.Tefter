namespace WebVella.Tefter.Database;

public class TfGinDatabaseIndexBuilder : TfDatabaseIndexBuilder
{
    public TfGinDatabaseIndexBuilder(string name, string tableName, TfDatabaseBuilder databaseBuilder)
        : base(name, tableName, databaseBuilder)
    {
    }

    public TfGinDatabaseIndexBuilder WithColumns(params string[] columnNames)
    {
        if (columnNames == null || columnNames.Length == 0) return this;

        //TODO validate

        _columns.AddRange(columnNames);
        return this;
    }

    internal override TfGinDatabaseIndex Build()
    {
        var index = new TfGinDatabaseIndex
        {
            Name = _name,
        };

        foreach (var columnName in _columns)
            index.AddColumn(columnName);

        return index;
    }
}