namespace WebVella.Tefter;

public partial interface ITfMetaProvider
{
	public ReadOnlyCollection<TfSpaceViewColumnTypeMeta> GetSpaceViewColumnTypesMeta();
}

public partial class TfMetaProvider
{
	private static readonly Dictionary<string, List<Type>> _columnTypeSupportDict =
		new Dictionary<string, List<Type>>();

	private static readonly List<TfSpaceViewColumnTypeMeta> _spaceViewColumnTypeMeta =
		new List<TfSpaceViewColumnTypeMeta>();

	public ReadOnlyCollection<TfSpaceViewColumnTypeMeta> GetSpaceViewColumnTypesMeta()
	{
		return _spaceViewColumnTypeMeta.AsReadOnly();
	}

	private static void ScanAndRegisterSpaceViewColumnComponents(
			Type type)
	{
		if (type.ImplementsInterface(typeof(ITfSpaceViewColumnComponent)))
		{
			var instance = (ITfSpaceViewColumnComponent)Activator.CreateInstance(type);

			foreach (var columType in instance.SupportedColumnTypes)
			{
				if(!_columnTypeSupportDict.ContainsKey(columType.FullName))
					_columnTypeSupportDict[columType.FullName] = new List<Type>();

				_columnTypeSupportDict[columType.FullName].Add(type);
			}
		}
	}

	private static void ScanAndRegisterSpaceViewColumnTypes(
		Type type)
	{
		if (type.ImplementsInterface(typeof(ITfSpaceViewColumnType)))
		{
			var instance = (ITfSpaceViewColumnType)Activator.CreateInstance(type);

			instance.SupportedComponentTypes = GetSupportedComponentTypesForColumnType(type.FullName);

			TfSpaceViewColumnTypeMeta meta = new TfSpaceViewColumnTypeMeta
			{
				Instance = instance
			};

			_spaceViewColumnTypeMeta.Add(meta);
			_typesMap[type.FullName] = type;
		}
	}

	private static List<Type> GetSupportedComponentTypesForColumnType(string typeFullName){ 
		return _columnTypeSupportDict.ContainsKey(typeFullName)
				? _columnTypeSupportDict[typeFullName]
				: new List<Type>();
	
	}
}


