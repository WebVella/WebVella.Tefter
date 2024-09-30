namespace WebVella.Tefter;

internal partial interface ITfScreenRegionComponentManager
{
	public List<TfScreenRegionComponentMeta> GetComponentMeta(
		ScreenRegion? region = null);
}

internal partial class TfScreenRegionComponentManager : ITfScreenRegionComponentManager
{
	public new List<TfScreenRegionComponentMeta> GetComponentMeta(
		ScreenRegion? region = null)
	{
		if (region == null)
		{
			return _meta
				.OrderBy(x => x.Position)
				.ToList();
		}

		return _meta
			.Where(x => x.ScreenRegion == region)
			.OrderBy(x => x.Position)
			.ToList();
	}
}
