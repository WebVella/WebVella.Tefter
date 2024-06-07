namespace WebVella.Tefter.Database;

public class DbAutoIncrementColumnBuilder : DbColumnBuilder
{
    public DbAutoIncrementColumnBuilder(string name, DatabaseBuilder databaseBuilder)
        : base(name, databaseBuilder)
    {
    }

    public DbAutoIncrementColumnBuilder(Guid id, string name, DatabaseBuilder databaseBuilder)
       : base(id, name, databaseBuilder)
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