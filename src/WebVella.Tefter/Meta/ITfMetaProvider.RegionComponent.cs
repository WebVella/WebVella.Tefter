namespace WebVella.Tefter;

public partial interface ITfMetaProvider
{
	public ReadOnlyCollection<TfRegionComponentMeta> GetRegionComponentsMeta();
	public ReadOnlyCollection<TfRegionComponentMeta> GetRegionComponentsMeta(Type context = null, Type scope = null);
}

public partial class TfMetaProvider
{
	private static readonly List<TfRegionComponentMeta> _regionComponentMeta =
		new List<TfRegionComponentMeta>();

	public ReadOnlyCollection<TfRegionComponentMeta> GetRegionComponentsMeta()
	{
		return _regionComponentMeta
			.OrderBy(x => x.PositionRank)
			.ThenBy(x => x.Name)
			.ToList()
			.AsReadOnly();
	}
	public ReadOnlyCollection<TfRegionComponentMeta> GetRegionComponentsMeta(Type context = null, Type scope = null)
	{
		return _regionComponentMeta
			.Where(x =>
				(scope is null || x.ScopeTypeFullNames.Contains(scope.FullName))
				&& (context is null || x.ComponentType.ImplementsGenericInterface(typeof(ITfRegionComponent<>),context))
				)
			.OrderBy(x => x.PositionRank)
			.ThenBy(x => x.Name)
			.ToList()
			.AsReadOnly();
	}

