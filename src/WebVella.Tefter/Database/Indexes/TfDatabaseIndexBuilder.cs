namespace WebVella.Tefter.Database;

public abstract class TfDatabaseIndexBuilder
{
    protected TfDatabaseBuilder _databaseBuilder;
    protected string _tableName;
    protected string _name;

    protected List<string> _columns;

    internal string Name { get { return _name; } }
    internal string TableName { get { return _tableName; } }

    internal TfDatabaseIndexBuilder(string name, string tableName, TfDatabaseBuilder databaseBuilder)
    {
        _name = name;
        _databaseBuilder = databaseBuilder;
        _columns = new List<string>();
    }

    internal abstract TfDatabaseIndex Build();
}