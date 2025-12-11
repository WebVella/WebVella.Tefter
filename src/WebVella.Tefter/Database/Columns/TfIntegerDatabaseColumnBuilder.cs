namespace WebVella.Tefter.Database;

public class TfIntegerDatabaseColumnBuilder : TfDatabaseColumnBuilder
{
    internal TfIntegerDatabaseColumnBuilder(string name, TfDatabaseBuilder databaseBuilder) 
        : base(name, databaseBuilder)
    {
    }
    internal TfIntegerDatabaseColumnBuilder(Guid id, string name, TfDatabaseBuilder databaseBuilder)
        : base(id, name, databaseBuilder)
    {
    }

    public TfIntegerDatabaseColumnBuilder WithDefaultValue(int? defaultValue )
    {
        _defaultValue = defaultValue;
        return this;
    }

    public TfIntegerDatabaseColumnBuilder Nullable()
    {
        _isNullable = true;
        return this;
    }

    public TfIntegerDatabaseColumnBuilder NotNullable()
    {
        _isNullable = false;
        return this;
    }
    internal override TfIntegerDatabaseColumnBuilder WithLastCommited(DateTime lastCommited)
    {
        base.WithLastCommited(lastCommited);

        return this;
    }

	public TfIntegerDatabaseColumnBuilder AsExpression(string expression)
	{
		_expression = expression;
		return this;
	}

	internal override TfIntegerDatabaseColumn Build()
    {
        return new TfIntegerDatabaseColumn
        {
            Id = _id,
            DefaultValue = _defaultValue,
            IsNullable = _isNullable,
            Name = _name,   
            Type = TfDatabaseColumnType.Integer,
			Expression = _expression
        }; 
    }
}