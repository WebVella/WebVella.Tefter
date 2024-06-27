namespace WebVella.Tefter.Database;

public class IntegerDatabaseColumnBuilder : DatabaseColumnBuilder
{
    internal IntegerDatabaseColumnBuilder(string name, DatabaseBuilder databaseBuilder) 
        : base(name, databaseBuilder)
    {
    }
    internal IntegerDatabaseColumnBuilder(Guid id, string name, DatabaseBuilder databaseBuilder)
        : base(id, name, databaseBuilder)
    {
    }

    public IntegerDatabaseColumnBuilder WithDefaultValue(decimal? defaultValue )
    {
        _defaultValue = defaultValue;
        return this;
    }

    public IntegerDatabaseColumnBuilder Nullable()
    {
        _isNullable = true;
        return this;
    }

    public IntegerDatabaseColumnBuilder NotNullable()
    {
        _isNullable = false;
        return this;
    }
    internal override IntegerDatabaseColumnBuilder WithLastCommited(DateTime lastCommited)
    {
        base.WithLastCommited(lastCommited);

        return this;
    }

    internal override IntegerDatabaseColumn Build()
    {
        return new IntegerDatabaseColumn
        {
            Id = _id,
            DefaultValue = _defaultValue,
            IsNullable = _isNullable,
            Name = _name,   
            Type = DatabaseColumnType.Integer
        }; 
    }
}