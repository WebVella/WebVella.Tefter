namespace WebVella.Tefter.Database;

public class TfShortTextDatabaseColumnBuilder : TfDatabaseColumnBuilder
{
    internal TfShortTextDatabaseColumnBuilder(string name, TfDatabaseBuilder databaseBuilder)
        : base(name, databaseBuilder)
    {
    }

    internal TfShortTextDatabaseColumnBuilder(Guid id, string name, TfDatabaseBuilder databaseBuilder)
        : base(id, name, databaseBuilder)
    {
    }

    public TfShortTextDatabaseColumnBuilder WithDefaultValue(string defaultValue)
    {
        _defaultValue = defaultValue;
        return this;
    }

    public TfShortTextDatabaseColumnBuilder Nullable()
    {
        _isNullable = true;
        return this;
    }

    public TfShortTextDatabaseColumnBuilder NotNullable()
    {
        _isNullable = false;
        return this;
    }

    internal override TfShortTextDatabaseColumnBuilder WithLastCommited(DateTime lastCommited)
    {
        base.WithLastCommited(lastCommited);

        return this;
    }

    internal override TfShortTextDatabaseColumn Build()
    {
        return new TfShortTextDatabaseColumn
        {
            Id = _id,
            DefaultValue = _defaultValue,
            IsNullable = _isNullable,
            Name = _name,
            Type = TfDatabaseColumnType.ShortText
		};
    }
}