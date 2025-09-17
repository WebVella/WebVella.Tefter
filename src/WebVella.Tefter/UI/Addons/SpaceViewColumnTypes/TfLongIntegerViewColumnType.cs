namespace WebVella.Tefter.UI.Addons;

public class TfLongIntegerViewColumnType : ITfSpaceViewColumnTypeAddon
{
	public const string ID = "22d5436c-1dec-4d1d-bb4d-f197f19c9d12";
	public const string NAME = "Long integer";
	public const string DESCRIPTION = "displays very big integer numbers.";
	public const string FLUENT_ICON_NAME = "NumberSymbol";
	public const string VALUE_ALIAS = "Value";

	public Guid AddonId { get; init; } = new Guid(ID);
	public string AddonName { get; init; } = NAME;
	public string AddonDescription { get; init; } = DESCRIPTION;
	public string AddonFluentIconName { get; init; } = FLUENT_ICON_NAME;
	public List<TfSpaceViewColumnAddonDataMapping> DataMapping { get; init; }
	public List<string> FilterAliases { get; init; }
	public List<string> SortAliases { get; init; }
	public Guid? DefaultDisplayComponentId { get; init; }
	public List<Guid> SupportedComponents { get; set; } = new();
	public Guid? DefaultEditComponentId { get; init; }

	public TfLongIntegerViewColumnType()
	{
		DataMapping = new List<TfSpaceViewColumnAddonDataMapping>
		{
			new TfSpaceViewColumnAddonDataMapping
				{
					Alias = VALUE_ALIAS,
					Description = "this column is compatible with all integer database column types",
					SupportedDatabaseColumnTypes = new List<TfDatabaseColumnType> {
						TfDatabaseColumnType.ShortInteger,
						TfDatabaseColumnType.Integer,
						TfDatabaseColumnType.LongInteger }
			}
		};

		FilterAliases = new List<string>() { VALUE_ALIAS };

		SortAliases = new List<string> { VALUE_ALIAS };

		DefaultEditComponentId = new Guid(TucLongIntegerEditColumnComponent.ID);
	}
}

