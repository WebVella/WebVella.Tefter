namespace WebVella.Tefter.Database;

public class DbHashIndexBuilder : DbIndexBuilder
{
    public DbHashIndexBuilder(string name, bool isNew, DbTableBuilder tableBuilder) 
        : base(name, isNew, tableBuilder)
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
            Name = _name , 
            IsNew = _isNew 
        };

        foreach (var columnName in _columns)
            index.AddColumn(columnName);

        return index;
    }
}