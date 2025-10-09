// namespace WebVella.Tefter.UI.Components;
//
// public partial class TucHeaderCurrentSpace : TfBaseComponent, IDisposable
// {
// 	private int _ellipsisCount = 30;
// 	private List<TfMenuItem> _menu = new();
// 	private bool _isLoading = true;
// 	private string _styles = String.Empty;
// 	public void Dispose()
// 	{
// 		Navigator.LocationChanged -= On_NavigationStateChanged;
// 	}
//
// 	protected override void OnInitialized()
// 	{
// 		_init(TfAuthLayout.AppState.NavigationState);
// 		Navigator.LocationChanged += On_NavigationStateChanged;
// 	}
// 	private void On_NavigationStateChanged(object? caller, LocationChangedEventArgs args)
// 	{
// 		await InvokeAsync(async () =>
// 		{
// 			if (UriInitialized != args.Uri)
// 			{
// 					_init(args);
// 					await InvokeAsync(StateHasChanged);
// 			}
// 		});
// 	}
// 	private void _init(TfNavigationState navState)
// 	{
// 		try
// 		{
// 			_menu = new();
// 			List<string> menuText = new();
//
// 			if (navState.HasNode(RouteDataNode.Home, 0))
// 			{
// 				menuText.Add(LOC("Home"));
// 			}
// 			else if (navState.HasNode(RouteDataNode.Admin, 0))
// 			{
// 				menuText.Add(navState.RouteNodes[0].ToDescriptionString());
// 				if (navState.RouteNodes.Count > 1)
// 				{
// 					menuText.Add(navState.RouteNodes[1].ToDescriptionString());
// 				}
// 			}
// 			else if (navState.HasNode(RouteDataNode.Space, 0))
// 			{
// 				menuText.Add(navState.RouteNodes[0].ToDescriptionString());
// 				if (navState.RouteNodes.Count > 2)
// 				{
// 					menuText.Add(navState.RouteNodes[2].ToDescriptionString());
// 				}
// 			}
// 			else if (navState.HasNode(RouteDataNode.Pages, 0))
// 			{
// 				menuText.Add(navState.RouteNodes[0].ToDescriptionString());
// 				if (navState.RouteNodes.Count > 1)
// 				{
// 					menuText.Add(navState.RouteNodes[1].ToDescriptionString());
// 				}
// 			}
// 			_menu.Add(new TfMenuItem
// 			{
// 				Id = "tf-current-space-name",
// 				Text = String.Join(" : ", menuText),
// 				Disabled = true,
// 			});
// 			var navMenu = TfAuthLayout.NavigationMenu;
// 			var colorName = navMenu.SpaceColor.GetColor().Name;
// 			var colorVariable = navMenu.SpaceColor.GetColor().Variable;
// 			var sb = new StringBuilder();
// 			sb.AppendLine("<style>");
// 			sb.AppendLine($"html:root body {{");
// 			sb.AppendLine($"--tf-layout-color: var({colorVariable});");
//
// 			//sb.AppendLine($"--accent-base-color: var(--tf-{colorName}-500);");
// 			//sb.AppendLine($"--accent-fill-rest: var(--tf-{colorName}-500);");
// 			//sb.AppendLine($"--accent-fill-hover: var(--tf-{colorName}-600);");
// 			//sb.AppendLine($"--accent-fill-active: var(--tf-{colorName}-700);");
// 			//sb.AppendLine($"--accent-fill-focus: var(--tf-{colorName}-600);");
//
//
// 			//sb.AppendLine($"--accent-foreground-rest: var(--tf-{colorName}-700);");
// 			//sb.AppendLine($"--accent-foreground-hover: var(--tf-{colorName}-500);");
// 			//sb.AppendLine($"--accent-foreground-active: var(--tf-{colorName}-400);");
// 			//sb.AppendLine($"--accent-foreground-focus: var(--tf-{colorName}-600);");
// 			//sb.AppendLine($"--accent-stroke-control-rest: linear-gradient(var(--tf-{colorName}-600) 90%, var(--tf-{colorName}-700) 100%);");
// 			//sb.AppendLine($"--accent-stroke-control-hover: linear-gradient(var(--tf-{colorName}-500) 90%, var(--tf-{colorName}-600) 100%);");
// 			//sb.AppendLine($"--accent-stroke-control-active: var(--tf-{colorName}-500);");
// 			//sb.AppendLine($"--accent-stroke-control-focus: linear-gradient(var(--tf-{colorName}-400) 90%, var(--tf-{colorName}-700) 100%);");
//
// 			sb.AppendLine("}");
// 			sb.AppendLine("</style>");
// 			_styles = sb.ToString();
// 		}
// 		finally
// 		{
// 			_isLoading = false;
// 		}
// 	}
// }