namespace WebVella.Tefter.Database;

public class DbBooleanColumnBuilder : DbColumnBuilder
{
    public DbBooleanColumnBuilder(string name, bool isNew, DbTableBuilder tableBuilder)
        : base(name, isNew, tableBuilder)
    {
    }

    public DbBooleanColumnBuilder WithDefaultValue(bool? devaultValue )
    {
        _defaultValue = devaultValue;
        return this;
    }

    public DbBooleanColumnBuilder Nullable()
    {
        _isNullable = true;
        return this;
    }

    public DbBooleanColumnBuilder NotNullable()
    {
        _isNullable = true;
        return this;
    }

    internal override DbBooleanColumn Build()
    {
        return new DbBooleanColumn
        {
            DefaultValue = _defaultValue,
            IsNullable = _isNullable,
            Name = _name,   
            IsNew = _isNew,
            Type = DbType.Boolean
        }; 
    }
}