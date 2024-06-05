namespace WebVella.Tefter.Database;

public class DbBooleanColumnBuilder : DbColumnBuilder
{
    public DbBooleanColumnBuilder Id(Guid id)
    {
        _id = id;
        return this;
    }

    public DbBooleanColumnBuilder Name(string name)
    {
        _name = name;
        return this;
    }

    public DbBooleanColumnBuilder DefaultValue(bool? devaultValue )
    {
        _defaultValue = devaultValue;
        return this;
    }

    public DbBooleanColumnBuilder IsNullable(bool isNullable)
    {
        _isNullable = isNullable;
        return this;
    }

    internal override DbBooleanColumn Build()
    {
        return new DbBooleanColumn
        {
            Id = _id,
            DefaultValue = _defaultValue,
            IsNullable = _isNullable,
            Name = _name,   
            Type = DbType.Boolean
        }; 
    }
}