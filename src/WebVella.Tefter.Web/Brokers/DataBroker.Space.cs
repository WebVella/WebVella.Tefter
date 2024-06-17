namespace WebVella.Tefter.Web.Brokers;

public partial interface IDataBroker
{
	ValueTask<List<Space>> GetSpacesAsync();
	ValueTask<List<Space>> GetSpacesForUserAsync(Guid userId);
	ValueTask<SpaceViewMeta> GetSpaceViewMetaAsync(Guid spaceViewId);
	ValueTask<Space> GetSpaceById(Guid id);
	ValueTask<SpaceData> GetSpaceDataById(Guid id);
	ValueTask<SpaceView> GetSpaceViewById(Guid id);
	ValueTask<List<SpaceView>> GetBookmaredByUserId(string search, Guid userId, int page, int pageSize);
}

public partial class DataBroker : IDataBroker
{
	private static List<Space> _spaces = null;
	private static Dictionary<Guid, Space> _spaceDict = null;
	private static Dictionary<Guid, SpaceData> _spaceDataDict = null;
	private static Dictionary<Guid, SpaceView> _spaceViewDict = null;

	public async ValueTask<List<Space>> GetSpacesAsync()
	{

		if (_spaces is not null) return _spaces;
		if (_spaceDict is null) _spaceDict = new(); else _spaceDict.Clear();
		if (_spaceDataDict is null) _spaceDataDict = new(); else _spaceDataDict.Clear();
		if (_spaceViewDict is null) _spaceViewDict = new(); else _spaceViewDict.Clear();

		var result = new List<Space>();
		var spaceCount = 3;
		var spaceDataCount = 15;
		var spaceViewCount = 5;

		//Ids need to be fixed for easier development
		var spaceIds = new List<Guid>(){
			new Guid("ce1b788f-b1b2-4d80-ba44-6ef21a3c1a05"),
			new Guid("db89b752-8885-447b-9ba9-af7b258d34dc"),
			new Guid("28dce3dd-a947-4c1b-9621-b4efdc496f45"),
		};

		for (int i = 0; i < spaceCount - spaceIds.Count; i++)
		{
			spaceIds.Add(Guid.NewGuid());
		}

		for (int i = 0; i < spaceCount; i++)
		{
			var spaceBuilder = new SpaceBuilder(Space.GetFaker().Generate());
			var spaceId = spaceIds[i];
			spaceBuilder.Id = spaceIds[i];
			spaceBuilder.Position = i + 1;
			for (int j = 0; j < spaceDataCount; j++)
			{
				var spaceDataBuilder = new SpaceDataBuilder(SpaceData.GetFaker().Generate());
				spaceDataBuilder.Id = Guid.NewGuid();

				spaceDataBuilder.SpaceId = spaceBuilder.Id;
				spaceDataBuilder.Position = j + 1;
				spaceDataBuilder.SpaceName = spaceBuilder.Name;
				for (int f = 0; f < spaceViewCount; f++)
				{
					var spaceViewBuilder = new SpaceViewBuilder(SpaceView.GetFaker().Generate());
					if (f == 0) spaceViewBuilder.Name = "Main";
					spaceViewBuilder.Id = Guid.NewGuid();
					spaceViewBuilder.Position = f;
					spaceViewBuilder.SpaceId = spaceBuilder.Id;
					spaceViewBuilder.SpaceDataId = spaceDataBuilder.Id;
					spaceViewBuilder.SpaceName = spaceBuilder.Name;
					spaceViewBuilder.SpaceDataName = spaceDataBuilder.Name;

					spaceViewBuilder.Meta = await GetSpaceViewMetaAsync(spaceViewBuilder.Id);

					_spaceViewDict[spaceViewBuilder.Id] = spaceViewBuilder.Buid();
					spaceDataBuilder.Views.Add(_spaceViewDict[spaceViewBuilder.Id]);
				}

				_spaceDataDict[spaceDataBuilder.Id] = spaceDataBuilder.Build();
				spaceBuilder.DataItems.Add(_spaceDataDict[spaceDataBuilder.Id]);
			}

			_spaceDict[spaceBuilder.Id] = spaceBuilder.Buid();
			result.Add(_spaceDict[spaceBuilder.Id]);
		}

		_spaces = result;
		return _spaces;
	}

