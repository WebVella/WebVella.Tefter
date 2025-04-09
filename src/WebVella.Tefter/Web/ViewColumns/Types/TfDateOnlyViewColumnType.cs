namespace WebVella.Tefter.Web.ViewColumns;

public class TfDateOnlyViewColumnType : ITfSpaceViewColumnAddon
{
	const string TF_COLUMN_DATETIME_ID = Constants.TF_GENERIC_DATEONLY_COLUMN_TYPE_ID;
	const string TF_COLUMN_DATETIME_NAME = "Date";
	const string TF_COLUMN_DATETIME_DESCRIPTION = "displays a date";
	const string TF_COLUMN_DATETIME_ICON = "CalendarMonth";
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

	public TfDateOnlyViewColumnType()
	{
		Id = new Guid(TF_COLUMN_DATETIME_ID);

		Name = TF_COLUMN_DATETIME_NAME;

		Description = TF_COLUMN_DATETIME_DESCRIPTION;

		FluentIconName = TF_COLUMN_DATETIME_ICON;

		DataMapping = new List<TfSpaceViewColumnAddonDataMapping>
		{
			new TfSpaceViewColumnAddonDataMapping
				{
					Alias = ALIAS,
					Description = "this column is compatible with the all Date and DateTime database column types, but its intented use is with Date",
					SupportedDatabaseColumnTypes = new List<TfDatabaseColumnType> {
						TfDatabaseColumnType.Date,
						TfDatabaseColumnType.DateTime 
					}
				}
		};

		FilterAliases = new List<string>() { ALIAS };

		SortAliases = new List<string> { ALIAS };

		DefaultComponentType = typeof(TfDateOnlyDisplayColumnComponent);
	}
}

