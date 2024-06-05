namespace WebVella.Tefter.Database;

public class DbDateTimeColumnBuilder : DbColumnBuilder
{
    private bool _autoDefaultValue = false;

    public DbDateTimeColumnBuilder(string name, bool isNew, DbTableBuilder tableBuilder) 
        : base(name, isNew, tableBuilder)
    {
    }

    public DbDateTimeColumnBuilder WithDefaultValue(DateTime? devaultValue)
    {
        _defaultValue = devaultValue;
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
        return new DbDateTimeColumn
        {
            DefaultValue = _defaultValue,
            IsNullable = _isNullable,
            AutoDefaultValue = _autoDefaultValue,
            IsNew = _isNew,
            Name = _name,
            Type = DbType.DateTime
        };
    }
}