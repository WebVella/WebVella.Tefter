namespace WebVella.Tefter.Database;

public class DbNumberColumnBuilder : DbColumnBuilder
{
    internal DbNumberColumnBuilder(string name, bool isNew, DbTableBuilder tableBuilder) 
        : base(name, isNew, tableBuilder)
    {
    }

    internal DbNumberColumnBuilder(DbNumberColumn column, DbTableBuilder tableBuilder)
       : base(column.Name, column.IsNew, tableBuilder)
    {
        _isNullable = column.IsNullable;
        _defaultValue = column.DefaultValue;
    }

    public DbNumberColumnBuilder WithDefaultValue(decimal? devaultValue )
    {
        _defaultValue = devaultValue;
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
        return new DbNumberColumn
        {
            DefaultValue = _defaultValue,
            IsNullable = _isNullable,
            Name = _name,   
            Type = DbType.Number
        }; 
    }
}