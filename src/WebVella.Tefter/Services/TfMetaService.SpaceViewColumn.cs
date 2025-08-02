namespace WebVella.Tefter.Services;
public partial interface ITfMetaService
{
	public ReadOnlyCollection<TfSpaceViewColumnTypeAddonMeta> GetSpaceViewColumnTypesMeta();
	public ReadOnlyCollection<TfSpaceViewColumnComponentAddonMeta> GetSpaceViewColumnComponentMeta();
	public ReadOnlyDictionary<Guid,TfSpaceViewColumnTypeAddonMeta> GetSpaceViewColumnTypeMetaDictionary();
	public ReadOnlyDictionary<Guid,TfSpaceViewColumnComponentAddonMeta> GetSpaceViewColumnComponentMetaDictionary();
	public TfSpaceViewColumnTypeAddonMeta GetSpaceViewColumnType(Guid addonId);
	public ITfSpaceViewColumnComponentAddon GetSpaceViewColumnComponent(Guid addonId);
	public List<ITfSpaceViewColumnComponentAddon> GetSpaceViewColumnTypeSupportedComponents(Guid addonId);
}

public partial class TfMetaService : ITfMetaService
{
	private static readonly List<TfSpaceViewColumnTypeAddonMeta> _columnTypeMetaList = new();
	private static readonly Dictionary<Guid, TfSpaceViewColumnTypeAddonMeta> _columnTypeMetaDict = new();
	private static readonly Dictionary<Guid, List<Guid>> _columnTypeComponentsDict = new();
	private static List<TfSpaceViewColumnComponentAddonMeta> _columnComponentMetaList = new();
	private static Dictionary<Guid, TfSpaceViewColumnComponentAddonMeta> _columnComponentMetaDict = new();

	public ReadOnlyCollection<TfSpaceViewColumnTypeAddonMeta> GetSpaceViewColumnTypesMeta()
	{
		return _columnTypeMetaList.AsReadOnly();
	}
	public ReadOnlyCollection<TfSpaceViewColumnComponentAddonMeta> GetSpaceViewColumnComponentMeta()
	{
		return _columnComponentMetaList.AsReadOnly();
	}

	public ReadOnlyDictionary<Guid,TfSpaceViewColumnTypeAddonMeta> GetSpaceViewColumnTypeMetaDictionary()
	{
		return _columnTypeMetaDict.AsReadOnly();
	}

	public ReadOnlyDictionary<Guid,TfSpaceViewColumnComponentAddonMeta> GetSpaceViewColumnComponentMetaDictionary()
	{
		return _columnComponentMetaDict.AsReadOnly();
	}

	public TfSpaceViewColumnTypeAddonMeta? GetSpaceViewColumnType(Guid addonId)
	{
		if (_columnTypeMetaDict.ContainsKey(addonId)) return _columnTypeMetaDict[addonId];
		return null;
	}

	public List<ITfSpaceViewColumnComponentAddon> GetSpaceViewColumnTypeSupportedComponents(Guid addonId)
	{
		var componentType = GetSpaceViewColumnType(addonId);
		if (componentType == null) return new List<ITfSpaceViewColumnComponentAddon>();
		var result = new List<ITfSpaceViewColumnComponentAddon>();
		var addedHs = new HashSet<Guid>();
		foreach (var compId in componentType.Instance.SupportedComponents)
		{
			if(addedHs.Contains(addonId)) continue;
			addedHs.Add(compId);

			var comp = GetSpaceViewColumnComponent(compId);
			if (comp is not null)
				result.Add(comp);
		}

		return result;
	}

	public ITfSpaceViewColumnComponentAddon GetSpaceViewColumnComponent(Guid addonId)
	{
		if (_columnComponentMetaDict.ContainsKey(addonId)) return _columnComponentMetaDict[addonId].Instance;
		return null;
	}

	//Private
	private static void ScanAndRegisterSpaceViewColumnComponents(
			Type type)
	{
		if (type.ImplementsInterface(typeof(ITfSpaceViewColumnComponentAddon)))
		{
			var instance = (ITfSpaceViewColumnComponentAddon)Activator.CreateInstance(type);

			TfSpaceViewColumnComponentAddonMeta meta = new TfSpaceViewColumnComponentAddonMeta
			{
				Instance = instance
			};
			_columnComponentMetaList.Add(meta);

			if(_addonIdHS.Contains(instance.AddonId))
				throw new Exception($"Duplicated Addon Id found: {instance.AddonId}");
			_addonIdHS.Add(instance.AddonId);

			_columnComponentMetaDict[instance.AddonId] = meta;

			_typesMap[type.FullName] = type;
			foreach (var addonId in instance.SupportedColumnTypes)
			{
				if (!_columnTypeComponentsDict.ContainsKey(addonId))
					_columnTypeComponentsDict[addonId] = new List<Guid>();

				_columnTypeComponentsDict[addonId].Add(instance.AddonId);
			}
		}
	}

	private static void ScanAndRegisterSpaceViewColumnTypes(
		Type type)
	{
		if (type.ImplementsInterface(typeof(ITfSpaceViewColumnTypeAddon)))
		{
			var instance = (ITfSpaceViewColumnTypeAddon)Activator.CreateInstance(type);

			if(_addonIdHS.Contains(instance.AddonId))
				throw new Exception($"Duplicated Addon Id found: {instance.AddonId}");
			_addonIdHS.Add(instance.AddonId);

			instance.SupportedComponents = GetSupportedComponentTypesForColumnType(instance.AddonId);

			TfSpaceViewColumnTypeAddonMeta meta = new TfSpaceViewColumnTypeAddonMeta
			{
				Instance = instance
			};
			_columnTypeMetaList.Add(meta);
			_columnTypeMetaDict[instance.AddonId] = meta;

			_typesMap[type.FullName] = type;
		}
	}

	private static List<Guid> GetSupportedComponentTypesForColumnType(Guid addonId)
	{
		return _columnTypeComponentsDict.ContainsKey(addonId)
				? _columnTypeComponentsDict[addonId]
				: new List<Guid>();

	}

}


