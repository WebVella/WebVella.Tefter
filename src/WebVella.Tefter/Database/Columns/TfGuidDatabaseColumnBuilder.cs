namespace WebVella.Tefter.Database;

public class TfGuidDatabaseColumnBuilder : TfDatabaseColumnBuilder
{
    internal TfGuidDatabaseColumnBuilder(string name, TfDatabaseBuilder databaseBuilder)
        : base(name, databaseBuilder)
    {
    }

    internal TfGuidDatabaseColumnBuilder(Guid id, string name, TfDatabaseBuilder databaseBuilder)
        : base(id, name, databaseBuilder)
    {
    }

    public TfGuidDatabaseColumnBuilder WithDefaultValue(Guid? defaultValue)
    {
        _defaultValue = defaultValue;
        return this;
    }

    public TfGuidDatabaseColumnBuilder Nullable()
    {
        _isNullable = true;
        return this;
    }

    public TfGuidDatabaseColumnBuilder NotNullable()
    {
        _isNullable = false;
        return this;
    }

    public TfGuidDatabaseColumnBuilder WithAutoDefaultValue()
    {
        _autoDefaultValue = true;
        return this;
    }

    public TfGuidDatabaseColumnBuilder WithoutAutoDefaultValue()
    {
        _autoDefaultValue = false;
        return this;
    }

    internal override TfGuidDatabaseColumnBuilder WithLastCommited(DateTime lastCommited)
    {
        base.WithLastCommited(lastCommited);

        return this;
    }

    internal override TfGuidDatabaseColumn Build()
    {
        return new TfGuidDatabaseColumn
        {
            Id = _id,
            DefaultValue = _defaultValue,
            IsNullable = _isNullable,
            AutoDefaultValue = _autoDefaultValue,
            Name = _name,
            Type = TfDatabaseColumnType.Guid
        };
    }
}