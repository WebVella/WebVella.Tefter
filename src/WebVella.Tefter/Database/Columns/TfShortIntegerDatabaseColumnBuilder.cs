namespace WebVella.Tefter.Database;

public class TfShortIntegerDatabaseColumnBuilder : TfDatabaseColumnBuilder
{
    internal TfShortIntegerDatabaseColumnBuilder(string name, TfDatabaseBuilder databaseBuilder) 
        : base(name, databaseBuilder)
    {
    }
    internal TfShortIntegerDatabaseColumnBuilder(Guid id, string name, TfDatabaseBuilder databaseBuilder)
        : base(id, name, databaseBuilder)
    {
    }

    public TfShortIntegerDatabaseColumnBuilder WithDefaultValue(short? defaultValue )
    {
        _defaultValue = defaultValue;
        return this;
    }

    public TfShortIntegerDatabaseColumnBuilder Nullable()
    {
        _isNullable = true;
        return this;
    }

    public TfShortIntegerDatabaseColumnBuilder NotNullable()
    {
        _isNullable = false;
        return this;
    }
    internal override TfShortIntegerDatabaseColumnBuilder WithLastCommited(DateTime lastCommited)
    {
        base.WithLastCommited(lastCommited);

        return this;
    }

    internal override TfShortIntegerDatabaseColumn Build()
    {
        return new TfShortIntegerDatabaseColumn
        {
            Id = _id,
            DefaultValue = _defaultValue,
            IsNullable = _isNullable,
            Name = _name,   
            Type = TfDatabaseColumnType.ShortInteger,
        }; 
    }
}