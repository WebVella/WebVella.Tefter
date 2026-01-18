namespace WebVella.Tefter.Services;

public partial interface ITfMetaService
{
	ITfDataProviderAddon? GetDataProviderType(
		Guid id);

	List<ITfDataProviderAddon> GetDataProviderTypes();
}

public partial class TfMetaService : ITfMetaService
{
	public static List<ITfDataProviderAddon> _providerTypes { get; internal set; } = new ();

	private static void ScanAndRegisterDataProvidersTypes(
		Type type)
	{
		if (type.GetInterfaces().Any(x => x == typeof(ITfDataProviderAddon)))
		{
			var instance = (ITfDataProviderAddon)Activator.CreateInstance(type);

			if(_addonIdHS.Contains(instance.AddonId))
				throw new Exception($"Duplicated Addon Id found: {instance.AddonId}");
			_addonIdHS.Add(instance.AddonId);

			_providerTypes.Add(instance);
			_typesMap[type.FullName] = type;
		}
	}
	public ITfDataProviderAddon? GetDataProviderType(
		Guid id)
	{
		var dictInstance = _providerTypes.SingleOrDefault(x => x.AddonId == id);
		if (dictInstance is null) return null;
		return (ITfDataProviderAddon?)Activator.CreateInstance(dictInstance.GetType());		
	}

	public List<ITfDataProviderAddon> GetDataProviderTypes()
	{
		var instances = new List<ITfDataProviderAddon>();
		foreach (var dictInstance in _providerTypes)
		{
			var instance = (ITfDataProviderAddon?)Activator.CreateInstance(dictInstance.GetType());
			if(instance is null) continue;
			instances.Add(instance);
		}
		return instances;
	}		
}


