namespace WebVella.Tefter;

public class TfDataRowCollection : IEnumerable
{
	private readonly List<TfDataRow> _rows;

	public TfDataTable DataTable { get; init; }

	public int Count => _rows.Count;

	public TfDataRow this[int rowIndex]
	{
		get { return _rows[rowIndex]; }
	}

	public TfDataRow this[Guid tfId]
	{
		get { return _rows.SingleOrDefault(x=> (Guid)x["tf_id"] == tfId) }
	}

	public TfDataRowCollection(TfDataTable table)
	{
		_rows = new List<TfDataRow>();
		DataTable = table;
	}

	public void Add(TfDataRow row)
	{
		if (row is null)
			throw new ArgumentNullException(nameof(row));

		_rows.Add(row);
	}

	public void Remove(TfDataRow row)
	{
		if (row is null)
			throw new ArgumentNullException(nameof(row));

		var index = _rows.IndexOf(row);

		if (index == -1)
			throw new Exception($"The row is not found in collection.");

		_rows.Remove(row);
	}

	public IEnumerator GetEnumerator()
	{
		return _rows.GetEnumerator();
	}

	public override string ToString()
	{
		return $"{Count} row(s)";
	}

}