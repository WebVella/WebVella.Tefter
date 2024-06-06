namespace WebVella.Tefter.Database;

public class DbTextColumnBuilder : DbColumnBuilder
{
    internal DbTextColumnBuilder(string name, DbTableBuilder tableBuilder) :
        base(name, tableBuilder)
    {
    }

    internal DbTextColumnBuilder(DbTextColumn column, DbTableBuilder tableBuilder)
        : base(column, tableBuilder)
    {
    }

    public DbTextColumnBuilder WithDefaultValue(string defaultValue)
    {
        _defaultValue = defaultValue;
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
            Id = _id,
            DefaultValue = _defaultValue,
            IsNullable = _isNullable,
            Name = _name,
            Type = DbType.Text
        };
    }
}