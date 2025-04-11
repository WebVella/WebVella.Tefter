namespace WebVella.Tefter.Web.Addons;

public class TfIntegerViewColumnType : ITfSpaceViewColumnTypeAddon
{
	const string TF_COLUMN_NUMBER_ID = TfConstants.TF_GENERIC_INTEGER_COLUMN_TYPE_ID;
	const string TF_COLUMN_NUMBER_NAME = "Integer";
	const string TF_COLUMN_NUMBER_DESCRIPTION = "displays integer numbers.";
	const string TF_COLUMN_NUMBER_ICON = "NumberSymbol";
	const string ALIAS = "Value";

	public Guid Id { get; init; }
	public string Name { get; init; }
	public string Description { get; init; }
	public string FluentIconName { get; init; }
	public List<TfSpaceViewColumnAddonDataMapping> DataMapping { get; init; }
	public List<string> FilterAliases { get; init; }
	public List<string> SortAliases { get; init; }
	public Guid? DefaultComponentId { get; init; }
	public List<Guid> SupportedComponents { get; set; } = new();

	public TfIntegerViewColumnType()
	{

		Id = new Guid(TF_COLUMN_NUMBER_ID);

		Name = TF_COLUMN_NUMBER_NAME;

		Description = TF_COLUMN_NUMBER_DESCRIPTION;

		FluentIconName = TF_COLUMN_NUMBER_ICON;

		DataMapping = new List<TfSpaceViewColumnAddonDataMapping>
		{
			new TfSpaceViewColumnAddonDataMapping
				{
					Alias = ALIAS,
					Description = "this column is compatible with integer and short integer database column types",
					SupportedDatabaseColumnTypes = new List<TfDatabaseColumnType> {
						TfDatabaseColumnType.ShortInteger,
						TfDatabaseColumnType.Integer
						}
			}
		};

		FilterAliases = new List<string>() { ALIAS };

		SortAliases = new List<string> { ALIAS };

		DefaultComponentId = new Guid(TfConstants.TF_COLUMN_COMPONENT_DISPLAY_INTEGER_ID);
	}
}

