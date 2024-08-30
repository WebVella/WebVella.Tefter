﻿namespace WebVella.Tefter.Web.Components.SpaceViewDetails;
public partial class TfSpaceViewDetails : TfBaseComponent
{
	[Inject] protected IState<SpaceState> SpaceState { get; set; }
	[Inject] protected IStateSelection<ScreenState, bool> ScreenStateSidebarExpanded { get; set; }
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		ScreenStateSidebarExpanded.Select(x => x?.SidebarExpanded ?? true);
	}

}