namespace WebVella.Tefter.Web.ViewColumns;

public class TfNumberViewColumnType : ITfSpaceViewColumnType
{
	const string TF_COLUMN_NUMBER_ID = Constants.TF_GENERIC_NUMBER_COLUMN_TYPE_ID;
	const string TF_COLUMN_NUMBER_NAME = "Number";
	const string TF_COLUMN_NUMBER_DESCRIPTION = "displays decimal numbers";
	const string TF_COLUMN_NUMBER_ICON = "NumberSymbol";
	const string ALIAS = "Value";

	public Guid Id { get; init; }
	public string Name { get; init; }
	public string Description { get; init; }
	public string FluentIconName { get; init; }
	public List<TfSpaceViewColumnDataMapping> DataMapping { get; init; }
	public Type DefaultComponentType { get; init; }
	public List<Type> SupportedComponentTypes { get; init; }
	public List<string> FilterAliases { get; init; }
	public List<string> SortAliases { get; init; }
	public List<Guid> SupportedAddonTypes { get; init; } = new();

	public TfNumberViewColumnType()
	{

		Id = new Guid(TF_COLUMN_NUMBER_ID);

		Name = TF_COLUMN_NUMBER_NAME;

		Description = TF_COLUMN_NUMBER_DESCRIPTION;

		FluentIconName = TF_COLUMN_NUMBER_ICON;

		DataMapping = new List<TfSpaceViewColumnDataMapping>
		{
			new TfSpaceViewColumnDataMapping
				{
					Alias = ALIAS,
					Description = "this column is compatible with all number and integer database column types",
					SupportedDatabaseColumnTypes = new List<TfDatabaseColumnType> {
						TfDatabaseColumnType.ShortInteger,
						TfDatabaseColumnType.Integer,
						TfDatabaseColumnType.LongInteger,
						TfDatabaseColumnType.Number }
			}
		};

		FilterAliases = new List<string>() { ALIAS };

		SortAliases = new List<string> { ALIAS };

		DefaultComponentType = typeof(TfNumberDisplayColumnComponent);

		SupportedComponentTypes = new List<Type> {
			typeof(TfNumberDisplayColumnComponent),
			typeof(TfNumberEditColumnComponent),
			typeof(TfTextDisplayColumnComponent)
			};
	}
}

