namespace WebVella.Tefter.Database;

public class DbIdColumnBuilder : DbColumnBuilder
{
    public DbIdColumnBuilder Id(Guid id)
    {
        _id = id;
        return this;
    }

    public DbIdColumnBuilder ApplicationId(Guid appId)
    {
        _applicationId = appId;
        return this;
    }

    public DbIdColumnBuilder DataProviderId(Guid dataProviderId)
    {
        _dataProviderId = dataProviderId;
        return this;
    }

    internal override DbIdColumn Build()
    {
        return new DbIdColumn
        {
            Id = _id,
            ApplicationId = _applicationId,
            DataProviderId = _dataProviderId,
            DefaultValue = null,
            IsNullable = false,
            Name = Constants.DB_TABLE_ID_COLUMN_NAME,
            Type = DbType.Id
        };
    }
}