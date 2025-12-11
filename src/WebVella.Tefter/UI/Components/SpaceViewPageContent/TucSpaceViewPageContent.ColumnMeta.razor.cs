namespace WebVella.Tefter.UI.Components;

public partial class TucSpaceViewPageContent
{
	private Dictionary<Guid, TfSpaceViewRowPresentationMeta> _rowMeta = new();
	private Dictionary<Guid, TfSpaceViewColumnPresentationMeta> _columnsMeta = new();
	private Dictionary<Guid, Dictionary<Guid, TfSpaceViewColumnBase>> _regionContextDict = new();
	private Dictionary<string, string> _queryNameToColumnNameDict = new();
	private Dictionary<Guid, Dictionary<string, Tuple<TfColor?, TfColor?>>> _rowColoringCacheDictionary = new();

	private void _generateMeta()
	{
		var meta = new SpaceViewMetaUtility().GenerateMeta(
			serviceProvider:ServiceProvider,
			tfService:TfService,
			currentUser:_currentUser,
			navState:_navState,
			data:_data,
			spaceView:_spaceView,
			preset:_preset,
			spaceViewColumns:_spaceViewColumns,
			selectedDataRows:_selectedDataRows,
			editAll:_editAll,
			editedDataRows:_editedDataRows,
			onRowChanged: EventCallback.Factory.Create<TfSpaceViewColumnDataChange>(this, _onRowChanged)
			);

		_rowMeta = meta.RowMeta;
		_columnsMeta = meta.ColumnsMeta;
		_regionContextDict = meta.RegionContextDict;
		_queryNameToColumnNameDict = meta.QueryNameToColumnNameDict;
		_rowColoringCacheDictionary = meta.RowColoringCacheDictionary;

	}
	private string? _getSafeColumnMetaString(Guid columnId, string propName)
	{
		if (!_columnsMeta.ContainsKey(columnId)) return String.Empty;
		object value = _columnsMeta[columnId].GetPropertyByName(propName) ?? String.Empty;
		return value.ToString();
	}
	
	private string? _getSafeRegionContextString(Guid rowId, Guid columnId, string propName)
	{
		if (!_regionContextDict.ContainsKey(rowId)) return String.Empty;
		if (!_regionContextDict[rowId].ContainsKey(columnId)) return String.Empty;
		object value = _regionContextDict[rowId][columnId].GetPropertyByName(propName) ?? String.Empty;
		return value.ToString();
	}	
	
}