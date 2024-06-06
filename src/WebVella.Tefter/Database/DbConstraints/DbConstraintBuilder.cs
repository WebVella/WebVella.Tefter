namespace WebVella.Tefter.Database;

public abstract class DbConstraintBuilder
{
    protected DbObjectState _state;
    protected DbTableBuilder _tableBuilder;
    protected string _name;

    protected List<string> _columns;
    protected List<string> _originalColumns;

    internal string Name { get { return _name; } }
    internal DbObjectState State { get { return _state; } set { _state = value; } }

    internal DbConstraintBuilder(string name, DbTableBuilder tableBuilder)
    {
        _name = name;
        _state = DbObjectState.New;
        _tableBuilder = tableBuilder;
        _columns = new List<string>();
        _originalColumns = new List<string>();
    }

    internal DbConstraintBuilder(DbConstraint constraint, DbTableBuilder tableBuilder)
    {
        if (constraint.State != DbObjectState.Commited)
            throw new DbBuilderException("Only committed constraints can use this constructor");

        _tableBuilder = tableBuilder;
        _name = constraint.Name;
        _state = constraint.State;
        _columns = constraint.Columns.ToList();
        _originalColumns = constraint.Columns.ToList();
    }

    internal virtual void CalculateState()
    {
        if (_state != DbObjectState.Commited)
            return;

        if (_originalColumns.Count() != _columns.Count())
        {
            _state = DbObjectState.Changed;
            return;
        }

        for (int i = 0; i < _columns.Count(); i++)
        {
            if (_columns[i] != _originalColumns[i])
            {
                _state = DbObjectState.Changed;
                return;
            }
        }
    }

    internal abstract DbConstraint Build();
}