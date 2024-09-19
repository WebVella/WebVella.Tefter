namespace WebVella.Tefter.Web.Components;
public partial class TfSpaceViewShareSelector : TfBaseComponent
{
    [Inject] protected IState<TfAppState> TfAppState { get; set; }
	private bool _open = false;
	private bool _selectorLoading = false;


	public async Task ToggleSelector()
	{
		_open = !_open;
		if (_open)
		{
			_selectorLoading = true;
			await InvokeAsync(StateHasChanged);
			await Task.Delay(1000);//loading components?

			_selectorLoading = false;
			await InvokeAsync(StateHasChanged);
		}
	}
	private async Task _copyLinkToClipboard(){ 
		await JSRuntime.InvokeVoidAsync("Tefter.copyToClipboard", Navigator.Uri);
		ToastService.ShowSuccess(LOC("Link copied"));
	}
	
}