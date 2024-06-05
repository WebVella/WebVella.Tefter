namespace WebVella.Tefter.Database;

public class DbTextColumnBuilder : DbColumnBuilder
{
    public DbTextColumnBuilder(string name, bool isNew, DbTableBuilder tableBuilder) :
        base(name, isNew, tableBuilder)
    {
    }

    public DbTextColumnBuilder WithDefaultValue(string devaultValue)
    {
        _defaultValue = devaultValue;
        return this;
    }

    public DbTextColumnBuilder Nullable()
    {
        _isNullable = true;
        return this;
    }

    public DbTextColumnBuilder NotNullable()
    {
        _isNullable = false;
        return this;
    }

    internal override DbTextColumn Build()
    {
        return new DbTextColumn
        {
            DefaultValue = _defaultValue,
            IsNullable = _isNullable,
            Name = _name,
            IsNew = _isNew,
            Type = DbType.Text
        };
    }
}