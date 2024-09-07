namespace WebVella.Tefter.Web.Models;

public record TucSpaceViewColumnType
{
	public Guid Id { get; set; }
	public string Name { get; set; }
	public string Description { get; set; }
	public Icon Icon { get; set; }
	public List<TucSpaceViewColumnDataMapping> DataMapping { get; set; }
	public Type DefaultComponentType { get; set; }
	public List<Type> SupportedComponentTypes { get; set; }
	public List<Option<string>> SupportedComponentTypeOptions
	{
		get
		{
			var result = new List<Option<string>>();
			if (SupportedComponentTypes is not null)
			{
				foreach (var item in SupportedComponentTypes)
				{
					result.Add(new Option<string> { Text = item.ToDescriptionString(), Value = item.FullName });
				}
			}

			return result;
		}
	}
	public List<string> FilterAliases { get; set; }
	public List<string> SortAliases { get; set; }
	public List<Guid> SupportedAddonTypes { get; set; }

	public TucSpaceViewColumnType() { }
	public TucSpaceViewColumnType(ITfSpaceViewColumnType model)
	{
		Id = model.Id;
		Name = model.Name;
		Description = model.Description;
		Icon = TfConstants.GetIcon(model.Icon);
		DataMapping = model.DataMapping.Select(x => new TucSpaceViewColumnDataMapping(x)).ToList();
		DefaultComponentType = model.DefaultComponentType;
		SupportedComponentTypes = model.SupportedComponentTypes;
		FilterAliases = model.FilterAliases;
		SortAliases = model.SortAliases;
		SupportedAddonTypes = model.SupportedAddonTypes;
	}
}
