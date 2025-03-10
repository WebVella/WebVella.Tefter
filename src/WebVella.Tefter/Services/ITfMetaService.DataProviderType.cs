namespace WebVella.Tefter.Services;

public partial interface ITfMetaService
{
	public ReadOnlyCollection<TfDataProviderTypeMeta> GetDataProviderTypesMeta();
}

public partial class TfMetaService : ITfMetaService
{
	private static readonly List<TfDataProviderTypeMeta> _dataProviderTypeMeta =
		new List<TfDataProviderTypeMeta>();

	public ReadOnlyCollection<TfDataProviderTypeMeta> GetDataProviderTypesMeta()
	{
		return _dataProviderTypeMeta.AsReadOnly();
	}

	private static void ScanAndRegisterDataProvidersTypes(
		Type type)
	{
		if (type.ImplementsInterface(typeof(ITfDataProviderType)))
		{
			var instance = (ITfDataProviderType)Activator.CreateInstance(type);
			
			TfDataProviderTypeMeta meta = new TfDataProviderTypeMeta
			{
				Instance = instance,
			};

			_dataProviderTypeMeta.Add(meta);
			_typesMap[type.FullName] = type;
		}
	}
}


