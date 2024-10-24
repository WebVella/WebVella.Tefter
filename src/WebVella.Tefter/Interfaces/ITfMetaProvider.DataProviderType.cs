namespace WebVella.Tefter;

public partial interface ITfMetaProvider
{
	public ReadOnlyCollection<TfDataProviderTypeMeta> GetDataProviderTypesMeta();
}

public partial class TfMetaProvider
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
		if (type.GetInterfaces().Any(x => x == typeof(ITfDataProviderType)))
		{
			var instance = (ITfDataProviderType)Activator.CreateInstance(type);
			
			TfDataProviderTypeMeta meta = new TfDataProviderTypeMeta
			{
				Instance = instance,
			};

			_dataProviderTypeMeta.Add(meta);
		}
	}
}


