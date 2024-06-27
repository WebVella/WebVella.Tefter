namespace WebVella.Tefter.Database;

public class ShortTextDatabaseColumnBuilder : DatabaseColumnBuilder
{
    internal ShortTextDatabaseColumnBuilder(string name, DatabaseBuilder databaseBuilder)
        : base(name, databaseBuilder)
    {
    }

    internal ShortTextDatabaseColumnBuilder(Guid id, string name, DatabaseBuilder databaseBuilder)
        : base(id, name, databaseBuilder)
    {
    }

    public ShortTextDatabaseColumnBuilder WithDefaultValue(string defaultValue)
    {
        _defaultValue = defaultValue;
        return this;
    }

    public ShortTextDatabaseColumnBuilder Nullable()
    {
        _isNullable = true;
        return this;
    }

    public ShortTextDatabaseColumnBuilder NotNullable()
    {
        _isNullable = false;
        return this;
    }

    internal override ShortTextDatabaseColumnBuilder WithLastCommited(DateTime lastCommited)
    {
        base.WithLastCommited(lastCommited);

        return this;
    }

    internal override ShortTextDatabaseColumn Build()
    {
        return new ShortTextDatabaseColumn
        {
            Id = _id,
            DefaultValue = _defaultValue,
            IsNullable = _isNullable,
            Name = _name,
            Type = DatabaseColumnType.ShortText
		};
    }
}