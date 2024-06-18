namespace WebVella.Tefter.Database;

public class BooleanDatabaseColumnBuilder : DatabaseColumnBuilder
{
    internal BooleanDatabaseColumnBuilder(string name, DatabaseBuilder databaseBuilder)
        : base(name, databaseBuilder)
    {
    }
    internal BooleanDatabaseColumnBuilder(Guid id, string name, DatabaseBuilder databaseBuilder)
        : base(id, name, databaseBuilder)
    {
    }


    public BooleanDatabaseColumnBuilder WithDefaultValue(bool? defaultValue)
    {
        _defaultValue = defaultValue;
        return this;
    }

    public BooleanDatabaseColumnBuilder Nullable()
    {
        _isNullable = true;
        return this;
    }

    public BooleanDatabaseColumnBuilder NotNullable()
    {
        _isNullable = false;
        return this;
    }

    internal override BooleanDatabaseColumnBuilder WithLastCommited(DateTime lastCommited)
    {
        base.WithLastCommited(lastCommited);

        return this;
    }

    internal override BooleanDatabaseColumn Build()
    {
        return new BooleanDatabaseColumn
        {
            Id = _id,
            DefaultValue = _defaultValue,
            IsNullable = _isNullable,
            Name = _name,
            Type = DatabaseColumnType.Boolean
        };
    }
}