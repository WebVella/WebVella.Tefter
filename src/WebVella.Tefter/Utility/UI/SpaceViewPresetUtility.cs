namespace WebVella.Tefter.Utility;
public static class SpaceViewPresetUtility
{
	public static List<TfSpaceViewPreset> FillSpaceViewPresetParents(this List<TfSpaceViewPreset>? presets)
	{
		if (presets is null) return new List<TfSpaceViewPreset>();
		
		var parents = new List<TfSpaceViewPreset>();

		foreach (var item in presets)
		{
			_fillParents(parents, item);
		}
		return parents;
	}	
	private static void _fillParents(List<TfSpaceViewPreset> parents, TfSpaceViewPreset current)
	{
		if (current.IsGroup) parents.Add(current);
		foreach (var item in current.Presets) _fillParents(parents, item);
	}		
}
