namespace WebVella.Tefter.Database;

public abstract class DbIndexBuilder
{
    protected DbTableBuilder _tableBuilder;
    protected string _name;

    protected List<string> _columns;

    internal string Name { get { return _name; } }

    internal DbIndexBuilder(string name, DbTableBuilder tableBuilder)
    {
        _name = name;
        _tableBuilder = tableBuilder;
        _columns = new List<string>();
    }

    internal DbIndexBuilder(DbIndex index, DbTableBuilder tableBuilder)
    {

        _tableBuilder = tableBuilder;
        _name = index.Name;
        _columns = index.Columns.ToList();
    }

    internal abstract DbIndex Build();
}