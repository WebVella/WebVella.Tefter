namespace WebVella.Tefter.Database;

public class DbTextColumnBuilder : DbColumnBuilder
{
    internal DbTextColumnBuilder(string name, DbTableBuilder tableBuilder) :
        base(name, DbObjectState.New, tableBuilder)
    {
    }

    internal DbTextColumnBuilder(DbTextColumn column, DbTableBuilder tableBuilder)
        : base(column, tableBuilder)
    {
    }

    public DbTextColumnBuilder WithDefaultValue(string defaultValue)
    {
        if (_state == DbObjectState.Commited && (string)_defaultValue != defaultValue)
            _state = DbObjectState.Changed;

        _defaultValue = defaultValue;
        return this;
    }

    public DbTextColumnBuilder Nullable()
    {
        if (_state == DbObjectState.Commited && _isNullable == false)
            _state = DbObjectState.Changed;

        _isNullable = true;
        return this;
    }

    public DbTextColumnBuilder NotNullable()
    {
        if (_state == DbObjectState.Commited && _isNullable == true)
            _state = DbObjectState.Changed;

        _isNullable = false;
        return this;
    }

    internal override DbTextColumn Build()
    {
        CalculateState();
        return new DbTextColumn
        {
            Id = _id,
            DefaultValue = _defaultValue,
            IsNullable = _isNullable,
            Name = _name,
            State = _state,
            Type = DbType.Text
        };
    }
}