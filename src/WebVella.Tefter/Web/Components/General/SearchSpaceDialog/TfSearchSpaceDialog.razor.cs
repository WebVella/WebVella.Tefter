namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.General.SearchSpaceDialog.TfSearchSpaceDialog", "WebVella.Tefter")]
public partial class TfSearchSpaceDialog : TfFormBaseComponent, IDialogContentComponent<Guid>
{
	[Inject] private AppStateUseCase UC { get; set; }
	[Parameter] public Guid Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; }

	private bool _isBusy = true;
	private string _search = null;
	private List<TucSpace> _allSpaces = new();
	private List<TucSpace> _items = new();

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);

		if (firstRender)
		{
			_allSpaces = await UC.GetAllUserSpaceListAsync(Content);
			_items = _allSpaces.ToList();
			_isBusy = false;
			await InvokeAsync(StateHasChanged);
		}

	}

	private async Task _cancel()
	{
		await Dialog.CancelAsync();
	}

	private void _onSearch(string value)
	{
		_search = value;
		if (String.IsNullOrWhiteSpace(_search))
		{
			_items = _allSpaces.ToList();
		}
		else
		{
			var search = _search.Trim().ToLowerInvariant();
			_items = _allSpaces.Where(x => x.Name.ToLowerInvariant().Contains(search)).ToList();
		}
	}



}
