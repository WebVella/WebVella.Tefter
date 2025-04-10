﻿namespace WebVella.Tefter.Utility;
public static class ModelHelpers
{
	internal static ITfSpaceViewColumnTypeAddon GetColumnTypeForDbType(TfDatabaseColumnType dbType, ReadOnlyCollection<ITfSpaceViewColumnTypeAddon> availableTypes)
	{
		ITfSpaceViewColumnTypeAddon selectedType = null;
		switch (dbType)
		{
			case TfDatabaseColumnType.ShortInteger:
				selectedType = availableTypes.FirstOrDefault(x => x.Id == new Guid(TfConstants.TF_GENERIC_SHORT_INTEGER_COLUMN_TYPE_ID));
				break;
			case TfDatabaseColumnType.AutoIncrement:
			case TfDatabaseColumnType.Integer:
				selectedType = availableTypes.FirstOrDefault(x => x.Id == new Guid(TfConstants.TF_GENERIC_INTEGER_COLUMN_TYPE_ID));
				break;
			case TfDatabaseColumnType.LongInteger:
				selectedType = availableTypes.FirstOrDefault(x => x.Id == new Guid(TfConstants.TF_GENERIC_LONG_INTEGER_COLUMN_TYPE_ID));
				break;
			case TfDatabaseColumnType.Number:
				selectedType = availableTypes.FirstOrDefault(x => x.Id == new Guid(TfConstants.TF_GENERIC_NUMBER_COLUMN_TYPE_ID));
				break;
			case TfDatabaseColumnType.ShortText:
			case TfDatabaseColumnType.Text:
				selectedType = availableTypes.FirstOrDefault(x => x.Id == new Guid(TfConstants.TF_GENERIC_TEXT_COLUMN_TYPE_ID));
				break;
			case TfDatabaseColumnType.Boolean:
				selectedType = availableTypes.FirstOrDefault(x => x.Id == new Guid(TfConstants.TF_GENERIC_BOOLEAN_COLUMN_TYPE_ID));
				break;
			case TfDatabaseColumnType.Guid:
				selectedType = availableTypes.FirstOrDefault(x => x.Id == new Guid(TfConstants.TF_GENERIC_GUID_COLUMN_TYPE_ID));
				break;
			case TfDatabaseColumnType.Date:
				selectedType = availableTypes.FirstOrDefault(x => x.Id == new Guid(TfConstants.TF_GENERIC_DATEONLY_COLUMN_TYPE_ID));
				break;
			case TfDatabaseColumnType.DateTime:
				selectedType = availableTypes.FirstOrDefault(x => x.Id == new Guid(TfConstants.TF_GENERIC_DATETIME_COLUMN_TYPE_ID));
				break;
			default:
				selectedType = availableTypes.FirstOrDefault(x => x.Id == new Guid(TfConstants.TF_GENERIC_TEXT_COLUMN_TYPE_ID));
				break;
		}

		if (selectedType is null)
			selectedType = availableTypes.FirstOrDefault(x => x.Id == new Guid(TfConstants.TF_GENERIC_TEXT_COLUMN_TYPE_ID));

		return selectedType;
	}

	internal static TucSpaceViewPreset GetPresetById(this List<TucSpaceViewPreset> presets, Guid presetId)
	{
		foreach (var item in presets)
		{
			if (item.Id == presetId) return item;
			var result = item.Nodes.GetPresetById(presetId);
			if (result != null) return result;
		}
		return null;
	}

	internal static void FillPresetPathById(List<TucSpaceViewPreset> presets, Guid? presetId, List<TucSpaceViewPreset> result)
	{
		if (presetId is null) return;

		var preset = presets.GetPresetById(presetId.Value);
		if (preset is not null)
		{
			result.Add(preset);
			FillPresetPathById(presets, preset.ParentId, result);
		}
		return;
	}

	internal static TucSpaceNode GetSpaceNodeById(this List<TucSpaceNode> presets, Guid nodeId)
	{
		foreach (var item in presets)
		{
			if (item.Id == nodeId) return item;
			var result = item.ChildNodes.GetSpaceNodeById(nodeId);
			if (result != null) return result;
		}
		return null;
	}

}
