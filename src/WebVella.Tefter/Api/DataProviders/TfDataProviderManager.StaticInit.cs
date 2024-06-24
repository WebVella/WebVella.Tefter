namespace WebVella.Tefter;

public partial interface ITfDataProviderManager { }

public partial class TfDataProviderManager : ITfDataProviderManager
{
	public static List<ITfDataProviderType> _providerTypes { get; internal set; } 

	static TfDataProviderManager()
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
				if (type.GetInterfaces().Any(x => x == typeof(ITfDataProviderType)))
				{
					var instance = (ITfDataProviderType)Activator.CreateInstance(type);
					_providerTypes.Add(instance);
				}
			}
		}
	}
}
