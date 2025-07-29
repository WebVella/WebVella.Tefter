namespace WebVella.Tefter.UI.Addons;

public class TfDateOnlyViewColumnType : ITfSpaceViewColumnTypeAddon
{
	public const string ID = "59037088-e8b7-4ec6-858c-016f4eacf58a";
	public const string NAME = "Date";
	public const string DESCRIPTION = "displays a date";
	public const string FLUENT_ICON_NAME = "CalendarMonth";
	public const string VALUE_ALIAS = "Value";

	public Guid AddonId { get; init; } = new Guid(ID);
	public string AddonName { get; init; } = NAME;
	public string AddonDescription { get; init; } = DESCRIPTION;
	public string AddonFluentIconName { get; init; } = FLUENT_ICON_NAME;
	public List<TfSpaceViewColumnAddonDataMapping> DataMapping { get; init; }
	public List<string> FilterAliases { get; init; }
	public List<string> SortAliases { get; init; }
	public Guid? DefaultComponentId { get; init; }
	public List<Guid> SupportedComponents { get; set; } = new();

	public TfDateOnlyViewColumnType()
	{
		DataMapping = new List<TfSpaceViewColumnAddonDataMapping>
		{
			new TfSpaceViewColumnAddonDataMapping
				{
					Alias = VALUE_ALIAS,
					Description = "this column is compatible with the all Date and DateTime database column types, but its intented use is with Date",
					SupportedDatabaseColumnTypes = new List<TfDatabaseColumnType> {
						TfDatabaseColumnType.DateOnly,
						TfDatabaseColumnType.DateTime
					}
				}
		};

		FilterAliases = new List<string>() { VALUE_ALIAS };

		SortAliases = new List<string> { VALUE_ALIAS };
		DefaultComponentId = new Guid(TucDateOnlyDisplayColumnComponent.ID);
	}
}

