namespace WebVella.Tefter.Core;

public class TfAutoIncrementViewColumnType : ITfSpaceViewColumnType
{
	const string TF_COLUMN_AUTO_INCREMENT_ID = "4f5a38e2-4f8b-4d6f-a7bb-3e4d07a6f25c";
	const string TF_COLUMN_AUTO_INCREMENT_NAME = "Auto increment number";
	const string TF_COLUMN_AUTO_INCREMENT_DESCRIPTION = "If you need an automatically incremented number with each new record, this is the column you need.";
	const string TF_COLUMN_AUTO_INCREMENT_ICON = "WebVella.Tefter.Core.Assets.AutoIncrementIcon.png";

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


	public TfAutoIncrementViewColumnType()
	{
		const string ALIAS = "Value";

		Id = new Guid(TF_COLUMN_AUTO_INCREMENT_ID);

		Name = TF_COLUMN_AUTO_INCREMENT_NAME;

		Description = TF_COLUMN_AUTO_INCREMENT_DESCRIPTION;

		Icon = TF_COLUMN_AUTO_INCREMENT_ICON;

		DataMapping = new List<TfSpaceViewColumnDataMapping>
		{
			new TfSpaceViewColumnDataMapping
				{
					Alias = ALIAS,
					Description = "This column works with Auto Increment database column.",
					SupportedDatabaseColumnTypes = new List<DatabaseColumnType> { DatabaseColumnType.AutoIncrement }
				}
		};

		FilterAliases = new List<string>() { ALIAS };

		SortAliases = new List<string> { ALIAS };

		DefaultComponentType = typeof(TfGeneralViewColumnComponent);

		SupportedComponentTypes = new List<Type> { typeof(TfGeneralViewColumnComponent) };
	}
}

