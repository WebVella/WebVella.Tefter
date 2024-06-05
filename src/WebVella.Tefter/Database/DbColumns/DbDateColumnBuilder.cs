namespace WebVella.Tefter.Database;

public class DbDateColumnBuilder : DbColumnBuilder
{
    private bool _autoDefaultValue = false;

    public DbDateColumnBuilder(string name, bool isNew, DbTableBuilder tableBuilder) 
        : base(name, isNew, tableBuilder)
    {
    }
    internal DbDateColumnBuilder(DbDateColumn column, DbTableBuilder tableBuilder)
      : base(column.Name, column.IsNew, tableBuilder)
    {
        _isNullable = column.IsNullable;
        _defaultValue = column.DefaultValue;
        _autoDefaultValue = column.AutoDefaultValue;
    }

    public DbDateColumnBuilder WithDefaultValue(DateOnly? devaultValue)
    {
        _defaultValue = devaultValue;
        return this;
    }

    public DbDateColumnBuilder Nullable()
    {
        _isNullable = true;
        return this;
    }

    public DbDateColumnBuilder NotNullable()
    {
        _isNullable = false;
        return this;
    }

    public DbDateColumnBuilder WithAutoDefaultValue()
    {
        _autoDefaultValue = true;
        return this;
    }

    public DbDateColumnBuilder WithNoAutoDefaultValue()
    {
        _autoDefaultValue = false;
        return this;
    }

    internal override DbDateColumn Build()
    {
        return new DbDateColumn
        {
            DefaultValue = _defaultValue,
            IsNullable = _isNullable,
            AutoDefaultValue = _autoDefaultValue,
            Name = _name,
            Type = DbType.Date
        };
    }
}