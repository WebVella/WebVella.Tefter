namespace WebVella.Tefter.Demo.Data;

public static partial class SampleData
{
	private static List<DataSource> _dataSource = null;
	private static Dictionary<Guid, DataSource> _dataSourceDict = null;
	public static List<DataSource> GetDataSource()
	{
		if (_dataSource is not null) return _dataSource;
		if(_dataSourceDict is null) _dataSourceDict= new(); else _dataSourceDict.Clear();

		var result = new List<DataSource>();

		for (int i = 0; i < 35; i++)
		{
			var item = DataSource.GetFaker().Generate();
			_dataSourceDict[item.Id] = item;
			result.Add(item);
		}
		_dataSource = result;
		return _dataSource;
	}

	public static DataSource GetDataSourceById(Guid id)
	{
		if (_dataSourceDict is null) GetDataSource();
		return _dataSourceDict.ContainsKey(id) ? _dataSourceDict[id] : null;
	}
}
