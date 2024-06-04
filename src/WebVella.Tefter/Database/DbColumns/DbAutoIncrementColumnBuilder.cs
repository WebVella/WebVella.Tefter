namespace WebVella.Tefter.Database;

public class DbAutoIncrementColumnBuilder : DbColumnBuilder
{
    public DbAutoIncrementColumnBuilder Id(Guid id)
    {
        _id = id;
        return this;
    }

    public DbAutoIncrementColumnBuilder ApplicationId(Guid appId)
    {
        _applicationId = appId;
        return this;
    }

    public DbAutoIncrementColumnBuilder DataProviderId(Guid dataProviderId)
    {
        _dataProviderId = dataProviderId;
        return this;
    }

    public DbAutoIncrementColumnBuilder Name(string name)
    {
        _name = name;
        return this;
    }

    internal override DbAutoIncrementColumn Build()
    {
        return new DbAutoIncrementColumn
        {
            Id = _id,
            ApplicationId = _applicationId,
            DataProviderId = _dataProviderId,
            DefaultValue = null,
            IsNullable = false,
            Name = _name,   
            Type = DbType.AutoIncrement
        }; 
    }
}