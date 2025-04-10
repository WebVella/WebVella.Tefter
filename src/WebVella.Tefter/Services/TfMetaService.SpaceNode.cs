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
			
			TfSpacePageAddonMeta meta = new TfSpacePageAddonMeta
			{
				Instance = instance,
				ComponentId = instance.Id,
			};

			_spacePageComponentMeta.Add(meta);
			_typesMap[type.FullName] = type;
		}
	}
}


