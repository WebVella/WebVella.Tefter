namespace WebVella.Tefter;

public abstract class TfFilterBase
{
	public string ColumnName { get; init; }

	public TfFilterBase(string columnName)
	{
		ColumnName = columnName;
	}

}
