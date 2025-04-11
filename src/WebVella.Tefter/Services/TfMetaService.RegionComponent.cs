namespace WebVella.Tefter.Services;

public partial interface ITfMetaService
{
	public ReadOnlyCollection<TfScreenRegionComponentMeta> GetRegionComponentsMeta();
	public ReadOnlyCollection<TfScreenRegionComponentMeta> GetRegionComponentsMeta(Type context = null, TfScreenRegionScope scope = null);
}

public partial class TfMetaService : ITfMetaService
{
	private static readonly List<TfScreenRegionComponentMeta> _regionComponentMeta = new();
	private static readonly Dictionary<Guid, TfScreenRegionComponentMeta> _regionComponentMetaDict = new();

	public ReadOnlyCollection<TfScreenRegionComponentMeta> GetRegionComponentsMeta()
	{
		return _regionComponentMeta
			.OrderBy(x => x.PositionRank)
			.ThenBy(x => x.Name)
			.ToList()
			.AsReadOnly();
	}
	public ReadOnlyCollection<TfScreenRegionComponentMeta> GetRegionComponentsMeta(Type context = null, TfScreenRegionScope scope = null)
	{
		var result = new List<TfScreenRegionComponentMeta>();
		foreach (var comp in _regionComponentMeta)
		{
			var contextMatched = false;
			if (context is null || comp.Type.ImplementsGenericInterface(typeof(ITfRegionComponent<>), context))
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

	public TfScreenRegionComponentMeta GetRegionComponentsMetaById(Guid addonId)
	{
		if (_regionComponentMetaDict.ContainsKey(addonId))
			return _regionComponentMetaDict[addonId];

		return null;
	}

	private static void ScanAndRegisterRegionComponents(Type type)
	{
		TfScreenRegionComponentMeta meta = null;

		#region << Try Get meta >>
		{
			if (type.ImplementsGenericInterface(typeof(ITfRegionComponent<>), typeof(TfAdminPageScreenRegionContext)))
			{
				var instance = (ITfRegionComponent<TfAdminPageScreenRegionContext>)Activator.CreateInstance(type);
				meta = new TfScreenRegionComponentMeta
				{
					Id = instance.Id,
					PositionRank = instance.PositionRank,
					Name = instance.Name,
					Description = instance.Description,
					FluentIconName = instance.FluentIconName,
					Type = type,
					Scopes = instance.Scopes
				};
			}
			else if (type.ImplementsGenericInterface(typeof(ITfRegionComponent<>), typeof(TfDataProviderManageSettingsScreenRegionContext)))
			{
				var instance = (ITfRegionComponent<TfDataProviderManageSettingsScreenRegionContext>)Activator.CreateInstance(type);
				meta = new TfScreenRegionComponentMeta
				{
					Id = instance.Id,
					PositionRank = instance.PositionRank,
					Name = instance.Name,
					Description = instance.Description,
					FluentIconName = instance.FluentIconName,
					Type = type,
					Scopes = instance.Scopes
				};
			}
			else if (type.ImplementsGenericInterface(typeof(ITfRegionComponent<>), typeof(TfDataProviderDisplaySettingsScreenRegionContext)))
			{
				var instance = (ITfRegionComponent<TfDataProviderDisplaySettingsScreenRegionContext>)Activator.CreateInstance(type);
				meta = new TfScreenRegionComponentMeta
				{
					Id = instance.Id,
					PositionRank = instance.PositionRank,
					Name = instance.Name,
					Description = instance.Description,
					FluentIconName = instance.FluentIconName,
					Type = type,
					Scopes = instance.Scopes
				};
			}
			else if (type.ImplementsGenericInterface(typeof(ITfRegionComponent<>), typeof(TfPageScreenRegionContext)))
			{
				var instance = (ITfRegionComponent<TfPageScreenRegionContext>)Activator.CreateInstance(type);
				meta = new TfScreenRegionComponentMeta
				{
					Id = instance.Id,
					PositionRank = instance.PositionRank,
					Name = instance.Name,
					Description = instance.Description,
					FluentIconName = instance.FluentIconName,
					Type = type,
					Scopes = instance.Scopes
				};
			}
			else if (type.ImplementsGenericInterface(typeof(ITfRegionComponent<>), typeof(TfSpaceViewSelectorActionScreenRegionContext)))
			{
				var instance = (ITfRegionComponent<TfSpaceViewSelectorActionScreenRegionContext>)Activator.CreateInstance(type);
				meta = new TfScreenRegionComponentMeta
				{
					Id = instance.Id,
					PositionRank = instance.PositionRank,
					Name = instance.Name,
					Description = instance.Description,
					FluentIconName = instance.FluentIconName,
					Type = type,
					Scopes = instance.Scopes
				};
			}
			else if (type.ImplementsGenericInterface(typeof(ITfRegionComponent<>), typeof(TfSpaceViewToolBarActionScreenRegionContext)))
			{
				var instance = (ITfRegionComponent<TfSpaceViewToolBarActionScreenRegionContext>)Activator.CreateInstance(type);
				meta = new TfScreenRegionComponentMeta
				{
					Id = instance.Id,
					PositionRank = instance.PositionRank,
					Name = instance.Name,
					Description = instance.Description,
					FluentIconName = instance.FluentIconName,
					Type = type,
					Scopes = instance.Scopes
				};
			}
			else if (type.ImplementsGenericInterface(typeof(ITfRegionComponent<>), typeof(TfSpaceViewColumnScreenRegionContext)))
			{
				var instance = (ITfRegionComponent<TfSpaceViewColumnScreenRegionContext>)Activator.CreateInstance(type);
				meta = new TfScreenRegionComponentMeta
				{
					Id = instance.Id,
					PositionRank = instance.PositionRank,
					Name = instance.Name,
					Description = instance.Description,
					FluentIconName = instance.FluentIconName,
					Type = type,
					Scopes = instance.Scopes
				};
			}
			else if (type.ImplementsGenericInterface(typeof(ITfRegionComponent<>), typeof(TfTemplateProcessorHelpScreenRegionContext)))
			{
				var instance = (ITfRegionComponent<TfTemplateProcessorHelpScreenRegionContext>)Activator.CreateInstance(type);
				meta = new TfScreenRegionComponentMeta
				{
					Id = instance.Id,
					PositionRank = instance.PositionRank,
					Name = instance.Name,
					Description = instance.Description,
					FluentIconName = instance.FluentIconName,
					Type = type,
					Scopes = instance.Scopes
				};
			}
			else if (type.ImplementsGenericInterface(typeof(ITfRegionComponent<>), typeof(TfTemplateProcessorManageSettingsScreenRegionContext)))
			{
				var instance = (ITfRegionComponent<TfTemplateProcessorManageSettingsScreenRegionContext>)Activator.CreateInstance(type);
				meta = new TfScreenRegionComponentMeta
				{
					Id = instance.Id,
					PositionRank = instance.PositionRank,
					Name = instance.Name,
					Description = instance.Description,
					FluentIconName = instance.FluentIconName,
					Type = type,
					Scopes = instance.Scopes
				};
			}
			else if (type.ImplementsGenericInterface(typeof(ITfRegionComponent<>), typeof(TfTemplateProcessorResultScreenRegionContext)))
			{
				var instance = (ITfRegionComponent<TfTemplateProcessorResultScreenRegionContext>)Activator.CreateInstance(type);
				meta = new TfScreenRegionComponentMeta
				{
					Id = instance.Id,
					PositionRank = instance.PositionRank,
					Name = instance.Name,
					Description = instance.Description,
					FluentIconName = instance.FluentIconName,
					Type = type,
					Scopes = instance.Scopes
				};
			}
			else if (type.ImplementsGenericInterface(typeof(ITfRegionComponent<>), typeof(TfTemplateProcessorResultPreviewScreenRegionContext)))
			{
				var instance = (ITfRegionComponent<TfTemplateProcessorResultPreviewScreenRegionContext>)Activator.CreateInstance(type);
				meta = new TfScreenRegionComponentMeta
				{
					Id = instance.Id,
					PositionRank = instance.PositionRank,
					Name = instance.Name,
					Description = instance.Description,
					FluentIconName = instance.FluentIconName,
					Type = type,
					Scopes = instance.Scopes
				};
			}
			else if (type.ImplementsGenericInterface(typeof(ITfRegionComponent<>), typeof(TfTemplateProcessorDisplaySettingsScreenRegionContext)))
			{
				var instance = (ITfRegionComponent<TfTemplateProcessorDisplaySettingsScreenRegionContext>)Activator.CreateInstance(type);
				meta = new TfScreenRegionComponentMeta
				{
					Id = instance.Id,
					PositionRank = instance.PositionRank,
					Name = instance.Name,
					Description = instance.Description,
					FluentIconName = instance.FluentIconName,
					Type = type,
					Scopes = instance.Scopes
				};
			}
		}
		#endregion
		if (meta is not null)
		{
			_regionComponentMeta.Add(meta);
			_regionComponentMetaDict[meta.Id] = meta;
			_typesMap[type.FullName] = type;
		}
	}

	private static bool ScopesMatch(TfScreenRegionScope requestedScope, TfScreenRegionScope compScope)
	{
		if (requestedScope is null || compScope is null) return true;
		if (requestedScope.ComponentId is null && requestedScope.ItemType is null) return true;
		if (compScope.ComponentId is null && compScope.ItemType is null) return true;
		var compMatched = false;
		var itemTypeMatched = false;
		if (requestedScope.ComponentId is null || compScope.ComponentId is null || requestedScope.ComponentId == compScope.ComponentId)
			compMatched = true;

		if (requestedScope.ItemType is null || compScope.ItemType is null || requestedScope.ItemType.FullName == compScope.ItemType.FullName)
			itemTypeMatched = true;

		return compMatched && itemTypeMatched;
	}
}


