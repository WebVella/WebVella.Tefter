namespace WebVella.Tefter;

public partial interface ITfMetaProvider
{
	public ReadOnlyCollection<TfScreenRegionComponentMeta> GetScreenRegionComponentsMeta(
		TfScreenRegion? region = null);
}

public partial class TfMetaProvider
{
	private static readonly List<TfScreenRegionComponentMeta> _screenRegionComponentMeta =
		new List<TfScreenRegionComponentMeta>();

	public ReadOnlyCollection<TfScreenRegionComponentMeta> GetScreenRegionComponentsMeta(
		TfScreenRegion? region = null)
	{
		if (region is null)
		{
			return _screenRegionComponentMeta
				.OrderBy(x => x.Position)
				.ThenBy(x => x.Name)
				.ToList()
				.AsReadOnly();
		}

		return _screenRegionComponentMeta
			.Where(x => x.ScreenRegion == region)
			.OrderBy(x => x.Position)
			.ThenBy(x => x.Name)
			.ToList()
			.AsReadOnly();
	}

	private static void ScanAndRegisterScreenRegionComponents(
		Type type)
	{
		if (type.GetInterfaces().Any(x => x == typeof(ITfScreenRegionComponent)))
		{
			var instance = (ITfScreenRegionComponent)Activator.CreateInstance(type);
			
			TfScreenRegionComponentMeta meta = new TfScreenRegionComponentMeta
			{
				ComponentType = type,
				Instance = instance,
			};
			
			_screenRegionComponentMeta.Add(meta);
			_typesMap[type.FullName] = type;
		}
	}
}


