namespace WebVella.Tefter;

public sealed class TfDataTable
{
	public string Sql { get; init; }
	public ReadOnlyCollection<NpgsqlParameter> SqlParameters { get; init; }
	public TfDataTableQueryInfo QueryInfo { get; set; }
	public TfDataColumnCollection Columns { get; init; }
	public TfDataRowCollection Rows { get; init; }

	internal TfDataTable()
	{
		Sql = string.Empty;
		SqlParameters = new List<NpgsqlParameter>().AsReadOnly();
		QueryInfo = null;

		Columns = new TfDataColumnCollection(this);

		Rows = new TfDataRowCollection(this);
	}

	internal TfDataTable(
		TfDataTable table,
		params int[] rowIndexes)
	{
		Sql = string.Empty;
		SqlParameters = new List<NpgsqlParameter>().AsReadOnly();
		QueryInfo = new TfDataTableQueryInfo(
			this,
			table.QueryInfo.DataProviderId,
			table.QueryInfo.SpaceDataId,
			table.QueryInfo.Page,
			table.QueryInfo.PageSize,
			table.QueryInfo.Search);

		Columns = new TfDataColumnCollection(this);

		foreach (var column in table.Columns)
		{
			Columns.Add(new TfDataColumn(
				this,
				column.Name,
				column.DbType,
				column.IsNullable,
				column.IsShared,
				column.IsSystem
			));
		}

		Rows = new TfDataRowCollection(this);
		if (rowIndexes is not null)
		{
			foreach (var rowIndex in rowIndexes)
			{
				Rows.Add(
					new TfDataRow(
					this,
					table.Rows[rowIndex].GetValues())
				);
			}
		}
	}

	internal TfDataTable(
		TfDataProvider dataProvider,
		TfDataTableQuery query,
		string sql,
		List<NpgsqlParameter> sqlParameters,
		List<string> onlyColumns = null)
	{
		if (dataProvider is null)
			throw new ArgumentNullException(nameof(dataProvider));

		QueryInfo = new TfDataTableQueryInfo(
			this,
			query.DataProviderId,
			query.SpaceDataId,
			query.Page,
			query.PageSize,
			query.Search);

		Columns = InitColumns(
			dataProvider,
			onlyColumns);

		Sql = sql;

		SqlParameters = sqlParameters.AsReadOnly();

		Rows = new TfDataRowCollection(this);
	}

	private TfDataColumnCollection InitColumns(
		TfDataProvider dataProvider,
		List<string> onlyColumns)
	{
		var columns = new TfDataColumnCollection(this);

		columns.Add(new TfDataColumn(
				this,
				"tf_id",
				TfDatabaseColumnType.Guid,
				isNullable: false,
				isShared: false,
				isSystem: true));

		//the case we return only tf_ids
		if (onlyColumns.Count == 1 && onlyColumns[0] == "tf_id")
		{
			return columns;
		}

		columns.Add(new TfDataColumn(
			this,
			"tf_row_index",
			TfDatabaseColumnType.Integer,
			isNullable: false,
			isShared: false,
			isSystem: true));

		columns.Add(new TfDataColumn(
			this,
			"tf_created_on",
			TfDatabaseColumnType.DateTime,
			isNullable: false,
			isShared: false,
			isSystem: true));

		columns.Add(new TfDataColumn(
			this,
			"tf_updated_on",
			TfDatabaseColumnType.DateTime,
			isNullable: false,
			isShared: false,
			isSystem: true));

		columns.Add(new TfDataColumn(
			this,
			"tf_search",
			TfDatabaseColumnType.Text,
			isNullable: false,
			isShared: false,
			isSystem: true));

		foreach (var sharedKey in dataProvider.SharedKeys)
		{
			string name = $"tf_sk_{sharedKey.DbName}_id";
			columns.Add(new TfDataColumn(
			this,
			name,
			TfDatabaseColumnType.Guid,
			isNullable: false,
			isShared: false,
			isSystem: true));

			name = $"tf_sk_{sharedKey.DbName}_version";
			columns.Add(new TfDataColumn(
			this,
			name,
			TfDatabaseColumnType.ShortInteger,
			isNullable: false,
			isShared: false,
			isSystem: true));

		}

		foreach (var providerColumn in dataProvider.Columns)
		{
			if (onlyColumns != null && !onlyColumns.Contains(providerColumn.DbName))
				continue;

			columns.Add(new TfDataColumn(
			this,
			providerColumn.DbName,
			providerColumn.DbType,
			isNullable: providerColumn.IsNullable,
			isShared: false,
			isSystem: false));
		}


		foreach (var providerColumn in dataProvider.SharedColumns)
		{
			if (onlyColumns != null && !onlyColumns.Contains(providerColumn.DbName))
				continue;

			columns.Add(new TfDataColumn(
			this,
			providerColumn.DbName,
			providerColumn.DbType,
			isNullable: true,
			isShared: true,
			isSystem: false));
		}

		return columns;
	}

	public TfDataTable NewTable(
		params int[] rows)
	{
		return new TfDataTable(this, rows);
	}

	public TfDataTable Clone()
	{
		int[] indexes = new int[this.Rows.Count];

		for (int i = 0; i < this.Rows.Count; i++)
			indexes[i] = i;

		return new TfDataTable(this, indexes);
	}

	public TfDataRow NewRow()
	{
		object[] values = new object[this.Columns.Count];
		return new TfDataRow(this, values);
	}

	public override string ToString()
	{
		return $"{Rows}  {Columns}  {QueryInfo}";
	}
}
