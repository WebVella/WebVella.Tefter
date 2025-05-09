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
			if (context is null || comp.Type.ImplementsGenericInterface(typeof(ITfScreenRegionComponent<>), context))
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
			if (type.ImplementsGenericInterface(typeof(ITfScreenRegionComponent<>), typeof(TfAdminPageScreenRegionContext)))
			{
				var instance = (ITfScreenRegionComponent<TfAdminPageScreenRegionContext>)Activator.CreateInstance(type);
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
			else if (type.ImplementsGenericInterface(typeof(ITfScreenRegionComponent<>), typeof(TfDataProviderManageSettingsScreenRegionContext)))
			{
				var instance = (ITfScreenRegionComponent<TfDataProviderManageSettingsScreenRegionContext>)Activator.CreateInstance(type);
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
			else if (type.ImplementsGenericInterface(typeof(ITfScreenRegionComponent<>), typeof(TfDataProviderDisplaySettingsScreenRegionContext)))
			{
				var instance = (ITfScreenRegionComponent<TfDataProviderDisplaySettingsScreenRegionContext>)Activator.CreateInstance(type);
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
			else if (type.ImplementsGenericInterface(typeof(ITfScreenRegionComponent<>), typeof(TfPageScreenRegionContext)))
			{
				var instance = (ITfScreenRegionComponent<TfPageScreenRegionContext>)Activator.CreateInstance(type);
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
			else if (type.ImplementsGenericInterface(typeof(ITfScreenRegionComponent<>), typeof(TfSpaceViewSelectorActionScreenRegionContext)))
			{
				var instance = (ITfScreenRegionComponent<TfSpaceViewSelectorActionScreenRegionContext>)Activator.CreateInstance(type);
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
			else if (type.ImplementsGenericInterface(typeof(ITfScreenRegionComponent<>), typeof(TfSpaceViewToolBarActionScreenRegionContext)))
			{
				var instance = (ITfScreenRegionComponent<TfSpaceViewToolBarActionScreenRegionContext>)Activator.CreateInstance(type);
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
			else if (type.ImplementsGenericInterface(typeof(ITfScreenRegionComponent<>), typeof(TfTemplateProcessorHelpScreenRegionContext)))
			{
				var instance = (ITfScreenRegionComponent<TfTemplateProcessorHelpScreenRegionContext>)Activator.CreateInstance(type);
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
			else if (type.ImplementsGenericInterface(typeof(ITfScreenRegionComponent<>), typeof(TfTemplateProcessorManageSettingsScreenRegionContext)))
			{
				var instance = (ITfScreenRegionComponent<TfTemplateProcessorManageSettingsScreenRegionContext>)Activator.CreateInstance(type);
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
			else if (type.ImplementsGenericInterface(typeof(ITfScreenRegionComponent<>), typeof(TfTemplateProcessorResultScreenRegionContext)))
			{
				var instance = (ITfScreenRegionComponent<TfTemplateProcessorResultScreenRegionContext>)Activator.CreateInstance(type);
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
			else if (type.ImplementsGenericInterface(typeof(ITfScreenRegionComponent<>), typeof(TfTemplateProcessorResultPreviewScreenRegionContext)))
			{
				var instance = (ITfScreenRegionComponent<TfTemplateProcessorResultPreviewScreenRegionContext>)Activator.CreateInstance(type);
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
			else if (type.ImplementsGenericInterface(typeof(ITfScreenRegionComponent<>), typeof(TfTemplateProcessorDisplaySettingsScreenRegionContext)))
			{
				var instance = (ITfScreenRegionComponent<TfTemplateProcessorDisplaySettingsScreenRegionContext>)Activator.CreateInstance(type);
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


