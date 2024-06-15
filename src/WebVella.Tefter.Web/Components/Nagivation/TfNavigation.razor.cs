namespace WebVella.Tefter.Web.Components;
public partial class TfNavigation: TfBaseComponent
{
	[Inject] protected IState<SessionState> SessionState { get; set; }

	private List<MenuItem> _menuItems = new();

	protected override void OnAfterRender(bool firstRender)
	{
		base.OnAfterRender(firstRender);
		if (firstRender)
		{
			GenerateSpaceDataMenu();
			StateHasChanged();
		}
	}

	private void GenerateSpaceDataMenu()
	{
		_menuItems.Clear();

		foreach (var space in SessionState.Value.SpaceList)
		{
			_menuItems.Add(new MenuItem{ 
				Icon = space.Icon,
				Id = RenderUtils.ConvertGuidToHtmlElementId(space.Id),
				Match = NavLinkMatch.Prefix,
				Url = $"/space/{space.Id}",
				Title = space.Name,
				IconColor = space.Color,
			});
		}

	}

	private void _addSpaceHandler(){
		ToastService.ShowToast(ToastIntent.Warning, "Will open add new space modal");
	}

}