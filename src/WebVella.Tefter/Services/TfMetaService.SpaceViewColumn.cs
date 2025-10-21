using System.Runtime.CompilerServices;

namespace WebVella.Tefter.Services;
public partial interface ITfMetaService
{
	ReadOnlyCollection<ITfSpaceViewColumnTypeAddon> GetSpaceViewColumnTypesMeta();
	ReadOnlyDictionary<Guid,ITfSpaceViewColumnTypeAddon> GetSpaceViewColumnTypeDictionary();
	ITfSpaceViewColumnTypeAddon? GetSpaceViewColumnType(Guid addonId);
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


