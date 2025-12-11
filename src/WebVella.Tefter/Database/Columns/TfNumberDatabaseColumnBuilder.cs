namespace WebVella.Tefter.Database;

public class TfNumberDatabaseColumnBuilder : TfDatabaseColumnBuilder
{
    internal TfNumberDatabaseColumnBuilder(string name, TfDatabaseBuilder databaseBuilder) 
        : base(name, databaseBuilder)
    {
    }
    internal TfNumberDatabaseColumnBuilder(Guid id, string name, TfDatabaseBuilder databaseBuilder)
        : base(id, name, databaseBuilder)
    {
    }

    public TfNumberDatabaseColumnBuilder WithDefaultValue(decimal? defaultValue )
    {
        _defaultValue = defaultValue;
        return this;
    }

    public TfNumberDatabaseColumnBuilder Nullable()
    {
        _isNullable = true;
        return this;
    }

    public TfNumberDatabaseColumnBuilder NotNullable()
    {
        _isNullable = false;
        return this;
    }
    internal override TfNumberDatabaseColumnBuilder WithLastCommited(DateTime lastCommited)
    {
        base.WithLastCommited(lastCommited);

        return this;
    }

	public TfNumberDatabaseColumnBuilder AsExpression(string expression)
	{
		_expression = expression;
		return this;
	}

	internal override TfNumberDatabaseColumn Build()
    {
        return new TfNumberDatabaseColumn
        {
            Id = _id,
            DefaultValue = _defaultValue,
            IsNullable = _isNullable,
            Name = _name,   
            Type = TfDatabaseColumnType.Number,
			Expression = _expression,
		}; 
    }
}