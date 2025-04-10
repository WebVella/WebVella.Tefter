﻿namespace WebVella.Tefter.Web.Addons;

public class TfGuidViewColumnType : ITfSpaceViewColumnTypeAddon
{
	const string TF_COLUMN_GUID_ID = TfConstants.TF_GENERIC_GUID_COLUMN_TYPE_ID;
	const string TF_COLUMN_GUID_NAME = "Unique identifier (GUID)";
	const string TF_COLUMN_GUID_DESCRIPTION = "displays GUID value";
	const string TF_COLUMN_GUID_ICON = "Key";
	const string ALIAS = "Value";

	public Guid Id { get; init; }
	public string Name { get; init; }
	public string Description { get; init; }
	public string FluentIconName { get; init; }
	public List<TfSpaceViewColumnAddonDataMapping> DataMapping { get; init; }
	public Type CustomOptionsComponentType { get; init; }
	public List<string> FilterAliases { get; init; }
	public List<string> SortAliases { get; init; }
	public Guid? DefaultComponentId { get; init; }
	public List<Guid> SupportedComponents { get; set; } = new();

	public TfGuidViewColumnType()
	{
		Id = new Guid(TF_COLUMN_GUID_ID);
		Name = TF_COLUMN_GUID_NAME;
		Description = TF_COLUMN_GUID_DESCRIPTION;
		FluentIconName = TF_COLUMN_GUID_ICON;
		DataMapping = new List<TfSpaceViewColumnAddonDataMapping>
		{
			new TfSpaceViewColumnAddonDataMapping
				{
					Alias = ALIAS,
					Description = "this column is compatible with the Guid database column",
					SupportedDatabaseColumnTypes = new List<TfDatabaseColumnType> { TfDatabaseColumnType.Guid }
				}
		};
		FilterAliases = new List<string>() { ALIAS };
		SortAliases = new List<string> { ALIAS };
		DefaultComponentId = new Guid(TfConstants.TF_COLUMN_COMPONENT_DISPLAY_GUID_ID);
	}
}

