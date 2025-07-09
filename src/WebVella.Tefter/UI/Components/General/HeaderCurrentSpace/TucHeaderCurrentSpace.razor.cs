namespace WebVella.Tefter.UI.Components;
public partial class TucHeaderCurrentSpace : TfBaseComponent, IDisposable
{
	[Inject] public ITfSpaceUIService TfSpaceUIService { get; set; } = default!;

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
		var navData = await TfSpaceUIService.GetSpaceNavigationData(Navigator);
		try
		{
			_menu = new();
			List<string> menuText = new();

			if (navData.State.RouteNodes.Count == 0)
			{
				menuText.Add(TfConstants.HomeMenuTitle);
			}
			else
			{
				menuText.Add(navData.State.RouteNodes[0].ToDescriptionString());
				if (navData.State.RouteNodes.Count > 1)
				{
					menuText.Add(navData.State.RouteNodes[1].ToDescriptionString());
				}
			}

			_menu.Add(new TfMenuItem
			{
				Id = "tf-current-space-name",
				Text = String.Join(" : ", menuText),
				Disabled = true,
			});

			var colorName = navData.SpaceColor.GetAttribute().Name;
			var colorVariable = navData.SpaceColor.GetAttribute().Variable;
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