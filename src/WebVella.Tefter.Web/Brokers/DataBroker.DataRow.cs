﻿namespace WebVella.Tefter.Web.Brokers;

public partial interface IDataBroker
{
	ValueTask<List<DemoDataRow>> GetDataAsync();
}

public partial class DataBroker : IDataBroker
{
	private static List<DemoDataRow> _data = null;
	private static Dictionary<Guid, DemoDataRow> _dataDict = null;
	public async ValueTask<List<DemoDataRow>> GetDataAsync()
	{
		if (_data is not null) return _data;
		if (_dataDict is null) _dataDict = new(); else _dataDict.Clear();

		var result = new List<DemoDataRow>();

		for (int i = 0; i < 35; i++)
		{
			var item = new DemoDataRow
			{
				Id = Guid.NewGuid(),
				CreatedOn = DateTime.Now,
				UpdatedOn = DateTime.Now,
				StringId = Guid.NewGuid().ToString(),
				Fields = new()
			};
			item.Fields["name"] = new DataField
			{
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
			item.Fields["comments"] = new DataField
			{
				Type = DataFieldType.Number,
				Value = (decimal)32
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

	public async ValueTask<DemoDataRow> GetDataById(Guid id)
	{
		if (_dataDict is null) await GetDataAsync();
		return _dataDict.ContainsKey(id) ? _dataDict[id] : null;
	}
}
