namespace WebVella.Tefter.Database;

public class ShortIntegerDatabaseColumnBuilder : DatabaseColumnBuilder
{
    internal ShortIntegerDatabaseColumnBuilder(string name, DatabaseBuilder databaseBuilder) 
        : base(name, databaseBuilder)
    {
    }
    internal ShortIntegerDatabaseColumnBuilder(Guid id, string name, DatabaseBuilder databaseBuilder)
        : base(id, name, databaseBuilder)
    {
    }

    public ShortIntegerDatabaseColumnBuilder WithDefaultValue(short? defaultValue )
    {
        _defaultValue = defaultValue;
        return this;
    }

    public ShortIntegerDatabaseColumnBuilder Nullable()
    {
        _isNullable = true;
        return this;
    }

    public ShortIntegerDatabaseColumnBuilder NotNullable()
    {
        _isNullable = false;
        return this;
    }
    internal override ShortIntegerDatabaseColumnBuilder WithLastCommited(DateTime lastCommited)
    {
        base.WithLastCommited(lastCommited);

        return this;
    }

    internal override ShortIntegerDatabaseColumn Build()
    {
        return new ShortIntegerDatabaseColumn
        {
            Id = _id,
            DefaultValue = _defaultValue,
            IsNullable = _isNullable,
            Name = _name,   
            Type = DatabaseColumnType.ShortInteger,
        }; 
    }
}