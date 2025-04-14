﻿namespace WebVella.Tefter.Utility;
public static class ModelHelpers
{
	internal static ITfSpaceViewColumnTypeAddon GetColumnTypeForDbType(TfDatabaseColumnType dbType, ReadOnlyCollection<ITfSpaceViewColumnTypeAddon> availableTypes)
	{
		ITfSpaceViewColumnTypeAddon selectedType = null;
		switch (dbType)
		{
			case TfDatabaseColumnType.ShortInteger:
				selectedType = availableTypes.FirstOrDefault(x => x.Id == new Guid(TfShortIntegerViewColumnType.ID));
				break;
			case TfDatabaseColumnType.AutoIncrement:
			case TfDatabaseColumnType.Integer:
				selectedType = availableTypes.FirstOrDefault(x => x.Id == new Guid(TfIntegerViewColumnType.ID));
				break;
			case TfDatabaseColumnType.LongInteger:
				selectedType = availableTypes.FirstOrDefault(x => x.Id == new Guid(TfLongIntegerViewColumnType.ID));
				break;
			case TfDatabaseColumnType.Number:
				selectedType = availableTypes.FirstOrDefault(x => x.Id == new Guid(TfNumberViewColumnType.ID));
				break;
			case TfDatabaseColumnType.ShortText:
			case TfDatabaseColumnType.Text:
				selectedType = availableTypes.FirstOrDefault(x => x.Id == new Guid(TfTextViewColumnType.ID));
				break;
			case TfDatabaseColumnType.Boolean:
				selectedType = availableTypes.FirstOrDefault(x => x.Id == new Guid(TfBooleanViewColumnType.ID));
				break;
			case TfDatabaseColumnType.Guid:
				selectedType = availableTypes.FirstOrDefault(x => x.Id == new Guid(TfGuidViewColumnType.ID));
				break;
			case TfDatabaseColumnType.DateOnly:
				selectedType = availableTypes.FirstOrDefault(x => x.Id == new Guid(TfDateOnlyViewColumnType.ID));
				break;
			case TfDatabaseColumnType.DateTime:
				selectedType = availableTypes.FirstOrDefault(x => x.Id == new Guid(TfDateTimeViewColumnType.ID));
				break;
			default:
				selectedType = availableTypes.FirstOrDefault(x => x.Id == new Guid(TfTextViewColumnType.ID));
				break;
		}

		if (selectedType is null)
			selectedType = availableTypes.FirstOrDefault(x => x.Id == new Guid(TfTextViewColumnType.ID));

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
