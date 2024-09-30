namespace WebVella.Tefter.Api;

internal class ITfScreenRegionComponentManager
{
}

internal class TfScreenRegionComponentManager : ITfScreenRegionComponentManager
{
}


internal class TfScreenRegionComponentMeta
{
	public ScreenRegion ScreenRegion { get; init; }
	public Type ComponentType { get; init; }
	public int Position { get; init; }
}