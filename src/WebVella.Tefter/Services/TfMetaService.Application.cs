namespace WebVella.Tefter.Services;

public partial interface ITfMetaService
{
	public ITfApplicationAddon? GetApplication(
		Guid appId);

}

public partial class TfMetaService : ITfMetaService
{
	private static List<ITfApplicationAddon> _applications = new();

	public static List<ITfApplicationAddon> GetApplications()
	{
		var instances = new List<ITfApplicationAddon>();
		foreach (var dictInstance in _applications)
		{
			var instance = (ITfApplicationAddon?)Activator.CreateInstance(dictInstance.GetType());
			if (instance is null) continue;
			instances.Add(instance);
		}
		return instances;
	}

	public ITfApplicationAddon? GetApplication(
		Guid appId)
	{
		var dictInstance = _applications.SingleOrDefault(x => x.AddonId == appId);
		if (dictInstance is null) return null;
		return (ITfApplicationAddon?)Activator.CreateInstance(dictInstance.GetType());
	}


	private static void ScanAndRegisterApplications(
		Type type)
	{
		if (type.ImplementsInterface(typeof(ITfApplicationAddon)))
		{
			var instance = (ITfApplicationAddon)Activator.CreateInstance(type);
			if (_addonIdHS.Contains(instance.AddonId))
				throw new Exception($"Duplicated Addon Id found: {instance.AddonId}");
			_addonIdHS.Add(instance.AddonId);
			_applications.Add(instance);
			_typesMap[type.FullName] = type;
		}
	}
}