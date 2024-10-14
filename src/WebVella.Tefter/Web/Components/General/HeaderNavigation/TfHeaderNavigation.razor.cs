namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpaceView.HeaderNavigation.TfHeaderNavigation", "WebVella.Tefter")]
public partial class TfHeaderNavigation : TfBaseComponent
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] private AppStateUseCase UC { get; set; }

	private List<TucMenuItem> _menu = new List<TucMenuItem>{
		new TucMenuItem{
			Title = "Status",
			Icon = TfConstants.GetIcon("Folder"),
			Url="./",
			Nodes = new List<TucMenuItem>{}
		},
		new TucMenuItem{
			Title = "Expiration Date",
			Icon = TfConstants.GetIcon("Folder"),
			Url="./",
			Nodes = new List<TucMenuItem>{}
		},
		new TucMenuItem{
			Title = "Space",
			Icon = TfConstants.SpaceViewIcon,
			Url="./",
			Active = true,
			Nodes= null
		},
		new TucMenuItem{
			Title = "Space",
			Icon = TfConstants.SpaceViewIcon,
			Url="./",
			Nodes= null
		},
		new TucMenuItem{
			Title = "Space",
			Icon = TfConstants.SpaceViewIcon,
			Url="./",
			Nodes= null
		},
		new TucMenuItem{
			Title = "Space",
			Icon = TfConstants.SpaceViewIcon,
			Url="./",
			Nodes= null
		},
		new TucMenuItem{
			Title = "Space",
			Icon = TfConstants.SpaceViewIcon,
			Url="./",
			Nodes= null
		},
	};
}