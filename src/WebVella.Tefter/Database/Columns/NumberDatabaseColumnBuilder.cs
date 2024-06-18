namespace WebVella.Tefter.Database;

public class NumberDatabaseColumnBuilder : DatabaseColumnBuilder
{
    internal NumberDatabaseColumnBuilder(string name, DatabaseBuilder databaseBuilder) 
        : base(name, databaseBuilder)
    {
    }
    internal NumberDatabaseColumnBuilder(Guid id, string name, DatabaseBuilder databaseBuilder)
        : base(id, name, databaseBuilder)
    {
    }

    public NumberDatabaseColumnBuilder WithDefaultValue(decimal? defaultValue )
    {
        _defaultValue = defaultValue;
        return this;
    }

    public NumberDatabaseColumnBuilder Nullable()
    {
        _isNullable = true;
        return this;
    }

    public NumberDatabaseColumnBuilder NotNullable()
    {
        _isNullable = false;
        return this;
    }
    internal override NumberDatabaseColumnBuilder WithLastCommited(DateTime lastCommited)
    {
        base.WithLastCommited(lastCommited);

        return this;
    }

    internal override NumberDatabaseColumn Build()
    {
        return new NumberDatabaseColumn
        {
            Id = _id,
            DefaultValue = _defaultValue,
            IsNullable = _isNullable,
            Name = _name,   
            Type = DatabaseColumnType.Number
        }; 
    }
}