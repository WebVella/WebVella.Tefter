namespace WebVella.Tefter.Database;

public class DbNumberColumnBuilder : DbColumnBuilder
{
    public DbNumberColumnBuilder Id(Guid id)
    {
        _id = id;
        return this;
    }

    public DbNumberColumnBuilder Name(string name)
    {
        _name = name;
        return this;
    }

    public DbNumberColumnBuilder DefaultValue(decimal? devaultValue )
    {
        _defaultValue = devaultValue;
        return this;
    }

    public DbNumberColumnBuilder IsNullable(bool isNullable)
    {
        _isNullable = isNullable;
        return this;
    }

    internal override DbNumberColumn Build()
    {
        return new DbNumberColumn
        {
            Id = _id,
            DefaultValue = _defaultValue,
            IsNullable = _isNullable,
            Name = _name,   
            Type = DbType.Number
        }; 
    }
}