namespace WebVella.Tefter.Database;

public class DbTextColumnBuilder : DbColumnBuilder
{
    internal DbTextColumnBuilder(string name, bool isNew, DbTableBuilder tableBuilder) :
        base(name, isNew, tableBuilder)
    {
    }

    internal DbTextColumnBuilder(DbTextColumn column, DbTableBuilder tableBuilder)
      : base(column.Name, column.IsNew, tableBuilder)
    {
        _isNullable = column.IsNullable;
        _defaultValue = column.DefaultValue;
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