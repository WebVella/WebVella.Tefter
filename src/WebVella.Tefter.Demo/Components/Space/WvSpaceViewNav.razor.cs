
namespace WebVella.Tefter.Demo.Components;
public partial class WvSpaceViewNav : WvBaseComponent
{
	private Space space = null;
	private SpaceData spaceData = null;
	private SpaceView spaceView = null;
	private List<MenuItem> _menuItems = new List<MenuItem>();
	private string _activeTabId = null;
	private string _emptyTabId = RenderUtils.ConvertGuidToHtmlElementId(Guid.NewGuid());

	public override async ValueTask DisposeAsync()
	{
		WvState.ActiveSpaceDataChanged -= onSpaceDataChange;
		await base.DisposeAsync();
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
		{
			init();
			WvState.ActiveSpaceDataChanged += onSpaceDataChange;
			await InvokeAsync(StateHasChanged);
		}
	}

	private void onSpaceDataChange(object sender, StateActiveSpaceDataChangedEventArgs args)
	{
		base.InvokeAsync(async () =>
		{
			init();
			await InvokeAsync(StateHasChanged);
		});

	}

	private void init()
	{
		var meta = WvState.GetActiveSpaceMeta();
		Console.WriteLine($"viewnav get* {meta.Space?.Id} **** {meta.SpaceData?.Id} *** {meta.SpaceView?.Id}");

		space = meta.Space;
		spaceData = meta.SpaceData;
		spaceView = meta.SpaceView;
		_menuItems.Clear();
		foreach (var view in spaceData.Views)
		{
			var menu = new MenuItem
			{
				Id = RenderUtils.ConvertGuidToHtmlElementId(view.Id),
				Title = view.Name,
				Icon = view.Icon,
			};
			if (view.Id == spaceData.MainViewId)
			{
				menu.Url = $"/space/{space.Id}/data/{spaceData.Id}";
			}
			else
			{
				menu.Url = $"/space/{space.Id}/data/{spaceData.Id}/view/{view.Id}";
			}
			_menuItems.Add(menu);
		}
		_activeTabId = RenderUtils.ConvertGuidToHtmlElementId(spaceView?.Id);
	}

	private void HandleOnTabChange(FluentTab tab)
	{

		if (tab.Id == RenderUtils.ConvertGuidToHtmlElementId(spaceData.MainViewId))
		{
			Navigator.NavigateTo($"/space/{space.Id}/data/{spaceData.Id}");
		}
		else
		{
			Navigator.NavigateTo($"/space/{space.Id}/data/{spaceData.Id}/view/{RenderUtils.ConvertHtmlElementIdToGuid(tab.Id)}");
		}
	}


}