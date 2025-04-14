namespace WebVella.Tefter.Seeds.Addons;

public class SampleViewColumnType : ITfSpaceViewColumnTypeAddon
{
	public const string ID = "cbe7a847-72a5-4af5-af7f-98771e043f14";
	public const string NAME = "Percent";
	public const string DESCRIPTION = "displays percent based of value between 0 and specified in settings value";
	public const string FLUENT_ICON_NAME = "PercentSymbol";
	public const string ALIAS = "Value";

	public Guid Id { get; init; } = new Guid(ID);
	public string Name { get; init; } = NAME;
	public string Description { get; init; } = DESCRIPTION;
	public string FluentIconName { get; init; } = FLUENT_ICON_NAME;
	public List<TfSpaceViewColumnAddonDataMapping> DataMapping { get; init; }
	public Guid? DefaultComponentId { get; init; }
	public List<Guid> SupportedComponents { get; set; } = new();
	public List<string> FilterAliases { get; init; }
	public List<string> SortAliases { get; init; }
	public List<Guid> SupportedAddonTypes { get; init; } = new();

	public SampleViewColumnType()
	{
		DataMapping = new List<TfSpaceViewColumnAddonDataMapping>
		{
			new TfSpaceViewColumnAddonDataMapping
				{
					Alias = ALIAS,
					Description = "this column is compatible with all column types, but its intended use is with text",
					SupportedDatabaseColumnTypes = new List<TfDatabaseColumnType> {
						TfDatabaseColumnType.ShortText,
						TfDatabaseColumnType.Text
					}
			}
		};
		FilterAliases = new List<string>() { ALIAS };
		SortAliases = new List<string> { ALIAS };
		DefaultComponentId = new Guid(SampleDisplayViewColumnComponent.ID);
	}
}

