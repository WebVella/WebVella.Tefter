namespace WebVella.Tefter.Database;

public class DbBooleanColumnBuilder : DbColumnBuilder
{
    internal DbBooleanColumnBuilder(string name, bool isNew, DbTableBuilder tableBuilder)
        : base(name, isNew, tableBuilder)
    {
    }
    internal DbBooleanColumnBuilder(DbBooleanColumn column, DbTableBuilder tableBuilder)
      : base(column.Name, column.IsNew, tableBuilder)
    {
        _isNullable = column.IsNullable;
        _defaultValue = column.DefaultValue;
    }

    public DbBooleanColumnBuilder WithDefaultValue(bool? devaultValue)
    {
        _defaultValue = devaultValue;
        return this;
    }

    public DbBooleanColumnBuilder Nullable()
    {
        _isNullable = true;
        return this;
    }

    public DbBooleanColumnBuilder NotNullable()
    {
        _isNullable = true;
        return this;
    }

    internal override DbBooleanColumn Build()
    {
        return new DbBooleanColumn
        {
            DefaultValue = _defaultValue,
            IsNullable = _isNullable,
            Name = _name,
            IsNew = _isNew,
            Type = DbType.Boolean
        };
    }
}