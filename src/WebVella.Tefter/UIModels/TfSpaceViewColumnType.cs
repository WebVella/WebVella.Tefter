namespace WebVella.Tefter.Models;

public record TfSpaceViewColumnType
{
	public Guid Id { get; set; }
	public string Name { get; set; } = default!;
	public string Description { get; set; } = default!;
	public Icon FluentIcon { get; set; } = default!;
	public List<TfSpaceViewColumnAddonDataMapping> DataMapping { get; set; } = new();
	public List<string> FilterAliases { get; set; } = new();
	public List<string> SortAliases { get; set; } = new();
	public Guid? DefaultDisplayComponentId { get; set; } = null;
	public List<Guid> SupportedDisplayComponents { get; set; } = new();
	public Guid? DefaultEditComponentId { get; init; }
	public Type Type { get; set; } = default!;

	public TfSpaceViewColumnType() { }
	public TfSpaceViewColumnType(ITfSpaceViewColumnTypeAddon model)
	{
		Id = model.AddonId;
		Name = model.AddonName;
		Description = model.AddonDescription;
		FluentIcon = TfConstants.GetIcon(model.AddonFluentIconName) ?? TfConstants.GetIcon("Document") ?? TfConstants.ErrorIcon;
		DataMapping = model.DataMapping;
		DefaultDisplayComponentId = model.DefaultDisplayComponentId;
		SupportedDisplayComponents = model.SupportedComponents;
		FilterAliases = model.FilterAliases;
		SortAliases = model.SortAliases;
		Type = model.GetType();
	}

}

