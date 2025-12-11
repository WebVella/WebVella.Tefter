namespace WebVella.Tefter.Database;

public class TfDateTimeDatabaseColumnBuilder : TfDatabaseColumnBuilder
{
    public TfDateTimeDatabaseColumnBuilder(string name, TfDatabaseBuilder databaseBuilder)
        : base(name, databaseBuilder)
    {
    }
    public TfDateTimeDatabaseColumnBuilder(Guid id, string name, TfDatabaseBuilder databaseBuilder)
        : base(id, name, databaseBuilder)
    {
    }

    public TfDateTimeDatabaseColumnBuilder WithDefaultValue(DateTime? defaultValue)
    {
        _defaultValue = defaultValue;
        return this;
    }

    public TfDateTimeDatabaseColumnBuilder Nullable()
    {
        _isNullable = true;
        return this;
    }
    public TfDateTimeDatabaseColumnBuilder NotNullable()
    {
        _isNullable = false;
        return this;
    }

    public TfDateTimeDatabaseColumnBuilder WithAutoDefaultValue()
    {
        _autoDefaultValue = true;
        return this;
    }
    public TfDateTimeDatabaseColumnBuilder WithoutAutoDefaultValue()
    {
        _autoDefaultValue = false;
        return this;
    }

    internal override TfDateTimeDatabaseColumnBuilder WithLastCommited(DateTime lastCommited)
    {
        base.WithLastCommited(lastCommited);

        return this;
    }

	public TfDateTimeDatabaseColumnBuilder AsExpression(string expression)
	{
		_expression = expression;
		return this;
	}

	internal override TfDateTimeDatabaseColumn Build()
    {
        return new TfDateTimeDatabaseColumn
        {
            Id = _id,
			DefaultValue = _defaultValue,
            IsNullable = _isNullable,
            AutoDefaultValue = _autoDefaultValue,
            Name = _name,
            Type = TfDatabaseColumnType.DateTime,
			Expression = _expression
		};
    }
}