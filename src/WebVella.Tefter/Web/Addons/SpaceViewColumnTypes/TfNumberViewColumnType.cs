namespace WebVella.Tefter.Web.Addons;

public class TfNumberViewColumnType : ITfSpaceViewColumnTypeAddon
{
	public const string ID = "5d246be4-d202-434c-961e-204e44ee0450";
	public const string NAME = "Number";
	public const string DESCRIPTION = "displays decimal numbers";
	public const string FLUENT_ICON_NAME = "NumberSymbol";
	public const string VALUE_ALIAS = "Value";

	public Guid Id { get; init; } = new Guid(ID);
	public string Name { get; init; } = NAME;
	public string Description { get; init; } = DESCRIPTION;
	public string FluentIconName { get; init; } = FLUENT_ICON_NAME;
	public List<TfSpaceViewColumnAddonDataMapping> DataMapping { get; init; }
	public List<string> FilterAliases { get; init; }
	public List<string> SortAliases { get; init; }
	public Guid? DefaultComponentId { get; init; }
	public List<Guid> SupportedComponents { get; set; } = new();

	public TfNumberViewColumnType()
	{
		DataMapping = new List<TfSpaceViewColumnAddonDataMapping>
		{
			new TfSpaceViewColumnAddonDataMapping
				{
					Alias = VALUE_ALIAS,
					Description = "this column is compatible with all number and integer database column types",
					SupportedDatabaseColumnTypes = new List<TfDatabaseColumnType> {
						TfDatabaseColumnType.ShortInteger,
						TfDatabaseColumnType.Integer,
						TfDatabaseColumnType.LongInteger,
						TfDatabaseColumnType.Number }
			}
		};

		FilterAliases = new List<string>() { VALUE_ALIAS };

		SortAliases = new List<string> { VALUE_ALIAS };

		DefaultComponentId = new Guid(TfNumberDisplayColumnComponent.ID);
	}
}

