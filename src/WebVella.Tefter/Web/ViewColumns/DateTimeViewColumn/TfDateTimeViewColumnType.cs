namespace WebVella.Tefter.Web.ViewColumns;

public class TfDateTimeViewColumnType : ITfSpaceViewColumnType
{
	const string TF_COLUMN_DATETIME_ID = "d41752c3-e356-4c51-83ed-7e1a4e5e5183";
	const string TF_COLUMN_DATETIME_NAME = "DateTime";
	const string TF_COLUMN_DATETIME_DESCRIPTION = "A date\\datetime column";
	const string TF_COLUMN_DATETIME_ICON = "CalendarMonth";
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

	public TfDateTimeViewColumnType()
	{
		Id = new Guid(TF_COLUMN_DATETIME_ID);

		Name = TF_COLUMN_DATETIME_NAME;

		Description = TF_COLUMN_DATETIME_DESCRIPTION;

		Icon = TF_COLUMN_DATETIME_ICON;

		DataMapping = new List<TfSpaceViewColumnDataMapping>
		{
			new TfSpaceViewColumnDataMapping
				{
					Alias = ALIAS,
					Description = "This column works with Date or DateTime database column.",
					SupportedDatabaseColumnTypes = new List<DatabaseColumnType> {
						DatabaseColumnType.Date,
						DatabaseColumnType.DateTime 
					}
				}
		};

		FilterAliases = new List<string>() { ALIAS };

		SortAliases = new List<string> { ALIAS };

		DefaultComponentType = typeof(TfDateTimeViewColumn);

		SupportedComponentTypes = new List<Type> { typeof(TfDateTimeViewColumn), typeof(TfTextViewColumn) };
	}
}

