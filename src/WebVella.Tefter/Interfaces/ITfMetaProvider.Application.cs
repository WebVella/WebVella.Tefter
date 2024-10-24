namespace WebVella.Tefter;

public partial interface ITfMetaProvider
{
	public ReadOnlyCollection<TfApplicationMeta> GetApplicationsMeta();
}

public partial class TfMetaProvider
{
	private static List<TfApplicationMeta> _applicationsMeta = 
		new List<TfApplicationMeta>();

	public ReadOnlyCollection<TfApplicationMeta> GetApplicationsMeta()
	{
		return _applicationsMeta.AsReadOnly();
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


