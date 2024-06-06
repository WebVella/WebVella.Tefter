namespace WebVella.Tefter.Database;

public class DbIdColumnBuilder : DbColumnBuilder
{
    public DbIdColumnBuilder( bool isNew, DbTableBuilder tableBuilder) 
        : base(Constants.DB_TABLE_ID_COLUMN_NAME, tableBuilder)
    {
    }

    public DbIdColumnBuilder(DbIdColumn column, DbTableBuilder tableBuilder)
        : base(column, tableBuilder)
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