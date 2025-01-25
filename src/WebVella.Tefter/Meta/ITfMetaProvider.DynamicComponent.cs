namespace WebVella.Tefter;

public partial interface ITfMetaProvider
{
	public ReadOnlyCollection<TfDynamicComponentMeta> GetDynamicComponentsMeta();
	public ReadOnlyCollection<TfDynamicComponentMeta> GetDynamicComponentsMeta(Type context = null, Type scope = null);
}

public partial class TfMetaProvider
{
	private static readonly List<TfDynamicComponentMeta> _dynamicComponentMeta =
		new List<TfDynamicComponentMeta>();

	public ReadOnlyCollection<TfDynamicComponentMeta> GetDynamicComponentsMeta()
	{
		return _dynamicComponentMeta
			.OrderBy(x => x.PositionRank)
			.ThenBy(x => x.Name)
			.ToList()
			.AsReadOnly();
	}
	public ReadOnlyCollection<TfDynamicComponentMeta> GetDynamicComponentsMeta(Type context = null, Type scope = null)
	{
		return _dynamicComponentMeta
			.Where(x =>
				(scope is null || x.ScopeTypeFullNames.Contains(scope.FullName))
				&& (context is null || x.ComponentType.ImplementsGenericInterface(typeof(ITfDynamicComponent<>),context))
				)
			.OrderBy(x => x.PositionRank)
			.ThenBy(x => x.Name)
			.ToList()
			.AsReadOnly();
	}

	private static void ScanAndRegisterDynamicComponents(Type type)
	{
		TfDynamicComponentMeta meta = null;

		#region << Try Get meta >>
		{
			if (type.ImplementsGenericInterface(typeof(ITfDynamicComponent<>), typeof(TfDataProviderManageSettingsComponentContext)))
			{
				var instance = (ITfDynamicComponent<TfDataProviderManageSettingsComponentContext>)Activator.CreateInstance(type);
				meta = new TfDynamicComponentMeta
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
			else if (type.ImplementsGenericInterface(typeof(ITfDynamicComponent<>), typeof(TfDataProviderViewSettingsComponentContext)))
			{
				var instance = (ITfDynamicComponent<TfDataProviderViewSettingsComponentContext>)Activator.CreateInstance(type);
				meta = new TfDynamicComponentMeta
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
			else if (type.ImplementsGenericInterface(typeof(ITfDynamicComponent<>), typeof(TfSpaceNodeManageComponentContext)))
			{
				var instance = (ITfDynamicComponent<TfSpaceNodeManageComponentContext>)Activator.CreateInstance(type);
				meta = new TfDynamicComponentMeta
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
			else if (type.ImplementsGenericInterface(typeof(ITfDynamicComponent<>), typeof(TfSpaceNodeViewComponentContext)))
			{
				var instance = (ITfDynamicComponent<TfSpaceNodeViewComponentContext>)Activator.CreateInstance(type);
				meta = new TfDynamicComponentMeta
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
			else if (type.ImplementsGenericInterface(typeof(ITfDynamicComponent<>), typeof(TfTemplateProcessorHelpComponentContext)))
			{
				var instance = (ITfDynamicComponent<TfTemplateProcessorHelpComponentContext>)Activator.CreateInstance(type);
				meta = new TfDynamicComponentMeta
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
			else if (type.ImplementsGenericInterface(typeof(ITfDynamicComponent<>), typeof(TfTemplateProcessorManageSettingsComponentContext)))
			{
				var instance = (ITfDynamicComponent<TfTemplateProcessorManageSettingsComponentContext>)Activator.CreateInstance(type);
				meta = new TfDynamicComponentMeta
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
			else if (type.ImplementsGenericInterface(typeof(ITfDynamicComponent<>), typeof(TfTemplateProcessorResultComponentContext)))
			{
				var instance = (ITfDynamicComponent<TfTemplateProcessorResultComponentContext>)Activator.CreateInstance(type);
				meta = new TfDynamicComponentMeta
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
			else if (type.ImplementsGenericInterface(typeof(ITfDynamicComponent<>), typeof(TfTemplateProcessorResultPreviewComponentContext)))
			{
				var instance = (ITfDynamicComponent<TfTemplateProcessorResultPreviewComponentContext>)Activator.CreateInstance(type);
				meta = new TfDynamicComponentMeta
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
			else if (type.ImplementsGenericInterface(typeof(ITfDynamicComponent<>), typeof(TfTemplateProcessorViewSettingsComponentContext)))
			{
				var instance = (ITfDynamicComponent<TfTemplateProcessorViewSettingsComponentContext>)Activator.CreateInstance(type);
				meta = new TfDynamicComponentMeta
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
			_dynamicComponentMeta.Add(meta);
			_typesMap[type.FullName] = type;
		}
	}
}


