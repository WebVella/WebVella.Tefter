namespace WebVella.Tefter.Web.Addons;

public class TfShortIntegerViewColumnType : ITfSpaceViewColumnTypeAddon
{
	public const string ID = "e8a52dfe-fcb8-4fd2-8011-bd9e0a5a81d9";
	public const string NAME = "Short Integer";
	public const string DESCRIPTION = "displays small integer numbers.";
	public const string FLUENT_ICON_NAME = "NumberSymbol";
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

	public TfShortIntegerViewColumnType()
	{
		DataMapping = new List<TfSpaceViewColumnAddonDataMapping>
		{
			new TfSpaceViewColumnAddonDataMapping
				{
					Alias = VALUE_ALIAS,
					Description = "this column is compatible with short integer database column types",
					SupportedDatabaseColumnTypes = new List<TfDatabaseColumnType> {
						TfDatabaseColumnType.ShortInteger
						}
			}
		};

		FilterAliases = new List<string>() { VALUE_ALIAS };

		SortAliases = new List<string> { VALUE_ALIAS };

		DefaultComponentId = new Guid(TfShortIntegerDisplayColumnComponent.ID);
	}
}

