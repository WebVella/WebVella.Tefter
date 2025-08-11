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
	public TfSort(){}

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
}

public enum TfSortDirection
{
	[Description("Ascending")]
	ASC = 0,
	[Description("Descending")]
	DESC = 1
}
