namespace WebVella.Tefter.Database;

public abstract class DbConstraintBuilder
{
    protected DbTableBuilder _tableBuilder;
    protected string _name;

    protected List<string> _columns;

    internal string Name { get { return _name; } }

    internal DbConstraintBuilder(string name, DbTableBuilder tableBuilder)
    {
        _name = name;
        _tableBuilder = tableBuilder;
        _columns = new List<string>();
    }

    internal DbConstraintBuilder(DbConstraint constraint, DbTableBuilder tableBuilder)
    {
        _tableBuilder = tableBuilder;
        _name = constraint.Name;
        _columns = constraint.Columns.ToList();
    }

    internal abstract DbConstraint Build();
}