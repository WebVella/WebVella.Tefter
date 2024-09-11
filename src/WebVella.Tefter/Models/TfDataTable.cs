namespace WebVella.Tefter;

public sealed class TfDataTable
{
	public TfDataTableQueryInfo QueryInfo { get; set; }
	public TfDataColumnCollection Columns { get; init; }
	public TfDataRowCollection Rows { get; init; }

	internal TfDataTable(
		TfDataProvider dataProvider,
		TfDataTableQuery query, 
		List<string> onlyColumns = null )
	{
		if (dataProvider is null)
			throw new ArgumentNullException(nameof(dataProvider));

		QueryInfo = new TfDataTableQueryInfo(
			this,
			query.DataProviderId,
			query.Page,
			query.PageSize,
			query.Search,
			query.ExcludeSharedColumns);

		Columns = InitColumns(
			dataProvider,
			onlyColumns,
			query.ExcludeSharedColumns);

		Rows = new TfDataRowCollection(this);
	}

	private TfDataColumnCollection InitColumns(
		TfDataProvider dataProvider,
		List<string> onlyColumns,
		bool excludeSharedColumns)
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

		if (!excludeSharedColumns)
		{
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
		}

		return columns;
	}

	public override string ToString()
	{
		return $"{Rows}  {Columns}  {QueryInfo}";
	}
}
