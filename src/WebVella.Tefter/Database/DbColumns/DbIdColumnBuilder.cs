namespace WebVella.Tefter.Database;

public class DbIdColumnBuilder : DbColumnBuilder
{
    public DbIdColumnBuilder Id(Guid id)
    {
        _id = id;
        return this;
    }

    internal override DbIdColumn Build()
    {
        return new DbIdColumn
        {
            Id = _id,
            DefaultValue = null,
            IsNullable = false,
            Name = Constants.DB_TABLE_ID_COLUMN_NAME,
            Type = DbType.Id
        };
    }
}