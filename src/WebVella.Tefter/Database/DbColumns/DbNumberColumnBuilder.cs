namespace WebVella.Tefter.Database;

public class DbNumberColumnBuilder : DbColumnBuilder
{
    public DbNumberColumnBuilder Id(Guid id)
    {
        _id = id;
        return this;
    }

    public DbNumberColumnBuilder ApplicationId(Guid appId)
    {
        _applicationId = appId;
        return this;
    }

    public DbNumberColumnBuilder DataProviderId(Guid dataProviderId)
    {
        _dataProviderId = dataProviderId;
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
            ApplicationId = _applicationId,
            DataProviderId = _dataProviderId,
            DefaultValue = _defaultValue,
            IsNullable = _isNullable,
            Name = _name,   
            Type = DbType.Number
        }; 
    }
}