	public async ValueTask<List<Space>> GetSpacesForUserAsync(Guid userId)
	{
		return await GetSpacesAsync();
	}

	public async ValueTask<SpaceViewMeta> GetSpaceViewMetaAsync(Guid spaceViewId)
	{
		var columns = new List<SpaceViewColumn>();
		var index = 1;
		columns.Add(new SpaceViewColumn
		{
			Id = new Guid("79e3eadb-be66-44aa-a3ce-dac7b1774f27"),
			SpaceViewId = spaceViewId,
			ColumnName = "name",
			Title = "Name",
			Position = index,
			DataType = SpaceViewColumnType.Name,
			IsVisible = true,
			CellComponent = "WebVella.Tefter.Web.Components.TfTextColumn",
			Width= 200
		});
		index++;
		columns.Add(new SpaceViewColumn
		{
			Id = new Guid("25677090-baee-49cc-a314-fd71c88ba1ee"),
			SpaceViewId = spaceViewId,
			ColumnName = "city",
			Title = "City",
			Position = index,
			DataType = SpaceViewColumnType.Text,
			IsVisible = true,
			CellComponent = "WebVella.Tefter.Web.Components.TfTextColumn"
		});
		index++;
		columns.Add(new SpaceViewColumn
		{
			Id = new Guid("2056384e-8e89-47fb-bcc1-e4acc1281c4d"),
			SpaceViewId = spaceViewId,
			ColumnName = "email",
			Title = "Email",
			Position = index,
			DataType = SpaceViewColumnType.Email,
			IsVisible = true,
			CellComponent = "WebVella.Tefter.Web.Components.TfTextColumn"
		});
		index++;
		columns.Add(new SpaceViewColumn
		{
			Id = new Guid("6278f849-0a16-430a-a933-577525f11304"),
			SpaceViewId = spaceViewId,
			ColumnName = "age",
			Title = "Age",
			Position = index,
			DataType = SpaceViewColumnType.Numeric,
			IsVisible = true,
			CellComponent = "WebVella.Tefter.Web.Components.TfTextColumn"
		});
		index++;
		columns.Add(new SpaceViewColumn
		{
			Id = new Guid("9331de72-47c7-4941-9105-985d4cfec0e3"),
			SpaceViewId = spaceViewId,
			ColumnName = "wage",
			Title = "Wage",
			Position = index,
			DataType = SpaceViewColumnType.Currency,
			IsVisible = true,
			CellComponent = "WebVella.Tefter.Web.Components.TfTextColumn"
		});
		index++;



		return new SpaceViewMeta { Columns = columns };
	}

	public async ValueTask<Space> GetSpaceById(Guid id)
	{
		if (_spaceDict is null) await GetSpacesAsync();

		return _spaceDict.ContainsKey(id) ? _spaceDict[id] : null;
	}

	public async ValueTask<SpaceData> GetSpaceDataById(Guid id)
	{
		if (_spaceDict is null) await GetSpacesAsync();

		return _spaceDataDict.ContainsKey(id) ? _spaceDataDict[id] : null;
	}

	public async ValueTask<SpaceView> GetSpaceViewById(Guid id)
	{
		if (_spaceDict is null) await GetSpacesAsync();

		return _spaceViewDict.ContainsKey(id) ? _spaceViewDict[id] : null;
	}

	public async ValueTask<List<SpaceView>> GetBookmaredByUserId(string search, Guid userId, int page, int pageSize)
	{
		var searchString = search?.Trim().ToLowerInvariant();
		var result = new List<SpaceView>();
		foreach (var viewId in _spaceViewDict.Keys)
		{
			var view = _spaceViewDict[viewId];
			if (!String.IsNullOrWhiteSpace(searchString) && !view.Name.ToLowerInvariant().Contains(searchString))
				continue;

			if (view.IsBookmarked)
				result.Add(view);
		}
		return result.Skip(RenderUtils.CalcSkip(pageSize, page)).Take(pageSize).ToList();
	}
}
