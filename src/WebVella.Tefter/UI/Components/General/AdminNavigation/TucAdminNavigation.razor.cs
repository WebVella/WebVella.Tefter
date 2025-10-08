namespace WebVella.Tefter.UI.Components;
public partial class TucAdminNavigation : TfBaseComponent,IDisposable
{
	public void Dispose()
	{
		TfState.NavigationStateChangedEvent -= On_NavigationStateChanged;
	}
	protected override void OnInitialized()
	{
		TfState.NavigationStateChangedEvent += On_NavigationStateChanged;
	}
	
	private async Task On_NavigationStateChanged(TfNavigationState args)
	{
		await InvokeAsync(StateHasChanged);
	}	
}