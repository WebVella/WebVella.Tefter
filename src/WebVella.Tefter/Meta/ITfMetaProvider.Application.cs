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
			_typesMap[type.FullName] = type;
		}
	}
}


