namespace WebVella.Tefter.Models;

public class TfDataProviderDataRow
{
	private readonly List<string> _warnings;
	private readonly List<string> _errors;
	private readonly Dictionary<string, object> _data;

	public ReadOnlyCollection<string> ColumnNames => _data.Keys.ToList().AsReadOnly();
	public ReadOnlyCollection<object> Values => _data.Values.ToList().AsReadOnly();
	public ReadOnlyCollection<string> Warnings => _warnings.ToList().AsReadOnly();
	public ReadOnlyCollection<string> Errors => _errors.ToList().AsReadOnly();

	public object this[string columnName]
	{
		get { return _data[columnName]; }
		set { _data[columnName] = value; }
	}

	public TfDataProviderDataRow()
	{
		_data = new Dictionary<string, object>();
		_warnings = new List<string>();
		_errors = new List<string>();
	}

	public void AddError(
		string error)
	{
		_errors.Add(error);
	}

	public void AddWarning(
		string warning)
	{
		_warnings.Add(warning);
	}
}
