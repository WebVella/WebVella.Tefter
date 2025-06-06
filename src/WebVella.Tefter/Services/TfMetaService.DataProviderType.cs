﻿namespace WebVella.Tefter.Services;

public partial interface ITfMetaService
{
	ITfDataProviderAddon GetDataProviderType(
		Guid id);

	ReadOnlyCollection<ITfDataProviderAddon> GetDataProviderTypes();
}

public partial class TfMetaService : ITfMetaService
{
	public static List<ITfDataProviderAddon> _providerTypes { get; internal set; } = new List<ITfDataProviderAddon>();

	private static void ScanAndRegisterDataProvidersTypes(
		Type type)
	{
		if (type.GetInterfaces().Any(x => x == typeof(ITfDataProviderAddon)))
		{
			var instance = (ITfDataProviderAddon)Activator.CreateInstance(type);

			if(_addonIdHS.Contains(instance.AddonId))
				throw new Exception($"Duplicated Addon Id found: {instance.AddonId}");
			_addonIdHS.Add(instance.AddonId);

			_providerTypes.Add(instance);
			_typesMap[type.FullName] = type;
		}
	}
	public ITfDataProviderAddon GetDataProviderType(
		Guid id)
	{
		return _providerTypes.SingleOrDefault(x => x.AddonId == id);
	}

	public ReadOnlyCollection<ITfDataProviderAddon> GetDataProviderTypes()
	{
		return _providerTypes.AsReadOnly();
	}
}


