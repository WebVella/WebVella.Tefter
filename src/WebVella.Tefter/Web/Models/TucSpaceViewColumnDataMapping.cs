namespace WebVella.Tefter.Web.Models;

public record TucSpaceViewColumnDataMapping
{
	public string Alias { get; init; }
	public string Description { get; init; }
	public List<TfDatabaseColumnType> SupportedDatabaseColumnTypes { get; init; }
	public TucSpaceViewColumnDataMapping() { }

	public TucSpaceViewColumnDataMapping(TfSpaceViewColumnAddonDataMapping model)
	{
		Alias = model.Alias;
		Description = model.Description;
		SupportedDatabaseColumnTypes = new();
		foreach (var colType in model.SupportedDatabaseColumnTypes)
		{
			SupportedDatabaseColumnTypes.Add(colType);
		}
	}

	public TfSpaceViewColumnAddonDataMapping ToModel()
	{
		var model = new TfSpaceViewColumnAddonDataMapping()
		{
			Alias = Alias,
			Description = Description,
			SupportedDatabaseColumnTypes = new()
		};
		foreach (var colType in SupportedDatabaseColumnTypes)
		{
			model.SupportedDatabaseColumnTypes.Add(colType);
		}
		return model;
	}
}
