namespace WebVella.Tefter.Models;

public class TfSort
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
}

public enum TfSortDirection
{
	ASC = 0,
	DESC = 1
}
