namespace WebVella.Tefter.Web.ViewColumns;

public class TfShortIntegerViewColumnType : ITfSpaceViewColumnAddon
{
	const string TF_COLUMN_NUMBER_ID = Constants.TF_GENERIC_SHORT_INTEGER_COLUMN_TYPE_ID;
	const string TF_COLUMN_NUMBER_NAME = "Short Integer";
	const string TF_COLUMN_NUMBER_DESCRIPTION = "displays small integer numbers.";
	const string TF_COLUMN_NUMBER_ICON = "NumberSymbol";
	const string ALIAS = "Value";

	public Guid Id { get; init; }
	public string Name { get; init; }
	public string Description { get; init; }
	public string FluentIconName { get; init; }
	public List<TfSpaceViewColumnAddonDataMapping> DataMapping { get; init; }
	public Type DefaultComponentType { get; init; }
	/// <summary>
	/// This property will be inited on application start
	/// </summary>
	public List<Type> SupportedComponentTypes { get; set; } = new();
	public List<string> FilterAliases { get; init; }
	public List<string> SortAliases { get; init; }
	public List<Guid> SupportedAddonTypes { get; init; } = new();

	public TfShortIntegerViewColumnType()
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
					Description = "this column is compatible with short integer database column types",
					SupportedDatabaseColumnTypes = new List<TfDatabaseColumnType> {
						TfDatabaseColumnType.ShortInteger
						}
			}
		};

		FilterAliases = new List<string>() { ALIAS };

		SortAliases = new List<string> { ALIAS };

		DefaultComponentType = typeof(TfShortIntegerDisplayColumnComponent);
	}
}

