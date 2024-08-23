﻿namespace WebVella.Tefter.Web.Pages;
public partial class SpaceViewPage : TfBasePage
{
	[Parameter] public Guid SpaceId { get; set; }
	[Parameter] public Guid SpaceViewId { get; set; }
	[Inject] protected IState<SpaceState> SpaceState { get; set; }

	[Inject] protected IStateSelection<ScreenState, bool> ScreenStateSidebarExpanded { get; set; }
	protected override void OnInitialized()
	{
		base.OnInitialized();
		ScreenStateSidebarExpanded.Select(x => x?.SidebarExpanded ?? true);
	}


}