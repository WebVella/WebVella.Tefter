using WebVella.Tefter.UI.Components;

namespace WebVella.Tefter.Talk.Components;

public partial class AssetSpaceViewSelectorAction : TfBaseComponent,
	ITfScreenRegionAddon<TfSpaceViewSelectorActionScreenRegion>
{
	public const string ID = "c899bbe1-eade-4a00-a16e-6af87348ac71";
	public const string NAME = "Add Assets to Selection";
	public const string DESCRIPTION = "";
	public const string FLUENT_ICON_NAME = "DocumentDataLink";
	public const int POSITION_RANK = 200;
	public Guid AddonId { get; init; } = new Guid(ID);
	public string AddonName { get; init; } = NAME;
	public string AddonDescription { get; init; } = DESCRIPTION;
	public string AddonFluentIconName { get; init; } = FLUENT_ICON_NAME;
	public int PositionRank { get; init; } = POSITION_RANK;
	public List<TfScreenRegionScope> Scopes { get; init; } = new();
	[Parameter]
	public TfSpaceViewSelectorActionScreenRegion RegionContext { get; set; }

	[CascadingParameter(Name = "TucSpaceViewPageContent")]
	public TucSpaceViewPageContent TucSpaceViewPageContent { get; set; } = null!;

	private IDialogReference _dialog;
	private async Task _onClick()
	{
		var context = new AssetsAttachModalContext
		{
			DataProviderId = RegionContext.SpaceData.DataProviderId,
			SelectedRowIds = RegionContext.SelectedDataRows,
			CurrentUser = RegionContext.CurrentUser
		};
		_dialog = await DialogService.ShowDialogAsync<AssetsAttachModal>(
				context,
				new ()
				{
					PreventDismissOnOverlayClick = true,
					PreventScroll = true,
					Width = TfConstants.DialogWidthLarge,
					TrapFocus = false,
					OnDialogClosing = EventCallback.Factory.Create<DialogInstance>(this, (instance) =>
					{
						var dataChange = TucSpaceViewPageContent.GetCurrentData().ApplyCountChange(
							countChange: ((AssetsAttachModalContext)instance.Content).CountChange);
						if (dataChange is null) return;
						TucSpaceViewPageContent.OnDataChange(dataChange);
					})
				});
	}

	private Dictionary<Guid, Dictionary<string, object>>? _applyCountChange(TfDataTable? data, Dictionary<Guid, Dictionary<string, long>>? countChange)
	{
		if (data is null || countChange is null || countChange.Keys.Count == 0) return null;
		var dataChange = new Dictionary<Guid, Dictionary<string, object>>();
		foreach (var rowId in countChange.Keys)
		{
			var row = data.Rows[rowId];
			if (row is null) continue;
			foreach (var columnName in countChange[rowId].Keys)
			{
				var column = data.Columns[columnName];
				if (column is null) continue;
				if (column.DbType != TfDatabaseColumnType.ShortInteger
				&& column.DbType != TfDatabaseColumnType.Integer
				&& column.DbType != TfDatabaseColumnType.LongInteger
				&& column.DbType != TfDatabaseColumnType.Number) continue;

                if (!dataChange.ContainsKey(rowId))
					dataChange[rowId] = new();
				dataChange[rowId][columnName] = (row[columnName].ToLong() ?? 0) + countChange[rowId][columnName];
			}
		}

		return dataChange;

	}
}