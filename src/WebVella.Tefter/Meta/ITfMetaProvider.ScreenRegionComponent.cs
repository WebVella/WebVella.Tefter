namespace WebVella.Tefter;

public partial interface ITfMetaProvider
{
	public ReadOnlyCollection<TfScreenRegionComponentMeta> GetScreenRegionComponentsMeta();
}

public partial class TfMetaProvider
{
	private static readonly List<TfScreenRegionComponentMeta> _screenRegionComponentMeta =
		new List<TfScreenRegionComponentMeta>();

	public ReadOnlyCollection<TfScreenRegionComponentMeta> GetScreenRegionComponentsMeta()
	{
		return _screenRegionComponentMeta
			.OrderBy(x => x.Position)
			.ToList()
			.AsReadOnly();
	}

	private static void ScanAndRegisterScreenRegionComponents(
		Type type)
	{
		if (type.GetInterfaces().Any(x => x == typeof(ITfScreenRegionComponent)))
		{
			var instance = (ITfScreenRegionComponent)Activator.CreateInstance(type);
			
			//TODO remove property init and leave only instance
			TfScreenRegionComponentMeta meta = new TfScreenRegionComponentMeta
			{
				Position = instance.Position,
				ScreenRegion = instance.ScreenRegion,
				UrlSlug = instance.UrlSlug,
				Name = instance.Name,
				ComponentType = type
				//Instance = instance,
			};
			
			_screenRegionComponentMeta.Add(meta);
		}
	}
}


