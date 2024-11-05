namespace WebVella.Tefter.Database;

public abstract class TfDatabaseConstraintBuilder
{
    protected TfDatabaseBuilder _databaseBuilder;
    protected string _name;

    protected List<string> _columns;

	internal string Name => _name;

    internal TfDatabaseConstraintBuilder(string name, TfDatabaseBuilder databaseBuilder)
    {
        _name = name;
        _databaseBuilder = databaseBuilder;
        _columns = new List<string>();
    }

    internal abstract TfDatabaseConstraint Build();
}