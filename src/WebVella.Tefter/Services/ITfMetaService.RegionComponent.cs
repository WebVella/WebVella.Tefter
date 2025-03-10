namespace WebVella.Tefter.Services;

public partial interface ITfMetaService
{
	public ReadOnlyCollection<TfRegionComponentMeta> GetRegionComponentsMeta();
	public ReadOnlyCollection<TfRegionComponentMeta> GetRegionComponentsMeta(Type context = null, TfRegionComponentScope scope = null);
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
	public ReadOnlyCollection<TfRegionComponentMeta> GetRegionComponentsMeta(Type context = null, TfRegionComponentScope scope = null)
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
			if (type.ImplementsGenericInterface(typeof(ITfRegionComponent<>), typeof(TfAdminPageComponentContext)))
			{
				var instance = (ITfRegionComponent<TfAdminPageComponentContext>)Activator.CreateInstance(type);
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
			else if (type.ImplementsGenericInterface(typeof(ITfRegionComponent<>), typeof(TfDataProviderManageSettingsComponentContext)))
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
					Scopes = instance.Scopes
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
					Scopes = instance.Scopes
				};
			}
			else if (type.ImplementsGenericInterface(typeof(ITfRegionComponent<>), typeof(TfPageComponentContext)))
			{
				var instance = (ITfRegionComponent<TfPageComponentContext>)Activator.CreateInstance(type);
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
					Scopes = instance.Scopes
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
					Scopes = instance.Scopes
				};
			}
			else if (type.ImplementsGenericInterface(typeof(ITfRegionComponent<>), typeof(TfSpaceViewSelectorActionComponentContext)))
			{
				var instance = (ITfRegionComponent<TfSpaceViewSelectorActionComponentContext>)Activator.CreateInstance(type);
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
			else if (type.ImplementsGenericInterface(typeof(ITfRegionComponent<>), typeof(TfSpaceViewToolBarActionComponentContext)))
			{
				var instance = (ITfRegionComponent<TfSpaceViewToolBarActionComponentContext>)Activator.CreateInstance(type);
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
			else if (type.ImplementsGenericInterface(typeof(ITfRegionComponent<>), typeof(TfSpaceViewToolBarActionComponentContext)))
			{
				var instance = (ITfRegionComponent<TfSpaceViewToolBarActionComponentContext>)Activator.CreateInstance(type);
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
					Scopes = instance.Scopes
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
					Scopes = instance.Scopes
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

	private static bool ScopesMatch(TfRegionComponentScope requestedScope, TfRegionComponentScope compScope)
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


