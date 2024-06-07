namespace WebVella.Tefter.Demo.Data;

public static partial class SampleData
{
	private static List<DataRow> _data = null;
	private static Dictionary<Guid, DataRow> _dataDict = null;

	public static List<DataRow> GetData()
	{
		if (_data is not null) return _data;
		if (_dataDict is null) _dataDict = new(); else _dataDict.Clear();

		var result = new List<DataRow>();

		for (int i = 0; i < 35; i++)
		{
			var item = new DataRow{
				Id = Guid.NewGuid(),
				CreatedOn = DateTime.Now,
				UpdatedOn = DateTime.Now,
				StringId = Guid.NewGuid().ToString(),
				Fields = new()
			};
			item.Fields["name"] = new DataField{ 
				Type = DataFieldType.Text,
				Value = $"Boz Zashev" + Guid.NewGuid().ToString().Split("-")[0],
			};
			item.Fields["email"] = new DataField
			{
				Type = DataFieldType.Text,
				Value = $"boz{i}@webvella.com"
			};
			item.Fields["age"] = new DataField
			{
				Type = DataFieldType.Number,
				Value = (decimal)19
			};
			item.Fields["wage"] = new DataField
			{
				Type = DataFieldType.Number,
				Value = (decimal)3255
			};
			item.Fields["city"] = new DataField
			{
				Type = DataFieldType.Text,
				Value = "Plovdiv"
			};

			_dataDict[item.Id] = item;
			result.Add(item);
		}
		_data = result;
		return _data;
	}

	public static DataRow GetDataById(Guid id)
	{
		if (_dataDict is null) GetData();
		return _dataDict.ContainsKey(id) ? _dataDict[id] : null;
	}


}
