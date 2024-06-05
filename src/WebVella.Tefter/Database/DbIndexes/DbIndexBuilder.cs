namespace WebVella.Tefter.Database;

public abstract class DbIndexBuilder
{
    protected bool _isNew;
    protected string _name;
    protected List<string> _columns;
    protected DbTableBuilder _tableBuilder;

    internal string Name { get { return _name; } }
   
    internal DbIndexBuilder(string name, bool isNew, DbTableBuilder tableBuilder)
    {
        _name = name;
        _isNew = isNew;
        _tableBuilder = tableBuilder;
        _columns = new List<string>();
    }
    internal abstract DbIndex Build();
}