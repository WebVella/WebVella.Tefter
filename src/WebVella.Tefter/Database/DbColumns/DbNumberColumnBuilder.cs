namespace WebVella.Tefter.Database;

public class DbNumberColumnBuilder : DbColumnBuilder
{
    internal DbNumberColumnBuilder(string name, DbTableBuilder tableBuilder) 
        : base(name, DbObjectState.New, tableBuilder)
    {
    }

    internal DbNumberColumnBuilder(DbNumberColumn column, DbTableBuilder tableBuilder)
        : base(column, tableBuilder)
    {
    }

    public DbNumberColumnBuilder WithDefaultValue(decimal? defaultValue )
    {
        _defaultValue = defaultValue;
        return this;
    }

    public DbNumberColumnBuilder Nullable()
    {
        _isNullable = true;
        return this;
    }

    public DbNumberColumnBuilder NotNullable()
    {
        _isNullable = false;
        return this;
    }

    internal override DbNumberColumn Build()
    {
        CalculateState();
        return new DbNumberColumn
        {
            Id = _id,
            DefaultValue = _defaultValue,
            IsNullable = _isNullable,
            Name = _name,   
            State = _state,
            Type = DbType.Number
        }; 
    }
}