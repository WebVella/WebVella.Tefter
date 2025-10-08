namespace WebVella.Tefter.UI.Components;

public partial class TucPageHeader : TfBaseComponent, IDisposable
{
	[Parameter] public string? Title { get; set; } = null;
	[Parameter] public RenderFragment? ChildContent { get; set; } = null;

	public void Dispose()
	{
		Navigator.LocationChanged -= On_NavigationStateChanged;
	}

	protected override void OnInitialized()
	{
		_init();
		Navigator.LocationChanged += On_NavigationStateChanged;
	}

	private void On_NavigationStateChanged(object? caller, LocationChangedEventArgs args)
	{
		_init();
		StateHasChanged();
	}

	private void _init()
	{
	}
}