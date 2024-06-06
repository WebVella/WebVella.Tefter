namespace WebVella.Tefter.Database;

public class DbGuidColumnBuilder : DbColumnBuilder
{
    internal DbGuidColumnBuilder(string name, DbTableBuilder tableBuilder)
        : base(name, tableBuilder)
    {
    }

    internal DbGuidColumnBuilder(Guid id, string name, DbTableBuilder tableBuilder)
        : base(id, name, tableBuilder)
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
        return new DbGuidColumn
        {
            Id = _id,
            DefaultValue = null,
            IsNullable = false,
            AutoDefaultValue = _autoDefaultValue,
            Name = _name,
            Type = DbType.Guid
        };
    }
}