namespace WebVella.Tefter.Migrations;

internal partial interface IMigrationManager
{
}

internal partial class MigrationManager : IMigrationManager
{
	private static readonly List<SystemMigrationMeta> _systemMigrations;
	private static readonly List<AddOnMigrationMeta> _addonMigrations;

	static MigrationManager()
	{
		_systemMigrations= new List<SystemMigrationMeta>();
		_addonMigrations = new List<AddOnMigrationMeta>();

		var assemblies = AppDomain.CurrentDomain.GetAssemblies()
							.Where(a => !(a.FullName.ToLowerInvariant().StartsWith("microsoft.")
							   || a.FullName.ToLowerInvariant().StartsWith("system.")));
		foreach (var assembly in assemblies)
		{
			foreach (Type type in assembly.GetTypes())
				ScanAndRegisterMigrationType(type);
		}

		_systemMigrations = _systemMigrations.OrderBy(x => x.Version).ToList();
		_addonMigrations = _addonMigrations.OrderBy(x => x.Version).ToList();
	}

	private static void ScanAndRegisterMigrationType(Type type)
	{
		var implSystemMigrationInterface = type.GetInterfaces().Any(x => x == typeof(ITefterSystemMigration));
		var attrs = type.GetCustomAttributes(typeof(TefterSystemMigrationAttribute), false);
		if (attrs.Length == 1 && type.IsClass && implSystemMigrationInterface )
		{
			var attr = (TefterSystemMigrationAttribute)attrs[0];
			var instance = Activator.CreateInstance(type);
			SystemMigrationMeta meta = new SystemMigrationMeta
			{
				Version = attr.Version,
				MigrationClassName = type.FullName,
				Instance = (ITefterSystemMigration)instance
			};
			_systemMigrations.Add(meta);
		}

		var implAddonMigrationInterface = type.GetInterfaces().Any(x => x == typeof(ITefterAddOnMigration));
		attrs = type.GetCustomAttributes(typeof(TefterAddOnMigrationAttribute), false);
		if (attrs.Length == 1 && type.IsClass && implAddonMigrationInterface )
		{
			var attr = (TefterAddOnMigrationAttribute)attrs[0];
			var instance = Activator.CreateInstance(type);
			AddOnMigrationMeta meta = new AddOnMigrationMeta
			{
				Version = attr.Version,
				MigrationClassName = type.FullName,
				AddOnTypeId = attr.AddOnTypeId,
				Instance = (ITefterAddOnMigration)instance
			};
			_addonMigrations.Add(meta);
		}
		
		
	}
}
