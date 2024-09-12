namespace WebVella.Tefter.Models;

public class TfSort
{
	public string DbName { get; set; }
	public TfSortDirection Direction { get; set; } = TfSortDirection.ASC;
}

public enum TfSortDirection
{
	ASC = 0,
	DESC = 1
}
