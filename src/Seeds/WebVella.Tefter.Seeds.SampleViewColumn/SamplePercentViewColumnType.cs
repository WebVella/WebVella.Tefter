namespace WebVella.Tefter.Seeds.SampleViewColumn;

public class SamplePercentViewColumnType : ITfSpaceViewColumnType
{
	const string SAMPLE_PERCENT_COLUMN__ID = "cbe7a847-72a5-4af5-af7f-98771e043f14";
	const string SAMPLE_PERCENT_COLUMN_NAME = "Percent";
	const string SAMPLE_PERCENT_COLUMN_DESCRIPTION = "displays percent based of value between 0 and 1";
	const string SAMPLE_PERCENT_COLUMN_ICON = "PercentSymbol";
	const string ALIAS = "Value";

	public Guid Id { get; init; }
	public string Name { get; init; }
	public string Description { get; init; }
	public string FluentIconName { get; init; }
	public List<TfSpaceViewColumnDataMapping> DataMapping { get; init; }
	public Type DefaultComponentType { get; init; }
	/// <summary>
	/// This property will be inited on application start
	/// </summary>
	public List<Type> SupportedComponentTypes { get; set; } = new();
	public List<string> FilterAliases { get; init; }
	public List<string> SortAliases { get; init; }
	public List<Guid> SupportedAddonTypes { get; init; } = new();

	public SamplePercentViewColumnType()
	{

		Id = new Guid(SAMPLE_PERCENT_COLUMN__ID);

		Name = SAMPLE_PERCENT_COLUMN_NAME;

		Description = SAMPLE_PERCENT_COLUMN_DESCRIPTION;

		FluentIconName = SAMPLE_PERCENT_COLUMN_ICON;

		DataMapping = new List<TfSpaceViewColumnDataMapping>
		{
			new TfSpaceViewColumnDataMapping
				{
					Alias = ALIAS,
					Description = "this column is compatible with all number based column types",
					SupportedDatabaseColumnTypes = new List<TfDatabaseColumnType> {
						TfDatabaseColumnType.Number,
						TfDatabaseColumnType.ShortInteger,
						TfDatabaseColumnType.Integer,
						TfDatabaseColumnType.LongInteger
					}
			}
		};

		FilterAliases = new List<string>() { ALIAS };

		SortAliases = new List<string> { ALIAS };

		DefaultComponentType = typeof(SamplePercentDisplayViewColumnComponent);
	}
}

