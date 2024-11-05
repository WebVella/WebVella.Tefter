﻿namespace WebVella.Tefter.Database;

public class TfLongIntegerDatabaseColumnBuilder : TfDatabaseColumnBuilder
{
    internal TfLongIntegerDatabaseColumnBuilder(string name, TfDatabaseBuilder databaseBuilder) 
        : base(name, databaseBuilder)
    {
    }
    internal TfLongIntegerDatabaseColumnBuilder(Guid id, string name, TfDatabaseBuilder databaseBuilder)
        : base(id, name, databaseBuilder)
    {
    }

    public TfLongIntegerDatabaseColumnBuilder WithDefaultValue(long? defaultValue )
    {
        _defaultValue = defaultValue;
        return this;
    }

    public TfLongIntegerDatabaseColumnBuilder Nullable()
    {
        _isNullable = true;
        return this;
    }

    public TfLongIntegerDatabaseColumnBuilder NotNullable()
    {
        _isNullable = false;
        return this;
    }
    internal override TfLongIntegerDatabaseColumnBuilder WithLastCommited(DateTime lastCommited)
    {
        base.WithLastCommited(lastCommited);

        return this;
    }

    internal override TfLongIntegerDatabaseColumn Build()
    {
        return new TfLongIntegerDatabaseColumn
        {
            Id = _id,
            DefaultValue = _defaultValue,
            IsNullable = _isNullable,
            Name = _name,   
            Type = TfDatabaseColumnType.LongInteger,
        }; 
    }
}