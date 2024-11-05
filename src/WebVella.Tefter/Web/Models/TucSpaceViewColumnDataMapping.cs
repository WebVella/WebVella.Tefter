namespace WebVella.Tefter.Web.Models;

public record TucSpaceViewColumnDataMapping
{
	public string Alias { get; init; }
	public string Description { get; init; }
	public List<TucDatabaseColumnType> SupportedDatabaseColumnTypes { get; init; }
	public TucSpaceViewColumnDataMapping() { }

	public TucSpaceViewColumnDataMapping(TfSpaceViewColumnDataMapping model)
	{
		Alias = model.Alias;
		Description = model.Description;
		SupportedDatabaseColumnTypes = new();
		foreach (var colType in model.SupportedDatabaseColumnTypes)
		{
			SupportedDatabaseColumnTypes.Add(colType.ConvertSafeToEnum<TfDatabaseColumnType, TucDatabaseColumnType>());
		}
	}

	public TfSpaceViewColumnDataMapping ToModel()
	{
		var model = new TfSpaceViewColumnDataMapping()
		{
			Alias = Alias,
			Description = Description,
			SupportedDatabaseColumnTypes = new()
		};
		foreach (var colType in SupportedDatabaseColumnTypes)
		{
			model.SupportedDatabaseColumnTypes.Add(colType.ConvertSafeToEnum<TucDatabaseColumnType, TfDatabaseColumnType>());
		}
		return model;
	}
}
