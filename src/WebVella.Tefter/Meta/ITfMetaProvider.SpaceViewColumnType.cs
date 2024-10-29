namespace WebVella.Tefter;

public partial interface ITfMetaProvider
{
	public ReadOnlyCollection<TfSpaceViewColumnTypeMeta> GetSpaceViewColumnTypesMeta();
}

public partial class TfMetaProvider
{
	private static readonly List<TfSpaceViewColumnTypeMeta> _spaceViewColumnTypeMeta =
		new List<TfSpaceViewColumnTypeMeta>();

	public ReadOnlyCollection<TfSpaceViewColumnTypeMeta> GetSpaceViewColumnTypesMeta()
	{
		return _spaceViewColumnTypeMeta.AsReadOnly();
	}

	private static void ScanAndRegisterSpaceViewColumnTypes(
		Type type)
	{
		if (type.GetInterfaces().Any(x => x == typeof(ITfSpaceViewColumnType)))
		{
			var instance = (ITfSpaceViewColumnType)Activator.CreateInstance(type);
			
			TfSpaceViewColumnTypeMeta meta = new TfSpaceViewColumnTypeMeta
			{
				Instance = instance
			};

			_spaceViewColumnTypeMeta.Add(meta);
			_typesMap[type.FullName] = type;
		}
	}
}


