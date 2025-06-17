namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Admin.AdminSharedColumnsNavigation.TfAdminSharedColumnsNavigation", "WebVella.Tefter")]
public partial class TfAdminSharedColumnsNavigation : TfBaseComponent
{
	[Inject] protected IStateSelection<TfUserState, bool> SidebarExpanded { get; set; }
	[Inject] protected IState<TfAppState> TfAppState { get; set; }

	private string search = null;
	private int _stringLimit = 30;

	protected override void OnInitialized()
	{
		base.OnInitialized();
		SidebarExpanded.Select(x => x.SidebarExpanded);
	}

	private async Task onAddClick()
	{
		var dialog = await DialogService.ShowDialogAsync<TfSharedColumnManageDialog>(
			new TucSharedColumn(),
			new DialogParameters()
			{
				PreventDismissOnOverlayClick = true,
				PreventScroll = true,
				Width = TfConstants.DialogWidthLarge,
				TrapFocus = false
			});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
			ToastService.ShowSuccess(LOC("Column successfully created!"));
			var column = (TucSharedColumn)result.Data;
			Navigator.NavigateTo(string.Format(TfConstants.AdminSharedColumnDetailsPageUrl, column.Id));
		}
	}
	private List<TucSharedColumn> _getColumns()
	{
		search = search?.Trim().ToLowerInvariant();
		var menuItems = new List<TucSharedColumn>();
		foreach (var column in TfAppState.Value.AdminSharedColumns)
		{
			if (!String.IsNullOrWhiteSpace(search) &&
				!(column.DbName.ToLowerInvariant().Contains(search))
				)
				continue;

			menuItems.Add(column);
		}

		return menuItems;
	}

	private void onSearch(string search)
	{
		this.search = search;
	}
}