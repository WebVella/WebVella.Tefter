namespace WebVella.Tefter.Database;

public class DbDateTimeColumnBuilder : DbColumnBuilder
{
    public DbDateTimeColumnBuilder(string name, DatabaseBuilder databaseBuilder)
        : base(name, databaseBuilder)
    {
    }
    public DbDateTimeColumnBuilder(Guid id, string name, DatabaseBuilder databaseBuilder)
        : base(id, name, databaseBuilder)
    {
    }

    public DbDateTimeColumnBuilder WithDefaultValue(DateTime? defaultValue)
    {
        _defaultValue = defaultValue;
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
    public DbDateTimeColumnBuilder WithoutAutoDefaultValue()
    {
        _autoDefaultValue = false;
        return this;
    }

    internal override DbDateTimeColumnBuilder WithLastCommited(DateTime lastCommited)
    {
        base.WithLastCommited(lastCommited);

        return this;
    }
    internal override DbDateTimeColumn Build()
    {
        return new DbDateTimeColumn
        {
            Id = _id,
            DefaultValue = _defaultValue,
            IsNullable = _isNullable,
            AutoDefaultValue = _autoDefaultValue,
            Name = _name,
            Type = DbType.DateTime
        };
    }
}