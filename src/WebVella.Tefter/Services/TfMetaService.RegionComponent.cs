namespace WebVella.Tefter.Services;

public partial interface ITfMetaService
{
	public List<TfScreenRegionComponentMeta> GetRegionComponentsMeta(Type? context = null, TfScreenRegionScope? scope = null);
	public List<TfScreenRegionComponentMeta> GetAdminAddonPages(string? search = null);
	public List<TfScreenRegionComponentMeta> GetHomeAddonPages(string? search = null);
}

public partial class TfMetaService : ITfMetaService
{
	private static readonly List<TfScreenRegionComponentMeta> _regionComponentMeta = new();
	private static readonly Dictionary<Guid, TfScreenRegionComponentMeta> _regionComponentMetaDict = new();


	public List<TfScreenRegionComponentMeta> GetRegionComponentsMeta(Type? context = null, TfScreenRegionScope? scope = null)
	{
		var result = new List<TfScreenRegionComponentMeta>();
		foreach (var compMeta in _regionComponentMeta)
		{
			var contextMatched = false;
			if (context is null || compMeta.Type.ImplementsGenericInterface(typeof(ITfScreenRegionAddon<>), context))
				contextMatched = true;
			if (!contextMatched) continue;

			var scopeMatched = false;
			if (scope is null || compMeta.Scopes is null || compMeta.Scopes.Count == 0)
			{
				scopeMatched = true;
			}
			else
			{
				foreach (var compScope in compMeta.Scopes)
				{
					if (ScopesMatch(scope, compScope))
					{
						scopeMatched = true;
						break;
					}
				}
			}

			if (!scopeMatched) continue;
			result.Add(compMeta);
		}

		return result
			.OrderBy(x => x.PositionRank)
			.ThenBy(x => x.Name)
			.ToList();
	}

	public List<TfScreenRegionComponentMeta> GetAdminAddonPages(string? search = null)
	{
		var addonPages = GetRegionComponentsMeta(
			context: typeof(TfAdminPageScreenRegion),
			scope: null
		);
		if (String.IsNullOrWhiteSpace(search))
		{
			return addonPages;
		}

		search = search.Trim().ToLowerInvariant();
		return addonPages.Where(x=> x.Name.ToLowerInvariant().Contains(search)).ToList();
	}
	
	public List<TfScreenRegionComponentMeta> GetHomeAddonPages(string? search = null)
	{
		var addonPages = GetRegionComponentsMeta(
			context: typeof(TfPageScreenRegion),
			scope: null
		);
		if (String.IsNullOrWhiteSpace(search))
		{
			return addonPages;
		}

		search = search.Trim().ToLowerInvariant();
		return addonPages.Where(x=> x.Name.ToLowerInvariant().Contains(search)).ToList();
	}		
	
