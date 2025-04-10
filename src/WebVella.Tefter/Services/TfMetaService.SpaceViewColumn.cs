namespace WebVella.Tefter.Services;
public partial interface ITfMetaService
{
	public ReadOnlyCollection<TfSpaceViewColumnTypeAddonMeta> GetSpaceViewColumnTypesMeta();

	internal ITfSpaceViewColumnTypeAddon GetSpaceViewColumnTypeByName(
		string fullTypeName);

	internal Type GetSpaceViewColumnComponentType(
		string componentTypeName);
}

public partial class TfMetaService : ITfMetaService
{
	private static readonly Dictionary<string, List<Type>> _columnTypeSupportDict = new();
	private static readonly Dictionary<Guid, List<Guid>> _columnTypeAddonIdSupportDict = new();
	private static readonly List<TfSpaceViewColumnTypeAddonMeta> _spaceViewColumnTypeMeta = new();
	private static List<TfSpaceViewColumnComponentAddonMeta> _spaceViewColumnComponentMeta = new();

	public ReadOnlyCollection<TfSpaceViewColumnTypeAddonMeta> GetSpaceViewColumnTypesMeta()
	{
		return _spaceViewColumnTypeMeta.AsReadOnly();
	}

	public ReadOnlyCollection<TfSpaceViewColumnComponentAddonMeta> GetSpaceViewColumnComponentMeta()
	{
		return _spaceViewColumnComponentMeta.AsReadOnly();
	}

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
			_spaceViewColumnComponentMeta.Add(meta);

			_typesMap[type.FullName] = type;
			_addonIdToTypeMap[instance.Id] = type;

			foreach (var columType in instance.SupportedColumnTypes)
			{
				if (!_columnTypeSupportDict.ContainsKey(columType.FullName))
					_columnTypeSupportDict[columType.FullName] = new List<Type>();

				_columnTypeSupportDict[columType.FullName].Add(type);
			}
			foreach (var addonId in instance.SupportedColumnTypeAddons)
			{
				if (!_columnTypeAddonIdSupportDict.ContainsKey(addonId))
					_columnTypeAddonIdSupportDict[addonId] = new List<Guid>();

				_columnTypeAddonIdSupportDict[addonId].Add(instance.Id);
			}
		}
	}

	private static void ScanAndRegisterSpaceViewColumnTypes(
		Type type)
	{
		if (type.ImplementsInterface(typeof(ITfSpaceViewColumnTypeAddon)))
		{
			var instance = (ITfSpaceViewColumnTypeAddon)Activator.CreateInstance(type);

			instance.SupportedComponentTypes = GetSupportedComponentTypesForColumnType(type.FullName);

			TfSpaceViewColumnTypeAddonMeta meta = new TfSpaceViewColumnTypeAddonMeta
			{
				Instance = instance
			};
			_spaceViewColumnTypeMeta.Add(meta);

			_typesMap[type.FullName] = type;
			_addonIdToTypeMap[instance.Id] = type;
		}
	}

	private static List<Type> GetSupportedComponentTypesForColumnType(string typeFullName)
	{
		return _columnTypeSupportDict.ContainsKey(typeFullName)
				? _columnTypeSupportDict[typeFullName]
				: new List<Type>();

	}

	public ITfSpaceViewColumnTypeAddon GetSpaceViewColumnTypeByName(
		string fullTypeName)
	{
		var meta = _spaceViewColumnTypeMeta.SingleOrDefault(x => x.Instance.GetType().FullName == fullTypeName);

		if (meta != null)
		{
			return (ITfSpaceViewColumnTypeAddon)meta.Instance;
		}

		return null;
	}

	public Type GetSpaceViewColumnComponentType(
		string componentTypeName)
	{
		if (_typesMap.ContainsKey(componentTypeName))
		{
			var type = _typesMap[componentTypeName];
			if (type.ImplementsInterface(typeof(ITfSpaceViewColumnComponentAddon)))
			{
				return type;
			}
		}

		return null;
	}
}


