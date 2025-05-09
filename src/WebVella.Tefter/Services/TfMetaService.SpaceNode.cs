namespace WebVella.Tefter.Services;

public partial interface ITfMetaService
{
	public ReadOnlyCollection<TfSpacePageAddonMeta> GetSpacePagesComponentsMeta();
}

public partial class TfMetaService : ITfMetaService
{
	private static List<TfSpacePageAddonMeta> _spacePageComponentMeta = 
		new List<TfSpacePageAddonMeta>();

	public ReadOnlyCollection<TfSpacePageAddonMeta> GetSpacePagesComponentsMeta()
	{
		return _spacePageComponentMeta.AsReadOnly();
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

			TfSpacePageAddonMeta meta = new TfSpacePageAddonMeta
			{
				Instance = instance,
				ComponentId = instance.AddonId,
			};

			_spacePageComponentMeta.Add(meta);
			_typesMap[type.FullName] = type;
		}
	}
}


