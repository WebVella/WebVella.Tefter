namespace WebVella.Tefter.Web.ViewColumns;

public class TfNumberViewColumnType : ITfSpaceViewColumnType
{
	const string TF_COLUMN_NUMBER_ID = "5d246be4-d202-434c-961e-204e44ee0450";
	const string TF_COLUMN_NUMBER_NAME = "Number";
	const string TF_COLUMN_NUMBER_DESCRIPTION = "Holds all kind of numbers.";
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

	public TfNumberViewColumnType()
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
					Description = "This column works with Number database column.",
					SupportedDatabaseColumnTypes = new List<DatabaseColumnType> {
						DatabaseColumnType.ShortInteger,
						DatabaseColumnType.Integer,
						DatabaseColumnType.LongInteger,
						DatabaseColumnType.Number }
			}
		};

		FilterAliases = new List<string>() { ALIAS };

		SortAliases = new List<string> { ALIAS };

		DefaultComponentType = typeof(TfNumberViewColumn);

		SupportedComponentTypes = new List<Type> { typeof(TfNumberViewColumn), typeof(TfTextViewColumn) };
	}
}

