namespace WebVella.Tefter.Database;

public class DbGuidColumnBuilder : DbColumnBuilder
{
    private bool _autoDefaultValue = false;

    internal DbGuidColumnBuilder(string name, bool isNew, DbTableBuilder tableBuilder)
        : base(name, isNew, tableBuilder)
    {
    }

    internal DbGuidColumnBuilder(DbGuidColumn column, DbTableBuilder tableBuilder)
        : base(column.Name, column.IsNew, tableBuilder)
    {
        _isNullable = column.IsNullable;
        _defaultValue = column.DefaultValue;
        _autoDefaultValue = column.AutoDefaultValue;
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
        return new DbGuidColumn
        {
            DefaultValue = null,
            IsNullable = false,
            AutoDefaultValue = _autoDefaultValue,
            Name = _name,
            Type = DbType.Guid
        };
    }
}