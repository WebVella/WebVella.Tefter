namespace WebVella.Tefter.Database;

public class DbBooleanColumnBuilder : DbColumnBuilder
{
    internal DbBooleanColumnBuilder(string name, DbTableBuilder tableBuilder)
        : base(name, DbObjectState.New, tableBuilder)
    {
    }

    internal DbBooleanColumnBuilder(DbBooleanColumn column, DbTableBuilder tableBuilder)
        : base(column, tableBuilder)
    {
    }

    public DbBooleanColumnBuilder WithDefaultValue(bool? defaultValue)
    {
        _defaultValue = defaultValue;
        return this;
    }

    public DbBooleanColumnBuilder Nullable()
    {
        _isNullable = true;
        return this;
    }

    public DbBooleanColumnBuilder NotNullable()
    {
        _isNullable = false;
        return this;
    }

    internal override DbBooleanColumn Build()
    {
        CalculateState();
        return new DbBooleanColumn
        {
            DefaultValue = _defaultValue,
            IsNullable = _isNullable,
            Name = _name,
            State = _state,
            Type = DbType.Boolean
        };
    }
}