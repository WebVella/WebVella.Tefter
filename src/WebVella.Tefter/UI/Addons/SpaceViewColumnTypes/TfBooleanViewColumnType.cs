namespace WebVella.Tefter.UI.Addons;

public class TfBooleanViewColumnType : ITfSpaceViewColumnTypeAddon
{
	public const string ID = "c28e933b-6800-4819-b22f-e091e3e3c961";
	public const string NAME = "Boolean";
	public const string DESCRIPTION = "displays a boolean";
	public const string FLUENT_ICON_NAME = "CircleMultipleSubtractCheckmark";
	public const string VALUE_ALIAS = "Value";

	public Guid AddonId { get; init; } = new Guid(ID);
	public string AddonName { get; init; } = NAME;
	public string AddonDescription { get; init; } = DESCRIPTION;
	public string AddonFluentIconName { get; init; } = FLUENT_ICON_NAME;
	public List<TfSpaceViewColumnAddonDataMapping> DataMapping { get; init; }
	public List<string> FilterAliases { get; init; }
	public List<string> SortAliases { get; init; }
	public Guid? DefaultDisplayComponentId { get; init; }
	public List<Guid> SupportedComponents { get; set; } = new();
	public Guid? DefaultEditComponentId { get; init; }

	public TfBooleanViewColumnType()
	{
		DataMapping = new List<TfSpaceViewColumnAddonDataMapping>
		{
			new TfSpaceViewColumnAddonDataMapping
				{
					Alias = VALUE_ALIAS,
					Description = "this column is compatible with the Boolean database column type",
					SupportedDatabaseColumnTypes = new List<TfDatabaseColumnType> { TfDatabaseColumnType.Boolean }
				}
		};

		FilterAliases = new List<string>() { VALUE_ALIAS };

		SortAliases = new List<string> { VALUE_ALIAS };

		DefaultDisplayComponentId = new Guid(TucBooleanDisplayColumnComponent.ID);
		DefaultEditComponentId = new Guid(TucBooleanEditColumnComponent.ID);
	}
}

