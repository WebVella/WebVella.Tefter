namespace WebVella.Tefter;

public interface ITfApplicationManager
{
	public TfApplicationBase GetApplication(
		Guid appId);
}

public class TfApplicationManager : ITfApplicationManager
{
	private static readonly List<TfApplicationBase> _applications;

	static TfApplicationManager()
	{
		_applications = new List<TfApplicationBase>();

		var assemblies = AppDomain.CurrentDomain.GetAssemblies()
							.Where(a => !(a.FullName.ToLowerInvariant().StartsWith("microsoft.")
							   || a.FullName.ToLowerInvariant().StartsWith("system.")));

		foreach (var assembly in assemblies)
		{
			foreach (Type type in assembly.GetTypes())
				ScanAndRegisterApplicationType(type);
		}
	}

	private static void ScanAndRegisterApplicationType(
		Type type)
	{
		if (!type.IsClass || type.GetTypeInfo().IsAbstract)
			return;

		if (type.IsAssignableTo(typeof(TfApplicationBase)))
		{
			var instance = (TfApplicationBase)Activator.CreateInstance(type);
			_applications.Add(instance);
		}
	}

	internal static List<TfApplicationBase> GetApplicationsInternal()
	{
		return _applications.ToList();
	}

	public TfApplicationBase GetApplication(
		Guid appId)
	{
		return _applications.SingleOrDefault(x => x.Id == appId);
	}


}
