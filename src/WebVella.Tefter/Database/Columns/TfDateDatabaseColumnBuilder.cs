namespace WebVella.Tefter.Database;

public class TfDateDatabaseColumnBuilder : TfDatabaseColumnBuilder
{
    public TfDateDatabaseColumnBuilder(string name, TfDatabaseBuilder databaseBuilder) 
        : base(name, databaseBuilder)
    {
    }

    public TfDateDatabaseColumnBuilder(Guid id, string name, TfDatabaseBuilder databaseBuilder)
       : base(id, name, databaseBuilder)
    {
    }

    public TfDateDatabaseColumnBuilder WithDefaultValue(DateOnly? defaultValue)
    {
        _defaultValue = defaultValue;
        return this;
    }

    public TfDateDatabaseColumnBuilder Nullable()
    {
        _isNullable = true;
        return this;
    }

    public TfDateDatabaseColumnBuilder NotNullable()
    {
        _isNullable = false;
        return this;
    }

    public TfDateDatabaseColumnBuilder WithAutoDefaultValue()
    {
        _autoDefaultValue = true;
        return this;
    }

    public TfDateDatabaseColumnBuilder WithoutAutoDefaultValue()
    {
        _autoDefaultValue = false;
        return this;
    }

    internal override TfDateDatabaseColumnBuilder WithLastCommited(DateTime lastCommited)
    {
        base.WithLastCommited(lastCommited);

        return this;
    }

    internal override TfDateDatabaseColumn Build()
    {
        return new TfDateDatabaseColumn
        {
            Id = _id,
            DefaultValue = _defaultValue,
            IsNullable = _isNullable,
            AutoDefaultValue = _autoDefaultValue,
            Name = _name,
            Type = TfDatabaseColumnType.Date
        };
    }
}