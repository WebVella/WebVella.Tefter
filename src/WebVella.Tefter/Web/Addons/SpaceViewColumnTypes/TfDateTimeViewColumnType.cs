namespace WebVella.Tefter.Web.Addons;

public class TfDateTimeViewColumnType : ITfSpaceViewColumnTypeAddon
{
	public const string ID = "d41752c3-e356-4c51-83ed-7e1a4e5e5183";
	public const string NAME = "DateTime";
	public const string DESCRIPTION = "displays date and time";
	public const string FLUENT_ICON_NAME = "CalendarMonth";
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

	public TfDateTimeViewColumnType()
	{
		DataMapping = new List<TfSpaceViewColumnAddonDataMapping>
		{
			new TfSpaceViewColumnAddonDataMapping
				{
					Alias = VALUE_ALIAS,
					Description = "this column is compatible with the all Date and DateTime database column types, but its intented use is with DateTime",
					SupportedDatabaseColumnTypes = new List<TfDatabaseColumnType> {
						TfDatabaseColumnType.DateOnly,
						TfDatabaseColumnType.DateTime
					}
				}
		};

		FilterAliases = new List<string>() { VALUE_ALIAS };

		SortAliases = new List<string> { VALUE_ALIAS };

		DefaultComponentId = new Guid(TfDateTimeDisplayColumnComponent.ID);

	}
}

