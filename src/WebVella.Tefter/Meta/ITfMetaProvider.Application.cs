using System.Security.AccessControl;

namespace WebVella.Tefter;

public partial interface ITfMetaProvider
{
	public ReadOnlyCollection<TfApplicationMeta> GetApplicationsMeta();

	public ITfApplication GetApplication(
		Guid appId);
}

public partial class TfMetaProvider
{
	private static List<TfApplicationMeta> _applicationsMeta = 
		new List<TfApplicationMeta>();

	public ReadOnlyCollection<TfApplicationMeta> GetApplicationsMeta()
	{
		return _applicationsMeta.AsReadOnly();
	}

	public static ReadOnlyCollection<ITfApplication> GetApplications()
	{
		return _applicationsMeta
			.Select(x => x.Instance)
			.ToList()
			.AsReadOnly();
	}

	public ITfApplication GetApplication(
		Guid appId)
	{
		return GetApplications()
			.SingleOrDefault(x => x.Id == appId);
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

				ScanAndRegisterSpaceNodeComponents(type);
				ScanAndRegisterDataProvidersTypes(type);
				ScanAndRegisterScreenRegionComponents(type);
				ScanAndRegisterSpaceViewColumnTypes(type);
				ScanAndRegisterApplications(type);
			}
		}
	}

	private static void ScanAndRegisterApplications(
		Type type)
	{

		if (type.GetInterfaces().Any(x => x == typeof(ITfApplication)))
		{
			var instance = (ITfApplication)Activator.CreateInstance(type);
			
			TfApplicationMeta meta = new TfApplicationMeta
			{
				Instance = instance,
			};

			_applicationsMeta.Add(meta);
		}
	}
}


