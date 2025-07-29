namespace WebVella.Tefter.Utility;
public static class ModelHelpers
{
	internal static ITfSpaceViewColumnTypeAddon? GetColumnTypeForDbType(TfDatabaseColumnType dbType, ReadOnlyCollection<ITfSpaceViewColumnTypeAddon> availableTypes)
	{
		ITfSpaceViewColumnTypeAddon? selectedType = null;
		switch (dbType)
		{
			case TfDatabaseColumnType.ShortInteger:
				selectedType = availableTypes.FirstOrDefault(x => x.AddonId == new Guid(TfShortIntegerViewColumnType.ID));
				break;
			case TfDatabaseColumnType.AutoIncrement:
			case TfDatabaseColumnType.Integer:
				selectedType = availableTypes.FirstOrDefault(x => x.AddonId == new Guid(TfIntegerViewColumnType.ID));
				break;
			case TfDatabaseColumnType.LongInteger:
				selectedType = availableTypes.FirstOrDefault(x => x.AddonId == new Guid(TfLongIntegerViewColumnType.ID));
				break;
			case TfDatabaseColumnType.Number:
				selectedType = availableTypes.FirstOrDefault(x => x.AddonId == new Guid(TfNumberViewColumnType.ID));
				break;
			case TfDatabaseColumnType.ShortText:
			case TfDatabaseColumnType.Text:
				selectedType = availableTypes.FirstOrDefault(x => x.AddonId == new Guid(TfTextViewColumnType.ID));
				break;
			case TfDatabaseColumnType.Boolean:
				selectedType = availableTypes.FirstOrDefault(x => x.AddonId == new Guid(TfBooleanViewColumnType.ID));
				break;
			case TfDatabaseColumnType.Guid:
				selectedType = availableTypes.FirstOrDefault(x => x.AddonId == new Guid(TfGuidViewColumnType.ID));
				break;
			case TfDatabaseColumnType.DateOnly:
				selectedType = availableTypes.FirstOrDefault(x => x.AddonId == new Guid(TfDateOnlyViewColumnType.ID));
				break;
			case TfDatabaseColumnType.DateTime:
				selectedType = availableTypes.FirstOrDefault(x => x.AddonId == new Guid(TfDateTimeViewColumnType.ID));
				break;
			default:
				selectedType = availableTypes.FirstOrDefault(x => x.AddonId == new Guid(TfTextViewColumnType.ID));
				break;
		}

		if (selectedType is null)
			selectedType = availableTypes.FirstOrDefault(x => x.AddonId == new Guid(TfTextViewColumnType.ID));

		return selectedType;
	}

	internal static TfSpaceViewPreset GetPresetById(this List<TfSpaceViewPreset> presets, Guid presetId)
	{
		foreach (var item in presets)
		{
			if (item.Id == presetId) return item;
			var result = item.Pages.GetPresetById(presetId);
			if (result != null) return result;
		}
		return null;
	}

	internal static void FillPresetPathById(List<TfSpaceViewPreset> presets, Guid? presetId, List<TfSpaceViewPreset> result)
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

	internal static TfSpacePage GetSpaceNodeById(this List<TfSpacePage> presets, Guid nodeId)
	{
		foreach (var item in presets)
		{
			if (item.Id == nodeId) return item;
			var result = item.ChildPages.GetSpaceNodeById(nodeId);
			if (result != null) return result;
		}
		return null;
	}

}
