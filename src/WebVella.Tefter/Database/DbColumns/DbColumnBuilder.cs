using Org.BouncyCastle.Tls;
using System;

namespace WebVella.Tefter.Database;

public abstract class DbColumnBuilder
{
    protected DbObjectState _state;
    protected DbTableBuilder _tableBuilder;

    protected string _name;
    protected object _defaultValue = null;
    protected bool _isNullable = true;
    protected bool _autoDefaultValue = false;

    protected object _originalDefaultValue = null;
    protected bool _originalIsNullable = true;
    protected bool _originalAutoDefaultValue = false;

    internal string Name { get { return _name; } }
    internal DbObjectState State { get { return _state; } set { _state = value; } }

    internal DbColumnBuilder(string name, DbObjectState state, DbTableBuilder tableBuilder)
    {
        _name = name;
        _state = state;
        _tableBuilder = tableBuilder;
    }

    internal DbColumnBuilder(DbColumn column, DbTableBuilder tableBuilder)
    {
        if (column.State != DbObjectState.Commited)
            throw new DbBuilderException("Only committed columns can use this constructor");

        _tableBuilder = tableBuilder;
        _name = column.Name;
        _state = column.State;
        _isNullable = column.IsNullable;
        _defaultValue = column.DefaultValue;
        if (column.GetType().IsAssignableFrom(typeof(DbColumnWithAutoDefaultValue)))
        {
            _autoDefaultValue = ((DbColumnWithAutoDefaultValue)column).AutoDefaultValue;
        }
        _originalDefaultValue = _defaultValue;
        _originalAutoDefaultValue = _autoDefaultValue;
        _originalIsNullable &= column.IsNullable;
    }

    internal virtual void CalculateState()
    {
        if (_state != DbObjectState.Commited)
            return;

        if (_originalIsNullable != _isNullable ||
            _originalDefaultValue != _defaultValue ||
            _originalAutoDefaultValue != _autoDefaultValue)
        {
            _state = DbObjectState.Changed;
        }
    }

    internal abstract DbColumn Build();
}