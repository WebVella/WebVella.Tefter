namespace WebVella.Tefter.Database;

public class DbDateColumnBuilder : DbColumnBuilder
{
    public DbDateColumnBuilder(string name, DatabaseBuilder databaseBuilder) 
        : base(name, databaseBuilder)
    {
    }

    public DbDateColumnBuilder(Guid id, string name, DatabaseBuilder databaseBuilder)
       : base(id, name, databaseBuilder)
    {
    }

    public DbDateColumnBuilder WithDefaultValue(DateOnly? defaultValue)
    {
        _defaultValue = defaultValue;
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

    public DbDateColumnBuilder WithoutAutoDefaultValue()
    {
        _autoDefaultValue = false;
        return this;
    }

    internal override DbDateColumn Build()
    {
        return new DbDateColumn
        {
            Id = _id,
            DefaultValue = _defaultValue,
            IsNullable = _isNullable,
            AutoDefaultValue = _autoDefaultValue,
            Name = _name,
            Type = DbType.Date
        };
    }
}