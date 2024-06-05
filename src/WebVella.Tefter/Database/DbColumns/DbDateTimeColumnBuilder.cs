namespace WebVella.Tefter.Database;

public class DbDateTimeColumnBuilder : DbColumnBuilder
{
    private bool _useCurrentTimeAsDefaultValue = false;

    public DbDateTimeColumnBuilder Id(Guid id)
    {
        _id = id;
        return this;
    }

    public DbDateTimeColumnBuilder Name(string name)
    {
        _name = name;
        return this;
    }

    public DbDateTimeColumnBuilder DefaultValue(DateTime? devaultValue)
    {
        _defaultValue = devaultValue;
        return this;
    }

    public DbDateTimeColumnBuilder IsNullable(bool isNullable)
    {
        _isNullable = isNullable;
        return this;
    }

    public DbDateTimeColumnBuilder UseCurrentTimeAsDefaultValue(bool useCurrentTimeAsDefaultValue)
    {
        _useCurrentTimeAsDefaultValue = useCurrentTimeAsDefaultValue;
        return this;
    }

    internal override DbDateTimeColumn Build()
    {
        return new DbDateTimeColumn
        {
            Id = _id,
            DefaultValue = _defaultValue,
            IsNullable = _isNullable,
            UseCurrentTimeAsDefaultValue = _useCurrentTimeAsDefaultValue,
            Name = _name,
            Type = DbType.DateTime
        };
    }
}