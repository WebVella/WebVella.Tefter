namespace WebVella.Tefter.Demo.Data;

public static partial class SampleData
{
	private static List<Space> _spaces = null;
	private static Dictionary<Guid, Space> _spaceDict = null;
	private static Dictionary<Guid, SpaceData> _spaceDataDict = null;
	private static Dictionary<Guid, SpaceView> _spaceViewDict = null;

	public static List<Space> GetSpaces()
	{

		if (_spaces is not null) return _spaces;
		if (_spaceDict is null) _spaceDict = new(); else _spaceDict.Clear();
		if (_spaceDataDict is null) _spaceDataDict = new(); else _spaceDataDict.Clear();
		if (_spaceViewDict is null) _spaceViewDict = new(); else _spaceViewDict.Clear();

		var result = new List<Space>();
		//Ids need to be fixed for easier development
		var spaceIds = new List<Guid>(){
			new Guid("ce1b788f-b1b2-4d80-ba44-6ef21a3c1a05"),
			new Guid("db89b752-8885-447b-9ba9-af7b258d34dc"),
			new Guid("28dce3dd-a947-4c1b-9621-b4efdc496f45"),
		};
		var spaceDataIds = new List<Guid>(){
			new Guid("e94c9dcb-a90f-4074-a767-c9ac8f27d801"),
			new Guid("6a3e5c9b-8164-4d0c-9596-fb8ea856fe7a"),
			new Guid("9ecccc6f-6a3f-4f15-8f3e-dd24f325c0b9"),
			new Guid("88d62271-bdf2-42e9-8cb1-37b874cc43a5"),
			new Guid("14ae38d4-dd7e-440b-857e-f99de9fcab65"),
			new Guid("48189e7d-243f-4a05-a71a-db6a236ad70d"),
			new Guid("41d68149-45b8-46ce-b67c-23f17357586d"),
			new Guid("528ac40a-bfc5-494f-870a-4c7fd6b7d100"),
			new Guid("5f2f0607-b300-44e5-bf57-8d0d300e0cc4"),
			new Guid("3cb1f386-43e5-4966-a5d0-bff6d01c82ec"),
			new Guid("8c3b5196-ab9d-4203-8a61-b05688a25741"),
			new Guid("4fa5c29c-6d80-4713-8464-aba655752b54"),
			new Guid("b2808f5c-a52b-46e9-be3e-fe3a935f2119"),
			new Guid("b7819a1b-cb81-468d-bb76-335ad844d095"),
			new Guid("da270d5a-15f8-4386-907e-69a9e2ca66aa"),
			new Guid("58d94bff-2227-4d40-adef-a04dd09fcd87"),
			new Guid("63556014-c4b7-42ba-97c7-92a8be35ad1e"),
			new Guid("b7fd161b-d3f8-4438-a446-183e1cfe00b3"),
			new Guid("7100fb6d-9159-4a74-b0ee-774feeec7c69"),
			new Guid("6801d54a-0853-41d6-84ef-308061380d1b"),
			new Guid("20774230-9e23-49d5-a888-3978e5d9e7f0"),
		};

		var spaceViewIds = new List<Guid>(){
			new Guid("39cd80e4-9b6d-4df9-a211-376cf1caf69f"),
			new Guid("5d6f56cc-b310-4497-8f32-2dd20116d81a"),
			new Guid("8f3ed396-49bc-46ca-aa74-158f50eb7d48"),
			new Guid("cedba99a-557a-4e4f-948f-71a503c4b8e7"),
			};
		for (int i = 0; i < 3; i++)
		{
			var space = Space.GetFaker().Generate();
			space.Id = spaceIds[i];
			space.Position = i + 1;
			for (int j = 0; j < 7; j++)
			{
				var spaceData = SpaceData.GetFaker().Generate();
				spaceData.Id = spaceDataIds[(i * 7) + j];
				spaceData.SpaceId = space.Id;
				spaceData.Position = j + 1;

				for (int f = 0; f < 4; f++)
				{
					var spaceView = SpaceView.GetFaker().Generate();
					if (f == 0) spaceView.Name = "Main";
					spaceView.Position = f;
					spaceView.SpaceId = space.Id;
					spaceView.SpaceDataId = spaceData.Id;
					_spaceViewDict[spaceView.Id] = spaceView;
					spaceData.Views.Add(spaceView);
				}
				spaceData.MainViewId = spaceData.Views[0].Id;

				_spaceDataDict[spaceData.Id] = spaceData;
				space.DataItems.Add(spaceData);
			}
			_spaceDict[space.Id] = space;
			result.Add(space);
		}

		_spaces = result;
		return _spaces;
	}

	public static Space GetSpaceById(Guid id)
	{
		if (_spaceDict is null) GetSpaces();

		return _spaceDict.ContainsKey(id) ? _spaceDict[id] : null;
	}

	public static SpaceData GetSpaceDataById(Guid id)
	{
		if (_spaceDataDict is null) GetSpaces();

		return _spaceDataDict.ContainsKey(id) ? _spaceDataDict[id] : null;
	}

	public static SpaceView GetSpaceViewById(Guid id)
	{
		if (_spaceViewDict is null) GetSpaces();

		return _spaceViewDict.ContainsKey(id) ? _spaceViewDict[id] : null;
	}

	public static List<SpaceView> GetBookmaredByUserId(string search, Guid userId, int page, int pageSize)
	{
		var searchString = search?.Trim().ToLowerInvariant();
		var result = new List<SpaceView>();
		foreach (var viewId in _spaceViewDict.Keys)
		{
			var view = _spaceViewDict[viewId];
			if(!String.IsNullOrWhiteSpace(searchString) && !view.Name.ToLowerInvariant().Contains(searchString))
				continue;

			if (view.IsBookmarked)
				result.Add(view);
		}
		return result.Skip(RenderUtils.CalcSkip(pageSize,page)).Take(pageSize).ToList();
	}
}