	private static void ScanAndRegisterRegionComponents(Type type)
	{
		TfScreenRegionComponentMeta meta = null;

		#region << Try Get meta >>
		{
			if (type.ImplementsGenericInterface(typeof(ITfScreenRegionAddon<>), typeof(TfAdminPageScreenRegion)))
			{
				var instance = (ITfScreenRegionAddon<TfAdminPageScreenRegion>)Activator.CreateInstance(type);
				if (_addonIdHS.Contains(instance.AddonId))
					throw new Exception($"Duplicated Addon Id found: {instance.AddonId}");
				_addonIdHS.Add(instance.AddonId);

				meta = new TfScreenRegionComponentMeta
				{
					Id = instance.AddonId,
					PositionRank = instance.PositionRank,
					Name = instance.AddonName,
					Description = instance.AddonDescription,
					FluentIconName = instance.AddonFluentIconName,
					Type = type,
					Scopes = instance.Scopes
				};
			}
			else if (type.ImplementsGenericInterface(typeof(ITfScreenRegionAddon<>), typeof(TfDataProviderManageSettingsScreenRegion)))
			{
				var instance = (ITfScreenRegionAddon<TfDataProviderManageSettingsScreenRegion>)Activator.CreateInstance(type);
				if (_addonIdHS.Contains(instance.AddonId))
					throw new Exception($"Duplicated Addon Id found: {instance.AddonId}");
				_addonIdHS.Add(instance.AddonId);
				meta = new TfScreenRegionComponentMeta
				{
					Id = instance.AddonId,
					PositionRank = instance.PositionRank,
					Name = instance.AddonName,
					Description = instance.AddonDescription,
					FluentIconName = instance.AddonFluentIconName,
					Type = type,
					Scopes = instance.Scopes
				};
			}
			else if (type.ImplementsGenericInterface(typeof(ITfScreenRegionAddon<>), typeof(TfDataProviderDisplaySettingsScreenRegion)))
			{
				var instance = (ITfScreenRegionAddon<TfDataProviderDisplaySettingsScreenRegion>)Activator.CreateInstance(type);
				if (_addonIdHS.Contains(instance.AddonId))
					throw new Exception($"Duplicated Addon Id found: {instance.AddonId}");
				_addonIdHS.Add(instance.AddonId);
				meta = new TfScreenRegionComponentMeta
				{
					Id = instance.AddonId,
					PositionRank = instance.PositionRank,
					Name = instance.AddonName,
					Description = instance.AddonDescription,
					FluentIconName = instance.AddonFluentIconName,
					Type = type,
					Scopes = instance.Scopes
				};
			}
			else if (type.ImplementsGenericInterface(typeof(ITfScreenRegionAddon<>), typeof(TfPageScreenRegion)))
			{
				var instance = (ITfScreenRegionAddon<TfPageScreenRegion>)Activator.CreateInstance(type);
				if (_addonIdHS.Contains(instance.AddonId))
					throw new Exception($"Duplicated Addon Id found: {instance.AddonId}");
				_addonIdHS.Add(instance.AddonId);
				meta = new TfScreenRegionComponentMeta
				{
					Id = instance.AddonId,
					PositionRank = instance.PositionRank,
					Name = instance.AddonName,
					Description = instance.AddonDescription,
					FluentIconName = instance.AddonFluentIconName,
					Type = type,
					Scopes = instance.Scopes
				};
			}
			else if (type.ImplementsGenericInterface(typeof(ITfScreenRegionAddon<>), typeof(TfSpaceViewSelectorActionScreenRegion)))
			{
				var instance = (ITfScreenRegionAddon<TfSpaceViewSelectorActionScreenRegion>)Activator.CreateInstance(type);
				if (_addonIdHS.Contains(instance.AddonId))
					throw new Exception($"Duplicated Addon Id found: {instance.AddonId}");
				_addonIdHS.Add(instance.AddonId);
				meta = new TfScreenRegionComponentMeta
				{
					Id = instance.AddonId,
					PositionRank = instance.PositionRank,
					Name = instance.AddonName,
					Description = instance.AddonDescription,
					FluentIconName = instance.AddonFluentIconName,
					Type = type,
					Scopes = instance.Scopes
				};
			}
			else if (type.ImplementsGenericInterface(typeof(ITfScreenRegionAddon<>), typeof(TfSpaceViewToolBarActionScreenRegion)))
			{
				var instance = (ITfScreenRegionAddon<TfSpaceViewToolBarActionScreenRegion>)Activator.CreateInstance(type);
				if (_addonIdHS.Contains(instance.AddonId))
					throw new Exception($"Duplicated Addon Id found: {instance.AddonId}");
				_addonIdHS.Add(instance.AddonId);
				meta = new TfScreenRegionComponentMeta
				{
					Id = instance.AddonId,
					PositionRank = instance.PositionRank,
					Name = instance.AddonName,
					Description = instance.AddonDescription,
					FluentIconName = instance.AddonFluentIconName,
					Type = type,
					Scopes = instance.Scopes
				};
			}
			else if (type.ImplementsGenericInterface(typeof(ITfScreenRegionAddon<>), typeof(TfTemplateProcessorHelpScreenRegion)))
			{
				var instance = (ITfScreenRegionAddon<TfTemplateProcessorHelpScreenRegion>)Activator.CreateInstance(type);
				if (_addonIdHS.Contains(instance.AddonId))
					throw new Exception($"Duplicated Addon Id found: {instance.AddonId}");
				_addonIdHS.Add(instance.AddonId);
				meta = new TfScreenRegionComponentMeta
				{
					Id = instance.AddonId,
					PositionRank = instance.PositionRank,
					Name = instance.AddonName,
					Description = instance.AddonDescription,
					FluentIconName = instance.AddonFluentIconName,
					Type = type,
					Scopes = instance.Scopes
				};
			}
			else if (type.ImplementsGenericInterface(typeof(ITfScreenRegionAddon<>), typeof(TfTemplateProcessorManageSettingsScreenRegion)))
			{
				var instance = (ITfScreenRegionAddon<TfTemplateProcessorManageSettingsScreenRegion>)Activator.CreateInstance(type);
				if (_addonIdHS.Contains(instance.AddonId))
					throw new Exception($"Duplicated Addon Id found: {instance.AddonId}");
				_addonIdHS.Add(instance.AddonId);
				meta = new TfScreenRegionComponentMeta
				{
					Id = instance.AddonId,
					PositionRank = instance.PositionRank,
					Name = instance.AddonName,
					Description = instance.AddonDescription,
					FluentIconName = instance.AddonFluentIconName,
					Type = type,
					Scopes = instance.Scopes
				};
			}
			else if (type.ImplementsGenericInterface(typeof(ITfScreenRegionAddon<>), typeof(TfTemplateProcessorResultScreenRegion)))
			{
				var instance = (ITfScreenRegionAddon<TfTemplateProcessorResultScreenRegion>)Activator.CreateInstance(type);
				if (_addonIdHS.Contains(instance.AddonId))
					throw new Exception($"Duplicated Addon Id found: {instance.AddonId}");
				_addonIdHS.Add(instance.AddonId);
				meta = new TfScreenRegionComponentMeta
				{
					Id = instance.AddonId,
					PositionRank = instance.PositionRank,
					Name = instance.AddonName,
					Description = instance.AddonDescription,
					FluentIconName = instance.AddonFluentIconName,
					Type = type,
					Scopes = instance.Scopes
				};
			}
			else if (type.ImplementsGenericInterface(typeof(ITfScreenRegionAddon<>), typeof(TfTemplateProcessorResultPreviewScreenRegion)))
			{
				var instance = (ITfScreenRegionAddon<TfTemplateProcessorResultPreviewScreenRegion>)Activator.CreateInstance(type);
				if (_addonIdHS.Contains(instance.AddonId))
					throw new Exception($"Duplicated Addon Id found: {instance.AddonId}");
				_addonIdHS.Add(instance.AddonId);
				meta = new TfScreenRegionComponentMeta
				{
					Id = instance.AddonId,
					PositionRank = instance.PositionRank,
					Name = instance.AddonName,
					Description = instance.AddonDescription,
					FluentIconName = instance.AddonFluentIconName,
					Type = type,
					Scopes = instance.Scopes
				};
			}
			else if (type.ImplementsGenericInterface(typeof(ITfScreenRegionAddon<>), typeof(TfTemplateProcessorDisplaySettingsScreenRegion)))
			{
				var instance = (ITfScreenRegionAddon<TfTemplateProcessorDisplaySettingsScreenRegion>)Activator.CreateInstance(type);
				if (_addonIdHS.Contains(instance.AddonId))
					throw new Exception($"Duplicated Addon Id found: {instance.AddonId}");
				_addonIdHS.Add(instance.AddonId);
				meta = new TfScreenRegionComponentMeta
				{
					Id = instance.AddonId,
					PositionRank = instance.PositionRank,
					Name = instance.AddonName,
					Description = instance.AddonDescription,
					FluentIconName = instance.AddonFluentIconName,
					Type = type,
					Scopes = instance.Scopes
				};
			}
			else if (type.ImplementsGenericInterface(typeof(ITfScreenRegionAddon<>), typeof(TfGlobalStateScreenRegion)))
			{
				var instance = (ITfScreenRegionAddon<TfGlobalStateScreenRegion>)Activator.CreateInstance(type);
				if (_addonIdHS.Contains(instance.AddonId))
					throw new Exception($"Duplicated Addon Id found: {instance.AddonId}");
				_addonIdHS.Add(instance.AddonId);
				meta = new TfScreenRegionComponentMeta
				{
					Id = instance.AddonId,
					PositionRank = instance.PositionRank,
					Name = instance.AddonName,
					Description = instance.AddonDescription,
					FluentIconName = instance.AddonFluentIconName,
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


