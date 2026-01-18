using System.Runtime.CompilerServices;

namespace WebVella.Tefter.Services;
public partial interface ITfMetaService
{
	List<ITfSpaceViewColumnTypeAddon> GetSpaceViewColumnTypesMeta();
	Dictionary<Guid,ITfSpaceViewColumnTypeAddon> GetSpaceViewColumnTypeDictionary();
	ITfSpaceViewColumnTypeAddon? GetSpaceViewColumnType(Guid addonId);
}

public partial class TfMetaService : ITfMetaService
{
	private static readonly List<ITfSpaceViewColumnTypeAddon> _columnTypeMetaList = new();
	private static readonly Dictionary<Guid, ITfSpaceViewColumnTypeAddon> _columnTypeMetaDict = new();


	public List<ITfSpaceViewColumnTypeAddon> GetSpaceViewColumnTypesMeta()
	{
		var instances = new List<ITfSpaceViewColumnTypeAddon>();
		foreach (var dictInstance in _columnTypeMetaList)
		{
			var instance = (ITfSpaceViewColumnTypeAddon?)Activator.CreateInstance(dictInstance.GetType());
			if(instance is null) continue;
			instances.Add(instance);
		}

		return instances.OrderBy(x=> x.AddonName).ToList();
	}		

	public Dictionary<Guid,ITfSpaceViewColumnTypeAddon> GetSpaceViewColumnTypeDictionary()
	{
		var newDict = new Dictionary<Guid, ITfSpaceViewColumnTypeAddon>();
		foreach (var addonId in _columnTypeMetaDict.Keys)
		{
			var dictInstance = _columnTypeMetaDict[addonId];
			var instance = (ITfSpaceViewColumnTypeAddon?)Activator.CreateInstance(dictInstance.GetType());
			if(instance is null) continue;
			newDict[addonId] = instance;
		}
		return newDict;
	}

	public ITfSpaceViewColumnTypeAddon? GetSpaceViewColumnType(Guid addonId)
	{
		if (_columnTypeMetaDict.ContainsKey(addonId)) return _columnTypeMetaDict[addonId];
		return null;
	}
	

	//Private

	private static void ScanAndRegisterSpaceViewColumnTypes(
		Type type)
	{
		if (type.ImplementsInterface(typeof(ITfSpaceViewColumnTypeAddon)))
		{
			var instance = (ITfSpaceViewColumnTypeAddon?)Activator.CreateInstance(type);
			if(instance is null) return;
			
			if(_addonIdHS.Contains(instance.AddonId))
				throw new Exception($"Duplicated Addon Id found: {instance.AddonId}");
			_addonIdHS.Add(instance.AddonId);

		
			_columnTypeMetaList.Add(instance);
			_columnTypeMetaDict[instance.AddonId] = instance;
		}
	}

}


