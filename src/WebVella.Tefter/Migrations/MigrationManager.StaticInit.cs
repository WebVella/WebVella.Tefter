namespace WebVella.Tefter.Migrations;

internal partial interface IMigrationManager
{
}

internal partial class MigrationManager : IMigrationManager
{
	private static readonly List<SystemMigrationMeta> _systemMigrations;
	private static readonly List<AddOnMigrationMeta> _applicationMigrations;

	static MigrationManager()
	{
		_systemMigrations = new List<SystemMigrationMeta>();
		_applicationMigrations = new List<AddOnMigrationMeta>();

		var assemblies = AppDomain.CurrentDomain.GetAssemblies()
							.Where(a => !(a.FullName.ToLowerInvariant().StartsWith("microsoft.")
							   || a.FullName.ToLowerInvariant().StartsWith("system.")));
		
		foreach (var assembly in assemblies)
		{
			foreach (Type type in assembly.GetTypes())
				ScanAndRegisterMigrationType(type);
		}

		_systemMigrations = _systemMigrations.OrderBy(x => x.Version).ToList();
		_applicationMigrations = _applicationMigrations.OrderBy(x => x.Version).ToList();
	}

	private static void ScanAndRegisterMigrationType(Type type)
	{
		if (type.IsAbstract || type.IsInterface)
			return;

		var attrs = type.GetCustomAttributes(typeof(TfSystemMigrationAttribute), false);
		if (attrs.Length == 1 && type.IsClass && type.IsAssignableTo(typeof(TfSystemMigration)))
		{
			var attr = (TfSystemMigrationAttribute)attrs[0];
			var instance = Activator.CreateInstance(type);
			SystemMigrationMeta meta = new SystemMigrationMeta
			{
				Version = attr.Version,
				MigrationClassName = type.FullName,
				Instance = (TfSystemMigration)instance
			};
			_systemMigrations.Add(meta);
		}

		var implAddonMigrationInterface = type.GetInterfaces().Any(x => x == typeof(ITfApplicationMigration));
		attrs = type.GetCustomAttributes(typeof(TfApplicationMigrationAttribute), false);
		if (attrs.Length == 1 && type.IsClass && implAddonMigrationInterface)
		{
			var attr = (TfApplicationMigrationAttribute)attrs[0];
			var instance = Activator.CreateInstance(type);
			AddOnMigrationMeta meta = new AddOnMigrationMeta
			{
				Version = attr.Version,
				MigrationClassName = type.FullName,
				ApplicationId = attr.ApplicationId,
				Instance = (ITfApplicationMigration)instance
			};
			_applicationMigrations.Add(meta);
		}


	}
}
