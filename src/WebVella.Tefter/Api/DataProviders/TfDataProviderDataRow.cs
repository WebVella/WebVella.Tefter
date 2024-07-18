namespace WebVella.Tefter;

public class TfDataProviderDataRow
{
	private readonly Dictionary<string, object> _data;
	public ReadOnlyCollection<string> ColumnNames => _data.Keys.ToList().AsReadOnly();
	public ReadOnlyCollection<object> Values => _data.Values.ToList().AsReadOnly();

	public object this[string columnName]
	{
		get { return _data[columnName]; }
		set { _data[columnName] = value; }
	}

	public TfDataProviderDataRow()
	{
		_data = new Dictionary<string, object>();
	}
}
 