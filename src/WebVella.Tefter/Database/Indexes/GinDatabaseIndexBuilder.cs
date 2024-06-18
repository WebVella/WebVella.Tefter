namespace WebVella.Tefter.Database;

public class GinDatabaseIndexBuilder : DatabaseIndexBuilder
{
    public GinDatabaseIndexBuilder(string name, string tableName, DatabaseBuilder databaseBuilder)
        : base(name, tableName, databaseBuilder)
    {
    }

    public GinDatabaseIndexBuilder WithColumns(params string[] columnNames)
    {
        if (columnNames == null || columnNames.Length == 0) return this;

        //TODO validate

        _columns.AddRange(columnNames);
        return this;
    }

    internal override GinDatabaseIndex Build()
    {
        var index = new GinDatabaseIndex
        {
            Name = _name,
        };

        foreach (var columnName in _columns)
            index.AddColumn(columnName);

        return index;
    }
}