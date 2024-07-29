namespace WebVella.Tefter;

public class TfDataRow : IEnumerable
{
	private readonly object[] _values;

	public TfDataTable DataTable { get; init; }

	public object this[int columnIndex]
	{
		get { return this[columnIndex]; }
	}

	public object this[string columnName]
	{
		get
		{
			if (string.IsNullOrWhiteSpace(columnName))
				throw new ArgumentException(nameof(columnName));

			int index = DataTable.Columns.IndexOf(x => x.Name == columnName);
			if (index == -1)
				throw new Exception($"A column with name {columnName} is not found in DataTable object.");

			return _values[index];
		}
	}

	public TfDataRow(TfDataTable dataTable, object[] values)
	{
		_values = values;
		this.DataTable = dataTable;
	}

	public IEnumerator GetEnumerator()
	{
		return _values.GetEnumerator();
	}
}
