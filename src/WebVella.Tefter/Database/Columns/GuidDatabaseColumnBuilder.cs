namespace WebVella.Tefter.Database;

public class GuidDatabaseColumnBuilder : DatabaseColumnBuilder
{
    internal GuidDatabaseColumnBuilder(string name, DatabaseBuilder databaseBuilder)
        : base(name, databaseBuilder)
    {
    }

    internal GuidDatabaseColumnBuilder(Guid id, string name, DatabaseBuilder databaseBuilder)
        : base(id, name, databaseBuilder)
    {
    }

    public GuidDatabaseColumnBuilder WithDefaultValue(Guid? defaultValue)
    {
        _defaultValue = defaultValue;
        return this;
    }

    public GuidDatabaseColumnBuilder Nullable()
    {
        _isNullable = true;
        return this;
    }

    public GuidDatabaseColumnBuilder NotNullable()
    {
        _isNullable = false;
        return this;
    }

    public GuidDatabaseColumnBuilder WithAutoDefaultValue()
    {
        _autoDefaultValue = true;
        return this;
    }

    public GuidDatabaseColumnBuilder WithoutAutoDefaultValue()
    {
        _autoDefaultValue = false;
        return this;
    }

    internal override GuidDatabaseColumnBuilder WithLastCommited(DateTime lastCommited)
    {
        base.WithLastCommited(lastCommited);

        return this;
    }

    internal override GuidDatabaseColumn Build()
    {
        return new GuidDatabaseColumn
        {
            Id = _id,
            DefaultValue = _defaultValue,
            IsNullable = _isNullable,
            AutoDefaultValue = _autoDefaultValue,
            Name = _name,
            Type = DatabaseColumnType.Guid
        };
    }
}