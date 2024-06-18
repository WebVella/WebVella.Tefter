namespace WebVella.Tefter.Core;

[TfColumn(
	Id:Constants.TF_COLUMN_AUTO_INCREMENT_ID,
	Name: Constants.TF_COLUMN_AUTO_INCREMENT_NAME,
	Description:Constants.TF_COLUMN_AUTO_INCREMENT_DESCRIPTION)]
[TfColumnIcon(Constants.TF_COLUMN_AUTO_INCREMENT_ICON)]
public class TfAutoIncrementColumn : ITfColumn
{
	public TfColumnData Data { get; private set; }

	public TfColumnFilter Filter { get; private set; }

	public TfColumnSort Sort { get; private set; }

	public TfAutoIncrementColumn()
	{
		const string ALIAS_VALUE = "Value";

		Data = new TfColumnData
		{
			DefaultComponentType = typeof(TfGeneralDisplayComponent),
			
			SupportedComponentTypes = new List<Type> { typeof(TfGeneralDisplayComponent) },

			DatabaseRequirements = new List<TfColumnColumnDatabaseRequirement>
			{
				new TfColumnColumnDatabaseRequirement
				{
					Name = ALIAS_VALUE,
					Description = "This column works with Auto Increment database column.",
					SupportedDatabaseColumnTypes = new List<DatabaseColumnType> { DatabaseColumnType.AutoIncrement }
				}
			},
		};
	
		Filter = new TfColumnFilter
		{
			DefaultComponentType = typeof(TfGeneralFilterComponent),
			
			SupportedComponentTypes = new List<Type> { typeof(TfGeneralFilterComponent) },
			
			DatabaseRequirementNames = new List<string> { ALIAS_VALUE }
		};
		
		Sort = new TfColumnSort
		{
			DefaultComponentType = typeof(TfGeneralSortComponent),
		
			SupportedComponentTypes = new List<Type> { typeof(TfGeneralSortComponent) },
			
			DatabaseRequirementNames = new List<string> { ALIAS_VALUE }
		};
	}
}

