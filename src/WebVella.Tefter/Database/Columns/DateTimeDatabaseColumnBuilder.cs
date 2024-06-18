namespace WebVella.Tefter.Database;

public class DateTimeDatabaseColumnBuilder : DatabaseColumnBuilder
{
    public DateTimeDatabaseColumnBuilder(string name, DatabaseBuilder databaseBuilder)
        : base(name, databaseBuilder)
    {
    }
    public DateTimeDatabaseColumnBuilder(Guid id, string name, DatabaseBuilder databaseBuilder)
        : base(id, name, databaseBuilder)
    {
    }

    public DateTimeDatabaseColumnBuilder WithDefaultValue(DateTime? defaultValue)
    {
        _defaultValue = defaultValue;
        return this;
    }

    public DateTimeDatabaseColumnBuilder Nullable()
    {
        _isNullable = true;
        return this;
    }
    public DateTimeDatabaseColumnBuilder NotNullable()
    {
        _isNullable = false;
        return this;
    }

    public DateTimeDatabaseColumnBuilder WithAutoDefaultValue()
    {
        _autoDefaultValue = true;
        return this;
    }
    public DateTimeDatabaseColumnBuilder WithoutAutoDefaultValue()
    {
        _autoDefaultValue = false;
        return this;
    }

    internal override DateTimeDatabaseColumnBuilder WithLastCommited(DateTime lastCommited)
    {
        base.WithLastCommited(lastCommited);

        return this;
    }
    internal override DateTimeDatabaseColumn Build()
    {
        return new DateTimeDatabaseColumn
        {
            Id = _id,
            DefaultValue = _defaultValue,
            IsNullable = _isNullable,
            AutoDefaultValue = _autoDefaultValue,
            Name = _name,
            Type = DatabaseColumnType.DateTime
        };
    }
}