namespace WebVella.Tefter.Database;

public class DbHashIndexBuilder : DbIndexBuilder
{
    public DbHashIndexBuilder(string name, DbTableBuilder tableBuilder)
        : base(name, tableBuilder)
    {
    }

    internal DbHashIndexBuilder(DbHashIndex index, DbTableBuilder tableBuilder)
        : base(index, tableBuilder)
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
        CalculateState();
        var index = new DbHashIndex
        {
            Name = _name,
            State = _state,
        };

        foreach (var columnName in _columns)
            index.AddColumn(columnName);

        return index;
    }
}