namespace WebVella.Tefter.Services;

public partial interface ITfMetaService
{
	public ReadOnlyCollection<TfSpacePageAddonMeta> GetSpaceNodesComponentsMeta();
}

public partial class TfMetaService : ITfMetaService
{
	private static List<TfSpacePageAddonMeta> _spaceNodeComponentMeta = 
		new List<TfSpacePageAddonMeta>();

	public ReadOnlyCollection<TfSpacePageAddonMeta> GetSpaceNodesComponentsMeta()
	{
		return _spaceNodeComponentMeta.AsReadOnly();
	}

	private static void ScanAndRegisterSpaceNodeComponents(
		Type type)
	{

		if (type.ImplementsInterface(typeof(ITfSpacePageAddon)))
		{
			var instance = (ITfSpacePageAddon)Activator.CreateInstance(type);
			
			TfSpacePageAddonMeta meta = new TfSpacePageAddonMeta
			{
				Instance = instance,
				ComponentType = type,
			};

			_spaceNodeComponentMeta.Add(meta);
			_typesMap[type.FullName] = type;
		}
	}
}


