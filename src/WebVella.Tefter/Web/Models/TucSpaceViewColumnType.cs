namespace WebVella.Tefter.Web.Models;

public record TucSpaceViewColumnType
{
	public Guid Id { get; set; }
	public string Name { get; set; }
	public string Description { get; set; }
	public Icon FluentIcon { get; set; }
	public List<TucSpaceViewColumnDataMapping> DataMapping { get; set; }
	public List<string> FilterAliases { get; set; }
	public List<string> SortAliases { get; set; }
	public Guid? DefaultComponentId { get; set; }
	public List<Guid> SupportedComponents { get; set; }
	public Type Type { get; set; }

	public TucSpaceViewColumnType() { }
	public TucSpaceViewColumnType(ITfSpaceViewColumnTypeAddon model)
	{
		Id = model.Id;
		Name = model.Name;
		Description = model.Description;
		FluentIcon = TfConstants.GetIcon(model.FluentIconName);
		DataMapping = model.DataMapping.Select(x => new TucSpaceViewColumnDataMapping(x)).ToList();
		DefaultComponentId = model.DefaultComponentId;
		SupportedComponents = model.SupportedComponents;
		FilterAliases = model.FilterAliases;
		SortAliases = model.SortAliases;
		Type = model.GetType();
	}

}

