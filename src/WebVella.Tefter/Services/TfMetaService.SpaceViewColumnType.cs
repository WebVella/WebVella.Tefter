namespace WebVella.Tefter.Services;
public partial interface ITfMetaService
{
	public ReadOnlyCollection<TfSpaceViewColumnAddonMeta> GetSpaceViewColumnTypesMeta();

	internal ITfSpaceViewColumnAddon GetSpaceViewColumnTypeByName(
		string fullTypeName);

	internal Type GetSpaceViewColumnComponentType(
		string componentTypeName);
}

public partial class TfMetaService : ITfMetaService
{
	private static readonly Dictionary<string, List<Type>> _columnTypeSupportDict =
		new Dictionary<string, List<Type>>();

	private static readonly List<TfSpaceViewColumnAddonMeta> _spaceViewColumnTypeMeta =
		new List<TfSpaceViewColumnAddonMeta>();

	public ReadOnlyCollection<TfSpaceViewColumnAddonMeta> GetSpaceViewColumnTypesMeta()
	{
		return _spaceViewColumnTypeMeta.AsReadOnly();
	}

	private static void ScanAndRegisterSpaceViewColumnComponents(
			Type type)
	{
		if (type.ImplementsInterface(typeof(ITfSpaceViewColumnComponentAddon)))
		{
			var instance = (ITfSpaceViewColumnComponentAddon)Activator.CreateInstance(type);

			_typesMap[type.FullName] = type;

			foreach (var columType in instance.SupportedColumnTypes)
			{
				if (!_columnTypeSupportDict.ContainsKey(columType.FullName))
					_columnTypeSupportDict[columType.FullName] = new List<Type>();

				_columnTypeSupportDict[columType.FullName].Add(type);
			}
		}
	}

	private static void ScanAndRegisterSpaceViewColumnTypes(
		Type type)
	{
		if (type.ImplementsInterface(typeof(ITfSpaceViewColumnAddon)))
		{
			var instance = (ITfSpaceViewColumnAddon)Activator.CreateInstance(type);

			instance.SupportedComponentTypes = GetSupportedComponentTypesForColumnType(type.FullName);

			TfSpaceViewColumnAddonMeta meta = new TfSpaceViewColumnAddonMeta
			{
				Instance = instance
			};

			_spaceViewColumnTypeMeta.Add(meta);
			_typesMap[type.FullName] = type;
		}
	}

	private static List<Type> GetSupportedComponentTypesForColumnType(string typeFullName)
	{
		return _columnTypeSupportDict.ContainsKey(typeFullName)
				? _columnTypeSupportDict[typeFullName]
				: new List<Type>();

	}

	public ITfSpaceViewColumnAddon GetSpaceViewColumnTypeByName(
		string fullTypeName)
	{
		var meta = _spaceViewColumnTypeMeta.SingleOrDefault(x => x.Instance.GetType().FullName == fullTypeName);

		if (meta != null)
		{
			return (ITfSpaceViewColumnAddon)meta.Instance;
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


