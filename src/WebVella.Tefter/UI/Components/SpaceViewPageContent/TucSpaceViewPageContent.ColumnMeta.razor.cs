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
		_generateColumnsMeta();
		_generateRowsMeta();
		//Regenerate row RenderingCache
		_rowColoringCacheDictionary = new();
	}

	private void _generateRowsMeta()
	{
		_rowMeta = new();
		_regionContextDict = new();
		if (_data is null) return;

		_queryNameToColumnNameDict = new Dictionary<string, string>();
		foreach (var column in _spaceViewColumns)
		{
			if (column.DataMapping.Keys.Count > 0)
			{
				var firstMappedValue = column.DataMapping[column.DataMapping.Keys.First()];
				if (String.IsNullOrWhiteSpace(firstMappedValue)) continue;
				var dataColumn = _data.Columns[firstMappedValue];
				if (dataColumn is null) continue;
				_queryNameToColumnNameDict[column.QueryName] = dataColumn.Name;
			}
		}


		foreach (TfDataRow row in _data.Rows)
		{
			var tfId = (Guid)row[TfConstants.TEFTER_ITEM_ID_PROP_NAME]!;
			_rowMeta[tfId] = new();
			_regionContextDict[tfId] = new();

			#region << Selected >>

			_rowMeta[tfId].Selected = _selectedDataRows is not null && _selectedDataRows.Contains(tfId);

			#endregion

			#region << EditMode >>

			_rowMeta[tfId].EditMode = _editAll || (_editedDataRows is not null && _editedDataRows.Contains(tfId));

			#endregion

			#region << Coloring rules >>

			//For optimization purposes it is generated adhoc for each row and filled in a cache dict for this component instance

			#endregion

			#region << Column Context >>

			foreach (TfSpaceViewColumn column in _spaceViewColumns)
			{
				if (_rowMeta[tfId].EditMode)
				{
					_regionContextDict[tfId][column.Id] = new TfSpaceViewColumnEditMode(_contextViewData)
					{
						TfService = TfService,
						ServiceProvider = ServiceProvider,
						ViewColumn = column,
						DataTable = _data,
						RowId = tfId,
						DataChanged = EventCallback.Factory.Create<TfSpaceViewColumnDataChange>(this, _onRowChanged)
					};
				}
				else
				{
					_regionContextDict[tfId][column.Id] = new TfSpaceViewColumnReadMode(_contextViewData)
					{
						TfService = TfService,
						ServiceProvider = ServiceProvider,
						ViewColumn = column,
						DataTable = _data,
						RowId = tfId,
						ForegroundColor = null,
						BackgroundColor = null,
					};
				}
			}

			#endregion
		}
	}

	private void _generateColumnsMeta()
	{
		_columnsMeta = new();
		if (_spaceView is null) return;

		Dictionary<Guid, TfViewPresetColumnPersonalization> userPersonalizationDict;
		List<Guid> freezeLeftColumnIdList = new();
		List<Guid> freezeRightColumnIdList = new();
		short freezeLeftWidth = 0;
		short freezeRightWidth = 0;

		#region << Init Freeze >>

		if (_spaceView.Settings.FreezeStartingNColumns is not null
		    && _spaceView.Settings.FreezeStartingNColumns.Value > 0)
		{
			var freezeFirstColumnsCount =
				Math.Min(_spaceView.Settings.FreezeStartingNColumns.Value, _spaceViewColumns.Count);

			foreach (var col in _spaceViewColumns.Take(freezeFirstColumnsCount))
			{
				freezeLeftColumnIdList.Add(col.Id);
			}
		}

		if (_spaceView.Settings.FreezeFinalNColumns is not null
		    && _spaceView.Settings.FreezeFinalNColumns.Value > 0)
		{
			var freezeFinalColumnsCount =
				Math.Min(_spaceView.Settings.FreezeFinalNColumns.Value, _spaceViewColumns.Count);

			foreach (var col in _spaceViewColumns.Skip(_spaceViewColumns.Count - freezeFinalColumnsCount))
			{
				freezeRightColumnIdList.Add(col.Id);
			}
		}

		#endregion

		#region << Init User Personalizations>

		{
			userPersonalizationDict = _currentUser.Settings.ViewPresetColumnPersonalizations.Where(x =>
				x.SpaceViewId == _spaceView.Id && x.PresetId == _preset?.Id).ToDictionary(x => x.SpaceViewColumnId);
		}

		#endregion

		#region << Checkbox >>

		_columnsMeta[Guid.Empty] = new() { Width = 40, IsCheckbox = true, FreezeLeftWidth = 0 };
		freezeLeftWidth += 40;

		#endregion

		foreach (var column in _spaceViewColumns)
		{
			_columnsMeta[column.Id] = new();

			TfViewPresetColumnPersonalization? userPersonalization =
				userPersonalizationDict.ContainsKey(column.Id) ? userPersonalizationDict[column.Id] : null;

			#region << Width >>

			_columnsMeta[column.Id].Width = userPersonalization is not null
				? (short?)userPersonalization.Width
				: column.Settings.Width;

			#endregion

			#region << Sort >>

			if (_navState.Sorts is not null)
			{
				var columnSort = _navState.Sorts.FirstOrDefault(x => x.Name == column.QueryName);
				if (columnSort != null)
				{
					_columnsMeta[column.Id].SortDirection = (TfSortDirection)columnSort.Direction;
				}
			}

			#endregion
 
			#region << Freeze Left >>

			if (freezeLeftColumnIdList.Contains(column.Id))
			{
				_columnsMeta[column.Id].IsLastFreezeLeft = freezeLeftColumnIdList.Last() == column.Id;
				_columnsMeta[column.Id].Width ??= 140;
				_columnsMeta[column.Id].FreezeLeftWidth = freezeLeftWidth;
				freezeLeftWidth += _columnsMeta[column.Id].Width!.Value;
			}

			#endregion
		}

		foreach (var column in _spaceViewColumns.Reverse<TfSpaceViewColumn>())
		{
			#region << Freeze Right >>

			if (freezeRightColumnIdList.Contains(column.Id))
			{
				_columnsMeta[column.Id].IsFirstFreezeRight = freezeRightColumnIdList.First() == column.Id;
				_columnsMeta[column.Id].Width ??= 140;
				_columnsMeta[column.Id].FreezeRightWidth = freezeRightWidth;
				freezeRightWidth += _columnsMeta[column.Id].Width!.Value;
			}

			#endregion
		}
	}

	private void _generateRowColoringDict(Guid tfId)
	{
		if (_rowMeta[tfId].EditMode)
		{
			//remove all generated coloring from context
			_rowMeta[tfId].ForegroundColor = null;
			_rowMeta[tfId].BackgroundColor = null;
			foreach (TfSpaceViewColumn column in _spaceViewColumns)
			{
				_regionContextDict[tfId][column.Id].ForegroundColor = null;
				_regionContextDict[tfId][column.Id].BackgroundColor = null;
			}

			return;
		}

		Dictionary<string, Tuple<TfColor?, TfColor?>> coloringDict;

		if (_rowColoringCacheDictionary.ContainsKey(tfId))
		{
			coloringDict = _rowColoringCacheDictionary[tfId];
		}
		else
		{
			coloringDict =
				_data!.GenerateColoring(tfId, _spaceView!.Settings.ColoringRules, _queryNameToColumnNameDict);
			
			_rowColoringCacheDictionary[tfId] = coloringDict;
		}
		

		if (coloringDict.ContainsKey(tfId.ToString()))
		{
			_rowMeta[tfId].ForegroundColor = coloringDict[tfId.ToString()].Item1;
			_rowMeta[tfId].BackgroundColor = coloringDict[tfId.ToString()].Item2;
		}

		foreach (TfSpaceViewColumn column in _spaceViewColumns)
		{
			TfColor? color = null;
			TfColor? backgroundColor = null;
			if (coloringDict.ContainsKey(column.QueryName))
			{
				color = coloringDict[column.QueryName].Item1;
				backgroundColor = coloringDict[column.QueryName].Item2;
			}

			if (_regionContextDict[tfId].ContainsKey(column.Id))
			{
				_regionContextDict[tfId][column.Id].ForegroundColor = color;
				_regionContextDict[tfId][column.Id].BackgroundColor = backgroundColor;
			}
		}
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