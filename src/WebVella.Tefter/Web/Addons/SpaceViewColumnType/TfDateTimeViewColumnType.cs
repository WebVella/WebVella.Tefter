﻿namespace WebVella.Tefter.Web.Addons;

public class TfDateTimeViewColumnType : ITfSpaceViewColumnTypeAddon
{
	const string TF_COLUMN_DATETIME_ID = TfConstants.TF_GENERIC_DATETIME_COLUMN_TYPE_ID;
	const string TF_COLUMN_DATETIME_NAME = "DateTime";
	const string TF_COLUMN_DATETIME_DESCRIPTION = "displays date and time";
	const string TF_COLUMN_DATETIME_ICON = "CalendarMonth";
	const string ALIAS = "Value";

	public Guid Id { get; init; }
	public string Name { get; init; }
	public string Description { get; init; }
	public string FluentIconName { get; init; }
	public List<TfSpaceViewColumnAddonDataMapping> DataMapping { get; init; }

	public List<string> FilterAliases { get; init; }
	public List<string> SortAliases { get; init; }
	public Guid? DefaultComponentId { get; init; }
	public List<Guid> SupportedComponents { get; set; } = new();

	public TfDateTimeViewColumnType()
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
					Description = "this column is compatible with the all Date and DateTime database column types, but its intented use is with DateTime",
					SupportedDatabaseColumnTypes = new List<TfDatabaseColumnType> {
						TfDatabaseColumnType.Date,
						TfDatabaseColumnType.DateTime
					}
				}
		};

		FilterAliases = new List<string>() { ALIAS };

		SortAliases = new List<string> { ALIAS };

		DefaultComponentId = new Guid(TfConstants.TF_COLUMN_COMPONENT_DISPLAY_DATETIME_ID);

	}
}

