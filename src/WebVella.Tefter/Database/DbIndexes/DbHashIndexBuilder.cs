namespace WebVella.Tefter.Database;

public class DbHashIndexBuilder : DbIndexBuilder
{
    public DbHashIndexBuilder(string name, string tableName, DatabaseBuilder databaseBuilder) 
        : base(name, tableName, databaseBuilder)
    {
    }

    public DbHashIndexBuilder WithColumn(string name)
    {
        //TODO validate name

        _columns.Add(name);
        return this;
    }

    internal override DbHashIndex Build()
    {
        var index = new DbHashIndex
        {
            Name = _name,
        };

        foreach (var columnName in _columns)
            index.AddColumn(columnName);

        return index;
    }
}