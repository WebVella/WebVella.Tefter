namespace WebVella.Tefter.UI.Components;
public partial class TucHeaderCurrentSpace : TfBaseComponent, IDisposable
{
	[Inject] public ITfUserUIService TfUserUIService { get; set; } = default!;
	[Inject] public ITfNavigationUIService TfNavigationUIService { get; set; } = default!;

	private int _ellipsisCount = 30;
	private List<TfMenuItem> _menu = new();
	private bool _isLoading = true;
	private string _styles = String.Empty;
	public void Dispose()
	{
		Navigator.LocationChanged -= On_LocationChanged;
	}

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		await _init();
		Navigator.LocationChanged += On_LocationChanged;
	}
	private async void On_LocationChanged(object? caller, LocationChangedEventArgs args)
	{
		await _init();
	}
	private async Task _init()
	{
		var navState = await TfNavigationUIService.GetNavigationStateAsync(Navigator);
		var currentUser = await TfUserUIService.GetCurrentUserAsync();
		var navMenu = await TfNavigationUIService.GetNavigationMenu(Navigator,currentUser);
		try
		{
			_menu = new();
			List<string> menuText = new();

			if (navState.HasNode(RouteDataNode.Home,0))
			{
				menuText.Add(TfConstants.HomeMenuTitle);
			}
			else if(navState.HasNode(RouteDataNode.Admin,0))
			{
				menuText.Add(navState.RouteNodes[0].ToDescriptionString());
				if (navState.RouteNodes.Count > 1)
				{
					menuText.Add(navState.RouteNodes[1].ToDescriptionString());
				}
			}
			else if(navState.HasNode(RouteDataNode.Space,0))
			{
				menuText.Add(navState.RouteNodes[0].ToDescriptionString());
				if (navState.RouteNodes.Count > 2)
				{
					menuText.Add(navState.RouteNodes[2].ToDescriptionString());
				}
			}
			else if(navState.HasNode(RouteDataNode.Pages,0))
			{
				menuText.Add(navState.RouteNodes[0].ToDescriptionString());
				if (navState.RouteNodes.Count > 1)
				{
					menuText.Add(navState.RouteNodes[1].ToDescriptionString());
				}
			}
			_menu.Add(new TfMenuItem
			{
				Id = "tf-current-space-name",
				Text = String.Join(" : ", menuText),
				Disabled = true,
			});

			var colorName = navMenu.SpaceColor.GetAttribute().Name;
			var colorVariable = navMenu.SpaceColor.GetAttribute().Variable;
			var sb = new StringBuilder();
			sb.AppendLine("<style>");
			sb.AppendLine($"html:root body {{");
			sb.AppendLine($"--tf-layout-color: var({colorVariable});");

			//sb.AppendLine($"--accent-base-color: var(--tf-{colorName}-500);");
			//sb.AppendLine($"--accent-fill-rest: var(--tf-{colorName}-500);");
			//sb.AppendLine($"--accent-fill-hover: var(--tf-{colorName}-600);");
			//sb.AppendLine($"--accent-fill-active: var(--tf-{colorName}-700);");
			//sb.AppendLine($"--accent-fill-focus: var(--tf-{colorName}-600);");


			//sb.AppendLine($"--accent-foreground-rest: var(--tf-{colorName}-700);");
			//sb.AppendLine($"--accent-foreground-hover: var(--tf-{colorName}-500);");
			//sb.AppendLine($"--accent-foreground-active: var(--tf-{colorName}-400);");
			//sb.AppendLine($"--accent-foreground-focus: var(--tf-{colorName}-600);");
			//sb.AppendLine($"--accent-stroke-control-rest: linear-gradient(var(--tf-{colorName}-600) 90%, var(--tf-{colorName}-700) 100%);");
			//sb.AppendLine($"--accent-stroke-control-hover: linear-gradient(var(--tf-{colorName}-500) 90%, var(--tf-{colorName}-600) 100%);");
			//sb.AppendLine($"--accent-stroke-control-active: var(--tf-{colorName}-500);");
			//sb.AppendLine($"--accent-stroke-control-focus: linear-gradient(var(--tf-{colorName}-400) 90%, var(--tf-{colorName}-700) 100%);");

			sb.AppendLine("}");
			sb.AppendLine("</style>");
			_styles = sb.ToString();
		}
		finally
		{
			_isLoading = false;
			await InvokeAsync(StateHasChanged);
		}
	}
}