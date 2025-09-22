namespace WebVella.Tefter.UI.Components;

public partial class TucSpaceViewPageContent : TfBaseComponent, IAsyncDisposable
{
	private Dictionary<Guid, TfSpaceViewRowPresentationMeta> _rowMeta = new();
	private Dictionary<Guid, TfSpaceViewColumnPresentationMeta> _columnsMeta = new();
	private Dictionary<Guid, Dictionary<Guid, Dictionary<string, object>>> _rowColumnContext = new();

	private void _generateMeta()
	{
		_generateColumnsMeta();
		_generateRowsMeta();
	}

	private void _generateRowsMeta()
	{
		_rowMeta = new();
		_rowColumnContext = new();
		if (_data is null) return;
		foreach (TfDataRow row in _data.Rows)
		{
			var tfId = (Guid)row[TfConstants.TEFTER_ITEM_ID_PROP_NAME];
			_rowMeta[tfId] = new();
			_rowColumnContext[tfId] = new();

			#region << Selected >>
			_rowMeta[tfId].Selected = _selectedDataRows is not null && _selectedDataRows.Contains(tfId);
			#endregion

			#region << EditMode >>
			_rowMeta[tfId].EditMode = _editAll || (_editedDataRows is not null && _editedDataRows.Contains(tfId));
			#endregion

			#region << Context >>
			foreach (TfSpaceViewColumn column in _spaceViewColumns)
			{
				_rowColumnContext[tfId][column.Id] = new();
				_rowColumnContext[tfId][column.Id][TfConstants.SPACE_VIEW_COMPONENT_ROW_CHANGED_PROPERTY_NAME] = EventCallback.Factory.Create<TfDataTable>(this, _onRowChanged);
				_rowColumnContext[tfId][column.Id][TfConstants.SPACE_VIEW_COMPONENT_CONTEXT_PROPERTY_NAME] = new TfSpaceViewColumnScreenRegionContext(_contextData)
				{
					Mode = TfComponentPresentationMode.Display,
					ComponentOptionsJson = _rowMeta[tfId].EditMode ? column.EditComponentOptionsJson : column.ComponentOptionsJson,
					DataMapping = column.DataMapping,
					DataTable = _data,
					RowId = tfId,
					QueryName = column.QueryName,
					SpaceViewId = column.SpaceViewId,
					SpaceViewColumnId = column.Id,
				};
			}
			#endregion

		}
	}

	private void _generateColumnsMeta()
	{
		_columnsMeta = new();
		if (_spaceView is null
			|| _spaceViewColumns is null
			|| _currentUser is null) return;

		var userPersonalizationDict = new Dictionary<Guid, TfViewPresetColumnPersonalization>();
		List<Guid> freezeLeftColumnIdList = new();
		List<Guid> freezeRightColumnIdList = new();
		short _freezeLeftWidth = 0;
		short _freezeRightWidth = 0;

		#region << Init Freeze >>
		if (_spaceView.Settings.FreezeStartingNColumns is not null
			&& _spaceView.Settings.FreezeStartingNColumns.Value > 0)
		{
			var freezeFirstColumnsCount = Math.Min(_spaceView.Settings.FreezeStartingNColumns.Value, _spaceViewColumns.Count);

			foreach (var col in _spaceViewColumns.Take(freezeFirstColumnsCount))
			{
				freezeLeftColumnIdList.Add(col.Id);
			}
		}
		if (_spaceView.Settings.FreezeFinalNColumns is not null
			&& _spaceView.Settings.FreezeFinalNColumns.Value > 0)
		{
			var freezeFinalColumnsCount = Math.Min(_spaceView.Settings.FreezeFinalNColumns.Value, _spaceViewColumns.Count);

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
		_columnsMeta[Guid.Empty] = new();
		_columnsMeta[Guid.Empty].Width = 40;
		_columnsMeta[Guid.Empty].IsCheckbox = true;
		_columnsMeta[Guid.Empty].FreezeLeftWidth = 0;
		_freezeLeftWidth += 40;

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
				if (_columnsMeta[column.Id].Width is null) _columnsMeta[column.Id].Width = 140; //fix the width
				_columnsMeta[column.Id].FreezeLeftWidth = _freezeLeftWidth;
				_freezeLeftWidth += _columnsMeta[column.Id].Width!.Value;
			}
			#endregion

			#region << Color >>
			_columnsMeta[column.Id].Color = column.Settings.Color is not null
				? $"var(--tf-td-color-{column.Settings.Color.Value.GetAttribute().Name})"
				: null;
			#endregion

			#region << BackgroundColor >>
			_columnsMeta[column.Id].BackgroundColor = column.Settings.BackgroundColor is not null
				? $"var(--tf-td-fill-{column.Settings.BackgroundColor.Value.GetAttribute().Name})"
				: null;
			#endregion
		}
		foreach (var column in _spaceViewColumns.Reverse<TfSpaceViewColumn>())
		{
			#region << Freeze Right >>
			if (freezeRightColumnIdList.Contains(column.Id))
			{
				_columnsMeta[column.Id].IsFirstFreezeRight = freezeRightColumnIdList.First() == column.Id;
				if (_columnsMeta[column.Id].Width is null) _columnsMeta[column.Id].Width = 140; //fix the width
				_columnsMeta[column.Id].FreezeRightWidth = _freezeRightWidth;
				_freezeRightWidth += _columnsMeta[column.Id].Width!.Value;
			}
			#endregion
		}
	}


}