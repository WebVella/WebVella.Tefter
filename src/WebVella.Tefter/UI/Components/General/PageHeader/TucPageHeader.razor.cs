namespace WebVella.Tefter.UI.Components;

public partial class TucPageHeader : TfBaseComponent, IDisposable
{
	[Parameter] public string? Title { get; set; } = null;
	[Parameter] public RenderFragment? PageToolbar { get; set; } = null;

	public void Dispose()
	{
		TfState.NavigationStateChangedEvent -= On_NavigationStateChanged;
	}

	protected override void OnInitialized()
	{
		_init();
		TfState.NavigationStateChangedEvent += On_NavigationStateChanged;
	}

	private async Task On_NavigationStateChanged(TfNavigationState args)
	{
		await InvokeAsync(() =>
		{
			_init();
			StateHasChanged();
		});
	}

	private void _init()
	{
		
	}
	
}