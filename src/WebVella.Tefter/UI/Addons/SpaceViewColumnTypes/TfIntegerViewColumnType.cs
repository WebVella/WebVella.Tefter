namespace WebVella.Tefter.UI.Addons;

public class TfIntegerViewColumnType : ITfSpaceViewColumnTypeAddon
{
	public const string ID = "a0708248-ebfc-4ba9-84e9-3f959c06989c";
	public const string NAME = "Integer";
	public const string DESCRIPTION = "displays integer numbers.";
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

	public TfIntegerViewColumnType()
	{
		DataMapping = new List<TfSpaceViewColumnAddonDataMapping>
		{
			new TfSpaceViewColumnAddonDataMapping
				{
					Alias = VALUE_ALIAS,
					Description = "this column is compatible with integer and short integer database column types",
					SupportedDatabaseColumnTypes = new List<TfDatabaseColumnType> {
						TfDatabaseColumnType.ShortInteger,
						TfDatabaseColumnType.Integer
						}
			}
		};

		FilterAliases = new List<string>() { VALUE_ALIAS };

		SortAliases = new List<string> { VALUE_ALIAS };

		DefaultComponentId = new Guid(TucIntegerDisplayColumnComponent.ID);
	}
}

