namespace WebVella.Tefter.Database;

public abstract class DbConstraintBuilder
{
    protected DatabaseBuilder _databaseBuilder;
    protected string _name;

    protected List<string> _columns;

    internal string Name { get { return _name; } }

    internal DbConstraintBuilder(string name, DatabaseBuilder databaseBuilder)
    {
        _name = name;
        _databaseBuilder = databaseBuilder;
        _columns = new List<string>();
    }

    internal abstract DbConstraint Build();
}