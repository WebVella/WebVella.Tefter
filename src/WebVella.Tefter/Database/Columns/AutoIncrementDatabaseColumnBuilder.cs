namespace WebVella.Tefter.Database;

public class AutoIncrementDatabaseColumnBuilder : DatabaseColumnBuilder
{
    public AutoIncrementDatabaseColumnBuilder(string name, DatabaseBuilder databaseBuilder)
        : base(name, databaseBuilder)
    {
    }

    public AutoIncrementDatabaseColumnBuilder(Guid id, string name, DatabaseBuilder databaseBuilder)
       : base(id, name, databaseBuilder)
    {
    }

    internal override AutoIncrementDatabaseColumnBuilder WithLastCommited(DateTime lastCommited)
    {
        base.WithLastCommited(lastCommited);

        return this;
    }

    internal override AutoIncrementDatabaseColumn Build()
    {
        return new AutoIncrementDatabaseColumn
        {
            Id = _id,
            DefaultValue = null,
            IsNullable = false,
            Name = _name,   
            Type = DatabaseColumnType.AutoIncrement
        }; 
    }
}