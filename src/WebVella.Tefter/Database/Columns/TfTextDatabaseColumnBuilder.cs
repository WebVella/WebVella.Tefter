namespace WebVella.Tefter.Database;

public class TfTextDatabaseColumnBuilder : TfDatabaseColumnBuilder
{
    internal TfTextDatabaseColumnBuilder(string name, TfDatabaseBuilder databaseBuilder)
        : base(name, databaseBuilder)
    {
    }

    internal TfTextDatabaseColumnBuilder(Guid id, string name, TfDatabaseBuilder databaseBuilder)
        : base(id, name, databaseBuilder)
    {
    }

    public TfTextDatabaseColumnBuilder WithDefaultValue(string defaultValue)
    {
        _defaultValue = defaultValue;
        return this;
    }

    public TfTextDatabaseColumnBuilder Nullable()
    {
        _isNullable = true;
        return this;
    }

    public TfTextDatabaseColumnBuilder NotNullable()
    {
        _isNullable = false;
        return this;
    }

    internal override TfTextDatabaseColumnBuilder WithLastCommited(DateTime lastCommited)
    {
        base.WithLastCommited(lastCommited);

        return this;
    }

	public TfTextDatabaseColumnBuilder AsExpression(string expression)
	{
		_expression = expression;
		return this;
	}

	internal override TfTextDatabaseColumn Build()
    {
        return new TfTextDatabaseColumn
        {
            Id = _id,
            DefaultValue = _defaultValue,
            IsNullable = _isNullable,
            Name = _name,
            Type = TfDatabaseColumnType.Text,
			Expression = _expression
        };
    }
}