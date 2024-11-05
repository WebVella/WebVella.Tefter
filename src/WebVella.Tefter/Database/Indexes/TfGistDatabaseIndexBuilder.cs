namespace WebVella.Tefter.Database;

public class TfGistDatabaseIndexBuilder : TfDatabaseIndexBuilder
{
    public TfGistDatabaseIndexBuilder(string name, string tableName, TfDatabaseBuilder databaseBuilder)
        : base(name, tableName, databaseBuilder)
    {
    }

    public TfGistDatabaseIndexBuilder WithColumns(params string[] columnNames)
    {
        if (columnNames == null || columnNames.Length == 0) return this;

        _columns.AddRange(columnNames);
        return this;
    }

    internal override TfGistDatabaseIndex Build()
    {
        var index = new TfGistDatabaseIndex
        {
            Name = _name,
        };

        foreach (var columnName in _columns)
            index.AddColumn(columnName);

        return index;
    }
}