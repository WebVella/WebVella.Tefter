namespace WebVella.Tefter;

public sealed class TfDataTable
{
	public string Sql { get; init; }
	public ReadOnlyCollection<NpgsqlParameter> SqlParameters { get; init; }
	public TfDataTableQueryInfo QueryInfo { get; set; }
	public TfDataColumnCollection Columns { get; init; }
	public TfDataRowCollection Rows { get; init; }

	private TfDataTable(
		TfDataTable table,
		params int[] rowIndexes)
	{
		Sql = string.Empty;
		SqlParameters = new List<NpgsqlParameter>().AsReadOnly();
		QueryInfo = new TfDataTableQueryInfo(
			this,
			table.QueryInfo.DataProviderId,
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
			DatabaseColumnType.Guid,
			isNullable: false,
			isShared: false,
			isSystem: true));

		columns.Add(new TfDataColumn(
			this,
			"tf_row_index",
			DatabaseColumnType.Integer,
			isNullable: false,
			isShared: false,
			isSystem: true));

		columns.Add(new TfDataColumn(
			this,
			"tf_created_on",
			DatabaseColumnType.DateTime,
			isNullable: false,
			isShared: false,
			isSystem: true));

		columns.Add(new TfDataColumn(
			this,
			"tf_updated_on",
			DatabaseColumnType.DateTime,
			isNullable: false,
			isShared: false,
			isSystem: true));

		columns.Add(new TfDataColumn(
			this,
			"tf_search",
			DatabaseColumnType.Text,
			isNullable: false,
			isShared: false,
			isSystem: true));

		foreach (var sharedKey in dataProvider.SharedKeys)
		{
			string name = $"tf_sk_{sharedKey.DbName}_id";
			columns.Add(new TfDataColumn(
			this,
			name,
			DatabaseColumnType.Guid,
			isNullable: false,
			isShared: false,
			isSystem: true));

			name = $"tf_sk_{sharedKey.DbName}_version";
			columns.Add(new TfDataColumn(
			this,
			name,
			DatabaseColumnType.ShortInteger,
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

	public TfDataRow NewRow()
	{
		object[] values = new object[this.Columns.Count];
		return new TfDataRow(this,values);
	}

	public override string ToString()
	{
		return $"{Rows}  {Columns}  {QueryInfo}";
	}
}
