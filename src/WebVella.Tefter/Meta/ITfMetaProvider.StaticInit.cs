namespace WebVella.Tefter;

public partial class TfMetaProvider
{
	static TfMetaProvider()
	{
		var assemblies = AppDomain.CurrentDomain.GetAssemblies()
							.Where(a => !(a.FullName.ToLowerInvariant().StartsWith("microsoft.")
							   || a.FullName.ToLowerInvariant().StartsWith("system.")));

		foreach (var assembly in assemblies)
		{
			foreach (Type type in assembly.GetTypes())
			{
				ScanAndRegisterSpaceNodeComponents(type);
				ScanAndRegisterDataProvidersTypes(type);
				ScanAndRegisterScreenRegionComponents(type);
				ScanAndRegisterSpaceViewColumnTypes(type);
				ScanAndRegisterApplications(type);
			}
		}
	}
}
