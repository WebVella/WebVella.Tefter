namespace WebVella.Tefter.Database;

public abstract class DbIndexBuilder
{
    protected DatabaseBuilder _databaseBuilder;
    protected string _tableName;
    protected string _name;

    protected List<string> _columns;

    internal string Name { get { return _name; } }
    internal string TableName { get { return _tableName; } }

    internal DbIndexBuilder(string name, string tableName, DatabaseBuilder databaseBuilder)
    {
        _name = name;
        _databaseBuilder = databaseBuilder;
        _columns = new List<string>();
    }

    internal abstract DbIndex Build();
}