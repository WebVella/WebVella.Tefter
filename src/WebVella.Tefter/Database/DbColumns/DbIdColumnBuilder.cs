namespace WebVella.Tefter.Database;

public class DbIdColumnBuilder : DbColumnBuilder
{
    internal DbIdColumnBuilder(DatabaseBuilder databaseBuilder)
        : base(Constants.DB_TABLE_ID_COLUMN_NAME, databaseBuilder)
    {
    }

    internal DbIdColumnBuilder(Guid id, DatabaseBuilder databaseBuilder)
        : base(id, Constants.DB_TABLE_ID_COLUMN_NAME, databaseBuilder)
    {
    }

    internal override DbIdColumn Build()
    {
        return new DbIdColumn
        {
            Id = _id,
            DefaultValue = null,
            IsNullable = false,
            Name = _name,
            Type = DbType.Id,
        };
    }
}