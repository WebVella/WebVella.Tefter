namespace WebVella.Tefter.Database;

public class DbDateColumnBuilder : DbColumnBuilder
{
    private bool _useCurrentTimeAsDefaultValue = false;

    public DbDateColumnBuilder Id(Guid id)
    {
        _id = id;
        return this;
    }

    public DbDateColumnBuilder Name(string name)
    {
        _name = name;
        return this;
    }

    public DbDateColumnBuilder DefaultValue(DateOnly? devaultValue)
    {
        _defaultValue = devaultValue;
        return this;
    }

    public DbDateColumnBuilder IsNullable(bool isNullable)
    {
        _isNullable = isNullable;
        return this;
    }

    public DbDateColumnBuilder UseCurrentTimeAsDefaultValue(bool useCurrentTimeAsDefaultValue)
    {
        _useCurrentTimeAsDefaultValue = useCurrentTimeAsDefaultValue;
        return this;
    }

    internal override DbDateColumn Build()
    {
        return new DbDateColumn
        {
            Id = _id,
            DefaultValue = _defaultValue,
            IsNullable = _isNullable,
            UseCurrentTimeAsDefaultValue = _useCurrentTimeAsDefaultValue,
            Name = _name,
            Type = DbType.Date
        };
    }
}