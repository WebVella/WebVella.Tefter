namespace WebVella.Tefter.Services;

public partial interface ITfMetaService
{
	public ReadOnlyCollection<TfApplicationAddonMeta> GetApplicationsMeta();

	public ITfApplicationAddon GetApplication(
		Guid appId);
}

public partial class TfMetaService : ITfMetaService
{
	private static List<TfApplicationAddonMeta> _applicationsMeta =
		new List<TfApplicationAddonMeta>();

	public ReadOnlyCollection<TfApplicationAddonMeta> GetApplicationsMeta()
	{
		return _applicationsMeta.AsReadOnly();
	}

	public static ReadOnlyCollection<ITfApplicationAddon> GetApplications()
	{
		return _applicationsMeta
			.Select(x => x.Instance)
			.ToList()
			.AsReadOnly();
	}

	public ITfApplicationAddon GetApplication(
		Guid appId)
	{
		return GetApplications()
			.SingleOrDefault(x => x.Id == appId);
	}



	private static void ScanAndRegisterApplications(
		Type type)
	{

		if (type.ImplementsInterface(typeof(ITfApplicationAddon)))
		{
			var instance = (ITfApplicationAddon)Activator.CreateInstance(type);

			TfApplicationAddonMeta meta = new TfApplicationAddonMeta
			{
				Instance = instance,
			};

			_applicationsMeta.Add(meta);
			_typesMap[type.FullName] = type;
		}
	}
}


