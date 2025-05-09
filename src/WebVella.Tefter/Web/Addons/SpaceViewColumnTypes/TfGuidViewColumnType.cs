namespace WebVella.Tefter.Web.Addons;

public class TfGuidViewColumnType : ITfSpaceViewColumnTypeAddon
{
	public const string ID = "b736b4f9-2138-44d2-a5d5-4a320b6556db";
	public const string NAME = "Unique identifier (GUID)";
	public const string DESCRIPTION = "displays GUID value";
	public const string FLUENT_ICON_NAME = "Key";
	public const string VALUE_ALIAS = "Value";

	public Guid AddonId { get; init; } = new Guid(ID);
	public string AddonName { get; init; } = NAME;
	public string AddonDescription { get; init; } = DESCRIPTION;
	public string AddonFluentIconName { get; init; } = FLUENT_ICON_NAME;
	public List<TfSpaceViewColumnAddonDataMapping> DataMapping { get; init; }
	public Type CustomOptionsComponentType { get; init; }
	public List<string> FilterAliases { get; init; }
	public List<string> SortAliases { get; init; }
	public Guid? DefaultComponentId { get; init; }
	public List<Guid> SupportedComponents { get; set; } = new();

	public TfGuidViewColumnType()
	{
		DataMapping = new List<TfSpaceViewColumnAddonDataMapping>
		{
			new TfSpaceViewColumnAddonDataMapping
				{
					Alias = VALUE_ALIAS,
					Description = "this column is compatible with the Guid database column",
					SupportedDatabaseColumnTypes = new List<TfDatabaseColumnType> { TfDatabaseColumnType.Guid }
				}
		};
		FilterAliases = new List<string>() { VALUE_ALIAS };
		SortAliases = new List<string> { VALUE_ALIAS };
		DefaultComponentId = new Guid(TfGuidDisplayColumnComponent.ID);
	}
}

