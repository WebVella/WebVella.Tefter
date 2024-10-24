namespace WebVella.Tefter;

public partial interface ITfTypeProvider
{
	public ReadOnlyCollection<TfSpaceNodeComponentMeta> GetSpaceNodesComponentMeta();
}

public partial class TfTypeProvider
{
	private static readonly List<TfSpaceNodeComponentMeta> _spaceNodeComponentMeta;

	public ReadOnlyCollection<TfSpaceNodeComponentMeta> GetSpaceNodesComponentMeta()
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
				Id = instance.Id,
				Description = instance.Description,
				Name = instance.Name,
				SettingsComponentType = instance.SettingsComponentType
			};
			_spaceNodeComponentMeta.Add(meta);
		}
	}
}


