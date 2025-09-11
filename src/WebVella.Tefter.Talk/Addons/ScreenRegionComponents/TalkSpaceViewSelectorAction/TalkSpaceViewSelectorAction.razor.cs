using WebVella.Tefter.UI.Components;

namespace WebVella.Tefter.Talk.Components;

public partial class TalkSpaceViewSelectorAction : TfBaseComponent,
	ITfScreenRegionComponent<TfSpaceViewSelectorActionScreenRegionContext>
{
	public const string ID = "942d6fb0-4662-4c5c-ae52-123dd40375ac";
	public const string NAME = "Add Talk Discussion to Selection";
	public const string DESCRIPTION = "";
	public const string FLUENT_ICON_NAME = "CommentMultiple";
	public const int POSITION_RANK = 100;
	public Guid AddonId { get; init; } = new Guid(ID);
	public string AddonName { get; init; } = NAME;
	public string AddonDescription { get; init; } = DESCRIPTION;
	public string AddonFluentIconName { get; init; } = FLUENT_ICON_NAME;
	public int PositionRank { get; init; } = POSITION_RANK;
	public List<TfScreenRegionScope> Scopes { get; init; } = new();
	[Parameter]
	public TfSpaceViewSelectorActionScreenRegionContext RegionContext { get; set; }

	[CascadingParameter(Name = "TucSpaceViewPageContent")]
	public TucSpaceViewPageContent TucSpaceViewPageContent { get; set; } = default!;

	private IDialogReference _dialog;
	private async Task _onClick()
	{
		var context = new TalkThreadModalContext
		{
			DataProviderId = RegionContext.SpaceData.DataProviderId,
			SelectedRowIds = RegionContext.SelectedDataRows,
			CurrentUser = RegionContext.CurrentUser
		};
		_dialog = await DialogService.ShowDialogAsync<TalkThreadModal>(
				context,
				new DialogParameters()
				{
					PreventDismissOnOverlayClick = true,
					PreventScroll = true,
					Width = TfConstants.DialogWidthLarge,
					TrapFocus = false,
					OnDialogClosing = EventCallback.Factory.Create<DialogInstance>(this, async (instance) =>
					{
						var dataChange = _applyCountChange(
							data: TucSpaceViewPageContent.GetCurrentData(),
							countChange: ((TalkThreadModalContext)instance.Content).CountChange);
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