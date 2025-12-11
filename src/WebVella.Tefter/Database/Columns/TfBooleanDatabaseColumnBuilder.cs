namespace WebVella.Tefter.Database;

public class TfBooleanDatabaseColumnBuilder : TfDatabaseColumnBuilder
{
    internal TfBooleanDatabaseColumnBuilder(string name, TfDatabaseBuilder databaseBuilder)
        : base(name, databaseBuilder)
    {
    }
    internal TfBooleanDatabaseColumnBuilder(Guid id, string name, TfDatabaseBuilder databaseBuilder)
        : base(id, name, databaseBuilder)
    {
    }


    public TfBooleanDatabaseColumnBuilder WithDefaultValue(bool? defaultValue)
    {
        _defaultValue = defaultValue;
        return this;
    }

    public TfBooleanDatabaseColumnBuilder Nullable()
    {
        _isNullable = true;
        return this;
    }

    public TfBooleanDatabaseColumnBuilder NotNullable()
    {
        _isNullable = false;
        return this;
    }

    internal override TfBooleanDatabaseColumnBuilder WithLastCommited(DateTime lastCommited)
    {
        base.WithLastCommited(lastCommited);

        return this;
    }

	public TfBooleanDatabaseColumnBuilder AsExpression(string expression)
	{
		_expression = expression;
		return this;
	}

	internal override TfBooleanDatabaseColumn Build()
    {
        return new TfBooleanDatabaseColumn
        {
            Id = _id,
			DefaultValue = _defaultValue,
            IsNullable = _isNullable,
            Name = _name,
            Type = TfDatabaseColumnType.Boolean,
			Expression = _expression
        };
    }
}