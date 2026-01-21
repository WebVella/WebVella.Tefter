namespace WebVella.Tefter.Database;

public class TfShortTextDatabaseColumnBuilder : TfDatabaseColumnBuilder
{
    internal TfShortTextDatabaseColumnBuilder(string name, TfDatabaseBuilder databaseBuilder)
        : base(name, databaseBuilder)
    {
    }

    internal TfShortTextDatabaseColumnBuilder(Guid id, string name, TfDatabaseBuilder databaseBuilder)
        : base(id, name, databaseBuilder)
    {
    }

    public TfShortTextDatabaseColumnBuilder WithDefaultValue(string defaultValue)
    {
        _defaultValue = defaultValue;
        return this;
    }

    public TfShortTextDatabaseColumnBuilder Nullable()
    {
        _isNullable = true;
        return this;
    }

    public TfShortTextDatabaseColumnBuilder NotNullable()
    {
        _isNullable = false;
        return this;
    }

    internal override TfShortTextDatabaseColumnBuilder WithLastCommited(DateTime lastCommited)
    {
        base.WithLastCommited(lastCommited);

        return this;
    }

	public TfShortTextDatabaseColumnBuilder WithAutoDefaultValue()
	{
		_autoDefaultValue = true;
		return this;
	}

	public TfShortTextDatabaseColumnBuilder WithoutAutoDefaultValue()
	{
		_autoDefaultValue = false;
		return this;
	}

	public TfShortTextDatabaseColumnBuilder AsExpression(string expression)
	{
		_expression = expression;
		return this;
	}

	public TfShortTextDatabaseColumnBuilder AsSha1ExpressionFromColumns(params string [] columns)
	{
		if (columns == null || columns.Length == 0)
			throw new ArgumentNullException(nameof(columns), "The columns array cannot be null or empty.");

		_expression = string.Format( TfConstants.DB_SHORT_TEXT_COLUMN_SHA1_FROM_COLUMNS_VALUE,
			string.Join(",", columns.Select( x=> x + "::TEXT") ));

		return this;
	}

	public TfShortTextDatabaseColumnBuilder AsSha1ExpressionFromColumnsWithPrefix(string prefix, params string[] columns)
	{
		if (columns == null || columns.Length == 0)
			throw new ArgumentNullException(nameof(columns), "The columns array cannot be null or empty.");

		if(string.IsNullOrWhiteSpace(prefix))
			throw new ArgumentNullException(nameof(prefix), "The prefix cannot be null or empty.");

		_expression = string.Format(TfConstants.DB_SHORT_TEXT_COLUMN_SHA1_FROM_COLUMNS_WITH_PREFIX_VALUE,
			"'" + prefix + "'::TEXT",
			string.Join(",", columns.Select(x => x + "::TEXT")));

		return this;
	}

	internal override TfShortTextDatabaseColumn Build()
    {
        return new TfShortTextDatabaseColumn
        {
            Id = _id,
            DefaultValue = _defaultValue,
			AutoDefaultValue = _autoDefaultValue,
            IsNullable = _isNullable,
            Name = _name,
            Type = TfDatabaseColumnType.ShortText,
			Expression = _expression
		};
    }
}