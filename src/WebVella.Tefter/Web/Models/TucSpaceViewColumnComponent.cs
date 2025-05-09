namespace WebVella.Tefter.Web.Models;

public record TucSpaceViewColumnComponent
{
	public Guid Id { get; set; }
	public string Name { get; set; }
	public string Description { get; set; }
	public Icon FluentIcon { get; set; }
	public List<Guid> SupportedColumnTypes { get; set; }
	public Type Type { get; set; }

	public TucSpaceViewColumnComponent() { }
	public TucSpaceViewColumnComponent(ITfSpaceViewColumnComponentAddon model)
	{
		Id = model.AddonId;
		Name = model.AddonName;
		Description = model.AddonDescription;
		FluentIcon = TfConstants.GetIcon(model.AddonFluentIconName);
		SupportedColumnTypes = model.SupportedColumnTypes;
		Type = model.GetType();
	}

}

