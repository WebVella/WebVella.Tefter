namespace WebVella.Tefter.Web.ViewColumns;

public class TfLongIntegerViewColumnType : ITfSpaceViewColumnType
{
	const string TF_COLUMN_NUMBER_ID = Constants.TF_GENERIC_LONG_INTEGER_COLUMN_TYPE_ID;
	const string TF_COLUMN_NUMBER_NAME = "Long integer";
	const string TF_COLUMN_NUMBER_DESCRIPTION = "displays very big integer numbers.";
	const string TF_COLUMN_NUMBER_ICON = "NumberSymbol";
	const string ALIAS = "Value";

	public Guid Id { get; init; }
	public string Name { get; init; }
	public string Description { get; init; }
	public string Icon { get; init; }
	public List<TfSpaceViewColumnDataMapping> DataMapping { get; init; }
	public Type DefaultComponentType { get; init; }
	public List<Type> SupportedComponentTypes { get; init; }
	public List<string> FilterAliases { get; init; }
	public List<string> SortAliases { get; init; }
	public List<Guid> SupportedAddonTypes { get; init; } = new();

	public TfLongIntegerViewColumnType()
	{

		Id = new Guid(TF_COLUMN_NUMBER_ID);

		Name = TF_COLUMN_NUMBER_NAME;

		Description = TF_COLUMN_NUMBER_DESCRIPTION;

		Icon = TF_COLUMN_NUMBER_ICON;

		DataMapping = new List<TfSpaceViewColumnDataMapping>
		{
			new TfSpaceViewColumnDataMapping
				{
					Alias = ALIAS,
					Description = "this column is compatible with all integer database column types",
					SupportedDatabaseColumnTypes = new List<DatabaseColumnType> {
						DatabaseColumnType.ShortInteger,
						DatabaseColumnType.Integer,
						DatabaseColumnType.LongInteger }
			}
		};

		FilterAliases = new List<string>() { ALIAS };

		SortAliases = new List<string> { ALIAS };

		DefaultComponentType = typeof(TfLongIntegerDisplayColumnComponent);

		SupportedComponentTypes = new List<Type> {
			typeof(TfLongIntegerDisplayColumnComponent),
			typeof(TfLongIntegerEditColumnComponent),
			typeof(TfTextDisplayColumnComponent)
			};
	}
}

