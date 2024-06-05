namespace WebVella.Tefter.Database;

public class DbGistIndexBuilder : DbIndexBuilder
{
    public DbGistIndexBuilder(string name, bool isNew, DbTableBuilder tableBuilder) 
        : base(name, isNew, tableBuilder)
    {
    }

    internal DbGistIndexBuilder(DbGistIndex index, DbTableBuilder tableBuilder)
       : base(index.Name, index.IsNew, tableBuilder)
    {
        _columns.AddRange(index.Columns);
    }

    public DbGistIndexBuilder WithColumns(params string[] columnNames)
    {
        if (columnNames == null || columnNames.Length == 0) return this;

        _columns.AddRange(columnNames);
        return this;
    }

    internal override DbGistIndex Build()
    {
        var index = new DbGistIndex
        {
            Name = _name ,
            IsNew = _isNew 
        };

        foreach (var columnName in _columns)
            index.AddColumn(columnName);

        return index;
    }
}