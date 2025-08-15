namespace WebVella.Tefter.UI.Components;
public partial class TucPageHeader : TfBaseComponent
{
	[Inject] public ITfNavigationUIService TfNavigationUIService { get; set; } = default!;
	[Parameter] public string? Title { get; set; }
	[Parameter] public string? Area { get; set; }
	[Parameter] public RenderFragment Toolbar { get; set; }
	[Parameter] public string? SubTitle { get; set; }
	[Parameter] public string? Class { get; set; }
	[Parameter] public string? Style { get; set; }
	[Parameter] public Icon? Icon { get; set; } = null;
	[Parameter] public TfColor? IconColor { get; set; } = null;
	private string _cssClass { get => $"tf-page-header {Class}"; }
	private string? _returnUrl = null;
	public void Dispose()
	{
		TfNavigationUIService.NavigationStateChanged -= On_NavigationStateChanged;
	}
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		await _init();
		TfNavigationUIService.NavigationStateChanged += On_NavigationStateChanged;
	}

	private async void On_NavigationStateChanged(object? caller, TfNavigationState args)
	{
		if (UriInitialized != args.Uri)
			await _init(args);
	}

	private async Task _init(TfNavigationState? navState = null)
	{
		if (navState is null)
			navState = await TfNavigationUIService.GetNavigationStateAsync(Navigator);
		try
		{
			_returnUrl = navState.ReturnUrl;
		}
		finally
		{
			UriInitialized = navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}

	private TfColor _iconForegroundColor
	{
		get
		{
			if (IconColor is null) return TfColor.White;

			var colorInfo = IconColor.Value.GetAttribute();
			if (colorInfo.UseWhiteForeground) return TfColor.White;
			return TfColor.Slate950;
		}
	}
	private TfColor _iconBackgroundColor { get => IconColor ?? TfColor.Slate600; }
}