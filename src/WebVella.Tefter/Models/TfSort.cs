namespace WebVella.Tefter.Models;

public record TfSort
{
	public string ColumnName { get; set; }
	public TfSortDirection Direction { get; set; } = TfSortDirection.ASC;

	public void FixPrefix(string prefix)
	{
		if (!ColumnName.StartsWith(prefix))
		{
			ColumnName = prefix + ColumnName;
		}
	}
	public TfSort() { }

	public TfSort(
		string columnName,
		TfSortDirection direction)
	{
		ColumnName = columnName;
		Direction = direction;
	}
	public TfSort(TfSortQuery model)
	{
		ColumnName = model.Name;
		Direction = (TfSortDirection)model.Direction;
	}
	public TfSortQuery ToQuery()
	{
		return new TfSortQuery
		{
			Name = ColumnName,
			Direction = (int)Direction
		};
	}
	public TfSort FromQuery(TfSortQuery model, List<TfSpaceViewColumn> viewColumns)
	{
		if (model is null) throw new ArgumentException("model is required",nameof(model));
		if (viewColumns is null) throw new ArgumentException("viewColumns is required",nameof(model));
		var column = viewColumns.FirstOrDefault(x => x.QueryName.ToLowerInvariant() == model.Name.ToLowerInvariant());
		var columnName = model.Name;
		if (column is not null)
			columnName = column.GetColumnNameFromDataMapping() ?? model.Name;

		var sort = new TfSort()
		{
			ColumnName = columnName,
			Direction = model.Direction.ConvertIntToEnum<TfSortDirection>(TfSortDirection.ASC)
		};
		return sort;
	}
}

public enum TfSortDirection
{
	[Description("Ascending")]
	ASC = 0,
	[Description("Descending")]
	DESC = 1
}
