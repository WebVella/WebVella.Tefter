namespace WebVella.Tefter.Database;

public class DbTextColumnBuilder : DbColumnBuilder
{
    public DbTextColumnBuilder Id(Guid id)
    {
        _id = id;
        return this;
    }

    public DbTextColumnBuilder Name(string name)
    {
        _name = name;
        return this;
    }

    public DbTextColumnBuilder DefaultValue(string devaultValue )
    {
        _defaultValue = devaultValue;
        return this;
    }

    public DbTextColumnBuilder IsNullable(bool isNullable)
    {
        _isNullable = isNullable;
        return this;
    }

    internal override DbTextColumn Build()
    {
        return new DbTextColumn
        {
            Id = _id,
            DefaultValue = _defaultValue,
            IsNullable = _isNullable,
            Name = _name,   
            Type = DbType.Text
        }; 
    }
}