using System.Runtime.CompilerServices;

namespace WebVella.Tefter.Services;
public partial interface ITfMetaService
{
	ReadOnlyCollection<ITfSpaceViewColumnTypeAddon> GetSpaceViewColumnTypesMeta();
	ReadOnlyDictionary<Guid,ITfSpaceViewColumnTypeAddon> GetSpaceViewColumnTypeDictionary();
	ITfSpaceViewColumnTypeAddon? GetSpaceViewColumnType(Guid addonId);

	ReadOnlyCollection<ITfSpaceViewColumnTypeAddon> GetCompatibleViewColumnTypesMeta(
		ITfSpaceViewColumnTypeAddon columnMeta);
}

public partial class TfMetaService : ITfMetaService
{
	private static readonly List<ITfSpaceViewColumnTypeAddon> _columnTypeMetaList = new();
	private static readonly Dictionary<Guid, ITfSpaceViewColumnTypeAddon> _columnTypeMetaDict = new();


	public ReadOnlyCollection<ITfSpaceViewColumnTypeAddon> GetSpaceViewColumnTypesMeta()
	{
		return _columnTypeMetaList.OrderBy(x=> x.AddonName).ToList().AsReadOnly();
	}
	public ReadOnlyDictionary<Guid,ITfSpaceViewColumnTypeAddon> GetSpaceViewColumnTypeDictionary()
	{
		return _columnTypeMetaDict.AsReadOnly();
	}

	public ITfSpaceViewColumnTypeAddon? GetSpaceViewColumnType(Guid addonId)
	{
		if (_columnTypeMetaDict.ContainsKey(addonId)) return _columnTypeMetaDict[addonId];
		return null;
	}

	/// <summary>
	/// Gets view column types that can replace the current type. Includes the current type too
	/// </summary>
	/// <param name="columnMeta"></param>
	/// <returns>the column type that is evaluated</returns>
	public ReadOnlyCollection<ITfSpaceViewColumnTypeAddon> GetCompatibleViewColumnTypesMeta(ITfSpaceViewColumnTypeAddon columnMeta)
	{
		var list = new List<ITfSpaceViewColumnTypeAddon>();
		var sourceCompatabilityHash = columnMeta.GetCompatabilityHash();
		foreach (var target in _columnTypeMetaList)
		{
			var targetCompatabilityHash = target.GetCompatabilityHash();
			if (sourceCompatabilityHash.Equals(targetCompatabilityHash))
			{
				list.Add(target);
			}
		}
		
		return list.OrderBy(x=> x.AddonName).ToList().AsReadOnly();
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


