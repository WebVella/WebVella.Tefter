namespace WebVella.Tefter.Services;

public partial interface ITfMetaService
{
	public List<ITfSpacePageAddon> GetSpacePagesComponents();
}

public partial class TfMetaService
{
	private static List<ITfSpacePageAddon> _spacePageComponents = new ();

	public List<ITfSpacePageAddon> GetSpacePagesComponents()
	{
		var instances = new List<ITfSpacePageAddon>();
		foreach (var dictInstance in _spacePageComponents)
		{
			var instance = (ITfSpacePageAddon?)Activator.CreateInstance(dictInstance.GetType());
			if(instance is null) continue;
			instances.Add(instance);
		}

		return instances.OrderBy(x => x.AddonName).ToList();
	}		

	private static void ScanAndRegisterSpacePageComponents(
		Type type)
	{

		if (type.ImplementsInterface(typeof(ITfSpacePageAddon)))
		{
			var instance = (ITfSpacePageAddon)Activator.CreateInstance(type);
			
			if(_addonIdHS.Contains(instance.AddonId))
				throw new Exception($"Duplicated Addon Id found: {instance.AddonId}");
			_addonIdHS.Add(instance.AddonId);
			_spacePageComponents.Add(instance);
			_typesMap[type.FullName] = type;
		}
	}
}


