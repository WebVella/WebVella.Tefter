namespace WebVella.Tefter.Services;

public partial interface ITfMetaService
{
	public ReadOnlyCollection<TfRegionComponentMeta> GetRegionComponentsMeta();
	public ReadOnlyCollection<TfRegionComponentMeta> GetRegionComponentsMeta(Type context = null, TfScreenRegionScope scope = null);
}

public partial class TfMetaService : ITfMetaService
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
	public ReadOnlyCollection<TfRegionComponentMeta> GetRegionComponentsMeta(Type context = null, TfScreenRegionScope scope = null)
	{
		var result = new List<TfRegionComponentMeta>();
		foreach (var comp in _regionComponentMeta)
		{
			var contextMatched = false;
			if (context is null || comp.ComponentType.ImplementsGenericInterface(typeof(ITfRegionComponent<>), context))
				contextMatched = true;
			if (!contextMatched) continue;

			var scopeMatched = false;
			if (scope is null || comp.Scopes is null || comp.Scopes.Count == 0)
			{
				scopeMatched = true;
			}
			else
			{
				foreach (var compScope in comp.Scopes)
				{
					if (ScopesMatch(scope, compScope))
					{
						scopeMatched = true;
						break;
					}
				}
			}

			if (!scopeMatched) continue;


			result.Add(comp);
		}

		return result
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
			if (type.ImplementsGenericInterface(typeof(ITfRegionComponent<>), typeof(TfAdminPageScreenRegion)))
			{
				var instance = (ITfRegionComponent<TfAdminPageScreenRegion>)Activator.CreateInstance(type);
				meta = new TfRegionComponentMeta
				{
					Id = instance.Id,
					PositionRank = instance.PositionRank,
					Name = instance.Name,
					Description = instance.Description,
					FluentIconName = instance.FluentIconName,
					ComponentType = type,
					Scopes = instance.Scopes
				};
			}
			else if (type.ImplementsGenericInterface(typeof(ITfRegionComponent<>), typeof(TfDataProviderManageSettingsScreenRegion)))
			{
				var instance = (ITfRegionComponent<TfDataProviderManageSettingsScreenRegion>)Activator.CreateInstance(type);
				meta = new TfRegionComponentMeta
				{
					Id = instance.Id,
					PositionRank = instance.PositionRank,
					Name = instance.Name,
					Description = instance.Description,
					FluentIconName = instance.FluentIconName,
					ComponentType = type,
					Scopes = instance.Scopes
				};
			}
			else if (type.ImplementsGenericInterface(typeof(ITfRegionComponent<>), typeof(TfDataProviderDisplaySettingsScreenRegion)))
			{
				var instance = (ITfRegionComponent<TfDataProviderDisplaySettingsScreenRegion>)Activator.CreateInstance(type);
				meta = new TfRegionComponentMeta
				{
					Id = instance.Id,
					PositionRank = instance.PositionRank,
					Name = instance.Name,
					Description = instance.Description,
					FluentIconName = instance.FluentIconName,
					ComponentType = type,
					Scopes = instance.Scopes
				};
			}
			else if (type.ImplementsGenericInterface(typeof(ITfRegionComponent<>), typeof(TfPageScreenRegion)))
			{
				var instance = (ITfRegionComponent<TfPageScreenRegion>)Activator.CreateInstance(type);
				meta = new TfRegionComponentMeta
				{
					Id = instance.Id,
					PositionRank = instance.PositionRank,
					Name = instance.Name,
					Description = instance.Description,
					FluentIconName = instance.FluentIconName,
					ComponentType = type,
					Scopes = instance.Scopes
				};
			}
			else if (type.ImplementsGenericInterface(typeof(ITfRegionComponent<>), typeof(TfSpaceNodeManageScreenRegion)))
			{
				var instance = (ITfRegionComponent<TfSpaceNodeManageScreenRegion>)Activator.CreateInstance(type);
				meta = new TfRegionComponentMeta
				{
					Id = instance.Id,
					PositionRank = instance.PositionRank,
					Name = instance.Name,
					Description = instance.Description,
					FluentIconName = instance.FluentIconName,
					ComponentType = type,
					Scopes = instance.Scopes
				};
			}
			else if (type.ImplementsGenericInterface(typeof(ITfRegionComponent<>), typeof(TfSpaceNodeViewScreenRegion)))
			{
				var instance = (ITfRegionComponent<TfSpaceNodeViewScreenRegion>)Activator.CreateInstance(type);
				meta = new TfRegionComponentMeta
				{
					Id = instance.Id,
					PositionRank = instance.PositionRank,
					Name = instance.Name,
					Description = instance.Description,
					FluentIconName = instance.FluentIconName,
					ComponentType = type,
					Scopes = instance.Scopes
				};
			}
			else if (type.ImplementsGenericInterface(typeof(ITfRegionComponent<>), typeof(TfSpaceViewSelectorActionScreenRegion)))
			{
				var instance = (ITfRegionComponent<TfSpaceViewSelectorActionScreenRegion>)Activator.CreateInstance(type);
				meta = new TfRegionComponentMeta
				{
					Id = instance.Id,
					PositionRank = instance.PositionRank,
					Name = instance.Name,
					Description = instance.Description,
					FluentIconName = instance.FluentIconName,
					ComponentType = type,
					Scopes = instance.Scopes
				};
			}
			else if (type.ImplementsGenericInterface(typeof(ITfRegionComponent<>), typeof(TfSpaceViewToolBarActionScreenRegion)))
			{
				var instance = (ITfRegionComponent<TfSpaceViewToolBarActionScreenRegion>)Activator.CreateInstance(type);
				meta = new TfRegionComponentMeta
				{
					Id = instance.Id,
					PositionRank = instance.PositionRank,
					Name = instance.Name,
					Description = instance.Description,
					FluentIconName = instance.FluentIconName,
					ComponentType = type,
					Scopes = instance.Scopes
				};
			}
			else if (type.ImplementsGenericInterface(typeof(ITfRegionComponent<>), typeof(TfSpaceViewToolBarActionScreenRegion)))
			{
				var instance = (ITfRegionComponent<TfSpaceViewToolBarActionScreenRegion>)Activator.CreateInstance(type);
				meta = new TfRegionComponentMeta
				{
					Id = instance.Id,
					PositionRank = instance.PositionRank,
					Name = instance.Name,
					Description = instance.Description,
					FluentIconName = instance.FluentIconName,
					ComponentType = type,
					Scopes = instance.Scopes
				};
			}
			else if (type.ImplementsGenericInterface(typeof(ITfRegionComponent<>), typeof(TfTemplateProcessorHelpScreenRegion)))
			{
				var instance = (ITfRegionComponent<TfTemplateProcessorHelpScreenRegion>)Activator.CreateInstance(type);
				meta = new TfRegionComponentMeta
				{
					Id = instance.Id,
					PositionRank = instance.PositionRank,
					Name = instance.Name,
					Description = instance.Description,
					FluentIconName = instance.FluentIconName,
					ComponentType = type,
					Scopes = instance.Scopes
				};
			}
			else if (type.ImplementsGenericInterface(typeof(ITfRegionComponent<>), typeof(TfTemplateProcessorManageSettingsScreenRegion)))
			{
				var instance = (ITfRegionComponent<TfTemplateProcessorManageSettingsScreenRegion>)Activator.CreateInstance(type);
				meta = new TfRegionComponentMeta
				{
					Id = instance.Id,
					PositionRank = instance.PositionRank,
					Name = instance.Name,
					Description = instance.Description,
					FluentIconName = instance.FluentIconName,
					ComponentType = type,
					Scopes = instance.Scopes
				};
			}
			else if (type.ImplementsGenericInterface(typeof(ITfRegionComponent<>), typeof(TfTemplateProcessorResultScreenRegion)))
			{
				var instance = (ITfRegionComponent<TfTemplateProcessorResultScreenRegion>)Activator.CreateInstance(type);
				meta = new TfRegionComponentMeta
				{
					Id = instance.Id,
					PositionRank = instance.PositionRank,
					Name = instance.Name,
					Description = instance.Description,
					FluentIconName = instance.FluentIconName,
					ComponentType = type,
					Scopes = instance.Scopes
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
					Scopes = instance.Scopes
				};
			}
			else if (type.ImplementsGenericInterface(typeof(ITfRegionComponent<>), typeof(TfTemplateProcessorDisplaySettingsScreenRegion)))
			{
				var instance = (ITfRegionComponent<TfTemplateProcessorDisplaySettingsScreenRegion>)Activator.CreateInstance(type);
				meta = new TfRegionComponentMeta
				{
					Id = instance.Id,
					PositionRank = instance.PositionRank,
					Name = instance.Name,
					Description = instance.Description,
					FluentIconName = instance.FluentIconName,
					ComponentType = type,
					Scopes = instance.Scopes
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

	private static bool ScopesMatch(TfScreenRegionScope requestedScope, TfScreenRegionScope compScope)
	{
		if(requestedScope is null || compScope is null) return true;
		if(requestedScope.ComponentId is null && requestedScope.ItemType is null) return true;
		if(compScope.ComponentId is null && compScope.ItemType is null) return true;
		var compMatched = false;
		var itemTypeMatched = false;
		if(requestedScope.ComponentId is null || compScope.ComponentId is null || requestedScope.ComponentId == compScope.ComponentId)
			compMatched = true;

		if(requestedScope.ItemType is null || compScope.ItemType is null || requestedScope.ItemType.FullName == compScope.ItemType.FullName)
			itemTypeMatched = true;

		return compMatched && itemTypeMatched;
	}
}


