namespace WebVella.Tefter;

internal partial interface ITfScreenRegionComponentManager
{
	public List<TfScreenRegionComponentMeta> GetComponentMeta(
		TfScreenRegion? region = null);
}

internal partial class TfScreenRegionComponentManager : ITfScreenRegionComponentManager
{
	public List<TfScreenRegionComponentMeta> GetComponentMeta(
		TfScreenRegion? region = null)
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
