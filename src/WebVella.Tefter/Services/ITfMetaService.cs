namespace WebVella.Tefter.Services;

public partial interface ITfMetaService
{
	public Type GetTypeForFullClassName(
		string fullClassName);
}

public partial class TfMetaService : ITfMetaService
{
	private static Dictionary<string, Type> _typesMap = new Dictionary<string, Type>();

	public Type GetTypeForFullClassName(
		string fullClassName)
	{
		if (fullClassName is null)
		{
			return null;
		}

		if (!_typesMap.ContainsKey(fullClassName))
		{
			return null;
		}

		return _typesMap[fullClassName];
	}

	internal static void Init()
	{
		var assemblies = AppDomain.CurrentDomain.GetAssemblies()
							.Where(a => !(a.FullName.ToLowerInvariant().StartsWith("microsoft.")
							   || a.FullName.ToLowerInvariant().StartsWith("system.")));

		foreach (var assembly in assemblies)
		{
			foreach (Type type in assembly.GetTypes())
			{
				if (type.IsAbstract || type.IsInterface)
					continue;

				var defaultConstructor = type.GetConstructor(Type.EmptyTypes);
				if (defaultConstructor is null)
					continue;

				ScanAndRegisterSpaceViewColumnComponents(type);
			}
		}

		foreach (var assembly in assemblies)
		{
			foreach (Type type in assembly.GetTypes())
			{
				if (type.IsAbstract || type.IsInterface)
					continue;

				var defaultConstructor = type.GetConstructor(Type.EmptyTypes);
				if (defaultConstructor is null)
					continue;

				ScanAndRegisterSpaceNodeComponents(type);
				ScanAndRegisterDataProvidersTypes(type);
				ScanAndRegisterScreenRegionComponents(type);
				ScanAndRegisterSpaceViewColumnTypes(type);
				ScanAndRegisterApplications(type);
				ScanAndRegisterRegionComponents(type);
			}
		}
	}
}
