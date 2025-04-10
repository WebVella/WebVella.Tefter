namespace WebVella.Tefter.Web.Models;

public record TucSpaceViewColumnType
{
	public Guid Id { get; set; }
	public string Name { get; set; }
	public string Description { get; set; }
	public Icon Icon { get; set; }
	public List<TucSpaceViewColumnDataMapping> DataMapping { get; set; }
	[JsonIgnore]
	public Type DefaultComponentType { get; set; }
	public List<Type> SupportedComponentTypes { get; set; } = new();
	public List<string> FilterAliases { get; set; }
	public List<string> SortAliases { get; set; }
	public List<Guid> SupportedAddonTypes { get; set; }

	[JsonIgnore]
	public Type SpaceViewColumnType { get; set; }


	public TucSpaceViewColumnType() { }
	public TucSpaceViewColumnType(ITfSpaceViewColumnTypeAddon model)
	{
		Id = model.Id;
		Name = model.Name;
		Description = model.Description;
		Icon = TfConstants.GetIcon(model.FluentIconName);
		DataMapping = model.DataMapping.Select(x => new TucSpaceViewColumnDataMapping(x)).ToList();
		DefaultComponentType = model.DefaultComponentType;
		SupportedComponentTypes = model.SupportedComponentTypes ?? new List<Type>();
		FilterAliases = model.FilterAliases;
		SortAliases = model.SortAliases;
		SupportedAddonTypes = model.SupportedAddonTypes;
		SpaceViewColumnType = model.GetType();
	}

}

