namespace WebVella.Tefter.Database;

public class LongIntegerDatabaseColumnBuilder : DatabaseColumnBuilder
{
    internal LongIntegerDatabaseColumnBuilder(string name, DatabaseBuilder databaseBuilder) 
        : base(name, databaseBuilder)
    {
    }
    internal LongIntegerDatabaseColumnBuilder(Guid id, string name, DatabaseBuilder databaseBuilder)
        : base(id, name, databaseBuilder)
    {
    }

    public LongIntegerDatabaseColumnBuilder WithDefaultValue(long? defaultValue )
    {
        _defaultValue = defaultValue;
        return this;
    }

    public LongIntegerDatabaseColumnBuilder Nullable()
    {
        _isNullable = true;
        return this;
    }

    public LongIntegerDatabaseColumnBuilder NotNullable()
    {
        _isNullable = false;
        return this;
    }
    internal override LongIntegerDatabaseColumnBuilder WithLastCommited(DateTime lastCommited)
    {
        base.WithLastCommited(lastCommited);

        return this;
    }

    internal override LongIntegerDatabaseColumn Build()
    {
        return new LongIntegerDatabaseColumn
        {
            Id = _id,
            DefaultValue = _defaultValue,
            IsNullable = _isNullable,
            Name = _name,   
            Type = DatabaseColumnType.LongInteger,
        }; 
    }
}