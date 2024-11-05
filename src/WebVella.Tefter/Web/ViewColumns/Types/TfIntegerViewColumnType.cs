namespace WebVella.Tefter.Web.ViewColumns;

public class TfIntegerViewColumnType : ITfSpaceViewColumnType
{
	const string TF_COLUMN_NUMBER_ID = Constants.TF_GENERIC_INTEGER_COLUMN_TYPE_ID;
	const string TF_COLUMN_NUMBER_NAME = "Integer";
	const string TF_COLUMN_NUMBER_DESCRIPTION = "displays integer numbers.";
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

	public TfIntegerViewColumnType()
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
					Description = "this column is compatible with integer and short integer database column types",
					SupportedDatabaseColumnTypes = new List<TfDatabaseColumnType> {
						TfDatabaseColumnType.ShortInteger,
						TfDatabaseColumnType.Integer
						}
			}
		};

		FilterAliases = new List<string>() { ALIAS };

		SortAliases = new List<string> { ALIAS };

		DefaultComponentType = typeof(TfIntegerDisplayColumnComponent);

		SupportedComponentTypes = new List<Type> {
			typeof(TfIntegerDisplayColumnComponent),
			typeof(TfIntegerEditColumnComponent),
			typeof(TfTextDisplayColumnComponent)
			};
	}
}

