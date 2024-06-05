namespace WebVella.Tefter.Database;

public class DbDateTimeColumnBuilder : DbColumnBuilder
{
    public DbDateTimeColumnBuilder(string name, DbTableBuilder tableBuilder)
        : base(name, DbObjectState.New, tableBuilder)
    {
    }

    internal DbDateTimeColumnBuilder(DbDateTimeColumn column, DbTableBuilder tableBuilder)
        : base(column, tableBuilder)
    {
    }

    public DbDateTimeColumnBuilder WithDefaultValue(DateTime? defaultValue)
    {
        _defaultValue = defaultValue;
        return this;
    }

    public DbDateTimeColumnBuilder Nullable()
    {
        _isNullable = true;
        return this;
    }
    public DbDateTimeColumnBuilder NotNullable()
    {
        _isNullable = false;
        return this;
    }

    public DbDateTimeColumnBuilder WithAutoDefaultValue()
    {
        _autoDefaultValue = true;
        return this;
    }
    public DbDateTimeColumnBuilder WithNoAutoDefaultValue()
    {
        _autoDefaultValue = false;
        return this;
    }

    internal override DbDateTimeColumn Build()
    {
        CalculateState();
        return new DbDateTimeColumn
        {
            DefaultValue = _defaultValue,
            IsNullable = _isNullable,
            AutoDefaultValue = _autoDefaultValue,
            State = _state,
            Name = _name,
            Type = DbType.DateTime
        };
    }
}