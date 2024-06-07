namespace WebVella.Tefter.Database;

public class DbBooleanColumnBuilder : DbColumnBuilder
{
    internal DbBooleanColumnBuilder(string name, DatabaseBuilder databaseBuilder)
        : base(name, databaseBuilder)
    {
    }
    internal DbBooleanColumnBuilder(Guid id, string name, DatabaseBuilder databaseBuilder)
        : base(id, name, databaseBuilder)
    {
    }

    public DbBooleanColumnBuilder WithDefaultValue(bool? defaultValue)
    {
        _defaultValue = defaultValue;
        return this;
    }

    public DbBooleanColumnBuilder Nullable()
    {
        _isNullable = true;
        return this;
    }

    public DbBooleanColumnBuilder NotNullable()
    {
        _isNullable = false;
        return this;
    }

    internal override DbBooleanColumn Build()
    {
        return new DbBooleanColumn
        {
            Id = _id,
            DefaultValue = _defaultValue,
            IsNullable = _isNullable,
            Name = _name,
            Type = DbType.Boolean
        };
    }
}