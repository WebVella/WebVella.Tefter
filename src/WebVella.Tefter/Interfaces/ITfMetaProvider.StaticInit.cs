namespace WebVella.Tefter;

public partial class TfMetaProvider
{
	static TfMetaProvider()
	{
		_spaceNodeComponentMeta = new List<TfSpaceNodeComponentMeta>();

		var assemblies = AppDomain.CurrentDomain.GetAssemblies()
							.Where(a => !(a.FullName.ToLowerInvariant().StartsWith("microsoft.")
							   || a.FullName.ToLowerInvariant().StartsWith("system.")));

		foreach (var assembly in assemblies)
		{
			foreach (Type type in assembly.GetTypes())
			{
				ScanAndRegisterSpaceNodeComponents(type);
			}
		}
	}
}
