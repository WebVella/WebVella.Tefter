namespace WebVella.Tefter;

internal partial interface ITfScreenRegionComponentManager
{
	public List<TfScreenRegionComponentMeta> GetComponentMeta(ScreenRegion? region);
}

internal partial class TfScreenRegionComponentManager : ITfScreenRegionComponentManager
{
	public new List<TfScreenRegionComponentMeta> GetComponentMeta(ScreenRegion? region)
	{
		if (region == null)
			return _meta.ToList();

		return _meta.Where(x => x.ScreenRegion == region).ToList();
	}
}
