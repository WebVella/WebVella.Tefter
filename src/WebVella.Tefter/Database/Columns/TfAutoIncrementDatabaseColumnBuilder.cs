namespace WebVella.Tefter.Database;

public class TfAutoIncrementDatabaseColumnBuilder : TfDatabaseColumnBuilder
{
    public TfAutoIncrementDatabaseColumnBuilder(string name, TfDatabaseBuilder databaseBuilder)
        : base(name, databaseBuilder)
    {
    }

    public TfAutoIncrementDatabaseColumnBuilder(Guid id, string name, TfDatabaseBuilder databaseBuilder)
       : base(id, name, databaseBuilder)
    {
    }

    internal override TfAutoIncrementDatabaseColumnBuilder WithLastCommited(DateTime lastCommited)
    {
        base.WithLastCommited(lastCommited);

        return this;
    }

    internal override TfAutoIncrementDatabaseColumn Build()
    {
        return new TfAutoIncrementDatabaseColumn
        {
            Id = _id,
            DefaultValue = null,
            IsNullable = false,
            Name = _name,   
            Type = TfDatabaseColumnType.AutoIncrement
        }; 
    }
}