using Bogus;
using System.Linq;

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
		QueryInfo = null;

		if (table.QueryInfo != null)
		{
			QueryInfo = new TfDataTableQueryInfo(
				this,
				table.QueryInfo.DataProviderId,
				table.QueryInfo.SpaceDataId,
				table.QueryInfo.Page,
				table.QueryInfo.PageSize,
				table.QueryInfo.Search);
		}

		Columns = new TfDataColumnCollection(this);

		foreach (var column in table.Columns)
		{
			Columns.Add(new TfDataColumn(
				this,
				column.Name,
				column.DbType,
				column.IsNullable,
				column.Origin,
				column.IsReadOnly
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
			TfDataTable table,
			params Guid[] tfIds)
	{
		Sql = string.Empty;
		SqlParameters = new List<NpgsqlParameter>().AsReadOnly();
		QueryInfo = null;

		if (table.QueryInfo != null)
		{
			QueryInfo = new TfDataTableQueryInfo(
				this,
				table.QueryInfo.DataProviderId,
				table.QueryInfo.SpaceDataId,
				table.QueryInfo.Page,
				table.QueryInfo.PageSize,
				table.QueryInfo.Search);
		}

		Columns = new TfDataColumnCollection(this);

		foreach (var column in table.Columns)
		{
			Columns.Add(new TfDataColumn(
				this,
				column.Name,
				column.DbType,
				column.IsNullable,
				column.Origin,
				column.IsReadOnly
			));
		}

		Rows = new TfDataRowCollection(this);
		if (tfIds is not null)
		{
			foreach (var tfId in tfIds)
			{
				if(table.Rows[tfId] is null) continue;
				Rows.Add(
					new TfDataRow(
					this,
					table.Rows[tfId]!.GetValues())
				);
			}
		}
	}

	internal TfDataTable(
		TfDataProvider dataProvider,
		List<SqlBuilderJoinData> joinData,
		List<SqlBuilderSharedColumnData> sharedColumnsData,
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
			query.DatasetId,
			query.Page,
			query.PageSize,
			query.Search);

		Columns = InitColumns(
			dataProvider,
			joinData,
			sharedColumnsData,
			onlyColumns);

		Sql = sql;

		SqlParameters = sqlParameters.AsReadOnly();

		Rows = new TfDataRowCollection(this);
	}

	private TfDataColumnCollection InitColumns(
		TfDataProvider dataProvider,
		List<SqlBuilderJoinData> joinData,
		List<SqlBuilderSharedColumnData> sharedColumnsData,
		List<string> onlyColumns)
	{
		var columns = new TfDataColumnCollection(this);

		columns.Add(new TfDataColumn(
				this,
				"tf_id",
				TfDatabaseColumnType.Guid,
				isNullable: false,
				origin:  TfDataColumnOriginType.System,
				isReadOnly:true));

		//the case we return only tf_ids
		if (onlyColumns != null && onlyColumns.Count == 1 && onlyColumns[0] == "tf_id")
		{
			return columns;
		}

		columns.Add(new TfDataColumn(
			this,
			"tf_row_index",
			TfDatabaseColumnType.Integer,
			isNullable: false,
			origin: TfDataColumnOriginType.System,
			isReadOnly:true));

		columns.Add(new TfDataColumn(
			this,
			"tf_created_on",
			TfDatabaseColumnType.DateTime,
			isNullable: false,
			origin: TfDataColumnOriginType.System,
			isReadOnly:true));

		columns.Add(new TfDataColumn(
			this,
			"tf_updated_on",
			TfDatabaseColumnType.DateTime,
			isNullable: false,
			origin: TfDataColumnOriginType.System,
			isReadOnly:true));

		columns.Add(new TfDataColumn(
			this,
			"tf_search",
			TfDatabaseColumnType.Text,
			isNullable: false,
			origin: TfDataColumnOriginType.System,
			isReadOnly:true));

		foreach (var providerColumn in dataProvider.Columns)
		{
			if (onlyColumns != null && !onlyColumns.Contains(providerColumn.DbName))
				continue;

			columns.Add(new TfDataColumn(
				this,
				providerColumn.DbName!,
				providerColumn.DbType,
				isNullable: providerColumn.IsNullable,
				origin: TfDataColumnOriginType.CurrentProvider,
				isReadOnly:providerColumn.IsReadOnly));
		}		
		
		foreach (var identity in dataProvider.Identities)
		{
			string name = $"tf_ide_{identity.DataIdentity}";
			columns.Add(new TfDataColumn(
				this,
				name,
				TfDatabaseColumnType.ShortText,
				isNullable: false,
				origin: TfDataColumnOriginType.Identity,
				isReadOnly:false));
		}

		foreach (var sharedColumnData in sharedColumnsData )
		{
			var combinedColumnName = $"{sharedColumnData.DataIdentity}.{sharedColumnData.DbName}";

			columns.Add(new TfDataColumn(
				this,
				combinedColumnName,
				sharedColumnData.DbType,
				isNullable: true,
				origin: TfDataColumnOriginType.SharedColumn,
				isReadOnly:false));
		}

		foreach (var data in joinData)
		{
			var columnName = $"jp$dp{data.Provider.Index}${data.DataIdentity}";
			foreach (var joinDataColumn in data.Columns)
			{
				var connectedProviderColumn = data.Provider.Columns.Single(x => x.DbName == joinDataColumn.DbName);

				var combinedColumnName = $"{joinDataColumn.DataIdentity}.{connectedProviderColumn.DbName}";

				columns.Add(new TfDataColumn(
					this,
					combinedColumnName,
					connectedProviderColumn.DbType,
					isNullable: connectedProviderColumn.IsNullable,
					origin: TfDataColumnOriginType.JoinedProviderColumn,
					isReadOnly:false));
			}

		}


		return columns;
	}

	public object? this[Guid rowId, string columnName]
	{
		get
		{
			var row = this.Rows[rowId];
			var column = this.Columns[columnName];
			if (row is null)
				throw new Exception($"Row with tf_id '{rowId}' is not found.");
			if (column is null)
				throw new Exception($"Column with name '{columnName}' is not found.");
			return row[columnName];
		}
		set
		{
			var row = this.Rows[rowId];
			var column = this.Columns[columnName];
			if (row is null)
				throw new Exception($"Row with tf_id '{rowId}' is not found.");
			if (column is null)
				throw new Exception($"Column with name '{columnName}' is not found.");
			row[columnName] = value;
		}
	}

	public object this[int rowIndex, string columnName]
	{
		get
		{
			var row = this.Rows[rowIndex];
			var column = this.Columns[columnName];
			if (row is null)
				throw new Exception($"Row with index '{rowIndex}' is not found.");
			if (column is null)
				throw new Exception($"Column with name '{columnName}' is not found.");
			return row[columnName];
		}
		set
		{
			var row = this.Rows[rowIndex];
			var column = this.Columns[columnName];
			if (row is null)
				throw new Exception($"Row with tf_id '{rowIndex}' is not found.");
			if (column is null)
				throw new Exception($"Column with name '{columnName}' is not found.");
			row[columnName] = value;
		}
	}

	public TfDataTable NewTable(
		params int[] rows)
	{
		return new TfDataTable(this, rows);
	}

	public TfDataTable NewTableFromRowIds(
		params Guid[] tfIds)
	{
		return new TfDataTable(this, tfIds);
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

	public DataTable ToDataTable()
	{
		DataTable dt = new DataTable();
		foreach (var column in this.Columns)
		{
			var columnType = GetTypeForDatabaseColumnType(column.DbType);
			dt.Columns.Add(column.Name, columnType);
		}

		foreach (TfDataRow row in this.Rows)
			dt.Rows.Add(row.Values);

		return dt;
	}

	private Type GetTypeForDatabaseColumnType(TfDatabaseColumnType dbType)
	{
		switch (dbType)
		{
			case TfDatabaseColumnType.Guid:
				return typeof(Guid);
			case TfDatabaseColumnType.ShortText:
				return typeof(string);
			case TfDatabaseColumnType.Text:
				return typeof(string);
			case TfDatabaseColumnType.ShortInteger:
				return typeof(short);
			case TfDatabaseColumnType.Integer:
				return typeof(int);
			case TfDatabaseColumnType.LongInteger:
				return typeof(long);
			case TfDatabaseColumnType.Number:
				return typeof(decimal);
			case TfDatabaseColumnType.DateOnly:
				return typeof(DateOnly);
			case TfDatabaseColumnType.DateTime:
				return typeof(DateTime);
			default:
				throw new Exception("Type is not supported");
		}
	}
}
