namespace WebVella.Tefter.Services;

public partial interface ITfMetaService
{
	public ReadOnlyCollection<TfSpaceNodeComponentMeta> GetSpaceNodesComponentsMeta();
}

public partial class TfMetaService : ITfMetaService
{
	private static List<TfSpaceNodeComponentMeta> _spaceNodeComponentMeta = 
		new List<TfSpaceNodeComponentMeta>();

	public ReadOnlyCollection<TfSpaceNodeComponentMeta> GetSpaceNodesComponentsMeta()
	{
		return _spaceNodeComponentMeta.AsReadOnly();
	}

	private static void ScanAndRegisterSpaceNodeComponents(
		Type type)
	{

		if (type.ImplementsInterface(typeof(ITfSpaceNodeComponent)))
		{
			var instance = (ITfSpaceNodeComponent)Activator.CreateInstance(type);
			
			TfSpaceNodeComponentMeta meta = new TfSpaceNodeComponentMeta
			{
				Instance = instance,
				ComponentType = type,
			};

			_spaceNodeComponentMeta.Add(meta);
			_typesMap[type.FullName] = type;
		}
	}
}