	private static void ScanAndRegisterRegionComponents(Type type)
	{
		TfRegionComponentMeta meta = null;

		#region << Try Get meta >>
		{
			if (type.ImplementsGenericInterface(typeof(ITfRegionComponent<>), typeof(TfDataProviderManageSettingsComponentContext)))
			{
				var instance = (ITfRegionComponent<TfDataProviderManageSettingsComponentContext>)Activator.CreateInstance(type);
				meta = new TfRegionComponentMeta
				{
					Id = instance.Id,
					PositionRank = instance.PositionRank,
					Name = instance.Name,
					Description = instance.Description,
					FluentIconName = instance.FluentIconName,
					ComponentType = type,
					ScopeTypeFullNames = type.GetGenericTypeFromImplementedGenericInterfaceAsStringList(typeof(ITfComponentScope<>)),
				};
			}
			else if (type.ImplementsGenericInterface(typeof(ITfRegionComponent<>), typeof(TfDataProviderDisplaySettingsComponentContext)))
			{
				var instance = (ITfRegionComponent<TfDataProviderDisplaySettingsComponentContext>)Activator.CreateInstance(type);
				meta = new TfRegionComponentMeta
				{
					Id = instance.Id,
					PositionRank = instance.PositionRank,
					Name = instance.Name,
					Description = instance.Description,
					FluentIconName = instance.FluentIconName,
					ComponentType = type,
					ScopeTypeFullNames = type.GetGenericTypeFromImplementedGenericInterfaceAsStringList(typeof(ITfComponentScope<>)),
				};
			}
			else if (type.ImplementsGenericInterface(typeof(ITfRegionComponent<>), typeof(TfSpaceNodeManageComponentContext)))
			{
				var instance = (ITfRegionComponent<TfSpaceNodeManageComponentContext>)Activator.CreateInstance(type);
				meta = new TfRegionComponentMeta
				{
					Id = instance.Id,
					PositionRank = instance.PositionRank,
					Name = instance.Name,
					Description = instance.Description,
					FluentIconName = instance.FluentIconName,
					ComponentType = type,
					ScopeTypeFullNames = type.GetGenericTypeFromImplementedGenericInterfaceAsStringList(typeof(ITfComponentScope<>)),
				};
			}
			else if (type.ImplementsGenericInterface(typeof(ITfRegionComponent<>), typeof(TfSpaceNodeViewComponentContext)))
			{
				var instance = (ITfRegionComponent<TfSpaceNodeViewComponentContext>)Activator.CreateInstance(type);
				meta = new TfRegionComponentMeta
				{
					Id = instance.Id,
					PositionRank = instance.PositionRank,
					Name = instance.Name,
					Description = instance.Description,
					FluentIconName = instance.FluentIconName,
					ComponentType = type,
					ScopeTypeFullNames = type.GetGenericTypeFromImplementedGenericInterfaceAsStringList(typeof(ITfComponentScope<>)),
				};
			}
			else if (type.ImplementsGenericInterface(typeof(ITfRegionComponent<>), typeof(TfTemplateProcessorHelpComponentContext)))
			{
				var instance = (ITfRegionComponent<TfTemplateProcessorHelpComponentContext>)Activator.CreateInstance(type);
				meta = new TfRegionComponentMeta
				{
					Id = instance.Id,
					PositionRank = instance.PositionRank,
					Name = instance.Name,
					Description = instance.Description,
					FluentIconName = instance.FluentIconName,
					ComponentType = type,
					ScopeTypeFullNames = type.GetGenericTypeFromImplementedGenericInterfaceAsStringList(typeof(ITfComponentScope<>)),
				};
			}
			else if (type.ImplementsGenericInterface(typeof(ITfRegionComponent<>), typeof(TfTemplateProcessorManageSettingsComponentContext)))
			{
				var instance = (ITfRegionComponent<TfTemplateProcessorManageSettingsComponentContext>)Activator.CreateInstance(type);
				meta = new TfRegionComponentMeta
				{
					Id = instance.Id,
					PositionRank = instance.PositionRank,
					Name = instance.Name,
					Description = instance.Description,
					FluentIconName = instance.FluentIconName,
					ComponentType = type,
					ScopeTypeFullNames = type.GetGenericTypeFromImplementedGenericInterfaceAsStringList(typeof(ITfComponentScope<>)),
				};
			}
			else if (type.ImplementsGenericInterface(typeof(ITfRegionComponent<>), typeof(TfTemplateProcessorResultComponentContext)))
			{
				var instance = (ITfRegionComponent<TfTemplateProcessorResultComponentContext>)Activator.CreateInstance(type);
				meta = new TfRegionComponentMeta
				{
					Id = instance.Id,
					PositionRank = instance.PositionRank,
					Name = instance.Name,
					Description = instance.Description,
					FluentIconName = instance.FluentIconName,
					ComponentType = type,
					ScopeTypeFullNames = type.GetGenericTypeFromImplementedGenericInterfaceAsStringList(typeof(ITfComponentScope<>)),
				};
			}
			else if (type.ImplementsGenericInterface(typeof(ITfRegionComponent<>), typeof(TfTemplateProcessorResultPreviewComponentContext)))
			{
				var instance = (ITfRegionComponent<TfTemplateProcessorResultPreviewComponentContext>)Activator.CreateInstance(type);
				meta = new TfRegionComponentMeta
				{
					Id = instance.Id,
					PositionRank = instance.PositionRank,
					Name = instance.Name,
					Description = instance.Description,
					FluentIconName = instance.FluentIconName,
					ComponentType = type,
					ScopeTypeFullNames = type.GetGenericTypeFromImplementedGenericInterfaceAsStringList(typeof(ITfComponentScope<>)),
				};
			}
			else if (type.ImplementsGenericInterface(typeof(ITfRegionComponent<>), typeof(TfTemplateProcessorDisplaySettingsComponentContext)))
			{
				var instance = (ITfRegionComponent<TfTemplateProcessorDisplaySettingsComponentContext>)Activator.CreateInstance(type);
				meta = new TfRegionComponentMeta
				{
					Id = instance.Id,
					PositionRank = instance.PositionRank,
					Name = instance.Name,
					Description = instance.Description,
					FluentIconName = instance.FluentIconName,
					ComponentType = type,
					ScopeTypeFullNames = type.GetGenericTypeFromImplementedGenericInterfaceAsStringList(typeof(ITfComponentScope<>)),
				};
			}
		}
		#endregion
		if (meta is not null)
		{
			_regionComponentMeta.Add(meta);
			_typesMap[type.FullName] = type;
		}
	}
}


