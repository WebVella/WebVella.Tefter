namespace WebVella.Tefter.Database;

public class DbAutoIncrementColumnBuilder : DbColumnBuilder
{
    public DbAutoIncrementColumnBuilder(string name, DbTableBuilder tableBuilder)
        : base(name, tableBuilder)
    {
    }

    internal DbAutoIncrementColumnBuilder(DbAutoIncrementColumn column, DbTableBuilder tableBuilder)
       : base(column, tableBuilder)
    {
    }

    internal override DbAutoIncrementColumn Build()
    {
        return new DbAutoIncrementColumn
        {
            Id = _id,
            DefaultValue = null,
            IsNullable = false,
            Name = _name,   
            Type = DbType.AutoIncrement
        }; 
    }
}