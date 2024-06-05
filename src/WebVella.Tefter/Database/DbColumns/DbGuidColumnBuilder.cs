namespace WebVella.Tefter.Database;

public class DbGuidColumnBuilder : DbColumnBuilder
{
    internal DbGuidColumnBuilder(string name, DbTableBuilder tableBuilder)
        : base(name, DbObjectState.New, tableBuilder)
    {
    }

    internal DbGuidColumnBuilder(DbGuidColumn column, DbTableBuilder tableBuilder)
        : base(column, tableBuilder)
    {
    }

    public DbGuidColumnBuilder WithDefaultValue(Guid? defaultValue)
    {
        _defaultValue = defaultValue;
        return this;
    }

    public DbGuidColumnBuilder Nullable()
    {
        _isNullable = true;
        return this;
    }

    public DbGuidColumnBuilder NotNullable()
    {
        _isNullable = false;
        return this;
    }

    public DbGuidColumnBuilder WithAutoDefaultValue()
    {
        _autoDefaultValue = true;
        return this;
    }

    public DbGuidColumnBuilder WithNoAutoDefaultValue()
    {
        _autoDefaultValue = false;
        return this;
    }

    internal override DbGuidColumn Build()
    {
        CalculateState();
        return new DbGuidColumn
        {
            DefaultValue = null,
            IsNullable = false,
            AutoDefaultValue = _autoDefaultValue,
            Name = _name,
            State = _state,
            Type = DbType.Guid
        };
    }
}