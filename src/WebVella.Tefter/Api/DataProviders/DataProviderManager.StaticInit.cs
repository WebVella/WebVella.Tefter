namespace WebVella.Tefter;

public partial interface IDataProviderManager { }

internal partial class DataProviderManager : IDataProviderManager
{
	public static List<ITfDataProviderType> _providerTypes { get; internal set; } 

	static DataProviderManager()
	{
		_providerTypes = new List<ITfDataProviderType>();
		ScanAndRegisterDataProviderTypes();
	}

	private static void ScanAndRegisterDataProviderTypes()
	{
		var assemblies = AppDomain.CurrentDomain.GetAssemblies()
							.Where(a => !(a.FullName.ToLowerInvariant().StartsWith("microsoft.")
							   || a.FullName.ToLowerInvariant().StartsWith("system.")));
		foreach (var assembly in assemblies)
		{
			foreach (Type type in assembly.GetTypes())
			{
				if (type.Name.ToLowerInvariant().Contains("csv"))
				{
					int br = 0;
				}
				var isProviderTypeClass = type.GetInterfaces().Any(x => x == typeof(ITfDataProviderType));

				if (isProviderTypeClass)
				{
					var instance = (ITfDataProviderType)Activator.CreateInstance(type);
					_providerTypes.Add(instance);
				}
			}
		}
	}
}
