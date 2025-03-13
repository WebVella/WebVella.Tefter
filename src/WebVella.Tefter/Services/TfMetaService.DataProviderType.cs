namespace WebVella.Tefter.Services;

public partial interface ITfMetaService
{
	ITfDataProviderType GetDataProviderType(
		Guid id);

	ReadOnlyCollection<ITfDataProviderType> GetDataProviderTypes();
}

public partial class TfMetaService : ITfMetaService
{
	public static List<ITfDataProviderType> _providerTypes { get; internal set; } = new List<ITfDataProviderType>();

	private static void ScanAndRegisterDataProvidersTypes(
		Type type)
	{
		if (type.GetInterfaces().Any(x => x == typeof(ITfDataProviderType)))
		{
			var instance = (ITfDataProviderType)Activator.CreateInstance(type);
			_providerTypes.Add(instance);
			_typesMap[type.FullName] = type;
		}
	}
	public ITfDataProviderType GetDataProviderType(
		Guid id)
	{
		return _providerTypes.SingleOrDefault(x => x.Id == id);
	}

	public ReadOnlyCollection<ITfDataProviderType> GetDataProviderTypes()
	{
		return _providerTypes.AsReadOnly();
	}
}


