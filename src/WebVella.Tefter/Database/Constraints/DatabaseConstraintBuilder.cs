namespace WebVella.Tefter.Database;

public abstract class DatabaseConstraintBuilder
{
    protected DatabaseBuilder _databaseBuilder;
    protected string _name;

    protected List<string> _columns;

	internal string Name => _name;

    internal DatabaseConstraintBuilder(string name, DatabaseBuilder databaseBuilder)
    {
        _name = name;
        _databaseBuilder = databaseBuilder;
        _columns = new List<string>();
    }

    internal abstract DatabaseConstraint Build();
}