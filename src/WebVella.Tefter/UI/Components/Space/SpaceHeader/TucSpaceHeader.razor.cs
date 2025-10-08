namespace WebVella.Tefter.UI.Components;
public partial class TucSpaceHeader : TfBaseComponent, IDisposable
{

	public void Dispose()
	{
		TfState.NavigationStateChangedEvent -= On_NavigationStateChanged;
	}
	protected override void OnInitialized()
	{
		TfState.NavigationStateChangedEvent += On_NavigationStateChanged;
	}
	
	private async void On_NavigationStateChanged(object? caller, TfNavigationState args)
	{
		await InvokeAsync(StateHasChanged);
	}	
	
}
