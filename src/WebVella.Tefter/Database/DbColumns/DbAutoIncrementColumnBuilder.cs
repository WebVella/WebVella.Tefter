﻿namespace WebVella.Tefter.Database;

public class DbAutoIncrementColumnBuilder : DbColumnBuilder
{
    public DbAutoIncrementColumnBuilder(string name, bool isNew, DbTableBuilder tableBuilder)
        : base(name, isNew, tableBuilder)
    {
    }

    internal DbAutoIncrementColumnBuilder(DbAutoIncrementColumn column, DbTableBuilder tableBuilder)
       : base(column.Name, column.IsNew, tableBuilder)
    {
        _isNullable = column.IsNullable;
        _defaultValue = column.DefaultValue;
    }

    internal override DbAutoIncrementColumn Build()
    {
        return new DbAutoIncrementColumn
        {
            DefaultValue = null,
            IsNullable = false,
            Name = _name,   
            IsNew = _isNew,
            Type = DbType.AutoIncrement
        }; 
    }
}