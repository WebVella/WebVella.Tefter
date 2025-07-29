namespace WebVella.Tefter.UI.Addons;
public class TfTextViewColumnType : ITfSpaceViewColumnTypeAddon
{
	public const string ID = "f061a3ce-7813-4fd6-98cb-a10cccea4797";
	public const string NAME = "Text";
	public const string DESCRIPTION = "displays text";
	public const string FLUENT_ICON_NAME = "TextCaseTitle";
	public const string VALUE_ALIAS = "Value";

	public Guid AddonId { get; init; } = new Guid(ID);
	public string AddonName { get; init; } = NAME;
	public string AddonDescription { get; init; } = DESCRIPTION;
	public string AddonFluentIconName { get; init; } = FLUENT_ICON_NAME;
	public List<TfSpaceViewColumnAddonDataMapping> DataMapping { get; init; }
	public List<string> FilterAliases { get; init; }
	public List<string> SortAliases { get; init; }
	public Guid? DefaultComponentId { get; init; }
	public List<Guid> SupportedComponents { get; set; } = new();
	public TfTextViewColumnType()
	{
		DataMapping = new List<TfSpaceViewColumnAddonDataMapping>
		{
			new TfSpaceViewColumnAddonDataMapping
				{
					Alias = VALUE_ALIAS,
					Description = "this column is compatible with all column types, but its intended use is with text",
					SupportedDatabaseColumnTypes = new List<TfDatabaseColumnType> {
						TfDatabaseColumnType.ShortText,
						TfDatabaseColumnType.Text
					}
				}
		};
		FilterAliases = new List<string>() { VALUE_ALIAS };
		SortAliases = new List<string> { VALUE_ALIAS };
		DefaultComponentId = new Guid(TucTextDisplayColumnComponent.ID);
	}
}

