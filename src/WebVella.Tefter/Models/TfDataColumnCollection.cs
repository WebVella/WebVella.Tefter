namespace WebVella.Tefter;

public class TfDataColumnCollection : IEnumerable
{
	private readonly List<TfDataColumn> _columns;

	public TfDataTable DataTable { get; init; }

	public int Count => _columns.Count;

	public TfDataColumn this[string columnName]
	{
		get
		{
			if (string.IsNullOrWhiteSpace(columnName))
				throw new ArgumentException(nameof(columnName));

			var column = _columns.SingleOrDefault(x => x.Name == columnName);

			if (column == null)
				throw new Exception($"A column with " +
					$"name {columnName} is not found in collection.");

			return column;
		}
	}

	public TfDataColumnCollection(TfDataTable dataTable)
	{
		if (dataTable is null)
			throw new ArgumentNullException(nameof(dataTable));

		_columns = new List<TfDataColumn>();
		DataTable = dataTable;
	}

	public void Add(TfDataColumn column)
	{
		if (column is null)
			throw new ArgumentNullException(nameof(column));

		if (_columns.Any(x => x.Name == column.Name))
		{
			throw new Exception($"A column with name " +
				$"{column.Name} already exists in collection.");
		}

		_columns.Add(column);
	}

	public void Remove(TfDataColumn column)
	{
		if (column is null)
			throw new ArgumentNullException(nameof(column));

		var index = _columns.IndexOf(column);

		if (index == -1)
			throw new Exception($"The column is not found in collection.");

		_columns.Remove(column);
	}


	public int IndexOf(Func<TfDataColumn, bool> predicate)
	{
		var index = 0;
		foreach (var item in _columns)
		{
			if (predicate.Invoke(item))
			{
				return index;
			}
			index++;
		}

		return -1;
	}

	public IEnumerator<TfDataColumn> GetEnumerator()
	{
		return _columns.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return _columns.GetEnumerator();
	}
}
