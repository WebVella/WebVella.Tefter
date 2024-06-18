namespace WebVella.Tefter.Database;

public class DateDatabaseColumnBuilder : DatabaseColumnBuilder
{
    public DateDatabaseColumnBuilder(string name, DatabaseBuilder databaseBuilder) 
        : base(name, databaseBuilder)
    {
    }

    public DateDatabaseColumnBuilder(Guid id, string name, DatabaseBuilder databaseBuilder)
       : base(id, name, databaseBuilder)
    {
    }

    public DateDatabaseColumnBuilder WithDefaultValue(DateOnly? defaultValue)
    {
        _defaultValue = defaultValue;
        return this;
    }

    public DateDatabaseColumnBuilder Nullable()
    {
        _isNullable = true;
        return this;
    }

    public DateDatabaseColumnBuilder NotNullable()
    {
        _isNullable = false;
        return this;
    }

    public DateDatabaseColumnBuilder WithAutoDefaultValue()
    {
        _autoDefaultValue = true;
        return this;
    }

    public DateDatabaseColumnBuilder WithoutAutoDefaultValue()
    {
        _autoDefaultValue = false;
        return this;
    }

    internal override DateDatabaseColumnBuilder WithLastCommited(DateTime lastCommited)
    {
        base.WithLastCommited(lastCommited);

        return this;
    }

    internal override DateDatabaseColumn Build()
    {
        return new DateDatabaseColumn
        {
            Id = _id,
            DefaultValue = _defaultValue,
            IsNullable = _isNullable,
            AutoDefaultValue = _autoDefaultValue,
            Name = _name,
            Type = DatabaseColumnType.Date
        };
    }
}