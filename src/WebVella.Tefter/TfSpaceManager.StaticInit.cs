namespace WebVella.Tefter;

public partial class TfSpaceManager : ITfSpaceManager
{
	public static List<ITfSpaceViewColumnType> _spaceViewColumnTypes { get; internal set; }

	static TfSpaceManager()
	{
		_spaceViewColumnTypes = new List<ITfSpaceViewColumnType>();
		ScanAndRegisterSpaceViewColumnTypes();
	}

	private static void ScanAndRegisterSpaceViewColumnTypes()
	{
		var assemblies = AppDomain.CurrentDomain.GetAssemblies()
							.Where(a => !(a.FullName.ToLowerInvariant().StartsWith("microsoft.")
							   || a.FullName.ToLowerInvariant().StartsWith("system.")));

		foreach (var assembly in assemblies)
		{
			foreach (Type type in assembly.GetTypes())
			{
				if (type.GetInterfaces().Any(x => x == typeof(ITfSpaceViewColumnType)))
				{
					var instance = (ITfSpaceViewColumnType)Activator.CreateInstance(type);
					_spaceViewColumnTypes.Add(instance);
				}
			}
		}
	}

	private ITfSpaceViewColumnType GetSpaceViewColumnTypeByName(
		string fullTypeName)
	{
		return _spaceViewColumnTypes.SingleOrDefault(x=>x.GetType().FullName == fullTypeName);
	}

	private static Type GetSpaceViewColumnComponentType(
		string componentTypeName )
	{
		return
			AppDomain.CurrentDomain.GetAssemblies()
				.Select(assembly => assembly.GetType(componentTypeName))
				.FirstOrDefault(t => t != null);
	}

}
