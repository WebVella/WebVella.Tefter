﻿namespace WebVella.Tefter.Utility;
public static class ModelHelpers
{
	internal static ITfSpaceViewColumnType GetColumnTypeForDbType(DatabaseColumnType dbType, ReadOnlyCollection<ITfSpaceViewColumnType> availableTypes)
	{
		ITfSpaceViewColumnType selectedType = null;
		switch (dbType)
		{
			case DatabaseColumnType.ShortInteger:
				selectedType = availableTypes.FirstOrDefault(x => x.Id == new Guid(Constants.TF_GENERIC_SHORT_INTEGER_COLUMN_TYPE_ID));
				break;
			case DatabaseColumnType.AutoIncrement:
			case DatabaseColumnType.Integer:
				selectedType = availableTypes.FirstOrDefault(x => x.Id == new Guid(Constants.TF_GENERIC_INTEGER_COLUMN_TYPE_ID));
				break;
			case DatabaseColumnType.LongInteger:
				selectedType = availableTypes.FirstOrDefault(x => x.Id == new Guid(Constants.TF_GENERIC_LONG_INTEGER_COLUMN_TYPE_ID));
				break;
			case DatabaseColumnType.Number:
				selectedType = availableTypes.FirstOrDefault(x => x.Id == new Guid(Constants.TF_GENERIC_NUMBER_COLUMN_TYPE_ID));
				break;
			case DatabaseColumnType.ShortText:
			case DatabaseColumnType.Text:
				selectedType = availableTypes.FirstOrDefault(x => x.Id == new Guid(Constants.TF_GENERIC_TEXT_COLUMN_TYPE_ID));
				break;
			case DatabaseColumnType.Boolean:
				selectedType = availableTypes.FirstOrDefault(x => x.Id == new Guid(Constants.TF_GENERIC_BOOLEAN_COLUMN_TYPE_ID));
				break;
			case DatabaseColumnType.Guid:
				selectedType = availableTypes.FirstOrDefault(x => x.Id == new Guid(Constants.TF_GENERIC_GUID_COLUMN_TYPE_ID));
				break;
			case DatabaseColumnType.Date:
				selectedType = availableTypes.FirstOrDefault(x => x.Id == new Guid(Constants.TF_GENERIC_DATEONLY_COLUMN_TYPE_ID));
				break;
			case DatabaseColumnType.DateTime:
				selectedType = availableTypes.FirstOrDefault(x => x.Id == new Guid(Constants.TF_GENERIC_DATETIME_COLUMN_TYPE_ID));
				break;
			default:
				selectedType = availableTypes.FirstOrDefault(x => x.Id == new Guid(Constants.TF_GENERIC_TEXT_COLUMN_TYPE_ID));
				break;
		}

		if (selectedType is null)
			selectedType = availableTypes.FirstOrDefault(x => x.Id == new Guid(Constants.TF_GENERIC_TEXT_COLUMN_TYPE_ID));

		return selectedType;
	}


}
