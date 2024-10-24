namespace WebVella.Tefter;

public partial interface ITfMetaProvider
{
	public ReadOnlyCollection<TfSpaceNodeComponentMeta> GetSpaceNodesComponentsMeta();
}

public partial class TfMetaProvider
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

		if (type.GetInterfaces().Any(x => x == typeof(ISpaceNodeComponent)))
		{
			var instance = (ISpaceNodeComponent)Activator.CreateInstance(type);
			
			TfSpaceNodeComponentMeta meta = new TfSpaceNodeComponentMeta
			{
				Instance = instance,
			};

			_spaceNodeComponentMeta.Add(meta);
		}
	}
}


