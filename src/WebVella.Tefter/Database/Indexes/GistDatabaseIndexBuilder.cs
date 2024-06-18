namespace WebVella.Tefter.Database;

public class GistDatabaseIndexBuilder : DatabaseIndexBuilder
{
    public GistDatabaseIndexBuilder(string name, string tableName, DatabaseBuilder databaseBuilder)
        : base(name, tableName, databaseBuilder)
    {
    }

    public GistDatabaseIndexBuilder WithColumns(params string[] columnNames)
    {
        if (columnNames == null || columnNames.Length == 0) return this;

        _columns.AddRange(columnNames);
        return this;
    }

    internal override GistDatabaseIndex Build()
    {
        var index = new GistDatabaseIndex
        {
            Name = _name,
        };

        foreach (var columnName in _columns)
            index.AddColumn(columnName);

        return index;
    }
}