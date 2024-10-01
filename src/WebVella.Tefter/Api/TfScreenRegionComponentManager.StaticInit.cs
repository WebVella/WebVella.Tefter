namespace WebVella.Tefter;

internal partial class TfScreenRegionComponentManager : ITfScreenRegionComponentManager
{
	private static readonly List<TfScreenRegionComponentMeta> _meta;

	static TfScreenRegionComponentManager()
	{
		_meta = new List<TfScreenRegionComponentMeta>();

		var assemblies = AppDomain.CurrentDomain.GetAssemblies()
							.Where(a => !(a.FullName.ToLowerInvariant().StartsWith("microsoft.")
							   || a.FullName.ToLowerInvariant().StartsWith("system.")));

		foreach (var assembly in assemblies)
		{
			foreach (Type type in assembly.GetTypes())
				ScanAndRegisterScreenRegionComponentTypes(type);
		}
	}

	private static void ScanAndRegisterScreenRegionComponentTypes(
		Type type)
	{
		if (!type.IsClass || type.GetTypeInfo().IsAbstract)
			return;

		var attrs = type.GetCustomAttributes(typeof(TfScreenRegionComponentAttribute), false);
		if (attrs.Length == 1 && type.IsClass && type.IsAssignableTo(typeof(ComponentBase)))
		{
			var attr = (TfScreenRegionComponentAttribute)attrs[0];

			TfScreenRegionComponentMeta meta = new TfScreenRegionComponentMeta
			{
				Position = attr.Position,
				ScreenRegion = attr.ScreenRegion,
				UrlSlug = attr.UrlSlug,
				Name = attr.Name,
				ComponentType = type
			};

			_meta.Add(meta);
		}
	}
}
