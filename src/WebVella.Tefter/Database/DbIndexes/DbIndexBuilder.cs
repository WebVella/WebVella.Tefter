namespace WebVella.Tefter.Database;

public abstract class DbIndexBuilder
{
    protected DbTableBuilder _tableBuilder;
    protected DbObjectState _state;
    protected string _name;

    protected List<string> _columns;
    protected List<string> _originalColumns;

    internal string Name { get { return _name; } }
    internal DbObjectState State { get { return _state; } set { _state = value; } }

    internal DbIndexBuilder(string name, DbTableBuilder tableBuilder)
    {
        _name = name;
        _state = DbObjectState.New;
        _tableBuilder = tableBuilder;
        _columns = new List<string>();
    }

    internal DbIndexBuilder(DbIndex index, DbTableBuilder tableBuilder)
    {
        if (index.State != DbObjectState.Commited)
            throw new DbBuilderException("Only committed indexes can use this constructor");

        _tableBuilder = tableBuilder;
        _name = index.Name;
        _state = index.State;
        _columns = index.Columns.ToList();
        _originalColumns = index.Columns.ToList();
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

    internal abstract DbIndex Build();
}